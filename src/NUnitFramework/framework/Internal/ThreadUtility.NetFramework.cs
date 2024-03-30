// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#if NETFRAMEWORK

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
            if (Thread.CurrentThread.ThreadState == ThreadState.AbortRequested)
                Thread.ResetAbort();
        }

        /// <summary>
        /// Cancels an "Thread.Abort" requested for the current thread.
        /// </summary>
        public static void Abort(Thread thread, object? stateInfo)
        {
            try
            {
                if (stateInfo is null)
                    thread.Abort();
                else
                    thread.Abort(stateInfo);
            }
            catch (ThreadStartException)
            {
                // Although obsolete, this use of Resume() takes care of
                // the odd case where a ThreadStateException is received.
#pragma warning disable CS0618 // Type or member is obsolete
                thread.Resume();
#pragma warning restore CS0618 // Type or member is obsolete
            }
        }
    }
}

#endif
