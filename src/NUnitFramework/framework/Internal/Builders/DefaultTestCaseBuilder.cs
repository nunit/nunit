// ***********************************************************************
// Copyright (c) 2008-2014 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Commands;

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
        private NUnitTestCaseBuilder _nunitTestCaseBuilder = new NUnitTestCaseBuilder();

        #region ITestCaseBuilder Methods
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
        /// <returns>True if the builder can create a test case from this method</returns>
        public bool CanBuildFrom(IMethodInfo method)
        {
            return method.IsDefined<ITestBuilder>(false)
                || method.IsDefined<ISimpleTestBuilder>(false);
        }

        /// <summary>
        /// Build a Test from the provided MethodInfo. Depending on
        /// whether the method takes arguments and on the availability
        /// of test case data, this method may return a single test
        /// or a group of tests contained in a ParameterizedMethodSuite.
        /// </summary>
        /// <param name="method">The method for which a test is to be built</param>
        /// <returns>A Test representing one or more method invocations</returns>
        public Test BuildFrom(IMethodInfo method)
        {
            return BuildFrom(method, null);
        }

        #endregion

        #region ITestCaseBuilder2 Members

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
        /// <returns>True if the builder can create a test case from this method</returns>
        public bool CanBuildFrom(IMethodInfo method, Test parentSuite)
        {
            return CanBuildFrom(method);
        }

        /// <summary>
        /// Build a Test from the provided MethodInfo. Depending on
        /// whether the method takes arguments and on the availability
        /// of test case data, this method may return a single test
        /// or a group of tests contained in a ParameterizedMethodSuite.
        /// </summary>
        /// <param name="method">The method for which a test is to be built</param>
        /// <param name="parentSuite">The test fixture being populated, or null</param>
        /// <returns>A Test representing one or more method invocations</returns>
        public Test BuildFrom(IMethodInfo method, Test parentSuite)
        {
            var tests = new List<TestMethod>();

            List<ITestBuilder> builders = new List<ITestBuilder>(
                method.GetCustomAttributes<ITestBuilder>(false));

            // See if we need a CombinatorialAttribute added
            bool needCombinatorial = true;
            foreach (var attr in builders)
            {
                if (attr is CombiningStrategyAttribute)
                    needCombinatorial = false;
            }

            // We could check to see if here are any data attributes specified
            // on the parameters but that's what CombinatorialAttribute does
            // and it simply won't return any cases if it finds nothing.
            // TODO: We need to add some other ITestBuilder than a combinatorial attribute
            // because we want the attribute to generate an error if it's present on
            // a generic method.
            if (needCombinatorial)
                builders.Add(new CombinatorialAttribute());

            foreach (var attr in builders)
            {
                foreach (var test in attr.BuildFrom(method, parentSuite))
                    tests.Add(test);
            }

            return tests.Count > 0
                ? BuildParameterizedMethodSuite(method, tests)
                : BuildSingleTestMethod(method, parentSuite);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Builds a ParameterizedMethodSuite containing individual test cases.
        /// </summary>
        /// <param name="method">The method for which a test is to be built.</param>
        /// <param name="tests">The list of test cases to include.</param>
        /// <returns>A ParameterizedMethodSuite populated with test cases</returns>
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
        /// <returns>A TestMethod.</returns>
        private Test BuildSingleTestMethod(IMethodInfo method, Test suite)
        {
            var builders = method.GetCustomAttributes<ISimpleTestBuilder>(false);
            return builders.Length > 0
                ? builders[0].BuildFrom(method, suite)
                : _nunitTestCaseBuilder.BuildTestMethod(method, suite, null);
        }

        #endregion
    }
}
