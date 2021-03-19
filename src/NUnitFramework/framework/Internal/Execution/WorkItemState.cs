// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal.Execution
{
    /// <summary>
    /// The current state of a work item
    /// </summary>
    public enum WorkItemState
    {
        /// <summary>
        /// Ready to run or continue
        /// </summary>
        Ready,

        /// <summary>
        /// Work Item is executing
        /// </summary>
        Running,

        /// <summary>
        /// Complete
        /// </summary>
        Complete
    }
}
