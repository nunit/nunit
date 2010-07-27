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
using NUnit.Framework.Api;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// TestCaseAttribute is used to mark parameterized test cases
    /// and provide them with their arguments.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TestCaseAttribute : DataAttribute, ITestCaseData, ITestCaseSource
    {
        private object[] arguments;
#if !NUNITLITE
        private Type expectedExceptionType;
        private string expectedExceptionName;
        private string expectedMessage;
        private MessageMatch matchType;
        private object result;
        private string description;
        private string testName;
        private bool isIgnored;
        private string ignoreReason;
#endif

        /// <summary>
        /// Construct a TestCaseAttribute with a list of arguments.
        /// This constructor is not CLS-Compliant
        /// </summary>
        /// <param name="arguments"></param>
        public TestCaseAttribute(params object[] arguments)
        {
         	if (arguments == null)
         		this.arguments = new object[] { null };
         	else
 	        	this.arguments = arguments;
        }

        /// <summary>
        /// Construct a TestCaseAttribute with a single argument
        /// </summary>
        /// <param name="arg"></param>
        public TestCaseAttribute(object arg)
        {
            this.arguments = new object[] { arg };
        }

        /// <summary>
        /// Construct a TestCaseAttribute with a two arguments
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        public TestCaseAttribute(object arg1, object arg2)
        {
            this.arguments = new object[] { arg1, arg2 };
        }

        /// <summary>
        /// Construct a TestCaseAttribute with a three arguments
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        public TestCaseAttribute(object arg1, object arg2, object arg3)
        {
            this.arguments = new object[] { arg1, arg2, arg3 };
        }

        /// <summary>
        /// Gets the list of arguments to a test case
        /// </summary>
        public object[] Arguments
        {
            get { return arguments; }
        }

#if !NUNITLITE
        /// <summary>
        /// Gets or sets the expected result.
        /// </summary>
        /// <value>The result.</value>
        public object Result
        {
            get { return result; }
            set { result = value; }
        }

        /// <summary>
        /// Gets or sets the expected exception.
        /// </summary>
        /// <value>The expected exception.</value>
        public Type ExpectedException
        {
            get { return expectedExceptionType;  }
            set
            {
                expectedExceptionType = value;
                expectedExceptionName = expectedExceptionType.FullName;
            }
        }

        /// <summary>
        /// Gets or sets the name the expected exception.
        /// </summary>
        /// <value>The expected name of the exception.</value>
        public string ExpectedExceptionName
        {
            get { return expectedExceptionName; }
            set
            {
                expectedExceptionName = value;
                expectedExceptionType = null;
            }
        }

        /// <summary>
        /// Gets or sets the expected message of the expected exception
        /// </summary>
        /// <value>The expected message of the exception.</value>
        public string ExpectedMessage
        {
            get { return expectedMessage; }
         	set { expectedMessage = value; }
        }

        /// <summary>
        ///  Gets or sets the type of match to be performed on the expected message
        /// </summary>
        public MessageMatch MatchType
        {
            get { return matchType; }
            set { matchType = value; }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        /// <summary>
        /// Gets or sets the name of the test.
        /// </summary>
        /// <value>The name of the test.</value>
        public string TestName
        {
            get { return testName; }
            set { testName = value; }
        }

        /// <summary>
        /// Gets or sets the ignored status of the test
        /// </summary>
        public bool Ignore
        {
            get { return isIgnored; }
            set { isIgnored = value; }
        }

        /// <summary>
        /// Gets or sets the ignored status of the test
        /// </summary>
        public bool Ignored
        {
            get { return isIgnored; }
            set { isIgnored = value; }
        }

        /// <summary>
        /// Gets the ignore reason.
        /// </summary>
        /// <value>The ignore reason.</value>
        public string IgnoreReason
        {
            get { return ignoreReason; }
            set
            {
                ignoreReason = value;
                isIgnored = ignoreReason != null && ignoreReason != string.Empty;
            }
        }

        /// <summary>
        /// NYI
        /// </summary>
        public IList Categories
        {
            get { return new string[0]; }
        }

        /// <summary>
        /// NYI
        /// </summary>
        public IDictionary Properties
        {
            get { return new Hashtable(); }
        }

#endif

        #region ITestCaseSource Members

        /// <summary>
        /// Returns an collection containing a single ITestCaseData item,
        /// constructed from the arguments provided in the constructor and
        /// possibly converted to match the specified method.
        /// </summary>
        /// <param name="method">The method for which data is being provided</param>
        /// <returns></returns>
#if CLR_2_0
        public System.Collections.Generic.IEnumerable<ITestCaseData> GetTestCasesFor(System.Reflection.MethodInfo method)
#else
        public System.Collections.IEnumerable GetTestCasesFor(System.Reflection.MethodInfo method)
#endif
        {
            ParameterSet parms;

            try
            {
                ParameterInfo[] parameters = method.GetParameters();
                int argsNeeded = parameters.Length;
                int argsProvided = Arguments.Length;

                parms = new ParameterSet(this);

                // Special handling for params arguments
                if (argsProvided > argsNeeded)
                {
                    ParameterInfo lastParameter = parameters[argsNeeded - 1];
                    Type lastParameterType = lastParameter.ParameterType;

                    if (lastParameterType.IsArray && lastParameter.IsDefined(typeof(ParamArrayAttribute), false))
                    {
                        object[] newArglist = new object[argsNeeded];
                        for (int i = 0; i < argsNeeded; i++)
                            newArglist[i] = parms.Arguments[i];

                        int length = argsProvided - argsNeeded + 1;
                        Array array = Array.CreateInstance(lastParameterType.GetElementType(), length);
                        for (int i = 0; i < length; i++)
                            array.SetValue(parms.Arguments[argsNeeded + i - 1], i);

                        newArglist[argsNeeded - 1] = array;
                        parms.Arguments = newArglist;
                        argsProvided = argsNeeded;
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
            
            return new ITestCaseData[] { parms };
        }

        #endregion

        #region Helper Methods
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
    }
}
