// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#if NET7_0_OR_GREATER

using System.Threading;
using System.Reflection;
using System.Runtime;

namespace NUnit.Framework.Internal
{
    public static partial class ThreadUtility
    {
        // Information obtained from:
        // https://github.com/dotnet/runtime/blob/main/src/coreclr/System.Private.CoreLib/src/System/Runtime/ControlledExecution.CoreCLR.cs

        private static readonly MethodInfo? GetNativeThreadHandleMethod =
            typeof(Thread).GetMethod("GetNativeHandle", BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly MethodInfo? AbortThreadMethod =
            typeof(ControlledExecution).GetMethod("AbortThread", BindingFlags.NonPublic | BindingFlags.Static);

        private static readonly MethodInfo? ResetAbortThreadMethod =
            typeof(ControlledExecution).GetMethod("ResetAbortThread", BindingFlags.NonPublic | BindingFlags.Static);

        /// <summary>
        /// Cancels an "Thread.Abort" requested for the current thread.
        /// </summary>
        public static void ResetAbort()
        {
            ResetAbortThreadMethod?.Invoke(null, null);
        }

        /// <summary>
        /// Abort a thread, helping to dislodging it if it is blocked in native code
        /// </summary>
        /// <param name="thread">The thread to abort</param>
        /// <param name="nativeId">The native thread id (if known), otherwise 0.</param>
        public static void Abort(Thread thread, int nativeId = 0)
        {
            if (GetNativeThreadHandleMethod is not null &&
                AbortThreadMethod is not null)
            {
                object threadHandle = GetNativeThreadHandleMethod?.Invoke(thread, null)!;
                AbortThreadMethod?.Invoke(null, new object?[] { threadHandle });
            }
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
        /// <param name="nativeId">The native thread id (if known), otherwise 0.
        /// If provided, allows the thread to be killed if it's in a message pump native blocking wait.
        /// This must have previously been captured by calling <see cref="GetCurrentThreadNativeId"/> from the running thread itself.</param>
        public static void Kill(Thread thread, object? stateInfo, int nativeId = 0)
        {
            Abort(thread, nativeId);
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
