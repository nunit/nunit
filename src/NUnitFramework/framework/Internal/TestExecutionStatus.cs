// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Enumeration indicating whether the tests are
    /// running normally or being cancelled.
    /// </summary>
    public enum TestExecutionStatus
    {
        /// <summary>
        /// Running normally with no stop requested
        /// </summary>
        Running,

        /// <summary>
        /// A graceful stop has been requested
        /// </summary>
        StopRequested,

        /// <summary>
        /// A forced stop has been requested
        /// </summary>
        AbortRequested
    }
}
