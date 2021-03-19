// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal.Execution
{
    /// <summary>
    /// An IWorkItemDispatcher handles execution of work items.
    /// </summary>
    public interface IWorkItemDispatcher
    {
        /// <summary>
        /// The level of parallelism supported. Zero if not supported.
        /// </summary>
        int LevelOfParallelism { get; }

        /// <summary>
        /// Start execution, performing any initialization. Sets
        /// the top level work item and dispatches it.
        /// </summary>
        void Start(WorkItem topLevelWorkItem);

        /// <summary>
        /// Dispatch a single work item for execution. The first
        /// work item dispatched is saved as the top-level
        /// work item and used when stopping the run.
        /// </summary>
        /// <param name="work">The item to dispatch</param>
        void Dispatch(WorkItem work);

        /// <summary>
        /// Cancel the ongoing run completely.
        /// If no run is in process, the call has no effect.
        /// </summary>
        /// <param name="force">true if the IWorkItemDispatcher should abort all currently running WorkItems, false if it should allow all currently running WorkItems to complete</param>
        void CancelRun(bool force);
    }
}
