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
using System.Collections.Generic;
using System.Reflection;
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
    public class TestCaseAttribute : TestCaseBuilderAttribute, ITestBuilder, ITestCaseData, IImplyFixture
    {
        #region Instance variables

        private object expectedResult;
        private IPropertyBag properties;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct a TestCaseAttribute with a list of arguments.
        /// This constructor is not CLS-Compliant
        /// </summary>
        /// <param name="arguments"></param>
        public TestCaseAttribute(params object[] arguments)
        {
            this.RunState = RunState.Runnable;
            
            if (arguments == null)
                this.Arguments = new object[] { null };
            else
                this.Arguments = arguments;
        }

        /// <summary>
        /// Construct a TestCaseAttribute with a single argument
        /// </summary>
        /// <param name="arg"></param>
        public TestCaseAttribute(object arg)
        {
            this.RunState = RunState.Runnable;			
            this.Arguments = new object[] { arg };
        }

        /// <summary>
        /// Construct a TestCaseAttribute with a two arguments
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        public TestCaseAttribute(object arg1, object arg2)
        {
            this.RunState = RunState.Runnable;			
            this.Arguments = new object[] { arg1, arg2 };
        }

        /// <summary>
        /// Construct a TestCaseAttribute with a three arguments
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        public TestCaseAttribute(object arg1, object arg2, object arg3)
        {
            this.RunState = RunState.Runnable;			
            this.Arguments = new object[] { arg1, arg2, arg3 };
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the list of arguments to a test case
        /// </summary>
        public object[] Arguments { get; private set;  }

        /// <summary>
        /// Gets or sets the expected result.
        /// </summary>
        /// <value>The result.</value>
        public object ExpectedResult
        {
            get { return expectedResult; }
            set 
            { 
                expectedResult = value;
                HasExpectedResult = true;
            }
        }

        /// <summary>
        /// Returns true if the expected result has been set
        /// </summary>
        public bool HasExpectedResult { get; private set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get { return this.Properties.Get(PropertyNames.Description) as string; }
            set { this.Properties.Set(PropertyNames.Description, value); }
        }

        /// <summary>
        /// Gets or sets the name of the test.
        /// </summary>
        /// <value>The name of the test.</value>
        public string TestName { get; set; }

        /// <summary>
        /// Gets or sets the ignored status of the test
        /// </summary>
        public bool Ignore 
        { 
            get { return this.RunState == RunState.Ignored; }
            set { this.RunState = value ? RunState.Ignored : RunState.Runnable; } 
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="NUnit.Framework.TestCaseAttribute"/> is explicit.
        /// </summary>
        /// <value>
        /// <c>true</c> if explicit; otherwise, <c>false</c>.
        /// </value>
        public bool Explicit 
        { 
            get { return this.RunState == RunState.Explicit; }
            set { this.RunState = value ? RunState.Explicit : RunState.Runnable; }
        }

        /// <summary>
        /// Gets or sets the RunState of this test case.
        /// </summary>
        public RunState RunState { get; private set; }
        
        /// <summary>
        /// Gets or sets the reason for not running the test.
        /// </summary>
        /// <value>The reason.</value>
        public string Reason 
        { 
            get { return this.Properties.Get(PropertyNames.SkipReason) as string; }
            set { this.Properties.Set(PropertyNames.SkipReason, value); }
        }

        /// <summary>
        /// Gets or sets the ignore reason. When set to a non-null
        /// non-empty value, the test is marked as ignored.
        /// </summary>
        /// <value>The ignore reason.</value>
        public string IgnoreReason
        {
            get { return this.Reason; }
            set
            {
                this.RunState = RunState.Ignored;
                this.Reason = value;
            }
        }

        /// <summary>
        /// Gets and sets the category for this fixture.
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
 
        /// <summary>
        /// NYI
        /// </summary>
        public IPropertyBag Properties
        {
            get
            {
                if (properties == null)
                    properties = new PropertyBag();

                return properties;
            }
        }

        #endregion

        #region Helper Methods

        private ParameterSet GetParametersForTestCase(MethodInfo method)
        {
            ParameterSet parms;

            try
            {
                ParameterInfo[] parameters = method.GetParameters();
                int argsNeeded = parameters.Length;
                int argsProvided = Arguments.Length;

                parms = new ParameterSet(this);

                // Special handling for params arguments
                if (argsNeeded > 0 && argsProvided >= argsNeeded - 1)
                {
                    ParameterInfo lastParameter = parameters[argsNeeded - 1];
                    Type lastParameterType = lastParameter.ParameterType;
                    Type elementType = lastParameterType.GetElementType();

                    if (lastParameterType.IsArray && lastParameter.IsDefined(typeof(ParamArrayAttribute), false))
                    {
                        if (argsProvided == argsNeeded)
                        {
                            Type lastArgumentType = parms.Arguments[argsProvided - 1].GetType();
                            if (!lastParameterType.IsAssignableFrom(lastArgumentType))
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
                parms = new ParameterSet(ex);
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
        private static void PerformSpecialConversions(object[] arglist, ParameterInfo[] parameters)
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

                if (arg is DBNull)
                {
                    arglist[i] = null;
                    continue;
                }

                bool convert = false;

                if (targetType == typeof(short) || targetType == typeof(byte) || targetType == typeof(sbyte))
                    convert = arg is int;
                else
                if (targetType == typeof(decimal))
                    convert = arg is double || arg is string || arg is int;
                else
                    if (targetType == typeof(DateTime) || targetType == typeof(TimeSpan))
                        convert = arg is string;

                if (convert)
                    arglist[i] = Convert.ChangeType(arg, targetType, System.Globalization.CultureInfo.InvariantCulture);
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
        public IEnumerable<TestMethod> BuildFrom(MethodInfo method, Test suite)
        {
            return new TestMethod[] { new NUnitTestCaseBuilder().BuildTestMethod(method, suite, GetParametersForTestCase(method)) };
        }

        #endregion
    }
}
