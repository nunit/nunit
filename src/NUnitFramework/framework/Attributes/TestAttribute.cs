// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;

namespace NUnit.Framework
{
    /// <summary>
    /// Marks the method as callable from the NUnit test runner.
    /// </summary>
    ///
    /// <example>
    /// [TestFixture]
    /// public class Fixture
    /// {
    ///   [Test]
    ///   public void MethodToTest()
    ///   {}
    ///
    ///   [Test(Description = "more detailed description")]
    ///   public void TestDescriptionMethod()
    ///   {}
    /// }
    /// </example>
    ///
    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false, Inherited=true)]
    public class TestAttribute : NUnitAttribute, ISimpleTestBuilder, IApplyToTest, IImplyFixture
    {
        private object? _expectedResult;
        private bool _hasExpectedResult = false; // needed in case result is set to null

        private readonly NUnitTestCaseBuilder _builder = new NUnitTestCaseBuilder();

        /// <summary>
        /// Descriptive text for this test
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// The author of this test
        /// </summary>
        public string? Author { get; set; }

        /// <summary>
        /// The type that this test is testing
        /// </summary>
        public Type? TestOf { get; set; }

        /// <summary>
        /// Gets or sets the expected result. Not valid if the test
        /// method has parameters.
        /// </summary>
        /// <value>The result.</value>
        public object? ExpectedResult
        {
            get { return _expectedResult; }
            set
            {
                _expectedResult = value;
                _hasExpectedResult = true;
            }
        }

        #region IApplyToTest Members

        /// <summary>
        /// Modifies a test by adding a description, if not already set.
        /// </summary>
        /// <param name="test">The test to modify</param>
        public void ApplyToTest(Test test)
        {
            Guard.ArgumentValid(test.Method is object, "This attribute must only be applied to tests that have an associated method.", nameof(test));

            if (!test.Properties.ContainsKey(PropertyNames.Description) && Description != null)
                test.Properties.Set(PropertyNames.Description, Description);

            if (!test.Properties.ContainsKey(PropertyNames.Author) && Author != null)
                test.Properties.Set(PropertyNames.Author, Author);

            if (!test.Properties.ContainsKey(PropertyNames.TestOf) && TestOf != null)
                test.Properties.Set(PropertyNames.TestOf, TestOf.FullName!);

            if (_hasExpectedResult && test.Method.GetParameters().Length > 0)
                test.MakeInvalid("The 'TestAttribute.ExpectedResult' property may not be used on parameterized methods.");

        }

        #endregion

        #region ISimpleTestBuilder Members

        /// <summary>
        /// Builds a single test from the specified method and context.
        /// </summary>
        /// <param name="method">The method for which a test is to be constructed.</param>
        /// <param name="suite">The suite to which the test will be added.</param>
        public TestMethod BuildFrom(IMethodInfo method, Test? suite)
        {
            TestCaseParameters? parms = null;

            if (_hasExpectedResult)
            {
                parms = new TestCaseParameters();
                parms.ExpectedResult = ExpectedResult;
            }

            return _builder.BuildTestMethod(method, suite, parms);
        }

        #endregion
    }
}
