// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Internal.Execution
{
    /// <summary>
    /// MainThreadWorkItemDispatcher handles execution of WorkItems by
    /// directly executing them on the main thread. This is different
    /// from the SimpleWorkItemDispatcher where the work item is dispatched
    /// onto its own thread.
    /// </summary>
    public class MainThreadWorkItemDispatcher : IWorkItemDispatcher
    {

        #region IWorkItemDispatcher Members

        /// <summary>
        ///  The level of parallelism supported
        /// </summary>
        public int LevelOfParallelism => 0;

        /// <summary>
        /// Start execution, dispatching the top level
        /// work into the main thread.
        /// </summary>
        public void Start(WorkItem topLevelWorkItem)
        {
            Dispatch(topLevelWorkItem);
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

        /// <summary>
        /// This method is not supported for 
        /// this dispatcher. Using it will throw a
        /// NotSupportedException.
        /// </summary>
        /// <param name="force">Not used</param>
        /// <exception cref="System.NotSupportedException">If used, it will always throw this.</exception>
        public void CancelRun(bool force)
        {
            throw new NotSupportedException();
        }
        #endregion
    }
}
