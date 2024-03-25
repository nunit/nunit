// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#if NET6_0

using System.Threading;

namespace NUnit.Framework.Internal
{
    public static partial class ThreadUtility
    {
        /// <summary>
        /// Cancels an "Thread.Abort" requested for the current thread.
        /// </summary>
        public static void ResetAbort()
        {
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
#pragma warning disable SYSLIB0006 // Type or member is obsolete
            thread.Abort();
#pragma warning restore SYSLIB0006 // Type or member is obsolete
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
            Abort(thread, nativeId);
        }

        /// <summary>
        /// Do our best to kill a thread, passing state info
        /// </summary>
        /// <param name="thread">The thread to kill</param>
        /// <param name="stateInfo">Info for the ThreadAbortException handler</param>
        /// <param name="nativeId">The native thread id (if known), otherwise 0.</param>
        public static void Kill(Thread thread, object? stateInfo, int nativeId = 0)
        {
#pragma warning disable SYSLIB0006 // Type or member is obsolete
            thread.Abort(stateInfo);
#pragma warning restore SYSLIB0006 // Type or member is obsolete
        }

        /// <summary>
        /// Captures the current thread's native id. If provided to <see cref="Kill(Thread,int)"/> later, allows the thread to be killed if it's in a message pump native blocking wait.
        /// </summary>
        public static int GetCurrentThreadNativeId()
        {
            return 0;
        }
    }
}

#endif
