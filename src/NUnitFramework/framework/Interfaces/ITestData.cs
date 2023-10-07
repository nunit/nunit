// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Interfaces
{
    /// <summary>
    /// The ITestData interface is implemented by a class that
    /// represents a single instance of a parameterized test.
    /// </summary>
    public interface ITestData
    {
        /// <summary>
        /// Gets the name to be used for the test
        /// </summary>
        string? TestName { get; }

        /// <summary>
        /// Gets the RunState for this test case.
        /// </summary>
        RunState RunState { get; }

        /// <summary>
        /// Gets the argument list to be provided to the test
        /// </summary>
        object?[] Arguments { get; }

        /// <summary>
        /// Gets the property dictionary for the test case
        /// </summary>
        IPropertyBag Properties { get; }
    }
}
