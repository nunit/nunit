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
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.Api;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// ParameterSet encapsulates method arguments and
    /// other selected parameters needed for constructing
    /// a parameterized test case.
    /// </summary>
    public class ParameterSet : ITestCaseData
    {
        #region Instance Fields
        private RunState runState;
        private Exception providerException;
        private object[] arguments;
        private object[] originalArguments;
        private System.Type expectedException;
        private string expectedExceptionName;
        private string expectedMessage;
        private MessageMatch matchType;
        private object result;
        private string testName;
        private bool isIgnored;
        private bool hasExpectedResult;

        /// <summary>
        /// A dictionary of properties, used to add information
        /// to tests without requiring the class to change.
        /// </summary>
        private IPropertyBag properties;

        #endregion

        #region Properties
        /// <summary>
        /// The RunState for this set of parameters.
        /// </summary>
        public RunState RunState
        {
            get { return runState; }
            set { runState = value; }
        }

        /// <summary>
        /// Holds any exception thrown by the parameter provider
        /// </summary>
        public Exception ProviderException
        {
            get { return providerException; }
        }

        /// <summary>
        /// The arguments to be used in running the test,
        /// which must match the method signature.
        /// </summary>
        public object[] Arguments
        {
            get { return arguments; }
            set 
            { 
                arguments = value;

                if (originalArguments == null)
                    originalArguments = value;
            }
        }

        /// <summary>
        /// The original arguments provided by the user,
        /// used for display purposes.
        /// </summary>
        public object[] OriginalArguments
        {
            get { return originalArguments; }
        }


        /// <summary>
        /// The Type of any exception that is expected.
        /// </summary>
        public System.Type ExpectedException
        {
            get { return expectedException; }
            set { expectedException = value; }
        }

        /// <summary>
        /// The FullName of any exception that is expected
        /// </summary>
        public string ExpectedExceptionName
        {
            get { return expectedExceptionName; }
            set { expectedExceptionName = value; }
        }

        /// <summary>
        /// The Message of any exception that is expected
        /// </summary>
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
        /// The expected result of the test, which
        /// must match the method return type.
        /// </summary>
        public object Result
        {
            get { return result; }
            set
            {
                result = value;
                hasExpectedResult = true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether an expected result was specified.
        /// </summary>
        public bool HasExpectedResult
        {
            get { return hasExpectedResult; }
        }

        /// <summary>
        /// A name to be used for this test case in lieu
        /// of the standard generated name containing
        /// the argument list.
        /// </summary>
        public string TestName
        {
            get { return testName; }
            set { testName = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ParameterSet"/> is ignored.
        /// </summary>
        /// <value><c>true</c> if ignored; otherwise, <c>false</c>.</value>
        public bool Ignored
        {
            get { return isIgnored; }
            set { isIgnored = value; }
        }

        /// <summary>
        /// Gets the property dictionary for this test
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

        #region Constructors
        /// <summary>
        /// Construct a non-runnable ParameterSet, specifying
        /// the provider exception that made it invalid.
        /// </summary>
        public ParameterSet(Exception exception)
        {
            this.runState = RunState.NotRunnable;
            this.providerException = exception;
        }

        /// <summary>
        /// Construct an empty parameter set, which
        /// defaults to being Runnable.
        /// </summary>
        public ParameterSet()
        {
            this.runState = RunState.Runnable;
        }

        /// <summary>
        /// Construct a ParameterSet from an object implementing ITestCaseData
        /// </summary>
        /// <param name="data"></param>
        public ParameterSet(ITestCaseData data)
        {
            this.RunState = RunState.Runnable;
            this.Arguments = data.Arguments;
            this.ExpectedException = data.ExpectedException;
            this.ExpectedExceptionName = data.ExpectedExceptionName;
            this.ExpectedMessage = data.ExpectedMessage;
            this.MatchType = data.MatchType;
            this.Result = data.Result;
            this.TestName = data.TestName;
            this.Ignored = data.Ignored;

            foreach (string key in data.Properties.Keys)
                this.Properties[key] = data.Properties[key];
        }
        #endregion
    }
}
