// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

namespace NUnit.Framework.Interfaces
{
    /// <summary>
    /// The ITestCaseBuilder interface is exposed by a class that knows how to
    /// build a test from a specified method, possibly containing child test cases.
    /// </summary>
    public interface ITestCaseBuilder
    {
        /// <summary>
        /// Examine the method and determine if it is suitable for
        /// this builder to use in building a TestCase to be
        /// included in the suite being populated.
        ///
        /// Note that returning false will cause the method to be ignored
        /// in loading the tests. If it is desired to load the method
        /// but label it as non-runnable, ignored, etc., then this
        /// method must return true.
        /// </summary>
        /// <param name="method">The test method to examine</param>
        /// <param name="suite">The suite being populated</param>
        bool CanBuildFrom(IMethodInfo method, Test? suite);

        /// <summary>
        /// Builds a single test from the specified method and context,
        /// possibly containing child test cases.
        /// </summary>
        /// <param name="method">The method to be used as a test case</param>
        /// <param name="suite">The test suite being populated, or null</param>
        Test? BuildFrom(IMethodInfo method, Test? suite);
    }
}
