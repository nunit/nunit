// ***********************************************************************
// Copyright (c) 2014-2015 Charlie Poole
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
#if NETCF
using System.Linq;
#endif

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
        private NUnitTestCaseBuilder _builder = new NUnitTestCaseBuilder();

        private ICombiningStrategy _strategy;
        private IParameterDataProvider _dataProvider;

        /// <summary>
        /// Construct a CombiningStrategyAttribute incorporating an
        /// ICombiningStrategy and an IParamterDataProvider.
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
        /// Construct one or more TestMethods from a given MethodInfo,
        /// using available parameter data.
        /// </summary>
        /// <param name="method">The MethodInfo for which tests are to be constructed.</param>
        /// <param name="suite">The suite to which the tests will be added.</param>
        /// <returns>One or more TestMethods</returns>
        public IEnumerable<TestMethod> BuildFrom(IMethodInfo method, Test suite)
        {
            List<TestMethod> tests = new List<TestMethod>();

#if NETCF
            if (method.ContainsGenericParameters)
            {
                var genericParams = method.GetGenericArguments();
                var numGenericParams = genericParams.Length;

                var o = new object();
                var tryArgs = Enumerable.Repeat(o, numGenericParams).ToArray();
                IMethodInfo mi;

                try
                {
                    // This fails if the generic method has constraints
                    // that are not met by object.
                    mi = method.MakeGenericMethodEx(tryArgs);
                    if (mi == null)
                        return tests;
                }
                catch
                {
                    return tests;
                }

                var par = mi.GetParameters();

                if (par.Length == 0)
                    return tests;

                var sourceData = par.Select(p => _dataProvider.GetDataFor(p)).ToArray();
                foreach (var parms in _strategy.GetTestCases(sourceData))
                {
                    mi = method.MakeGenericMethodEx(parms.Arguments);
                    if (mi == null)
                    {
                        var tm = new TestMethod(method, suite);
                        tm.RunState = RunState.NotRunnable;
                        tm.Properties.Set(PropertyNames.SkipReason, "Incompatible arguments");
                        tests.Add(tm);
                    }
                    else
                        tests.Add(_builder.BuildTestMethod(mi, suite, (TestCaseParameters)parms));
                }

                return tests;
            }
#endif
            IParameterInfo[] parameters = method.GetParameters();

            if (parameters.Length > 0)
            {
                IEnumerable[] sources = new IEnumerable[parameters.Length];

                try
                {
                    for (int i = 0; i < parameters.Length; i++)
                        sources[i] = _dataProvider.GetDataFor(parameters[i]);
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
