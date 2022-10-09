// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework
{
    /// <summary>
    /// The different targets a test action attribute can be applied to
    /// </summary>
    [Flags]
    public enum ActionTargets
    {
        /// <summary>
        /// Default target, which is determined by where the action attribute is attached
        /// </summary>
        Default = 0,

        /// <summary>
        /// Target a individual test case
        /// </summary>
        Test = 1,

        /// <summary>
        /// Target a suite of test cases
        /// </summary>
        Suite = 2
    }
}
