// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Interfaces
{
    /// <summary>
    /// The ITestCaseData interface is implemented by a class
    /// that is able to return complete test cases for use by
    /// a parameterized test method.
    /// </summary>
    public interface ITestCaseData : ITestData
    {
        /// <summary>
        /// Gets the expected result of the test case
        /// </summary>
        object? ExpectedResult { get; }

        /// <summary>
        /// Returns true if an expected result has been set
        /// </summary>
        bool HasExpectedResult { get; }
    }
}
