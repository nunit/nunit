// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework
{
    /// <summary>
    /// Specifies the life cycle for a test fixture.
    /// </summary>
    public enum LifeCycle
    {
        /// <summary>
        /// A single instance is created and shared for all test cases.
        /// </summary>
        SingleInstance,

        /// <summary>
        /// A new instance is created for each test case.
        /// </summary>
        InstancePerTestCase
    }
}
