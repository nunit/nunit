// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal.Execution
{
    /// <summary>
    /// Enumeration representing the strategy to follow in executing a work item.
    /// The value is only relevant when running under the parallel dispatcher.
    /// </summary>
    public enum ParallelExecutionStrategy
    {
        /// <summary>
        /// Run directly on same thread
        /// </summary>
        Direct,

        /// <summary>
        /// Enqueue for parallel execution
        /// </summary>
        Parallel,

        /// <summary>
        /// Enqueue for non-parallel execution
        /// </summary>
        NonParallel,
    }
}
