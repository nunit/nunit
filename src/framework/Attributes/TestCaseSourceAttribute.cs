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
        /// Construct with the name of the method, property or field that will prvide data
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
        /// <param name="method">The method for which data is needed.</param>
        /// <returns></returns>
        public IEnumerable<ITestCaseData> GetTestCasesFor(MethodInfo method)
        {
            List<ITestCaseData> data = new List<ITestCaseData>();
            IEnumerable source = GetTestCaseSource(method);

            if (source != null)
            {
                try
                {
                    ParameterInfo[] parameters = method.GetParameters();

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
                                if (args.Length != parameters.Length)
                                    args = new object[] { item };
                            }
                            //else if (parameters.Length == 1 && parameters[0].ParameterType.IsAssignableFrom(item.GetType()))
                            //{
                            //    args = new object[] { item };
                            //}
                            else if (item is Array)
                            {
                                Array array = item as Array;

                                if (array.Rank == 1 && array.Length == parameters.Length)
                                {
                                    args = new object[array.Length];
                                    for (int i = 0; i < array.Length; i++)
                                        args[i] = (object)array.GetValue(i);
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

        private IEnumerable GetTestCaseSource(MethodInfo method)
        {
            IEnumerable source = null;

            Type sourceType = this.SourceType;
            if (sourceType == null)
                sourceType = method.ReflectedType;

            if (this.SourceName == null)
            {
                return Reflect.Construct(sourceType, _sourceConstructorParameters) as IEnumerable;
            }

            MemberInfo[] members = sourceType.GetMember(SourceName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            if (members.Length == 1)
            {
                MemberInfo member = members[0];
                object sourceobject = Internal.Reflect.Construct(sourceType, _sourceConstructorParameters);
                switch (member.MemberType)
                {
                    case MemberTypes.Field:
                        FieldInfo field = member as FieldInfo;
                        source = (IEnumerable)field.GetValue(sourceobject);
                        break;
                    case MemberTypes.Property:
                        PropertyInfo property = member as PropertyInfo;
                        source = (IEnumerable)property.GetValue(sourceobject, null);
                        break;
                    case MemberTypes.Method:
                        MethodInfo m = member as MethodInfo;
                        source = (IEnumerable)m.Invoke(sourceobject, null);
                        break;
                }
            }
            return source;
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
        public IEnumerable<TestMethod> BuildFrom(MethodInfo method, Test suite)
        {
            List<TestMethod> tests = new List<TestMethod>();

            foreach (ParameterSet parms in GetTestCasesFor(method))
                tests.Add(_builder.BuildTestMethod(method, suite, parms));

            return tests;
        }

        #endregion
    }
}
