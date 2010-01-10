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
using System.Text;
using System.Collections;
using System.Xml;

namespace NUnit.Framework.Api
{
	/// <summary>
	/// The TestResult class represents
	/// the result of a test and is used to
	/// communicate results across AppDomains.
	/// </summary>
	/// 
	[Serializable]
	public class TestResult
	{
		#region Fields
		/// <summary>
		/// Indicates the result of the test
		/// </summary>
		private ResultState resultState;

		/// <summary>
		/// The elapsed time for executing this test
		/// </summary>
		private double time = 0.0;

		/// <summary>
		/// The test that this result pertains to
		/// </summary>
		private readonly ITest test;

		/// <summary>
		/// The stacktrace at the point of failure
		/// </summary>
		private string stackTrace;

		/// <summary>
		/// Message giving the reason for failure
		/// </summary>
		private string message;

		/// <summary>
		/// List of child results
		/// </summary>
		private IList results;

		/// <summary>
		/// Number of asserts executed by this test
		/// </summary>
		private int assertCount = 0;

		#endregion

		#region Constructor
		/// <summary>
		/// Construct a test result given a Test
		/// </summary>
		/// <param name="test">The test to be used</param>
		public TestResult(ITest test)
		{
			this.test = test;
			this.message = test.IgnoreReason;
		}

		#endregion

        #region Properties

		/// <summary>
		/// Gets the ResultState of the test result, which 
		/// indicates the success or failure of the test.
		/// </summary>
        public ResultState ResultState
        {
            get { return resultState; }
        }

		/// <summary>
		/// Indicates whether the test executed
		/// </summary>
        public bool Executed
        {
            get
            {
                return resultState == ResultState.Success ||
                       resultState == ResultState.Failure ||
                       resultState == ResultState.Error ||
                       resultState == ResultState.Inconclusive;
            }
        }

		/// <summary>
		/// Gets the name of the test result
		/// </summary>
		public virtual string Name
		{
			get { return test.Name; }
		}

		/// <summary>
		/// Gets the full name of the test result
		/// </summary>
		public virtual string FullName
		{
			get { return test.FullName; }
		}

		/// <summary>
		/// Gets the test associated with this result
		/// </summary>
        public ITest Test
        {
            get { return test; }
        }

		/// <summary>
		/// Indicates whether the test ran successfully
		/// </summary>
        public virtual bool IsSuccess
        {
            get { return resultState == ResultState.Success; }
        }

        /// <summary>
        /// Indicates whether the test failed
        /// </summary>
        public virtual bool IsFailure
        {
            get { return resultState == ResultState.Failure;  }
        }

	    /// <summary>
	    /// Indicates whether the test had an error (as opposed to a failure)
	    /// </summary>
        public virtual bool IsError
	    {
            get { return resultState == ResultState.Error;  }   
	    }

		/// <summary>
		/// Gets a description associated with the test
		/// </summary>
        public string Description
        {
            get { return test.Description; }
        }

		/// <summary>
		/// Gets the elapsed time for running the test
		/// </summary>
        public double Time
        {
            get { return time; }
            set { time = value; }
        }

		/// <summary>
		/// Gets the message associated with a test
		/// failure or with not running the test
		/// </summary>
        public string Message
        {
            get { return message; }
        }

		/// <summary>
		/// Gets any stacktrace associated with an
		/// error or failure.
		/// </summary>
        public virtual string StackTrace
        {
            get { return stackTrace; }
            set { stackTrace = value; }
        }

		/// <summary>
		/// Gets or sets the count of asserts executed
		/// when running the test.
		/// </summary>
        public int AssertCount
        {
            get { return assertCount; }
            set { assertCount = value; }
        }

        /// <summary>
        /// Return true if this result has any child results
        /// </summary>
	    public bool HasResults
	    {
            get { return results != null && results.Count > 0; }    
	    }

		/// <summary>
		/// Gets a list of the child results of this TestResult
		/// </summary>
		public IList Results
		{
			get { return results; }
		}

		#endregion

