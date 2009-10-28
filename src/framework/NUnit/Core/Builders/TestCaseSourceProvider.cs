// ***********************************************************************
// Copyright (c) 2008 Charlie Poole
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
using System.Reflection;
using NUnit.Core.Extensibility;
using NUnit.Framework;

namespace NUnit.Core.Builders
{
    /// <summary>
    /// TestCaseSourceProvider provides data for methods
    /// annotated with the TestCaseSourceAttribute.
    /// </summary>
    public class TestCaseSourceProvider : ITestCaseProvider2
    {
        #region ITestCaseProvider Members

        /// <summary>
        /// Determine whether any test cases are available for a parameterized method.
        /// </summary>
        /// <param name="method">A MethodInfo representing a parameterized test</param>
        /// <returns>True if any cases are available, otherwise false.</returns>
        public bool HasTestCasesFor(MethodInfo method)
        {
            return method.IsDefined(typeof(TestCaseSourceAttribute), false);
        }

        /// <summary>
        /// Return an IEnumerable providing test cases for use in
        /// running a parameterized test.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public IEnumerable GetTestCasesFor(MethodInfo method)
        {
            return GetTestCasesFor(method, null);
        }
        #endregion

        #region ITestCaseProvider2 Members

        /// <summary>
        /// Determine whether any test cases are available for a parameterized method.
        /// </summary>
        /// <param name="method">A MethodInfo representing a parameterized test</param>
        /// <returns>True if any cases are available, otherwise false.</returns>
        public bool HasTestCasesFor(MethodInfo method, Test suite)
        {
            return HasTestCasesFor(method);
        }

        /// <summary>
        /// Return an IEnumerable providing test cases for use in
        /// running a parameterized test.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public IEnumerable GetTestCasesFor(MethodInfo method, Test parentSuite)
        {
            ArrayList parameterList = new ArrayList();

            foreach (ProviderReference info in GetSourcesFor(method, parentSuite))
            {
                foreach (object o in info.GetInstance())
                    parameterList.Add(o);
            }

            return parameterList;
        }
        #endregion

        #region Helper Methods
        private static IList GetSourcesFor(MethodInfo method, Test parent)
        {
            ArrayList sources = new ArrayList();
            TestFixture parentSuite = parent as TestFixture;

            foreach (TestCaseSourceAttribute sourceAttr in method.GetCustomAttributes(typeof(TestCaseSourceAttribute), false))
            {
                Type sourceType = sourceAttr.SourceType;
                string sourceName = sourceAttr.SourceName;

                if (sourceType == null)
                {
                    if (parentSuite != null)
                        sources.Add(new ProviderReference(parentSuite.FixtureType, parentSuite.arguments, sourceName));
                    else
                        sources.Add(new ProviderReference(method.ReflectedType, sourceName));
                }
                else
                {
                    sources.Add(new ProviderReference(sourceType, sourceName));
                }

            }
            return sources;
        }
        #endregion
    }
}
