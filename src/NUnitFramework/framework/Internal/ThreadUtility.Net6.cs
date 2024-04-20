// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#if NET6_0

using System.Threading;

#pragma warning disable SYSLIB0006 // Type or member is obsolete

namespace NUnit.Framework.Internal
{
    public static partial class ThreadUtility
    {
        /// <summary>
        /// Cancels an "Thread.Abort" requested for the current thread.
        /// </summary>
        public static void ResetAbort()
        {
            Thread.ResetAbort();
        }

        /// <summary>
        /// Cancels an "Thread.Abort" requested for the current thread.
        /// </summary>
        public static void Abort(Thread thread, object? stateInfo)
        {
            if (stateInfo is null)
                thread.Abort();
            else
                thread.Abort(stateInfo);
        }
    }
}

#endif
