// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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
using NUnit.Framework;
using ObjectList = NUnit.ObjectList;

namespace NUnitLite
{
    /// <summary>
    /// Represents the final result state from running a test
    /// </summary>
    public enum ResultState
    {
        /// <summary>
        /// The test was not run
        /// </summary>
        NotRun,
        /// <summary>
        /// The test passed
        /// </summary>
        Success,
        /// <summary>
        /// The test failed
        /// </summary>
        Failure,
        /// <summary>
        /// The test terminated with an error
        /// </summary>
        Error
    }

    /// <summary>
    /// TestResult represents the result from running a test
    /// </summary>
    public class TestResult
    {
        private ITest test;

        private ResultState resultState = ResultState.NotRun;

        private string message;

#if !NETCF_1_0
        private string stackTrace;
#endif

        private ObjectList results;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestResult"/> class.
        /// </summary>
        /// <param name="test">The test to which this result applies.</param>
        public TestResult(ITest test)
        {
            this.test = test;
        }

        /// <summary>
        /// Gets the test to which this result applies.
        /// </summary>
        /// <value>The test.</value>
        public ITest Test
        {
            get { return test; }
        }

        /// <summary>
        /// Gets the state of the result.
        /// </summary>
        /// <value>The state of the result.</value>
        public ResultState ResultState
        {
            get { return resultState; }
        }

        /// <summary>
        /// Gets the child results if any for this result
        /// </summary>
        /// <value>A list of child results</value>
        public IList Results
        {
            get 
            {
                if (results == null)
                    results = new ObjectList();

                return results;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the test was executed.
        /// </summary>
        /// <value><c>true</c> if executed; otherwise, <c>false</c>.</value>
        public bool Executed
        {
            get { return resultState != ResultState.NotRun; }
        }

        /// <summary>
        /// Gets a value indicating whether the test passed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the test passed; otherwise, <c>false</c>.
        /// </value>
        public bool IsSuccess
        {
            get { return resultState == ResultState.Success; }
        }

        /// <summary>
        /// Gets a value indicating whether the test failed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the test failed; otherwise, <c>false</c>.
        /// </value>
        public bool IsFailure
        {
            get { return resultState == ResultState.Failure; }
        }

        /// <summary>
        /// Gets a value indicating whether the test caused an error.
        /// </summary>
        /// <value><c>true</c> if the test caused an error; otherwise, <c>false</c>.</value>
        public bool IsError
        {
            get { return resultState == ResultState.Error; }
        }

        /// <summary>
        /// Gets the message associated with a TestResult.
        /// </summary>
        /// <value>The message.</value>
        public string Message
        {
            get { return message; }
        }

#if !NETCF_1_0
        /// <summary>
        /// Gets the stack trace.
        /// </summary>
        /// <value>The stack trace.</value>
        public string StackTrace
        {
            get { return stackTrace; }
        }
#endif

        /// <summary>
        /// Adds a child result.
        /// </summary>
        /// <param name="result">The result to add.</param>
        public void AddResult(TestResult result)
        {
            if (results == null)
                results = new ObjectList();

            results.Add(result);

            switch (result.ResultState)
            {
                case ResultState.Error:
                case ResultState.Failure:
                    this.Failure("Component test failure");
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Marks this instance as passing.
        /// </summary>
        public void Success()
        {
            this.resultState = ResultState.Success;
            this.message = null;
        }


        /// <summary>
        /// Marks this instance as failing
        /// </summary>
        /// <param name="message">The failure message</param>
	    public void Failure(string message)
	    {
                this.resultState = ResultState.Failure;
                if (this.message == null || this.message == string.Empty)
                    this.message = message;
                else
                    this.message = this.message + NUnit.Framework.Internal.Env.NewLine + message;
            }

        /// <summary>
        /// Marks this instance as an errr
        /// </summary>
        /// <param name="message">The error message</param>
        public void Error(string message)
        {
            this.resultState = ResultState.Error;
            if (this.message == null || this.message == string.Empty)
                this.message = message;
            else
                this.message = this.message + NUnit.Framework.Internal.Env.NewLine + message;
        }

#if !NETCF_1_0
        /// <summary>
        /// Marks this instance as failing
        /// </summary>
        /// <param name="message">The failure message</param>
        /// <param name="stackTrace">The stacktrace</param>
        public void Failure(string message, string stackTrace)
        {
            this.Failure(message);
            this.stackTrace = stackTrace;
        }
#endif

        /// <summary>
        /// Marks this instance as an error
        /// </summary>
        /// <param name="ex">The exception causing the error</param>
        public void Error(Exception ex)
        {
            this.resultState = ResultState.Error;
            this.message = ex.GetType().ToString() + " : " + ex.Message;
#if !NETCF_1_0
            this.stackTrace = ex.StackTrace;
#endif
        }

        /// <summary>
        /// Marks this instance as not run
        /// </summary>
        /// <param name="message">Message giving the reason the test was not run</param>
        public void NotRun(string message)
        {
            this.resultState = ResultState.NotRun;
            this.message = message;
        }

        /// <summary>
        /// Records an exception.
        /// </summary>
        /// <param name="ex">The exception.</param>
        public void RecordException(Exception ex)
        {
            if (ex is AssertionException)
#if NETCF_1_0
		this.Failure(ex.Message);
#else
                this.Failure(ex.Message, StackFilter.Filter(ex.StackTrace));
#endif
            else
                this.Error(ex);
        }
    }
}
