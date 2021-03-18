// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Interfaces
{
    /// <summary>
    /// AssertionStatus enumeration represents the possible outcomes of an assertion.
    /// The order of definition is significant, higher level values override lower
    /// ones in determining the overall result of a test.
    /// </summary>
    public enum AssertionStatus
    {
        /// <summary>
        /// An assumption failed
        /// </summary>
        Inconclusive,

        /// <summary>
        /// The assertion succeeded
        /// </summary>
        Passed,

        /// <summary>
        /// A warning message was issued
        /// </summary>
        Warning,

        /// <summary>
        /// The assertion failed
        /// </summary>
        Failed,

        /// <summary>
        /// An unexpected exception was thrown
        /// </summary>
        Error
    }
}