        #region Public Methods
        /// <summary>
        /// Mark the test as succeeding
        /// </summary>
        public void Success()
        {
            SetResult( ResultState.Success, null, null );
        }

        /// <summary>
        /// Mark the test as succeeding and set a message
        /// </summary>
        public void Success( string message )
        {
            SetResult( ResultState.Success, message, null );
        }

        /// <summary>
		/// Mark the test as ignored.
		/// </summary>
		/// <param name="reason">The reason the test was not run</param>
		public void Ignore(string reason)
		{
			SetResult( ResultState.Ignored, reason, null );
		}

		/// <summary>
		/// Mark the test as ignored.
		/// </summary>
		/// <param name="ex">The ignore exception that was thrown</param>
		public void Ignore( Exception ex )
		{
			SetResult( ResultState.Ignored, ex.Message, BuildStackTrace( ex ) );
		}

		/// <summary>
		/// Mark the test as ignored.
		/// </summary>
		/// <param name="reason">The reason the test was not run</param>
		/// <param name="stackTrace">Stack trace giving the location of the command</param>
		public void Ignore(string reason, string stackTrace)
		{
			SetResult( ResultState.Ignored, reason, stackTrace );
		}

		/// <summary>
		/// Mark the test as skipped.
		/// </summary>
		/// <param name="reason">The reason the test was not run</param>
        public void Skip(string reason)
        {
            SetResult(ResultState.Skipped, reason, null);
        }

        /// <summary>
        /// Mark the test a not runnable with a reason
        /// </summary>
        /// <param name="reason">The reason the test is invalid</param>
        public void Invalid( string reason )
        {
            SetResult( ResultState.NotRunnable, reason, null );
        }

        /// <summary>
        /// Mark the test as not runnable due to a builder exception
        /// </summary>
        /// <param name="ex">The exception thrown by the builder or an addin</param>
        public void Invalid(Exception ex)
        {
            SetResult(ResultState.NotRunnable, BuildMessage( ex ), BuildStackTrace(ex));
        }

	    /// <summary>
		/// Set the result of the test
		/// </summary>
		/// <param name="resultState">The ResultState to use in the result</param>
		/// <param name="reason">The reason the test was not run</param>
        /// <param name="stackTrace">Stack trace giving the location of the command</param>
        public void SetResult(ResultState resultState, string reason, string stackTrace)
		{
		    this.resultState = resultState;
			this.message = reason;
			this.stackTrace = stackTrace;
		}

        /// <summary>
        /// Set the test result based on the type of exception thrown
        /// </summary>
        /// <param name="ex">The exception that was thrown</param>
        public void SetResult(Exception ex)
        {
            if (ex is System.Threading.ThreadAbortException)
                SetResult(ResultState.Cancelled, "Test cancelled by user", ex.StackTrace);
            else if (ex is AssertionException)
                SetResult(ResultState.Failure, ex.Message, ex.StackTrace);
            else if (ex is IgnoreException)
                SetResult(ResultState.Ignored, ex.Message, ex.StackTrace);
            else if (ex is InconclusiveException)
                SetResult(ResultState.Inconclusive, ex.Message, ex.StackTrace);
            else if (ex is SuccessException)
                SetResult(ResultState.Success, ex.Message, ex.StackTrace);
            else
                SetResult(ResultState.Error, BuildMessage(ex), BuildStackTrace(ex));
        }

        /// <summary>
		/// Mark the test as a failure due to an
		/// assertion having failed.
		/// </summary>
		/// <param name="message">Message to display</param>
		/// <param name="stackTrace">Stack trace giving the location of the failure</param>
		public void Failure(string message, string stackTrace)
        {
            SetResult(ResultState.Failure, message, stackTrace);
        }

		/// <summary>
		/// Marks the result as an error due to an exception thrown
		/// by the test.
		/// </summary>
		/// <param name="exception">The exception that was caught</param>
        public void Error(Exception exception)
        {
            string message = BuildMessage(exception);
            string stackTrace = BuildStackTrace(exception);

            SetResult(ResultState.Error, message, stackTrace);
        }

