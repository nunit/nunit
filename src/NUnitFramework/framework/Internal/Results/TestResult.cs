// ***********************************************************************
// Copyright (c) 2010-2014 Charlie Poole, Rob Prouse
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
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using NUnit.Compatibility;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// The TestResult class represents the result of a test.
    /// </summary>
    public abstract class TestResult : LongLivedMarshalByRefObject, ITestResult
    {
        #region Fields

        /// <summary>
        /// Error message for when child tests have errors
        /// </summary>
        internal static readonly string CHILD_ERRORS_MESSAGE = "One or more child tests had errors";

        /// <summary>
        /// Error message for when child tests have warnings
        /// </summary>
        internal static readonly string CHILD_WARNINGS_MESSAGE = "One or more child tests had warnings";

        /// <summary>
        /// Error message for when child tests are ignored
        /// </summary>
        internal static readonly string CHILD_IGNORE_MESSAGE = "One or more child tests were ignored";

        /// <summary>
        /// The minimum duration for tests
        /// </summary>
        internal const double MIN_DURATION = 0.000001d;

        //        static Logger log = InternalTrace.GetLogger("TestResult");

        private readonly StringBuilder _output = new StringBuilder();
        private double _duration;

        /// <summary>
        /// Aggregate assertion count
        /// </summary>
        protected int InternalAssertCount;

        private ResultState _resultState;
        private string _message;
        private string _stackTrace;

        private readonly List<AssertionResult> _assertionResults = new List<AssertionResult>();
        private readonly List<TestAttachment> _testAttachments = new List<TestAttachment>();

#if PARALLEL
        /// <summary>
        /// ReaderWriterLock
        /// </summary>
#if NET20
        protected ReaderWriterLock RwLock = new ReaderWriterLock();
#else
        protected ReaderWriterLockSlim RwLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
#endif
#endif

        #endregion

        #region Constructor

        /// <summary>
        /// Construct a test result given a Test
        /// </summary>
        /// <param name="test">The test to be used</param>
        public TestResult(ITest test)
        {
            Test = test;
            ResultState = ResultState.Inconclusive;

#if NETSTANDARD1_6
            OutWriter = new StringWriter(_output);
#else
            OutWriter = TextWriter.Synchronized(new StringWriter(_output));
#endif
        }

        #endregion

        #region ITestResult Members

        /// <summary>
        /// Gets the test with which this result is associated.
        /// </summary>
        public ITest Test { get; }

        /// <summary>
        /// Gets the ResultState of the test result, which
        /// indicates the success or failure of the test.
        /// </summary>
        public ResultState ResultState
        {
            get
            {
#if PARALLEL
                RwLock.EnterReadLock();
#endif
                try
                {
                    return _resultState;
                }
                finally
                {
#if PARALLEL
                    RwLock.ExitReadLock();
#endif
                }
            }
            private set { _resultState = value; }
        }

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
        /// Adds a test attachment to the test result
        /// </summary>
        /// <param name="attachment">The TestAttachment object to attach</param>
        internal void AddTestAttachment(TestAttachment attachment)
        {
            _testAttachments.Add(attachment);
        }

        /// <summary>
        /// Gets the collection of files attached to the test
        /// </summary>
        public ICollection<TestAttachment> TestAttachments => new ReadOnlyCollection<TestAttachment>(_testAttachments);

        /// <summary>
        /// Gets the message associated with a test
        /// failure or with not running the test
        /// </summary>
        public string Message
        {
            get
            {
#if PARALLEL
                RwLock.EnterReadLock();
#endif
                try
                {
                    return _message;
                }
                finally
                {
#if PARALLEL
                    RwLock.ExitReadLock();
#endif
                }

            }
            private set
            {
                _message = value;
            }
        }

        /// <summary>
        /// Gets any stack trace associated with an
        /// error or failure.
        /// </summary>
        public virtual string StackTrace
        {
            get
            {
#if PARALLEL
                RwLock.EnterReadLock();
#endif
                try
                {
                    return _stackTrace;
                }
                finally
                {
#if PARALLEL
                    RwLock.ExitReadLock();
#endif
                }
            }

            private set
            {
                _stackTrace = value;
            }
        }

        /// <summary>
        /// Gets or sets the count of asserts executed
        /// when running the test.
        /// </summary>
        public int AssertCount
        {
            get
            {
#if PARALLEL
                RwLock.EnterReadLock();
#endif
                try
                {
                    return InternalAssertCount;
                }
                finally
                {
#if PARALLEL
                    RwLock.ExitReadLock ();
#endif
                }
            }

            internal set
            {
                InternalAssertCount = value;
            }
        }

        /// <summary>
        /// Gets the number of test cases that failed
        /// when running the test and all its children.
        /// </summary>
        public abstract int FailCount { get; }

        /// <summary>
        /// Gets the number of test cases that had warnings
        /// when running the test and all its children.
        /// </summary>
        public abstract int WarningCount { get; }

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
        /// </summary>
        public abstract bool HasChildren { get; }

        /// <summary>
        /// Gets the collection of child results.
        /// </summary>
        public abstract IEnumerable<ITestResult> Children { get; }

        /// <summary>
        /// Gets a TextWriter, which will write output to be included in the result.
        /// </summary>
        public TextWriter OutWriter { get; }

        /// <summary>
        /// Gets any text output written to this result.
        /// </summary>
        public string Output
        {
            get { return _output.ToString(); }
        }

        /// <summary>
        /// Gets a list of assertion results associated with the test.
        /// </summary>
        public IList<AssertionResult> AssertionResults
        {
            get { return _assertionResults; }
        }

        #endregion

        #region IXmlNodeBuilder Members

        /// <summary>
        /// Returns the XML representation of the result.
        /// </summary>
        /// <param name="recursive">If true, descendant results are included</param>
        /// <returns>An XmlNode representing the result</returns>
        public TNode ToXml(bool recursive)
        {
            return AddToXml(new TNode("dummy"), recursive);
        }

        /// <summary>
        /// Adds the XML representation of the result as a child of the
        /// supplied parent node..
        /// </summary>
        /// <param name="parentNode">The parent node.</param>
        /// <param name="recursive">If true, descendant results are included</param>
        /// <returns></returns>
        public virtual TNode AddToXml(TNode parentNode, bool recursive)
        {
            // A result node looks like a test node with extra info added
            TNode thisNode = Test.AddToXml(parentNode, false);

            thisNode.AddAttribute("result", ResultState.Status.ToString());
            if (ResultState.Label != string.Empty) // && ResultState.Label != ResultState.Status.ToString())
                thisNode.AddAttribute("label", ResultState.Label);
            if (ResultState.Site != FailureSite.Test)
                thisNode.AddAttribute("site", ResultState.Site.ToString());

            thisNode.AddAttribute("start-time", StartTime.ToString("u"));
            thisNode.AddAttribute("end-time", EndTime.ToString("u"));
            thisNode.AddAttribute("duration", Duration.ToString("0.000000", NumberFormatInfo.InvariantInfo));

            if (Test is TestSuite)
            {
                thisNode.AddAttribute("total", (PassCount + FailCount + SkipCount + InconclusiveCount).ToString());
                thisNode.AddAttribute("passed", PassCount.ToString());
                thisNode.AddAttribute("failed", FailCount.ToString());
                thisNode.AddAttribute("warnings", WarningCount.ToString());
                thisNode.AddAttribute("inconclusive", InconclusiveCount.ToString());
                thisNode.AddAttribute("skipped", SkipCount.ToString());
            }

            thisNode.AddAttribute("asserts", AssertCount.ToString());

            switch (ResultState.Status)
            {
                case TestStatus.Failed:
                    AddFailureElement(thisNode);
                    break;
                case TestStatus.Skipped:
                case TestStatus.Passed:
                case TestStatus.Inconclusive:
                case TestStatus.Warning:
                    if (Message != null && Message.Trim().Length > 0)
                        AddReasonElement(thisNode);
                    break;
            }

            if (Output.Length > 0)
                AddOutputElement(thisNode);

            if (AssertionResults.Count > 0)
                AddAssertionsElement(thisNode);

            if (_testAttachments.Count > 0)
                AddAttachmentsElement(thisNode);

            if (recursive && HasChildren)
                foreach (TestResult child in Children)
                    child.AddToXml(thisNode, recursive);

            return thisNode;
        }

        #endregion

        #region Other Public Properties

        /// <summary>
        /// Gets a count of pending failures (from Multiple Assert)
        /// </summary>
        public int PendingFailures
        {
            get { return AssertionResults.Count(ar => ar.Status == AssertionStatus.Failed); }
        }

        /// <summary>
        /// Gets the worst assertion status (highest enum) in all the assertion results
        /// </summary>
        public AssertionStatus WorstAssertionStatus
        {
            get { return AssertionResults.Aggregate((ar1, ar2) => ar1.Status > ar2.Status ? ar1 : ar2).Status; }
        }

        #endregion

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
#if PARALLEL
            RwLock.EnterWriteLock();
#endif
            try
            {
                ResultState = resultState;
                Message = message;
                StackTrace = stackTrace;
            }
            finally
            {
#if PARALLEL
                RwLock.ExitWriteLock();
#endif
            }
        }

        /// <summary>
        /// Set the test result based on the type of exception thrown
        /// </summary>
        /// <param name="ex">The exception that was thrown</param>
        public void RecordException(Exception ex)
        {
            var result = new ExceptionResult(ex, FailureSite.Test);

            SetResult(result.ResultState, result.Message, result.StackTrace);

            if (AssertionResults.Count > 0 && result.ResultState == ResultState.Error)
            {
                // Add pending failures to the legacy result message
                Message += CreateLegacyFailureMessage();

                // Add to the list of assertion errors, so that newer runners will see it
                AssertionResults.Add(new AssertionResult(AssertionStatus.Error, result.Message, result.StackTrace));
            }
        }

        /// <summary>
        /// Set the test result based on the type of exception thrown
        /// </summary>
        /// <param name="ex">The exception that was thrown</param>
        /// <param name="site">The FailureSite to use in the result</param>
        public void RecordException(Exception ex, FailureSite site)
        {
            var result = new ExceptionResult(ex, site);

            SetResult(result.ResultState, result.Message, result.StackTrace);
        }

        /// <summary>
        /// RecordTearDownException appends the message and stack trace
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
            ex = ValidateAndUnwrap(ex);

            ResultState resultState = ResultState == ResultState.Cancelled
                ? ResultState.Cancelled
                : ResultState.Error;
            if (Test.IsSuite)
                resultState = resultState.WithSite(FailureSite.TearDown);

            string message = "TearDown : " + ExceptionHelper.BuildMessage(ex);
            if (Message != null)
                message = Message + Environment.NewLine + message;

            string stackTrace = "--TearDown" + Environment.NewLine + ExceptionHelper.BuildStackTrace(ex);
            if (StackTrace != null)
                stackTrace = StackTrace + Environment.NewLine + stackTrace;

            SetResult(resultState, message, stackTrace);
        }

        private static Exception ValidateAndUnwrap(Exception ex)
        {
            Guard.ArgumentNotNull(ex, nameof(ex));

            if ((ex is NUnitException || ex is TargetInvocationException) && ex.InnerException != null)
                return ex.InnerException;

            return ex;
        }

        private struct ExceptionResult
        {
            public ResultState ResultState { get; }
            public string Message { get; }
            public string StackTrace { get; }

            public ExceptionResult(Exception ex, FailureSite site)
            {
                ex = ValidateAndUnwrap(ex);

                if (ex is ResultStateException)
                {
                    ResultState = ((ResultStateException)ex).ResultState.WithSite(site);
                    Message = ex.Message;
                    StackTrace = StackFilter.DefaultFilter.Filter(ex.StackTrace);
                }
#if !NETSTANDARD1_6
                else if (ex is ThreadAbortException)
                {
                    ResultState = ResultState.Cancelled.WithSite(site);
                    Message = "Test cancelled by user";
                    StackTrace = ex.StackTrace;
                }
#endif
                else
                {
                    ResultState = ResultState.Error.WithSite(site);
                    Message = ExceptionHelper.BuildMessage(ex);
                    StackTrace = ExceptionHelper.BuildStackTrace(ex);
                }
            }
        }

        /// <summary>
        /// Update overall test result, including legacy Message, based
        /// on AssertionResults that have been saved to this point.
        /// </summary>
        public void RecordTestCompletion()
        {
            switch (AssertionResults.Count)
            {
                case 0:
                    SetResult(ResultState.Success);
                    break;
                case 1:
                    SetResult(
                        AssertionStatusToResultState(AssertionResults[0].Status),
                        AssertionResults[0].Message,
                        AssertionResults[0].StackTrace);
                    break;
                default:
                    SetResult(
                        AssertionStatusToResultState(WorstAssertionStatus),
                        CreateLegacyFailureMessage());
                    break;
            }
        }

        /// <summary>
        /// Record an assertion result
        /// </summary>
        public void RecordAssertion(AssertionResult assertion)
        {
            _assertionResults.Add(assertion);
        }

        /// <summary>
        /// Record an assertion result
        /// </summary>
        public void RecordAssertion(AssertionStatus status, string message, string stackTrace)
        {
            RecordAssertion(new AssertionResult(status, message, stackTrace));
        }

        /// <summary>
        /// Record an assertion result
        /// </summary>
        public void RecordAssertion(AssertionStatus status, string message)
        {
            RecordAssertion(status, message, null);
        }


        /// <summary>
        /// Creates a failure message incorporating failures
        /// from a Multiple Assert block for use by runners
        /// that don't know about AssertionResults.
        /// </summary>
        /// <returns>Message as a string</returns>
        private string CreateLegacyFailureMessage()
        {
            var writer = new StringWriter();

            if (AssertionResults.Count > 1)
                writer.WriteLine("Multiple failures or warnings in test:");

            int counter = 0;
            foreach (var assertion in AssertionResults)
                writer.WriteLine(string.Format("  {0}) {1}", ++counter, assertion.Message));

            return writer.ToString();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Adds a reason element to a node and returns it.
        /// </summary>
        /// <param name="targetNode">The target node.</param>
        /// <returns>The new reason element.</returns>
        private TNode AddReasonElement(TNode targetNode)
        {
            TNode reasonNode = targetNode.AddElement("reason");
            return reasonNode.AddElementWithCDATA("message", Message);
        }

        /// <summary>
        /// Adds a failure element to a node and returns it.
        /// </summary>
        /// <param name="targetNode">The target node.</param>
        /// <returns>The new failure element.</returns>
        private TNode AddFailureElement(TNode targetNode)
        {
            TNode failureNode = targetNode.AddElement("failure");

            if (Message != null && Message.Trim().Length > 0)
                failureNode.AddElementWithCDATA("message", Message);

            if (StackTrace != null && StackTrace.Trim().Length > 0)
                failureNode.AddElementWithCDATA("stack-trace", StackTrace);

            return failureNode;
        }

        private TNode AddOutputElement(TNode targetNode)
        {
            return targetNode.AddElementWithCDATA("output", Output);
        }

        private TNode AddAssertionsElement(TNode targetNode)
        {
            var assertionsNode = targetNode.AddElement("assertions");

            foreach (var assertion in AssertionResults)
            {
                TNode assertionNode = assertionsNode.AddElement("assertion");
                assertionNode.AddAttribute("result", assertion.Status.ToString());
                if (assertion.Message != null)
                    assertionNode.AddElementWithCDATA("message", assertion.Message);
                if (assertion.StackTrace != null)
                    assertionNode.AddElementWithCDATA("stack-trace", assertion.StackTrace);
            }

            return assertionsNode;
        }

        private ResultState AssertionStatusToResultState(AssertionStatus status)
        {
            switch (status)
            {
                case AssertionStatus.Inconclusive:
                    return ResultState.Inconclusive;
                default:
                case AssertionStatus.Passed:
                    return ResultState.Success;
                case AssertionStatus.Warning:
                    return ResultState.Warning;
                case AssertionStatus.Failed:
                    return ResultState.Failure;
                case AssertionStatus.Error:
                    return ResultState.Error;
            }
        }

        /// <summary>
        /// Adds an attachments element to a node and returns it.
        /// </summary>
        /// <param name="targetNode">The target node.</param>
        /// <returns>The new attachments element.</returns>
        private TNode AddAttachmentsElement(TNode targetNode)
        {
            TNode attachmentsNode = targetNode.AddElement("attachments");

            foreach (var attachment in _testAttachments)
            {
                var attachmentNode = attachmentsNode.AddElement("attachment");

                attachmentNode.AddElement("filePath", attachment.FilePath);

                if (attachment.Description != null)
                    attachmentNode.AddElementWithCDATA("description", attachment.Description);
            }

            return attachmentsNode;
        }

        #endregion
    }
}
