// ***********************************************************************
// Copyright (c) 2008-2019 Charlie Poole, Rob Prouse
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
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;

namespace NUnit.Framework.Attributes
{
    /// <summary>
    /// Marks a method as a parameterized test suite and provides arguments for each test case.
    /// Generic type parameters
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TestCaseGenericAttribute : TestCaseAttribute, ITestBuilder
    {
        #region Constructors

        /// <summary>
        /// Construct a TestCaseGenericAttribute with a list of arguments.
        /// This constructor is not CLS-Compliant
        /// </summary>
        /// <param name="arguments"></param>
        public TestCaseGenericAttribute(params object[] arguments)
            : base(arguments)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the generic argument list to be provided to the test
        /// </summary>
        public Type[] TypeArguments { get; set; }

        #endregion

        #region ITestBuilder Members

        /// <summary>
        /// Builds a single test from the specified method and context.
        /// </summary>
        /// <param name="method">The MethodInfo for which tests are to be constructed.</param>
        /// <param name="suite">The suite to which the tests will be added.</param>
        IEnumerable<TestMethod> ITestBuilder.BuildFrom(IMethodInfo method, Test suite)
        {
            if (!method.IsGenericMethodDefinition)
            {
                return BuildFrom(method, suite);
            }

            if (TypeArguments == null || TypeArguments.Length != method.GetGenericArguments().Length)
            {
                var parms = new TestCaseParameters { RunState = RunState.NotRunnable };
                parms.Properties.Set("_SKIPREASON", $"{nameof(TypeArguments)} should have {method.GetGenericArguments().Length} elements");
                return new[] { new NUnitTestCaseBuilder().BuildTestMethod(method, suite, parms) };
            }

            var genMethod = method.MakeGenericMethod(TypeArguments);
            return BuildFrom(genMethod, suite);
        }

        #endregion
    }
}
