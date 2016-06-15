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
using System.Collections.Generic;
using System.Reflection;
using NUnit.Compatibility;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;

namespace NUnit.Framework
{
    /// <summary>
    /// TestCaseAttribute is used to mark parameterized test cases
    /// and provide them with their arguments.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited=false)]
    public class TestCaseAttribute : NUnitAttribute, ITestBuilder, ITestCaseData, IImplyFixture
    {
        #region Constructors

        /// <summary>
        /// Construct a TestCaseAttribute with a list of arguments.
        /// This constructor is not CLS-Compliant
        /// </summary>
        /// <param name="arguments"></param>
        public TestCaseAttribute(params object[] arguments)
        {
            RunState = RunState.Runnable;
            
            if (arguments == null)
                Arguments = new object[] { null };
            else
                Arguments = arguments;

            Properties = new PropertyBag();
        }

        /// <summary>
        /// Construct a TestCaseAttribute with a single argument
        /// </summary>
        /// <param name="arg"></param>
        public TestCaseAttribute(object arg)
        {
            RunState = RunState.Runnable;			
            Arguments = new object[] { arg };
            Properties = new PropertyBag();
        }

        /// <summary>
        /// Construct a TestCaseAttribute with a two arguments
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        public TestCaseAttribute(object arg1, object arg2)
        {
            RunState = RunState.Runnable;			
            Arguments = new object[] { arg1, arg2 };
            Properties = new PropertyBag();
        }

        /// <summary>
        /// Construct a TestCaseAttribute with a three arguments
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        public TestCaseAttribute(object arg1, object arg2, object arg3)
        {
            RunState = RunState.Runnable;			
            Arguments = new object[] { arg1, arg2, arg3 };
            Properties = new PropertyBag();
        }

        #endregion

        #region ITestData Members

        /// <summary>
        /// Gets or sets the name of the test.
        /// </summary>
        /// <value>The name of the test.</value>
        public string TestName { get; set; }

        /// <summary>
        /// Gets or sets the RunState of this test case.
        /// </summary>
        public RunState RunState { get; private set; }

        /// <summary>
        /// Gets the list of arguments to a test case
        /// </summary>
        public object[] Arguments { get; private set; }

        /// <summary>
        /// Gets the properties of the test case
        /// </summary>
        public IPropertyBag Properties { get; private set; }

        #endregion

        #region ITestCaseData Members

        /// <summary>
        /// Gets or sets the expected result.
        /// </summary>
        /// <value>The result.</value>
        public object ExpectedResult
        {
            get { return _expectedResult; }
            set
            {
                _expectedResult = value;
                HasExpectedResult = true;
            }
        }
        private object _expectedResult;

        /// <summary>
        /// Returns true if the expected result has been set
        /// </summary>
        public bool HasExpectedResult { get; private set; }

        #endregion

        #region Other Properties

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get { return Properties.Get(PropertyNames.Description) as string; }
            set { Properties.Set(PropertyNames.Description, value); }
        }

        /// <summary>
        /// The author of this test
        /// </summary>
        public string Author
        {
            get { return Properties.Get(PropertyNames.Author) as string; }
            set { Properties.Set(PropertyNames.Author, value); }
        }

        /// <summary>
        /// The type that this test is testing
        /// </summary>
        public Type TestOf
        {
            get { return _testOf; }
            set
            {
                _testOf = value;
                Properties.Set(PropertyNames.TestOf, value.FullName);
            }
        }
        private Type _testOf;

