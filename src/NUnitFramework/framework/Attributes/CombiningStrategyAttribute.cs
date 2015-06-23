// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
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
    public abstract class CombiningStrategyAttribute : TestCaseBuilderAttribute, ITestBuilder, IApplyToTest
    {
        private NUnitTestCaseBuilder _builder = new NUnitTestCaseBuilder();
        private IParameterDataProvider _dataProvider = new ParameterDataProvider();

        private ICombiningStrategy _strategy;

        /// <summary>
        /// Construct a CombiningStrategyAttribute incorporating an object
        /// that implements ICombiningStrategy.
        /// </summary>
        /// <param name="strategy">Combining strategy to be used</param>
        protected CombiningStrategyAttribute(ICombiningStrategy strategy)
        {
            _strategy = strategy;
        }

        /// <summary>
        /// Construct a CombiningStrategyAttribute incorporating an object
        /// that implements ICombiningStrategy. This constructor is provided
        /// for CLS compliance.
        /// </summary>
        /// <param name="strategy">Combining strategy to be used</param>
        protected CombiningStrategyAttribute(object strategy)
            : this((ICombiningStrategy)strategy)
        {
        }

        #region ITestBuilder Members

        /// <summary>
        /// Construct one or more TestMethods from a given MethodInfo,
        /// using available parameter data.
        /// </summary>
        /// <param name="fixtureType">The parameter containing type of the test fixture class. 
        /// This may be different from the reflected member info</param>
        /// <param name="method">The MethodInfo for which tests are to be constructed.</param>
        /// <param name="suite">The suite to which the tests will be added.</param>
        /// <returns>One or more TestMethods</returns>
        public IEnumerable<TestMethod> BuildFrom(Type fixtureType, MethodInfo method, Test suite)
        {
            List<TestMethod> tests = new List<TestMethod>();

#if NETCF
            if (method.ContainsGenericParameters)
            {
                var genericParams = method.GetGenericArguments();
                var numGenericParams = genericParams.Length;

                var o = new object();
                var tryArgs = Enumerable.Repeat(o, numGenericParams).ToArray();

                var mi = method.MakeGenericMethodEx(tryArgs);
                if (mi == null)
                    return tests;

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
                        tests.Add(_builder.BuildTestMethod(mi, suite, (ParameterSet)parms));
                }

                return tests;
            }
#endif
            ParameterInfo[] parameters = method.GetParameters();

            if (parameters.Length > 0)
            {
                IEnumerable[] sources = new IEnumerable[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                    sources[i] = _dataProvider.GetDataFor(fixtureType, parameters[i]);

                foreach (var parms in _strategy.GetTestCases(sources))
                    tests.Add(_builder.BuildTestMethod(fixtureType, method, suite, (ParameterSet)parms));
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
