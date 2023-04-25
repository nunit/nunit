// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;

namespace NUnit.Framework
{
    using Interfaces;
    using Internal;
    using Internal.Builders;

    /// <summary>
    /// Marks a test as using a particular CombiningStrategy to join any supplied parameter data.
    /// Since this is the default, the attribute is optional.
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
        /// <param name="method">The MethodInfo for which tests are to be constructed.</param>
        /// <param name="suite">The suite to which the tests will be added.</param>
        public IEnumerable<TestMethod> BuildFrom(IMethodInfo method, Test? suite)
        {
            List<TestMethod> tests = new List<TestMethod>();

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
            if (joinType.EndsWith("Strategy", StringComparison.Ordinal))
                joinType = joinType.Substring(0, joinType.Length - 8);

            test.Properties.Set(PropertyNames.JoinType, joinType);
        }

        #endregion
    }
}