		/// <summary>
		/// Marks the result as an error due to an exception thrown
		/// in the TearDown phase.
		/// </summary>
		/// <param name="exception">The exception that was caught</param>
		public void TearDownError( Exception exception )
		{
            string message = BuildMessage(exception);
            string stackTrace = BuildStackTrace(exception);

            message = "TearDown : " + message;
            stackTrace = "--TearDown" + Environment.NewLine + stackTrace;

            if (this.message != null)
                message = this.message + Environment.NewLine + message;
            if (this.stackTrace != null)
                stackTrace = this.stackTrace + Environment.NewLine + stackTrace;

            SetResult(ResultState.Error, message, stackTrace);
        }

		/// <summary>
		/// Add a child result
		/// </summary>
		/// <param name="result">The child result to be added</param>
		public void AddResult(TestResult result) 
		{
			if ( results == null )
				results = new ArrayList();

			this.results.Add(result);

            switch (result.ResultState)
            {
                case ResultState.Failure:
                case ResultState.Error:
                    if (!this.IsFailure && !this.IsError)
                        this.Failure("Child test failed", null);
                    break;
                case ResultState.Success:
                    if (this.ResultState == ResultState.Inconclusive)
                        this.Success();
                    break;
                case ResultState.Cancelled:
                    this.SetResult(ResultState.Cancelled, result.Message, null);
                    break;
            }
		}

        public string ToXml()
        {
            System.IO.StringWriter buffer = new System.IO.StringWriter();
            XmlTextWriter xml = new XmlTextWriter(buffer);

            WriteXml(xml);
           
            return buffer.ToString();
        }

        private void WriteXml(XmlTextWriter xml)
        {
            if (this.Test.IsSuite)
                xml.WriteStartElement("test-suite");
            else
                xml.WriteStartElement("test-case");

            xml.WriteAttributeString("name", this.Name);
            xml.WriteAttributeString("fullname", this.FullName);
            xml.WriteAttributeString("result", this.ResultState.ToString());
            xml.WriteAttributeString("time", string.Format("0.000", this.Time, System.Globalization.CultureInfo.InvariantCulture));

            if (this.IsFailure || this.IsError)
            {
                WriteFailureElement(xml);
            }
            else if (!this.Executed)
            {
                WriteReasonElement(xml);
            }

            if (this.HasResults)
                foreach (TestResult childResult in Results)
                    childResult.WriteXml(xml);

            xml.WriteEndElement();
        }

        private void WriteReasonElement(XmlTextWriter xml)
        {
            xml.WriteStartElement("reason");

            xml.WriteElementString("message", this.Message);

            xml.WriteEndElement();
        }

        private void WriteFailureElement(XmlTextWriter xml)
        {
            xml.WriteStartElement("failure");

            if (this.Message != null)
                xml.WriteElementString("message", this.Message);

            if (this.StackTrace != null)
                xml.WriteElementString("stacktrace", this.StackTrace);

            xml.WriteEndElement();
        }

        #endregion

        #region Exception Helpers

        private static string BuildMessage(Exception exception)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat( "{0} : {1}", exception.GetType().ToString(), exception.Message );

			Exception inner = exception.InnerException;
			while( inner != null )
			{
				sb.Append( Environment.NewLine );
				sb.AppendFormat( "  ----> {0} : {1}", inner.GetType().ToString(), inner.Message );
				inner = inner.InnerException;
			}

			return sb.ToString();
		}
		
		private static string BuildStackTrace(Exception exception)
		{
            StringBuilder sb = new StringBuilder( GetStackTrace( exception ) );

            Exception inner = exception.InnerException;
            while( inner != null )
            {
                sb.Append( Environment.NewLine );
                sb.Append( "--" );
                sb.Append( inner.GetType().Name );
                sb.Append( Environment.NewLine );
                sb.Append( GetStackTrace( inner ) );

                inner = inner.InnerException;
            }

            return sb.ToString();
		}

		private static string GetStackTrace(Exception exception)
		{
			try
			{
				return exception.StackTrace;
			}
			catch( Exception )
			{
				return "No stack trace available";
			}
		}

		#endregion
	}
}
