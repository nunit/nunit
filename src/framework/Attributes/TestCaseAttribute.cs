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
using System.Collections.Specialized;
using NUnit.Framework.Api;

namespace NUnit.Framework
{
    /// <summary>
    /// TestCaseAttribute is used to mark parameterized test cases
    /// and provide them with their arguments.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TestCaseAttribute : PropertyAttribute, ITestCaseData
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
    }
}
