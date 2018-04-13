// ***********************************************************************
// Copyright (c) 2014-2018 Charlie Poole, Rob Prouse
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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace NUnit.Framework
{
    using Interfaces;
    using Internal;
    using Internal.Builders;

    /// <summary>
    /// Marks a test to use a particular CombiningStrategy to join
    /// any parameter data provided. Since this is the default, the
    /// attribute is optional.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public abstract class CombiningStrategyAttribute : NUnitAttribute, ITestBuilder, IApplyToTest
    {
        private readonly NUnitTestCaseBuilder _builder = new NUnitTestCaseBuilder();

        private readonly ICombiningStrategy _strategy;
        private readonly IParameterDataProvider _dataProvider;

        /// <summary>
        /// Construct a CombiningStrategyAttribute incorporating an
        /// ICombiningStrategy and an IParameterDataProvider.
        /// </summary>
        /// <param name="strategy">Combining strategy to be used in combining data</param>
        /// <param name="provider">An IParameterDataProvider to supply data</param>
        protected CombiningStrategyAttribute(ICombiningStrategy strategy, IParameterDataProvider provider)
        {
            _strategy = strategy;
            _dataProvider = provider;
        }

        /// <summary>
        /// Construct a CombiningStrategyAttribute incorporating an object
        /// that implements ICombiningStrategy and an IParameterDataProvider.
        /// This constructor is provided for CLS compliance.
        /// </summary>
        /// <param name="strategy">Combining strategy to be used in combining data</param>
        /// <param name="provider">An IParameterDataProvider to supply data</param>
        protected CombiningStrategyAttribute(object strategy, object provider)
            : this((ICombiningStrategy)strategy, (IParameterDataProvider)provider)
        {
        }

        #region ITestBuilder Members

        /// <summary>
        /// Builds any number of tests from the specified method and context.
        /// </summary>
        /// <param name="method">The method to be used as a test.</param>
        /// <param name="suite">The parent to which the test will be added.</param>
        public IEnumerable<TestMethod> BuildFrom(FixtureMethod method, Test suite)
        {
            List<TestMethod> tests = new List<TestMethod>();

            ParameterInfo[] parameters = method.Method.GetParameters();

            if (parameters.Length > 0)
            {
                IEnumerable[] sources = new IEnumerable[parameters.Length];

                try
                {
                    for (int i = 0; i < parameters.Length; i++)
                        sources[i] = _dataProvider.GetDataFor(method.FixtureType, parameters[i]);
                }
                catch (InvalidDataSourceException ex)
                {
                    var parms = new TestCaseParameters();
                    parms.RunState = RunState.NotRunnable;
                    parms.Properties.Set(PropertyNames.SkipReason, ex.Message);
                    tests.Add(_builder.BuildTestMethod(method, suite, parms));
                    return tests;
                }

                foreach (var parms in _strategy.GetTestCases(sources))
                    tests.Add(_builder.BuildTestMethod(method, suite, (TestCaseParameters)parms));
            }

            return tests;
        }

        #endregion

        #region IApplyToTest Members

        /// <summary>
        /// Modify the test by adding the name of the combining strategy
        /// to the properties.
        /// </summary>
        /// <param name="test">The test to modify</param>
        public void ApplyToTest(Test test)
        {
            var joinType = _strategy.GetType().Name;
            if (joinType.EndsWith("Strategy"))
                joinType = joinType.Substring(0, joinType.Length - 8);

            test.Properties.Set(PropertyNames.JoinType, joinType);
        }

        #endregion
    }
}
