// ***********************************************************************
// Copyright (c) 2008-2015 Charlie Poole
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
using NUnit.Compatibility;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Builders;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Attributes
{
    /// <summary>
    /// TestCaseSourceAttribute indicates the source to be used to
    /// provide test cases for a test method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public abstract class BaseTestCaseSourceAttribute : NUnitAttribute, ITestBuilder, IImplyFixture
    {
        private NUnitTestCaseBuilder _builder = new NUnitTestCaseBuilder();

        #region Properties
        /// <summary>
        /// A set of parameters passed to the method, works only if the Source Name is a method. 
        /// If the source name is a field or property has no effect.
        /// </summary>
        public object[] MethodParams { get; protected set; }
        /// <summary>
        /// The name of a the method, property or fiend to be used as a source
        /// </summary>
        public string SourceName { get; protected set; }

        /// <summary>
        /// A Type to be used as a source
        /// </summary>
        public Type SourceType { get; protected set; }

        /// <summary>
        /// Gets or sets the category associated with every fixture created from
        /// this attribute. May be a single category or a comma-separated list.
        /// </summary>
        public string Category { get; set; }

        #endregion

        #region ITestBuilder Members

        /// <summary>
        /// Construct one or more TestMethods from a given MethodInfo,
        /// using available parameter data.
        /// </summary>
        /// <param name="method">The IMethod for which tests are to be constructed.</param>
        /// <param name="suite">The suite to which the tests will be added.</param>
        /// <returns>One or more TestMethods</returns>
        public IEnumerable<TestMethod> BuildFrom(IMethodInfo method, Test suite)
        {
            foreach (TestCaseParameters parms in GetTestCasesFor(method))
                yield return _builder.BuildTestMethod(method, suite, parms);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Returns a set of ITestCaseDataItems for use as arguments
        /// to a parameterized test method.
        /// </summary>
        /// <param name="method">The method for which data is needed.</param>
        /// <returns></returns>
        private IEnumerable<ITestCaseData> GetTestCasesFor(IMethodInfo method)
        {
            List<ITestCaseData> data = new List<ITestCaseData>();

            try
            {
                IEnumerable source = GetTestCaseSource(method);

                if (source != null)
                {
                    foreach (object item in source)
                    {
                        // First handle two easy cases:
                        // 1. Source is null. This is really an error but if we
                        //    throw an exception we simply get an invalid fixture
                        //    without good info as to what caused it. Passing a
                        //    single null argument will cause an error to be 
                        //    reported at the test level, in most cases.
                        // 2. User provided an ITestCaseData and we just use it.
                        ITestCaseData parms = item == null
                            ? new TestCaseParameters(new object[] { null })
                            : item as ITestCaseData;

                        if (parms == null)
                        {
                            // 3. An array was passed, it may be an object[]
                            //    or possibly some other kind of array, which
                            //    TestCaseSource can accept.
                            var args = item as object[];
                            if (args == null && item is Array)
                            {
                                Array array = item as Array;
#if NETCF
                                bool netcfOpenType = method.IsGenericMethodDefinition;
#else
                                bool netcfOpenType = false;
#endif
                                int numParameters = netcfOpenType ? array.Length : method.GetParameters().Length;
                                if (array != null && array.Rank == 1 && array.Length == numParameters)
                                {
                                    // Array is something like int[] - convert it to
                                    // an object[] for use as the argument array.
                                    args = new object[array.Length];
                                    for (int i = 0; i < array.Length; i++)
                                        args[i] = array.GetValue(i);
                                }
                            }

                            // Check again if we have an object[]
                            if (args != null)
                            {
#if NETCF
                                if (method.IsGenericMethodDefinition)
                                {
                                    var mi = method.MakeGenericMethodEx(args);
                                    if (mi == null)
                                        throw new NotSupportedException("Cannot determine generic Type");
                                    method = mi;
                                }
#endif

                                var parameters = method.GetParameters();
                                var argsNeeded = parameters.Length;
                                var argsProvided = args.Length;

                                // If only one argument is needed, our array may actually
                                // be the bare argument. If it is, we should wrap it in
                                // an outer object[] representing the list of arguments.
                                if (argsNeeded == 1)
                                {
                                    var singleParmType = parameters[0].ParameterType;

                                    if (argsProvided == 0 || typeof(object[]).IsAssignableFrom(singleParmType))
                                    {
                                        if (argsProvided > 1 || singleParmType.IsAssignableFrom(args.GetType()))
                                        {
                                            args = new object[] { item };
                                        }
                                    }
                                }
                            }
                            else // It may be a scalar or a multi-dimensioned array. Wrap it in object[]
                            {
                                args = new object[] { item };
                            }

                            parms = new TestCaseParameters(args);
                        }

                        if (this.Category != null)
                            foreach (string cat in this.Category.Split(new char[] { ',' }))
                                parms.Properties.Add(PropertyNames.Category, cat);

                        data.Add(parms);
                    }
                }
            }
            catch (Exception ex)
            {
                data.Clear();
                data.Add(new TestCaseParameters(ex));
            }

            return data;
        }

        /// <summary>
        /// Return 
        /// </summary>
        /// <param name="errorMessage">message to return indicating reason why a testcase cannot be executed.</param>
        /// <returns></returns>
        public IEnumerable ReturnErrorAsParameter(string errorMessage)
        {
            var parms = new TestCaseParameters();
            parms.RunState = RunState.NotRunnable;
            parms.Properties.Set(PropertyNames.SkipReason, errorMessage);
            return new TestCaseParameters[] { parms };
        }

        /// <summary>
        /// Retrieves the method's signature where the source data's fields will be injected
        /// </summary>
        /// <param name="methodInfo">name of the method to be analyzed.</param>
        /// <returns>List of test cases based on signature and data</returns>
        protected abstract IEnumerable GetTestCaseSource(IMethodInfo methodInfo);

        #endregion

    }


}