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
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    using Interfaces;
    using Internal.Builders;

    /// <summary>
    /// Adding this attribute to a method within a <seealso cref="TestFixtureAttribute"/> 
    /// class makes the method callable from the NUnit test runner. There is a property 
    /// called Description which is optional which you can provide a more detailed test
    /// description. This class cannot be inherited.
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
    ///   publc void TestDescriptionMethod()
    ///   {}
    /// }
    /// </example>
    /// 
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited=true)]
    public class TheoryAttribute : TestCaseBuilderAttribute, ITestBuilder, IImplyFixture
    {
        private NUnitTestCaseBuilder _builder = new NUnitTestCaseBuilder();
        private IParameterDataProvider _dataProvider = new DatapointProvider();

        #region ITestBuilder Members

        /// <summary>
        /// Construct one or more TestMethods from a given MethodInfo,
        /// using available parameter data.
        /// </summary>
        /// <param name="method">The MethodInfo for which tests are to be constructed.</param>
        /// <param name="suite">The suite to which the tests will be added.</param>
        /// <returns>One or more TestMethods</returns>
        public IEnumerable<TestMethod> BuildFrom(MethodInfo method, Internal.Test suite)
        {
            ParameterInfo[] parameters = method.GetParameters();

            List<TestMethod> tests = new List<TestMethod>();

            if (parameters.Length > 0)
            {
                IEnumerable[] sources = new IEnumerable[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                    sources[i] = _dataProvider.GetDataFor(parameters[i]);

                foreach (var parms in new CombinatorialStrategy().GetTestCases(sources))
                    tests.Add(_builder.BuildTestMethod(method, suite, (ParameterSet)parms));
            }

            return tests;
        }

        #endregion
    }
}