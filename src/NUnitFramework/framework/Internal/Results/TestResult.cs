// ***********************************************************************
// Copyright (c) 2010-2014 Charlie Poole
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
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// The TestResult class represents the result of a test.
    /// </summary>
    public abstract class TestResult : ITestResult
    {
        #region Fields

        /// <summary>
        /// Error message for when child tests have errors
        /// </summary>
        internal static readonly string CHILD_ERRORS_MESSAGE = "One or more child tests had errors";

        /// <summary>
        /// Error message for when child tests are ignored
        /// </summary>
        internal static readonly string CHILD_IGNORE_MESSAGE = "One or more child tests were ignored";

        /// <summary>
        /// The minimum duration for tests
        /// </summary>
        internal const double MIN_DURATION = 0.000001d;

//        static Logger log = InternalTrace.GetLogger("TestResult");

        /// <summary>
        /// List of child results
        /// </summary>
        private System.Collections.Generic.List<ITestResult> _children;

        private StringWriter _outWriter;
        private double _duration;

        #endregion

        #region Constructor

        /// <summary>
        /// Construct a test result given a Test
        /// </summary>
        /// <param name="test">The test to be used</param>
        public TestResult(ITest test)
        {
            this.Test = test;
            this.ResultState = ResultState.Inconclusive;
        }

        #endregion

        #region ITestResult Members

        /// <summary>
        /// Gets the test with which this result is associated.
        /// </summary>
        public ITest Test { get; private set; }

        /// <summary>
        /// Gets the ResultState of the test result, which 
        /// indicates the success or failure of the test.
        /// </summary>
        public ResultState ResultState { get; private set; }

        /// <summary>
        /// Gets the name of the test result
        /// </summary>
        public virtual string Name
        {
            get { return Test.Name; }
        }

        /// <summary>
        /// Gets the full name of the test result
        /// </summary>
        public virtual string FullName
        {
            get { return Test.FullName; }
        }

        /// <summary>
        /// Gets or sets the elapsed time for running the test in seconds
        /// </summary>
        public double Duration
        {
            get { return _duration; }
            set { _duration = value >= MIN_DURATION ? value : MIN_DURATION; }
        }

        /// <summary>
        /// Gets or sets the time the test started running.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the time the test finished running.
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Gets the message associated with a test
        /// failure or with not running the test
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Gets the escaped message associated with a test
        /// failure or with not running the test
        /// </summary>
        public string EscapedMessage
        {
            get
            {
                return EscapeInvalidXmlCharacters(Message);
            }
        }

        /// <summary>
        /// Gets any stacktrace associated with an
        /// error or failure.
        /// </summary>
        public virtual string StackTrace { get; private set; }

        /// <summary>
        /// Gets or sets the count of asserts executed
        /// when running the test.
        /// </summary>
        public int AssertCount { get; set; }

        /// <summary>
        /// Gets the number of test cases that failed
        /// when running the test and all its children.
        /// </summary>
        public abstract int FailCount { get; }

        /// <summary>
        /// Gets the number of test cases that passed
        /// when running the test and all its children.
        /// </summary>
        public abstract int PassCount { get; }

        /// <summary>
        /// Gets the number of test cases that were skipped
        /// when running the test and all its children.
        /// </summary>
        public abstract int SkipCount { get; }

        /// <summary>
        /// Gets the number of test cases that were inconclusive
        /// when running the test and all its children.
        /// </summary>
        public abstract int InconclusiveCount { get; }

        /// <summary>
        /// Indicates whether this result has any child results.
        /// Test HasChildren before accessing Children to avoid
        /// the creation of an empty collection.
        /// </summary>
        public bool HasChildren
        {
            get { return _children != null && _children.Count > 0; }
        }

        /// <summary>
        /// Gets the collection of child results.
        /// </summary>
        public System.Collections.Generic.IList<ITestResult> Children
        {
            get
            {
                if (_children == null)
                    _children = new System.Collections.Generic.List<ITestResult>();

                return _children;
            }
        }

        /// <summary>
        /// Gets a TextWriter, which will write output to be included in the result.
        /// </summary>
        public StringWriter OutWriter
        {
            get
            {
                if (_outWriter == null)
                    _outWriter = new StringWriter();

                return _outWriter;
            }
        }

        /// <summary>
        /// Gets any text output written to this result.
        /// </summary>
        public string Output
        {
            get { return _outWriter == null
                ? string.Empty
                : _outWriter.ToString();  }
        }

        #endregion

        #region IXmlNodeBuilder Members

        /// <summary>
        /// Returns the Xml representation of the result.
        /// </summary>
        /// <param name="recursive">If true, descendant results are included</param>
        /// <returns>An XmlNode representing the result</returns>
        public XmlNode ToXml(bool recursive)
        {
            XmlNode topNode = XmlNode.CreateTopLevelElement("dummy");

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
            XmlNode thisNode = this.Test.AddToXml(parentNode, false);

            thisNode.AddAttribute("result", ResultState.Status.ToString());
            if (ResultState.Label != string.Empty) // && ResultState.Label != ResultState.Status.ToString())
                thisNode.AddAttribute("label", ResultState.Label);
            if (ResultState.Site != FailureSite.Test)
                thisNode.AddAttribute("site", ResultState.Site.ToString());

            thisNode.AddAttribute("start-time", StartTime.ToString("u"));
            thisNode.AddAttribute("end-time", EndTime.ToString("u"));
            thisNode.AddAttribute("duration", Duration.ToString("0.000000", NumberFormatInfo.InvariantInfo));
            var testAssembly = TestUtility.GetTestAssembly(Test);
            if (testAssembly != null)
            {
                thisNode.AddAttribute("assembly", testAssembly.FullName);
            }

            if (this.Test is TestSuite)
            {
                thisNode.AddAttribute("total", (PassCount + FailCount + SkipCount + InconclusiveCount).ToString());
                thisNode.AddAttribute("passed", PassCount.ToString());
                thisNode.AddAttribute("failed", FailCount.ToString());
                thisNode.AddAttribute("inconclusive", InconclusiveCount.ToString());
                thisNode.AddAttribute("skipped", SkipCount.ToString());
            }

            thisNode.AddAttribute("asserts", this.AssertCount.ToString());

            switch (ResultState.Status)
            {
                case TestStatus.Failed:
                    AddFailureElement(thisNode);
                    break;
                case TestStatus.Skipped:
                    AddReasonElement(thisNode);
                    break;
                case TestStatus.Passed:
                case TestStatus.Inconclusive:
                    if (this.Message != null)
                        AddReasonElement(thisNode);
                    break;
            }

            if (Output.Length > 0)
                AddOutputElement(thisNode);


            if (recursive && HasChildren)
                foreach (TestResult child in Children)
                    child.AddToXml(thisNode, recursive);

            return thisNode;
        }

        #endregion

        /// <summary>
        /// Adds a child result to this result, setting this result's
        /// ResultState to Failure if the child result failed.
        /// </summary>
        /// <param name="result">The result to be added</param>
        public virtual void AddResult(ITestResult result)
        {
            this.Children.Add(result);

            //this.AssertCount += result.AssertCount;

            // If this result is marked cancelled, don't change it
            if (this.ResultState != ResultState.Cancelled)
                switch (result.ResultState.Status)
                {
                    case TestStatus.Passed:

                        if (this.ResultState.Status == TestStatus.Inconclusive)
                            this.SetResult(ResultState.Success);

                        break;

                    case TestStatus.Failed:


                        if (this.ResultState.Status != TestStatus.Failed)
                            this.SetResult(ResultState.ChildFailure, CHILD_ERRORS_MESSAGE);

                        break;

                    case TestStatus.Skipped:

                        if (result.ResultState.Label == "Ignored")
                            if (this.ResultState.Status == TestStatus.Inconclusive || this.ResultState.Status == TestStatus.Passed)
                                this.SetResult(ResultState.Ignored, CHILD_IGNORE_MESSAGE);

                        break;

                    default:
                        break;
                }
        }

        #region Other Public Methods

        /// <summary>
        /// Set the result of the test
        /// </summary>
        /// <param name="resultState">The ResultState to use in the result</param>
        public void SetResult(ResultState resultState)
        {
            SetResult(resultState, null, null);
        }

        /// <summary>
        /// Set the result of the test
        /// </summary>
        /// <param name="resultState">The ResultState to use in the result</param>
        /// <param name="message">A message associated with the result state</param>
        public void SetResult(ResultState resultState, string message)
        {
            SetResult(resultState, message, null);
        }

        /// <summary>
        /// Set the result of the test
        /// </summary>
        /// <param name="resultState">The ResultState to use in the result</param>
        /// <param name="message">A message associated with the result state</param>
        /// <param name="stackTrace">Stack trace giving the location of the command</param>
        public void SetResult(ResultState resultState, string message, string stackTrace)
        {
            this.ResultState = resultState;
            this.Message = message;
            this.StackTrace = stackTrace;

            // Set pseudo-counts for a test case
            //if (IsTestCase(this.test))
            //{
            //    this.passCount = 0;
            //    this.failCount = 0;
            //    this.skipCount = 0;
            //    this.inconclusiveCount = 0;

            //    switch (this.ResultState.Status)
            //    {
            //        case TestStatus.Passed:
            //            this.passCount++;
            //            break;
            //        case TestStatus.Failed:
            //            this.failCount++;
            //            break;
            //        case TestStatus.Skipped:
            //            this.skipCount++;
            //            break;
            //        default:
            //        case TestStatus.Inconclusive:
            //            this.inconclusiveCount++;
            //            break;
            //    }
            //}
        }

        /// <summary>
        /// Set the test result based on the type of exception thrown
        /// </summary>
        /// <param name="ex">The exception that was thrown</param>
        public void RecordException(Exception ex)
        {
            if (ex is NUnitException)
                ex = ex.InnerException;

            if (ex is ResultStateException)
                SetResult(((ResultStateException)ex).ResultState,
                    ex.Message,
                    StackFilter.Filter(ex.StackTrace));
#if !PORTABLE
            else if (ex is System.Threading.ThreadAbortException)
                SetResult(ResultState.Cancelled,
                    "Test cancelled by user",
                    ex.StackTrace);
#endif
            else
                SetResult(ResultState.Error,
                    ExceptionHelper.BuildMessage(ex),
                    ExceptionHelper.BuildStackTrace(ex));
        }

        /// <summary>
        /// Set the test result based on the type of exception thrown
        /// </summary>
        /// <param name="ex">The exception that was thrown</param>
        /// <param name="site">THe FailureSite to use in the result</param>
        public void RecordException(Exception ex, FailureSite site)
        {
            if (ex is NUnitException)
                ex = ex.InnerException;

            if (ex is ResultStateException)
                SetResult(((ResultStateException)ex).ResultState.WithSite(site),
                    ex.Message,
                    StackFilter.Filter(ex.StackTrace));
#if !PORTABLE
            else if (ex is System.Threading.ThreadAbortException)
                SetResult(ResultState.Cancelled.WithSite(site),
                    "Test cancelled by user",
                    ex.StackTrace);
#endif
            else
                SetResult(ResultState.Error.WithSite(site),
                    ExceptionHelper.BuildMessage(ex),
                    ExceptionHelper.BuildStackTrace(ex));
        }

        /// <summary>
        /// RecordTearDownException appends the message and stacktrace
        /// from an exception arising during teardown of the test
        /// to any previously recorded information, so that any
        /// earlier failure information is not lost. Note that
        /// calling Assert.Ignore, Assert.Inconclusive, etc. during
        /// teardown is treated as an error. If the current result
        /// represents a suite, it may show a teardown error even
        /// though all contained tests passed.
        /// </summary>
        /// <param name="ex">The Exception to be recorded</param>
        public void RecordTearDownException(Exception ex)
        {
            if (ex is NUnitException)
                ex = ex.InnerException;

            ResultState resultState = this.ResultState == ResultState.Cancelled
                ? ResultState.Cancelled
                : ResultState.Error;
            if (Test.IsSuite)
                resultState = resultState.WithSite(FailureSite.TearDown);

            string message = "TearDown : " + ExceptionHelper.BuildMessage(ex);
            if (this.Message != null)
                message = this.Message + NUnit.Env.NewLine + message;

            string stackTrace = "--TearDown" + NUnit.Env.NewLine + ExceptionHelper.BuildStackTrace(ex);
            if (this.StackTrace != null)
                stackTrace = this.StackTrace + NUnit.Env.NewLine + stackTrace;

            SetResult(resultState, message, stackTrace);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Adds a reason element to a node and returns it.
        /// </summary>
        /// <param name="targetNode">The target node.</param>
        /// <returns>The new reason element.</returns>
        private XmlNode AddReasonElement(XmlNode targetNode)
        {
            XmlNode reasonNode = targetNode.AddElement("reason");
            reasonNode.AddElement("message").TextContent = this.EscapedMessage;
            return reasonNode;
        }

        /// <summary>
        /// Adds a failure element to a node and returns it.
        /// </summary>
        /// <param name="targetNode">The target node.</param>
        /// <returns>The new failure element.</returns>
        private XmlNode AddFailureElement(XmlNode targetNode)
        {
            XmlNode failureNode = targetNode.AddElement("failure");

            if (this.Message != null)
            {
                failureNode.AddElement("message").TextContent = this.EscapedMessage;
            }

            if (this.StackTrace != null)
            {
                failureNode.AddElement("stack-trace").TextContent = this.StackTrace;
            }

            return failureNode;
        }

        private XmlNode AddOutputElement(XmlNode targetNode)
        {
            XmlNode outputNode = targetNode.AddElement("output");
            outputNode.TextContent = this.Output;

            return outputNode;
        }

        static string EscapeInvalidXmlCharacters(string str)
        {
            // Based on the XML spec http://www.w3.org/TR/xml/#charsets
            // For detailed explanation of the regex see http://mnaoumov.wordpress.com/2014/06/15/escaping-invalid-xml-unicode-characters/

            var invalidXmlCharactersRegex = new Regex("[^\u0009\u000a\u000d\u0020-\ufffd]|([\ud800-\udbff](?![\udc00-\udfff]))|((?<![\ud800-\udbff])[\udc00-\udfff])");
            return invalidXmlCharactersRegex.Replace(str, match => CharToUnicodeSequence(match.Value[0]));
        }

        static string CharToUnicodeSequence(char symbol)
        {
            return string.Format("\\u{0}", ((int) symbol).ToString("x4"));
        }

        #endregion
    }
}
