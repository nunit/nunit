// ***********************************************************************
// Copyright (c) 2011 Charlie Poole, Rob Prouse
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// ThreadUtility provides a set of static methods convenient
    /// for working with threads.
    /// </summary>
    public static class ThreadUtility
    {
        internal static void BlockingDelay(int milliseconds)
        {
#if NETSTANDARD1_6
            System.Threading.Tasks.Task.Delay(milliseconds).GetAwaiter().GetResult();
#else
            Thread.Sleep(milliseconds);
#endif
        }

#if THREAD_ABORT
        private const int ThreadAbortedCheckDelay = 100;

        /// <summary>
        /// Pre-Task compatibility
        /// </summary>
        public static void Delay(int milliseconds, WaitCallback threadPoolWork, object state = null)
        {
            new DelayClosure(milliseconds, threadPoolWork, state);
        }

        private sealed class DelayClosure
        {
            private readonly Timer _timer;
            private readonly WaitCallback _threadPoolWork;
            private readonly object _state;

            public DelayClosure(int milliseconds, WaitCallback threadPoolWork, object state)
            {
                _threadPoolWork = threadPoolWork;
                _state = state;
                _timer = new Timer(Callback, null, Timeout.Infinite, Timeout.Infinite);
                // Make sure _timer is set before starting the timer so that it is disposed.
                _timer.Change(milliseconds, Timeout.Infinite);
            }

            private void Callback(object _)
            {
                _timer.Dispose();
                _threadPoolWork.Invoke(_state);
            }
        }


        /// <summary>
        /// Abort a thread, helping to dislodging it if it is blocked in native code
        /// </summary>
        /// <param name="thread">The thread to abort</param>
        /// <param name="nativeId">The native thread id (if known), otherwise 0.
        /// If provided, allows the thread to be killed if it's in a message pump native blocking wait.
        /// This must have previously been captured by calling <see cref="GetCurrentThreadNativeId"/> from the running thread itself.</param>
        public static void Abort(Thread thread, int nativeId = 0)
        {
            if (nativeId != 0)
                DislodgeThreadInNativeMessageWait(thread, nativeId);

            thread.Abort();
        }


        /// <summary>
        /// Do our best to kill a thread
        /// </summary>
        /// <param name="thread">The thread to kill</param>
        /// <param name="nativeId">The native thread id (if known), otherwise 0.
        /// If provided, allows the thread to be killed if it's in a message pump native blocking wait.
        /// This must have previously been captured by calling <see cref="GetCurrentThreadNativeId"/> from the running thread itself.</param>
        public static void Kill(Thread thread, int nativeId = 0)
        {
            Kill(thread, null, nativeId);
        }

        /// <summary>
        /// Do our best to kill a thread, passing state info
        /// </summary>
        /// <param name="thread">The thread to kill</param>
        /// <param name="stateInfo">Info for the ThreadAbortException handler</param>
        /// <param name="nativeId">The native thread id (if known), otherwise 0.
        /// If provided, allows the thread to be killed if it's in a message pump native blocking wait.
        /// This must have previously been captured by calling <see cref="GetCurrentThreadNativeId"/> from the running thread itself.</param>
        public static void Kill(Thread thread, object stateInfo, int nativeId = 0)
        {
            if (nativeId != 0)
                DislodgeThreadInNativeMessageWait(thread, nativeId);


            try
            {
                if (stateInfo == null)
                    thread.Abort();
                else
                    thread.Abort(stateInfo);
            }
            catch (ThreadStateException)
            {
                // Although obsolete, this use of Resume() takes care of
                // the odd case where a ThreadStateException is received.
#pragma warning disable 0618,0612    // Thread.Resume has been deprecated
                thread.Resume();
#pragma warning restore 0618,0612   // Thread.Resume has been deprecated
            }

            if ( (thread.ThreadState & ThreadState.WaitSleepJoin) != 0 )
                thread.Interrupt();
        }

        /// <summary>
        /// Schedule a threadpool thread to check on the aborting thread in case it's in a message pump native blocking wait
        /// </summary>
        private static void DislodgeThreadInNativeMessageWait(Thread thread, int nativeId)
        {
            if (nativeId == 0) throw new ArgumentOutOfRangeException(nameof(nativeId), "Native thread ID must not be zero.");

            // Schedule a threadpool thread to check on the aborting thread in case it's in a message pump native blocking wait
            Delay(ThreadAbortedCheckDelay, CheckOnAbortingThread, new CheckOnAbortingThreadState(thread, nativeId));
        }

        private static void CheckOnAbortingThread(object state)
        {
            var context = (CheckOnAbortingThreadState)state;

            switch (context.Thread.ThreadState)
            {
                case ThreadState.Aborted:
                    return;
                case ThreadState.AbortRequested:
                    PostThreadCloseMessage(context.NativeId);
                    break;
            }

            Delay(ThreadAbortedCheckDelay, CheckOnAbortingThread, state);
        }

        private sealed class CheckOnAbortingThreadState
        {
            public Thread Thread { get; }
            public int NativeId { get; }

            public CheckOnAbortingThreadState(Thread thread, int nativeId)
            {
                Thread = thread;
                NativeId = nativeId;
            }
        }



        private static bool isNotOnWindows;

        /// <summary>
        /// Captures the current thread's native id. If provided to <see cref="Kill(Thread,int)"/> later, allows the thread to be killed if it's in a message pump native blocking wait.
        /// </summary>
        [SecuritySafeCritical]
        public static int GetCurrentThreadNativeId()
        {
            if (isNotOnWindows) return 0;
            try
            {
                return GetCurrentThreadId();
            }
            catch (EntryPointNotFoundException)
            {
                isNotOnWindows = true;
                return 0;
            }
        }


        /// <summary>
        /// Sends a message to the thread to dislodge it from native code and allow a return to managed code, where a ThreadAbortException can be generated.
        /// The message is meaningless (WM_CLOSE without a window handle) but it will end any blocking message wait.
        /// </summary>
        [SecuritySafeCritical]
        private static void PostThreadCloseMessage(int nativeId)
        {
            if (!PostThreadMessage(nativeId, WM.CLOSE, IntPtr.Zero, IntPtr.Zero))
            {
                const int ERROR_INVALID_THREAD_ID = 0x5A4;
                var errorCode = Marshal.GetLastWin32Error();
                if (errorCode != ERROR_INVALID_THREAD_ID)
                    throw new Win32Exception(errorCode);
            }
        }


        [DllImport("kernel32.dll")]
        private static extern int GetCurrentThreadId();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool PostThreadMessage(int id, WM msg, IntPtr wParam, IntPtr lParam);

        private enum WM : uint
        {
            CLOSE = 0x0010
        }
#endif
    }
}
