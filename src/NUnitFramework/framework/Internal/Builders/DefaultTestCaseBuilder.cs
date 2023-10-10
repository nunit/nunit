// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Builders
{
    /// <summary>
    /// Class to build ether a parameterized or a normal NUnitTestMethod.
    /// There are four cases that the builder must deal with:
    ///   1. The method needs no params and none are provided
    ///   2. The method needs params and they are provided
    ///   3. The method needs no params but they are provided in error
    ///   4. The method needs params but they are not provided
    /// This could have been done using two different builders, but it
    /// turned out to be simpler to have just one. The BuildFrom method
    /// takes a different branch depending on whether any parameters are
    /// provided, but all four cases are dealt with in lower-level methods
    /// </summary>
    public class DefaultTestCaseBuilder : ITestCaseBuilder
    {
        private readonly NUnitTestCaseBuilder _nunitTestCaseBuilder = new();

        /// <summary>
        /// Determines if the method can be used to build an NUnit test
        /// test method of some kind. The method must normally be marked
        /// with an identifying attribute for this to be true.
        ///
        /// Note that this method does not check that the signature
        /// of the method for validity. If we did that here, any
        /// test methods with invalid signatures would be passed
        /// over in silence in the test run. Since we want such
        /// methods to be reported, the check for validity is made
        /// in BuildFrom rather than here.
        /// </summary>
        /// <param name="method">An IMethodInfo for the method being used as a test method</param>
        public bool CanBuildFrom(IMethodInfo method)
        {
            return method.IsDefined<ITestBuilder>(true)
                || method.IsDefined<ISimpleTestBuilder>(true);
        }

        /// <summary>
        /// Builds a single test from the specified method and context,
        /// possibly containing child test cases.
        /// </summary>
        /// <param name="method">The method for which a test is to be built</param>
        public Test BuildFrom(IMethodInfo method)
        {
            return BuildFrom(method, null);
        }

        #region ITestCaseBuilder Members

        /// <summary>
        /// Determines if the method can be used to build an NUnit test
        /// test method of some kind. The method must normally be marked
        /// with an identifying attribute for this to be true.
        ///
        /// Note that this method does not check that the signature
        /// of the method for validity. If we did that here, any
        /// test methods with invalid signatures would be passed
        /// over in silence in the test run. Since we want such
        /// methods to be reported, the check for validity is made
        /// in BuildFrom rather than here.
        /// </summary>
        /// <param name="method">An IMethodInfo for the method being used as a test method</param>
        /// <param name="parentSuite">The test suite being built, to which the new test would be added</param>
        public bool CanBuildFrom(IMethodInfo method, Test? parentSuite)
        {
            return CanBuildFrom(method);
        }

        /// <summary>
        /// Builds a single test from the specified method and context,
        /// possibly containing child test cases.
        /// </summary>
        /// <param name="method">The method for which a test is to be built</param>
        /// <param name="parentSuite">The test fixture being populated, or null</param>
        public Test BuildFrom(IMethodInfo method, Test? parentSuite)
        {
            var tests = new List<TestMethod>();

            try
            {
                var metadata = MethodInfoCache.Get(method);

                List<ITestBuilder> builders = new(metadata.TestBuilderAttributes);

                // See if we need to add a CombinatorialAttribute for parameterized data
                if (method.MethodInfo.GetParameters().Any(param => param.HasAttribute<IParameterDataSource>(false))
                    && !builders.Any(builder => builder is CombiningStrategyAttribute))
                {
                    builders.Add(new CombinatorialAttribute());
                }

                foreach (var attr in builders)
                {
                    foreach (var test in attr.BuildFrom(method, parentSuite))
                        tests.Add(test);
                }

                return builders.Count > 0 && method.GetParameters().Length > 0 || tests.Count > 0
                    ? BuildParameterizedMethodSuite(method, tests)
                    : BuildSingleTestMethod(method, parentSuite);
            }
            catch (Exception ex)
            {
                var testMethod = new TestMethod(method, parentSuite);
                testMethod.MakeInvalid(ex, "Failure building Test");
                return testMethod;
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Builds a ParameterizedMethodSuite containing individual test cases.
        /// </summary>
        /// <param name="method">The method for which a test is to be built.</param>
        /// <param name="tests">The list of test cases to include.</param>
        private Test BuildParameterizedMethodSuite(IMethodInfo method, IEnumerable<TestMethod> tests)
        {
            ParameterizedMethodSuite methodSuite = new ParameterizedMethodSuite(method);
            methodSuite.ApplyAttributesToTest(method.MethodInfo);

            foreach (TestMethod test in tests)
                methodSuite.Add(test);

            return methodSuite;
        }

        /// <summary>
        /// Build a simple, non-parameterized TestMethod for this method.
        /// </summary>
        /// <param name="method">The MethodInfo for which a test is to be built</param>
        /// <param name="suite">The test suite for which the method is being built</param>
        private Test BuildSingleTestMethod(IMethodInfo method, Test? suite)
        {
            var builders = method.GetCustomAttributes<ISimpleTestBuilder>(false);
            return builders.Length > 0
                ? builders[0].BuildFrom(method, suite)
                : _nunitTestCaseBuilder.BuildTestMethod(method, suite, null);
        }

        #endregion
    }
}
