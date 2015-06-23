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
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;

namespace NUnit.Framework
{
    /// <summary>
    /// TestCaseSourceAttribute indicates the source to be used to
    /// provide test cases for a test method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class TestCaseSourceAttribute : TestCaseBuilderAttribute, ITestBuilder, IImplyFixture
    {
        private readonly object[] _sourceConstructorParameters;

        private NUnitTestCaseBuilder _builder = new NUnitTestCaseBuilder();

        /// <summary>
        /// Construct with the name of the method, property or field that will provide data
        /// </summary>
        /// <param name="sourceName">The name of the method, property or field that will provide data</param>
        public TestCaseSourceAttribute(string sourceName)
        {
            this.SourceName = sourceName;
        }

        /// <summary>
        /// Construct with a Type and name
        /// </summary>
        /// <param name="sourceType">The Type that will provide data</param>
        /// <param name="sourceName">The name of the method, property or field that will provide data</param>
        /// <param name="constructorParameters">The constructor parameters to be used when instantiating the sourceType.</param>
        public TestCaseSourceAttribute(Type sourceType, string sourceName, params object[] constructorParameters)
        {
            this.SourceType = sourceType;
            this.SourceName = sourceName;
            _sourceConstructorParameters = constructorParameters;
        }

        /// <summary>
        /// Construct with a Type
        /// </summary>
        /// <param name="sourceType">The type that will provide data</param>
        public TestCaseSourceAttribute(Type sourceType)
        {
            this.SourceType = sourceType;
        }

        /// <summary>
        /// The name of a the method, property or fiend to be used as a source
        /// </summary>
        public string SourceName { get; private set; }

        /// <summary>
        /// A Type to be used as a source
        /// </summary>
        public Type SourceType { get; private set; }

        /// <summary>
        /// Gets or sets the category associated with this test.
        /// May be a single category or a comma-separated list.
        /// </summary>
        public string Category { get; set; }

        #region ITestCaseSource Members
        /// <summary>
        /// Returns a set of ITestCaseDataItems for use as arguments
        /// to a parameterized test method.
        /// </summary>
        /// <param name="fixtureType">The parameter containing type of the test fixture class. 
        /// This may be different from the reflected member info</param>
        /// <param name="method">The method for which data is needed.</param>
        /// <returns></returns>
        public IEnumerable<ITestCaseData> GetTestCasesFor(Type fixtureType, MethodInfo method)
        {
            List<ITestCaseData> data = new List<ITestCaseData>();
            IEnumerable source = GetTestCaseSource(fixtureType);

            if (source != null)
            {
                try
                {
#if NETCF
                    ParameterInfo[] parameters = method.IsGenericMethodDefinition ? new ParameterInfo[0] : method.GetParameters();
#else
                    ParameterInfo[] parameters = method.GetParameters();
#endif

                    foreach (object item in source)
                    {
                        ParameterSet parms;
                        ITestCaseData testCaseData = item as ITestCaseData;

                        if (testCaseData != null)
                            parms = new ParameterSet(testCaseData);
                        else
                        {
                            object[] args = item as object[];
                            if (args != null)
                            {
#if NETCF
                                if (method.IsGenericMethodDefinition)
                                {
                                    var mi = method.MakeGenericMethodEx(args);
                                    parameters = mi == null ? new ParameterInfo[0] : mi.GetParameters();
                                }
#endif
                                if (args.Length != parameters.Length)
                                    args = new object[] { item };
                            }
                            // else if (parameters.Length == 1 && parameters[0].ParameterType.IsAssignableFrom(item.GetType()))
                            // {
                            //    args = new object[] { item };
                            // }
                            else if (item is Array)
                            {
                                Array array = item as Array;

#if NETCF
                                if (array.Rank == 1 && (method.IsGenericMethodDefinition || array.Length == parameters.Length))
#else
                                if (array.Rank == 1 && array.Length == parameters.Length)
#endif
                                {
                                    args = new object[array.Length];
                                    for (int i = 0; i < array.Length; i++)
                                        args[i] = array.GetValue(i);
#if NETCF
                                    if (method.IsGenericMethodDefinition)
                                    {
                                        var mi = method.MakeGenericMethodEx(args);

                                        if (mi == null || array.Length != mi.GetParameters().Length)
                                            args = new object[] {item};
                                    }
#endif
                                }
                                else
                                {
                                    args = new object[] { item };
                                }
                            }
                            else
                            {
                                args = new object[] { item };
                            }

                            parms = new ParameterSet(args);
                        }

                        if (this.Category != null)
                            foreach (string cat in this.Category.Split(new char[] { ',' }))
                                parms.Properties.Add(PropertyNames.Category, cat);

                        data.Add(parms);
                    }
                }
                catch (Exception ex)
                {
                    data.Clear();
                    data.Add(new ParameterSet(ex));
                }
            }

            return data;
        }

        private IEnumerable GetTestCaseSource(Type fixtureType)
        {
            Type sourceType = this.SourceType;
            if (sourceType == null)
                sourceType = fixtureType;

            if (SourceName == null)
                return Reflect.Construct(sourceType, _sourceConstructorParameters) as IEnumerable;

            MemberInfo[] members = sourceType.GetMember(SourceName,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            if (members.Length == 1)
            {
                MemberInfo member = members[0];
                object sourceobject = Reflect.Construct(sourceType, _sourceConstructorParameters);

                var field = member as FieldInfo;
                if (field != null)
                    return (IEnumerable)field.GetValue(sourceobject);

                var property = member as PropertyInfo;
                if (property != null)
                    return (IEnumerable)property.GetValue(sourceobject, null);

                var m = member as MethodInfo;
                if (m != null)
                    return (IEnumerable)m.Invoke(sourceobject, null);
            }

            return null;
        }
        #endregion

        #region ITestBuilder Members

        /// <summary>
        /// Construct one or more TestMethods from a given MethodInfo,
        /// using available parameter data.
        /// </summary>
        /// <param name="method">The MethodInfo for which tests are to be constructed.</param>
        /// <param name="suite">The suite to which the tests will be added.</param>
        /// <returns>One or more TestMethods</returns>
        public IEnumerable<TestMethod> BuildFrom(Type fixtureType, MethodInfo method, Test suite)
        {
            List<TestMethod> tests = new List<TestMethod>();

            foreach (ParameterSet parms in GetTestCasesFor(fixtureType, method))
                tests.Add(_builder.BuildTestMethod(fixtureType, method, suite, parms));

            return tests;
        }

        #endregion
    }
}
