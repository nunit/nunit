// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading;

#if THREAD_ABORT
using System.ComponentModel;
using System.Runtime.InteropServices;
#endif

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
            Thread.Sleep(milliseconds);
        }

#if THREAD_ABORT
        private const int ThreadAbortedCheckDelay = 100;

        /// <summary>
        /// Pre-Task compatibility
        /// </summary>
        public static void Delay(int milliseconds, WaitCallback threadPoolWork, object? state = null)
        {
            new DelayClosure(milliseconds, threadPoolWork, state);
        }

        private sealed class DelayClosure
        {
            private readonly Timer _timer;
            private readonly WaitCallback _threadPoolWork;
            private readonly object? _state;

            public DelayClosure(int milliseconds, WaitCallback threadPoolWork, object? state)
            {
                _threadPoolWork = threadPoolWork;
                _state = state;
                _timer = new Timer(Callback, null, Timeout.Infinite, Timeout.Infinite);
                // Make sure _timer is set before starting the timer so that it is disposed.
                _timer.Change(milliseconds, Timeout.Infinite);
            }

            private void Callback(object? _)
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
        public static void Kill(Thread thread, object? stateInfo, int nativeId = 0)
        {
            if (nativeId != 0)
                DislodgeThreadInNativeMessageWait(thread, nativeId);

            try
            {
                if (stateInfo is null)
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

            if ((thread.ThreadState & ThreadState.WaitSleepJoin) != 0)
                thread.Interrupt();
        }

        /// <summary>
        /// Schedule a thread pool thread to check on the aborting thread in case it's in a message pump native blocking wait
        /// </summary>
        private static void DislodgeThreadInNativeMessageWait(Thread thread, int nativeId)
        {
            if (nativeId == 0) throw new ArgumentOutOfRangeException(nameof(nativeId), "Native thread ID must not be zero.");

            // Schedule a thread pool thread to check on the aborting thread in case it's in a message pump native blocking wait
            Delay(ThreadAbortedCheckDelay, CheckOnAbortingThread, new CheckOnAbortingThreadState(thread, nativeId));
        }

        private static void CheckOnAbortingThread(object? state)
        {
            var context = (CheckOnAbortingThreadState)state!;

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

        private static bool _isNotOnWindows;

        /// <summary>
        /// Captures the current thread's native id. If provided to <see cref="Kill(Thread,int)"/> later, allows the thread to be killed if it's in a message pump native blocking wait.
        /// </summary>
        public static int GetCurrentThreadNativeId()
        {
            if (_isNotOnWindows) return 0;
            try
            {
                return GetCurrentThreadId();
            }
            catch (EntryPointNotFoundException)
            {
                _isNotOnWindows = true;
                return 0;
            }
        }

        private const int ERROR_INVALID_THREAD_ID = 0x5A4;

        /// <summary>
        /// Sends a message to the thread to dislodge it from native code and allow a return to managed code, where a ThreadAbortException can be generated.
        /// The message is meaningless (WM_CLOSE without a window handle) but it will end any blocking message wait.
        /// </summary>
        private static void PostThreadCloseMessage(int nativeId)
        {
            if (!PostThreadMessage(nativeId, WM.CLOSE, IntPtr.Zero, IntPtr.Zero))
            {
                // ReSharper disable once InconsistentNaming
                // P/invoke doesn’t need to follow naming convention
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
            // ReSharper disable once InconsistentNaming
            // P/invoke doesn’t need to follow naming convention
            CLOSE = 0x0010
        }
#endif

        /// <summary>Gets <see cref="Thread.CurrentPrincipal"/> or <see langword="null" /> if the current platform does not support it.</summary>
        public static System.Security.Principal.IPrincipal? GetCurrentThreadPrincipal()
        {
            try
            {
                return Thread.CurrentPrincipal;
            }
            catch (PlatformNotSupportedException) //E.g. Mono.WASM
            {
                return null;
            }
        }

        /// <summary>Sets <see cref="Thread.CurrentPrincipal"/> if current platform supports it.</summary>
        /// <param name="principal">Value to set. If the current platform does not support <see cref="Thread.CurrentPrincipal"/> then the only allowed value is <see langword="null"/>.</param>
        public static void SetCurrentThreadPrincipal(System.Security.Principal.IPrincipal? principal)
        {
            try
            {
                Thread.CurrentPrincipal = principal;
            }
            catch (PlatformNotSupportedException) when (principal is null) //E.g. Mono.WASM
            { }
        }
    }
}
