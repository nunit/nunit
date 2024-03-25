// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#if NET7_0_OR_GREATER

using System.Threading;
using System.Reflection;
using System.Runtime;
using System;

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
        /// Cancels an "Thread.Abort" requested for the current thread.
        /// </summary>
        public static void Abort(Thread thread, object? stateInfo)
        {
            if (GetNativeThreadHandleMethod is not null &&
                AbortThreadMethod is not null)
            {
                object threadHandle = GetNativeThreadHandleMethod.Invoke(thread, null)!;
                AbortThreadMethod.Invoke(null, new object?[] { threadHandle });
            }
            else
            {
                throw new NotImplementedException("Internal of methods for abort must have changed. Please raise an issue at nunit.");
            }
        }
    }
}

#endif
