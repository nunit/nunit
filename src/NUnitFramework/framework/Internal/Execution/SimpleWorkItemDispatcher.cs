// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading;

namespace NUnit.Framework.Internal.Execution
{
    /// <summary>
    /// SimpleWorkItemDispatcher handles execution of WorkItems by
    /// directly executing them. It is provided so that a dispatcher
    /// is always available in the context, thereby simplifying the
    /// code needed to run child tests.
    /// </summary>
    public class SimpleWorkItemDispatcher : IWorkItemDispatcher
    {
        // The first WorkItem to be dispatched, assumed to be top-level item
        private WorkItem _topLevelWorkItem;

        // Thread used to run and cancel tests
        private Thread _runnerThread;

        #region IWorkItemDispatcher Members

        /// <summary>
        ///  The level of parallelism supported
        /// </summary>
        public int LevelOfParallelism { get { return 0; } }

        /// <summary>
        /// Start execution, creating the execution thread,
        /// setting the top level work and dispatching it.
        /// </summary>
        public void Start(WorkItem topLevelWorkItem)
        {
            _topLevelWorkItem = topLevelWorkItem;
            _runnerThread = new Thread(RunnerThreadProc);

            if (topLevelWorkItem.TargetApartment != ApartmentState.Unknown)
            {
                try
                {
                    _runnerThread.SetApartmentState(topLevelWorkItem.TargetApartment);
                }
                catch (PlatformNotSupportedException)
                {
                    topLevelWorkItem.MarkNotRunnable("Apartment state cannot be set on this platform.");
                }
            }

            _runnerThread.Start();
        }

        /// <summary>
        /// Dispatch a single work item for execution by
        /// executing it directly.
        /// </summary>
        /// <param name="work">The item to dispatch</param>
        public void Dispatch(WorkItem work)
        {
            work?.Execute();
        }

        private void RunnerThreadProc()
        {
            _topLevelWorkItem.Execute();
        }

        private readonly object cancelLock = new object();

        /// <summary>
        /// Cancel (abort or stop) the ongoing run.
        /// If no run is in process, the call has no effect.
        /// </summary>
        /// <param name="force">true if the run should be aborted, false if it should allow its currently running test to complete</param>
        public void CancelRun(bool force)
        {
            lock (cancelLock)
            {
                if (_topLevelWorkItem != null)
                {
                    _topLevelWorkItem.Cancel(force);
                    if (force)
                        _topLevelWorkItem = null;
                }
            }
        }
        #endregion
    }
}
