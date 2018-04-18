// ***********************************************************************
// Copyright (c) 2008-2018 Charlie Poole, Rob Prouse
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
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;

namespace NUnit.Framework
{
    /// <summary>
    /// TestCaseSourceAttribute indicates the source to be used to
    /// provide test cases for a test method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class TestCaseSourceAttribute : NUnitAttribute, ITestBuilder, IImplyFixture
    {
        private readonly NUnitTestCaseBuilder _builder = new NUnitTestCaseBuilder();

        #region Constructors

        /// <summary>
        /// Construct with the name of the method, property or field that will provide data
        /// </summary>
        /// <param name="sourceName">The name of a static method, property or field that will provide data.</param>
        public TestCaseSourceAttribute(string sourceName)
        {
            this.SourceName = sourceName;
        }

        /// <summary>
        /// Construct with a Type and name
        /// </summary>
        /// <param name="sourceType">The Type that will provide data</param>
        /// <param name="sourceName">The name of a static method, property or field that will provide data.</param>
        /// <param name="methodParams">A set of parameters passed to the method, works only if the Source Name is a method. 
        ///                     If the source name is a field or property has no effect.</param>
        public TestCaseSourceAttribute(Type sourceType, string sourceName, object[] methodParams)
        {
            this.MethodParams = methodParams;
            this.SourceType = sourceType;
            this.SourceName = sourceName;
        }
        /// <summary>
        /// Construct with a Type and name
        /// </summary>
        /// <param name="sourceType">The Type that will provide data</param>
        /// <param name="sourceName">The name of a static method, property or field that will provide data.</param>
        public TestCaseSourceAttribute(Type sourceType, string sourceName)
        {
            this.SourceType = sourceType;
            this.SourceName = sourceName;
        }

        /// <summary>
        /// Construct with a name
        /// </summary>
        /// <param name="sourceName">The name of a static method, property or field that will provide data.</param>
        /// <param name="methodParams">A set of parameters passed to the method, works only if the Source Name is a method. 
        ///                     If the source name is a field or property has no effect.</param>
        public TestCaseSourceAttribute(string sourceName, object[] methodParams)
        {
            this.MethodParams = methodParams;
            this.SourceName = sourceName;
        }
        /// <summary>
        /// Construct with a Type
        /// </summary>
        /// <param name="sourceType">The type that will provide data</param>
        public TestCaseSourceAttribute(Type sourceType)
        {
            this.SourceType = sourceType;
        }

        #endregion

        #region Properties
        /// <summary>
        /// A set of parameters passed to the method, works only if the Source Name is a method. 
        /// If the source name is a field or property has no effect.
        /// </summary>
        public object[] MethodParams { get; }
        /// <summary>
        /// The name of a the method, property or fiend to be used as a source
        /// </summary>
        public string SourceName { get; }

        /// <summary>
        /// A Type to be used as a source
        /// </summary>
        public Type SourceType { get; }

        /// <summary>
        /// Gets or sets the category associated with every fixture created from
        /// this attribute. May be a single category or a comma-separated list.
        /// </summary>
        public string Category { get; set; }

        #endregion

        #region ITestBuilder Members

        /// <summary>
        /// Builds any number of tests from the specified method and context.
        /// </summary>
        /// <param name="method">The method to be used as a test.</param>
        /// <param name="suite">The parent to which the test will be added.</param>
        public IEnumerable<TestMethod> BuildFrom(FixtureMethod method, Test suite)
        {
            int count = 0;

            foreach (TestCaseParameters parms in GetTestCasesFor(method))
            {
                count++;
                yield return _builder.BuildTestMethod(method, suite, parms);
            }

            // If count > 0, error messages will be shown for each case
            // but if it's 0, we need to add an extra "test" to show the message.
            if (count == 0 && method.Method.GetParameters().Length == 0)
            {
                var parms = new TestCaseParameters();
                parms.RunState = RunState.NotRunnable;
                parms.Properties.Set(PropertyNames.SkipReason, "TestCaseSourceAttribute may not be used on a method without parameters");
                    
                yield return _builder.BuildTestMethod(method, suite, parms);
            }
        }

        #endregion

        #region Helper Methods

        private IEnumerable<ITestCaseData> GetTestCasesFor(FixtureMethod method)
        {
            List<ITestCaseData> data = new List<ITestCaseData>();

            try
            {
                IEnumerable source = GetTestCaseSource(method.FixtureType);

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
                            object[] args = null;

                            // 3. An array was passed, it may be an object[]
                            //    or possibly some other kind of array, which
                            //    TestCaseSource can accept.
                            var array = item as Array;
                            if (array != null)
                            {
                                // If array has the same number of elements as parameters
                                // and it does not fit exactly into single existing parameter
                                // we believe that this array contains arguments, not is a bare
                                // argument itself.
                                var parameters = method.Method.GetParameters();
                                var argsNeeded = parameters.Length;
                                if (argsNeeded > 0 && argsNeeded == array.Length && parameters[0].ParameterType != array.GetType())
                                {
                                    args = new object[array.Length];
                                    for (var i = 0; i < array.Length; i++)
                                        args[i] = array.GetValue(i);
                                }
                            }

                            if (args == null)
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
                else
                {
                    data.Clear();
                    data.Add(new TestCaseParameters(new Exception("The test case source could not be found.")));
                }
            }
            catch (Exception ex)
            {
                data.Clear();
                data.Add(new TestCaseParameters(ex));
            }

            return data;
        }

        private IEnumerable GetTestCaseSource(Type type)
        {
            Type sourceType = SourceType ?? type;

            // Handle Type implementing IEnumerable separately
            if (SourceName == null)
                return Reflect.Construct(sourceType, null) as IEnumerable;

            MemberInfo[] members = sourceType.GetMember(SourceName,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

            if (members.Length == 1)
            {
                MemberInfo member = members[0];

                var field = member as FieldInfo;
                if (field != null)
                    return field.IsStatic
                        ? (MethodParams == null ? (IEnumerable)field.GetValue(null) 
                                                : ReturnErrorAsParameter(ParamGivenToField))
                        : ReturnErrorAsParameter(SourceMustBeStatic);

                var property = member as PropertyInfo;
                if (property != null)
                    return property.GetGetMethod(true).IsStatic
                        ? (MethodParams == null ? (IEnumerable)property.GetValue(null, null) 
                                                : ReturnErrorAsParameter(ParamGivenToProperty))
                        : ReturnErrorAsParameter(SourceMustBeStatic);

                var m = member as MethodInfo;
                

                    if (m != null)
                    return m.IsStatic
                        ? (MethodParams == null || m.GetParameters().Length == MethodParams.Length ? (IEnumerable)m.Invoke(null, MethodParams) 
                                                              : ReturnErrorAsParameter(NumberOfArgsDoesNotMatch))
                        : ReturnErrorAsParameter(SourceMustBeStatic);
            }

            return null;
        }
        
        private static IEnumerable ReturnErrorAsParameter(string errorMessage)
        {
            var parms = new TestCaseParameters();
            parms.RunState = RunState.NotRunnable;
            parms.Properties.Set(PropertyNames.SkipReason, errorMessage);
            return new TestCaseParameters[] { parms };
        }

        private const string SourceMustBeStatic =
            "The sourceName specified on a TestCaseSourceAttribute must refer to a static field, property or method.";
        private const string ParamGivenToField = "You have specified a data source field but also given a set of parameters. Fields cannot take parameters, " +
                                                 "please revise the 3rd parameter passed to the TestCaseSourceAttribute and either remove " +
                                                 "it or specify a method.";
        private const string ParamGivenToProperty = "You have specified a data source property but also given a set of parameters. " +
                                                    "Properties cannot take parameters, please revise the 3rd parameter passed to the " +
                                                    "TestCaseSource attribute and either remove it or specify a method.";
        private const string NumberOfArgsDoesNotMatch = "You have given the wrong number of arguments to the method in the TestCaseSourceAttribute" +
                                                        ", please check the number of parameters passed in the object is correct in the 3rd parameter for the " +
                                                        "TestCaseSourceAttribute and this matches the number of parameters in the target method and try again.";

#endregion
    }
}
