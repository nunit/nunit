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
using System.Collections.Generic;
#if PARALLEL
using System.Collections.Concurrent;
#endif
using System.Globalization;
using System.IO;
using System.Text;
#if NETCF || NET_2_0
using NUnit.Framework.Compatibility;
#endif
using System.Threading;
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

        private StringBuilder _output = new StringBuilder();
        private double _duration;

        /// <summary>
        /// Aggregate assertion count
        /// </summary>
        protected int InternalAssertCount;

        private ResultState _resultState;
        private string _message;
        private string _stackTrace;

#if PARALLEL
        /// <summary>
        /// ReaderWriterLock
        /// </summary>
#if NET_2_0
        protected ReaderWriterLock RwLock = new ReaderWriterLock();
#elif NETCF
        protected ReaderWriterLockSlim RwLock = new ReaderWriterLockSlim();
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

#if PORTABLE || SILVERLIGHT
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
        public ITest Test { get; private set; }

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
        /// Gets any stacktrace associated with an
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
        public TextWriter OutWriter { get; private set; }

        /// <summary>
        /// Gets any text output written to this result.
        /// </summary>
        public string Output
        {
            get { return _output.ToString(); }
        }

        #endregion

        #region IXmlNodeBuilder Members

        /// <summary>
        /// Returns the Xml representation of the result.
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
                    if (Message != null)
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

            // Set pseudo-counts for a test case
            //if (IsTestCase(test))
            //{
            //    passCount = 0;
            //    failCount = 0;
            //    skipCount = 0;
            //    inconclusiveCount = 0;

            //    switch (ResultState.Status)
            //    {
            //        case TestStatus.Passed:
            //            passCount++;
            //            break;
            //        case TestStatus.Failed:
            //            failCount++;
            //            break;
            //        case TestStatus.Skipped:
            //            skipCount++;
            //            break;
            //        default:
            //        case TestStatus.Inconclusive:
            //            inconclusiveCount++;
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

            ResultState resultState = ResultState == ResultState.Cancelled
                ? ResultState.Cancelled
                : ResultState.Error;
            if (Test.IsSuite)
                resultState = resultState.WithSite(FailureSite.TearDown);

            string message = "TearDown : " + ExceptionHelper.BuildMessage(ex);
            if (Message != null)
                message = Message + NUnit.Env.NewLine + message;

            string stackTrace = "--TearDown" + NUnit.Env.NewLine + ExceptionHelper.BuildStackTrace(ex);
            if (StackTrace != null)
                stackTrace = StackTrace + NUnit.Env.NewLine + stackTrace;

            SetResult(resultState, message, stackTrace);
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

            if (Message != null)
                failureNode.AddElementWithCDATA("message", Message);

            if (StackTrace != null)
                failureNode.AddElementWithCDATA("stack-trace", StackTrace);

            return failureNode;
        }

        private TNode AddOutputElement(TNode targetNode)
        {
            return targetNode.AddElementWithCDATA("output", Output);
        }

        #endregion
    }
}