        /// <summary>
        /// Gets or sets the reason for ignoring the test
        /// </summary>
        public string Ignore 
        { 
            get { return IgnoreReason; }
            set { IgnoreReason = value; } 
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="NUnit.Framework.TestCaseAttribute"/> is explicit.
        /// </summary>
        /// <value>
        /// <c>true</c> if explicit; otherwise, <c>false</c>.
        /// </value>
        public bool Explicit
        {
            get { return RunState == RunState.Explicit; }
            set { RunState = value ? RunState.Explicit : RunState.Runnable; }
        }

        /// <summary>
        /// Gets or sets the reason for not running the test.
        /// </summary>
        /// <value>The reason.</value>
        public string Reason 
        { 
            get { return Properties.Get(PropertyNames.SkipReason) as string; }
            set { Properties.Set(PropertyNames.SkipReason, value); }
        }

        /// <summary>
        /// Gets or sets the ignore reason. When set to a non-null
        /// non-empty value, the test is marked as ignored.
        /// </summary>
        /// <value>The ignore reason.</value>
        public string IgnoreReason
        {
            get { return Reason; }
            set
            {
                RunState = RunState.Ignored;
                Reason = value;
            }
        }
        
#if !PORTABLE
        /// <summary>
        /// Comma-delimited list of platforms to run the test for
        /// </summary>
        public string IncludePlatform { get; set; }

        /// <summary>
        /// Comma-delimited list of platforms to not run the test for
        /// </summary>
        public string ExcludePlatform { get; set; }
#endif

        /// <summary>
        /// Gets and sets the category for this test case.
        /// May be a comma-separated list of categories.
        /// </summary>
        public string Category
        {
            get { return Properties.Get(PropertyNames.Category) as string; }
            set 
            { 
                foreach (string cat in value.Split(new char[] { ',' }) )
                    Properties.Add(PropertyNames.Category, cat); 
            }
        }
 
        #endregion

        #region Helper Methods

        private TestCaseParameters GetParametersForTestCase(IMethodInfo method)
        {
            TestCaseParameters parms;

            try
            {
#if NETCF
                var tmethod = method.MakeGenericMethodEx(Arguments);
                if (tmethod == null)
                    throw new NotSupportedException("Cannot determine generic types from probing");
                method = tmethod;
#endif

                IParameterInfo[] parameters = method.GetParameters();
                int argsNeeded = parameters.Length;
                int argsProvided = Arguments.Length;

                parms = new TestCaseParameters(this);

                // Special handling for params arguments
                if (argsNeeded > 0 && argsProvided >= argsNeeded - 1)
                {
                    IParameterInfo lastParameter = parameters[argsNeeded - 1];
                    Type lastParameterType = lastParameter.ParameterType;
                    Type elementType = lastParameterType.GetElementType();

                    if (lastParameterType.IsArray && lastParameter.IsDefined<ParamArrayAttribute>(false))
                    {
                        if (argsProvided == argsNeeded)
                        {
                            Type lastArgumentType = parms.Arguments[argsProvided - 1].GetType();
                            if (!lastParameterType.GetTypeInfo().IsAssignableFrom(lastArgumentType.GetTypeInfo()))
                            {
                                Array array = Array.CreateInstance(elementType, 1);
                                array.SetValue(parms.Arguments[argsProvided - 1], 0);
                                parms.Arguments[argsProvided - 1] = array;
                            }
                        }
                        else
                        {
                            object[] newArglist = new object[argsNeeded];
                            for (int i = 0; i < argsNeeded && i < argsProvided; i++)
                                newArglist[i] = parms.Arguments[i];

                            int length = argsProvided - argsNeeded + 1;
                            Array array = Array.CreateInstance(elementType, length);
                            for (int i = 0; i < length; i++)
                                array.SetValue(parms.Arguments[argsNeeded + i - 1], i);

                            newArglist[argsNeeded - 1] = array;
                            parms.Arguments = newArglist;
                            argsProvided = argsNeeded;
                        }
                    }
                }

#if !NETCF
                //Special handling for optional parameters
                if (parms.Arguments.Length < argsNeeded)
                {
                    object[] newArgList = new object[parameters.Length];
                    Array.Copy(parms.Arguments, newArgList, parms.Arguments.Length);

                    for (var i = 0; i < parameters.Length; i++)
                    {
                        if (parameters[i].IsOptional)
                            newArgList[i] = Type.Missing;
                        else
                        {
                            if (i < parms.Arguments.Length)
                                newArgList[i] = parms.Arguments[i];
                            else
                                throw new TargetParameterCountException("Incorrect number of parameters specified for TestCase");
                        }
                    }
                    parms.Arguments = newArgList;
                }
#endif

                //if (method.GetParameters().Length == 1 && method.GetParameters()[0].ParameterType == typeof(object[]))
                //    parms.Arguments = new object[]{parms.Arguments};

                // Special handling when sole argument is an object[]
                if (argsNeeded == 1 && method.GetParameters()[0].ParameterType == typeof(object[]))
                {
                    if (argsProvided > 1 ||
                        argsProvided == 1 && parms.Arguments[0].GetType() != typeof(object[]))
                    {
                        parms.Arguments = new object[] { parms.Arguments };
                    }
                }

                if (argsProvided == argsNeeded)
                    PerformSpecialConversions(parms.Arguments, parameters);
            }
            catch (Exception ex)
            {
                parms = new TestCaseParameters(ex);
            }

            return parms;
        }

        /// <summary>
        /// Performs several special conversions allowed by NUnit in order to
        /// permit arguments with types that cannot be used in the constructor
        /// of an Attribute such as TestCaseAttribute or to simplify their use.
        /// </summary>
        /// <param name="arglist">The arguments to be converted</param>
        /// <param name="parameters">The ParameterInfo array for the method</param>
        private static void PerformSpecialConversions(object[] arglist, IParameterInfo[] parameters)
        {
            for (int i = 0; i < arglist.Length; i++)
            {
                object arg = arglist[i];
                Type targetType = parameters[i].ParameterType;

                if (arg == null)
                    continue;

                if (arg is SpecialValue && (SpecialValue)arg == SpecialValue.Null)
                {
                    arglist[i] = null;
                    continue;
                }

                if (targetType.IsAssignableFrom(arg.GetType()))
                    continue;
#if !PORTABLE
                if (arg is DBNull)
                {
                    arglist[i] = null;
                    continue;
                }
#endif
                bool convert = false;

                if (targetType == typeof(short) || targetType == typeof(byte) || targetType == typeof(sbyte) ||
                    targetType == typeof(short?) || targetType == typeof(byte?) || targetType == typeof(sbyte?) || targetType == typeof(double?))
                {
                    convert = arg is int;
                }
                else if (targetType == typeof(decimal) || targetType == typeof(decimal?))
                {
                    convert = arg is double || arg is string || arg is int;
                }
                else if (targetType == typeof(DateTime) || targetType == typeof(DateTime?))
                {
                    convert = arg is string;
                }

                if (convert)
                {
                    Type convertTo = targetType.GetTypeInfo().IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>) ? 
                        targetType.GetGenericArguments()[0] : targetType;
                    arglist[i] = Convert.ChangeType(arg, convertTo, System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                // Convert.ChangeType doesn't work for TimeSpan from string
                if ((targetType == typeof(TimeSpan) || targetType == typeof(TimeSpan?)) && arg is string)
                {
                    arglist[i] = TimeSpan.Parse((string)arg);
                }
            }
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
        public IEnumerable<TestMethod> BuildFrom(IMethodInfo method, Test suite)
        {
            TestMethod test = new NUnitTestCaseBuilder().BuildTestMethod(method, suite, GetParametersForTestCase(method));
            
#if !PORTABLE
            if (test.RunState != RunState.NotRunnable &&
                test.RunState != RunState.Ignored)
            {
                PlatformHelper platformHelper = new PlatformHelper();
                
                if (!platformHelper.IsPlatformSupported(this))
                {
                    test.RunState = RunState.Skipped;
                    test.Properties.Add(PropertyNames.SkipReason, platformHelper.Reason);
                }
            }
#endif

            yield return test;
        }

        #endregion
    }
}
