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
using System.Globalization;
using System.Xml;
using NUnit.Framework.Api;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// The TestResult class represents the result of a test.
    /// </summary>
    public abstract class TestResult : ITestResult
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
		protected readonly ITest test;

#if !NETCF_1_0
		/// <summary>
		/// The stacktrace at the point of failure
		/// </summary>
		private string stackTrace;
#endif

		/// <summary>
		/// Message giving the reason for failure, error or skipping the test
		/// </summary>
		private string message;

		/// <summary>
		/// Number of asserts executed by this test
		/// </summary>
		private int assertCount = 0;

        /// <summary>
        /// List of child results
        /// </summary>
#if CLR_2_0 || CLR_4_0
        private System.Collections.Generic.List<ITestResult> children;
#else
        private System.Collections.ArrayList children;
#endif

        #endregion

		#region Constructor

		/// <summary>
		/// Construct a test result given a Test
		/// </summary>
		/// <param name="test">The test to be used</param>
		public TestResult(ITest test)
		{
			this.test = test;
            this.resultState = ResultState.Inconclusive;
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
        /// Gets or sets the elapsed time for running the test
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

#if !NETCF_1_0
        /// <summary>
        /// Gets any stacktrace associated with an
        /// error or failure. Not available in
        /// the Compact Framework 1.0.
        /// </summary>
        public virtual string StackTrace
        {
            get { return stackTrace; }
        }
#endif

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
        /// Indicates whether this result has any child results.
        /// Test HasChildren before accessing Children to avoid
        /// the creation of an empty collection.
        /// </summary>
        public bool HasChildren
        {
            get { return children != null && children.Count > 0; }
        }

        /// <summary>
        /// Gets the collection of child results.
        /// </summary>
#if CLR_2_0 || CLR_4_0
        public System.Collections.Generic.IList<ITestResult> Children
        {
            get
            {
                if (children == null)
                    children = new System.Collections.Generic.List<ITestResult>();

                return children;
            }
        }
#else
        public System.Collections.IList Children
        {
            get 
            {
                if (children == null)
                    children = new System.Collections.ArrayList();

                return children; 
            }
        }
#endif

        #endregion

        #region Public Methods

        /// <summary>
        /// Set the result of the test
        /// </summary>
        /// <param name="resultState">The ResultState to use in the result</param>
        public void SetResult(ResultState resultState)
        {
            this.resultState = resultState;
            this.message = null;
        }

        /// <summary>
        /// Set the result of the test
        /// </summary>
        /// <param name="resultState">The ResultState to use in the result</param>
        /// <param name="message">A message associated with the result state</param>
        public void SetResult(ResultState resultState, string message)
        {
            this.resultState = resultState;
            this.message = message;
        }

#if !NETCF_1_0
        /// <summary>
        /// Set the result of the test
        /// </summary>
        /// <param name="resultState">The ResultState to use in the result</param>
        /// <param name="message">A message associated with the result state</param>
        /// <param name="stackTrace">Stack trace giving the location of the command</param>
        public void SetResult(ResultState resultState, string message, string stackTrace)
        {
            this.resultState = resultState;
            this.message = message;
            this.stackTrace = stackTrace;
        }
#endif

        /// <summary>
        /// Set the test result based on the type of exception thrown
        /// </summary>
        /// <param name="ex">The exception that was thrown</param>
        public void RecordException(Exception ex)
        {
            if (ex is NUnitException)
                ex = ex.InnerException;

#if !NETCF_1_0
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
                SetResult(ResultState.Error, 
                    ExceptionHelper.BuildMessage(ex), 
                    ExceptionHelper.BuildStackTrace(ex));
#else
            if (ex is AssertionException)
                SetResult(ResultState.Failure, ex.Message);
            else if (ex is IgnoreException)
                SetResult(ResultState.Ignored, ex.Message);
            else if (ex is InconclusiveException)
                SetResult(ResultState.Inconclusive, ex.Message);
            else if (ex is SuccessException)
                SetResult(ResultState.Success, ex.Message);
            else
                SetResult(ResultState.Error, ExceptionHelper.BuildMessage(ex));
#endif
        }

        /// <summary>
        /// Returns the Xml representation of the result.
        /// </summary>
        /// <param name="recursive">If true, descendant results are included</param>
        /// <returns>An XmlNode representing the result</returns>
        public XmlNode ToXml(bool recursive)
        {
            XmlNode topNode = XmlHelper.CreateTopLevelElement("dummy");

            AddToXml(topNode, recursive);

            return topNode.FirstChild;
        }

        /// <summary>
        /// Adds the XML representation of the result as a child of the
        /// supplied parent node..
        /// </summary>
        /// <param name="parentNode">The parent node.</param>
        /// <param name="recursive">If true, descendant results are included</param>
        /// <returns></returns>
        public virtual XmlNode AddToXml(XmlNode parentNode, bool recursive)
        {
            // A result node looks like a test node with extra info added
            XmlNode thisNode = this.test.AddToXml(parentNode, false);

            XmlHelper.AddAttribute(thisNode, "result", ResultState.Status.ToString());
            if (ResultState.Label != ResultState.Status.ToString())
                XmlHelper.AddAttribute(thisNode, "label", ResultState.Label);

            XmlHelper.AddAttribute(thisNode, "time", this.Time.ToString("0.000", System.Globalization.CultureInfo.InvariantCulture));

            switch (ResultState.Status)
            {
                case TestStatus.Failed:
                    AddFailureElement(thisNode);
                    break;
                case TestStatus.Skipped:
                    AddReasonElement(thisNode);
                    break;
                case TestStatus.Passed:
                    break;
                case TestStatus.Inconclusive:
                    break;
            }

            return thisNode;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Adds a reason element to a note and returns it.
        /// </summary>
        /// <param name="targetNode">The target node.</param>
        /// <returns>The new reason element.</returns>
        private XmlNode AddReasonElement(XmlNode targetNode)
        {
            XmlNode reasonNode = XmlHelper.AddElement(targetNode, "reason");
            XmlHelper.AddElementWithCDataSection(reasonNode, "message", this.Message);
            return reasonNode;
        }

        /// <summary>
        /// Adds a failure element to a node and returns it.
        /// </summary>
        /// <param name="targetNode">The target node.</param>
        /// <returns>The new failure element.</returns>
        private XmlNode AddFailureElement(XmlNode targetNode)
        {
            XmlNode failureNode = XmlHelper.AddElement(targetNode, "failure");

            if (this.Message != null)
            {
                XmlHelper.AddElementWithCDataSection(failureNode, "message", this.Message);
            }

#if !NETCF_1_0
            if (this.StackTrace != null)
            {
                XmlHelper.AddElementWithCDataSection(failureNode, "stack-trace", this.StackTrace);
            }
#endif 

            return failureNode;
        }

        #endregion
    }
}
