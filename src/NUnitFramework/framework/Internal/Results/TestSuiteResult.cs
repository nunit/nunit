// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;
using System.Collections.Concurrent;
using NUnit.Framework.Interfaces;
using System;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Represents the result of running a test suite
    /// </summary>
    public class TestSuiteResult : TestResult
    {
        private int _passCount = 0;
        private int _failCount = 0;
        private int _warningCount = 0;
        private int _skipCount = 0;
        private int _inconclusiveCount = 0;
        private int _totalCount = 0;
        private readonly ConcurrentQueue<ITestResult> _children = new();

        /// <summary>
        /// Construct a TestSuiteResult base on a TestSuite
        /// </summary>
        /// <param name="suite">The TestSuite to which the result applies</param>
        public TestSuiteResult(TestSuite suite) : base(suite)
        {
        }

        /// <summary>
        /// <inheritdoc cref="TestResult"/>
        /// </summary>
        /// <param name="other">The TestSuite from which the result shall be copied</param>
        private TestSuiteResult(TestSuiteResult other) : base(other)
        {
            _passCount = other._passCount;
            _failCount = other._failCount;
            _warningCount = other._warningCount;
            _skipCount = other._skipCount;
            _inconclusiveCount = other._inconclusiveCount;
            _totalCount = other._totalCount;
            _children = new ConcurrentQueue<ITestResult>(other._children);
        }

        /// <summary>
        /// <inheritdoc cref="TestResult"/>
        /// </summary>
        /// <param name="latest">The latest result.</param>
        /// <param name="previous">The previous result.</param>
        private TestSuiteResult(TestSuiteResult latest, TestResult previous) : base(latest, previous)
        {
            _passCount = latest._passCount - previous.PassCount;
            _failCount = latest._failCount - previous.FailCount;
            _warningCount = latest._warningCount - previous.WarningCount;
            _skipCount = latest._skipCount - previous.SkipCount;
            _inconclusiveCount = latest._inconclusiveCount - previous.InconclusiveCount;
            _totalCount = latest._totalCount - previous.TotalCount;
        }

        /// <summary>
        /// <inheritdoc cref="TestResult.Clone"/>
        /// </summary>
        public override TestResult Clone()
        {
            return new TestSuiteResult(this);
        }

        #region Overrides

        /// <summary>
        /// Gets the total number of test cases enumerated
        /// when running the test and all its children.
        /// </summary>
        /// <remarks>
        /// Test cases excluded by <see cref="CategoryAttribute"/>
        /// are excluded from this count.
        /// </remarks>
        public override int TotalCount
        {
            get
            {
                RwLock.EnterReadLock();
                try
                {
                    return _totalCount;
                }
                finally
                {
                    RwLock.ExitReadLock();
                }
            }
        }

        /// <summary>
        /// Gets the number of test cases that failed
        /// when running the test and all its children.
        /// </summary>
        /// <remarks>
        /// Count reflects test cases that return with
        /// <see cref="Assert.Fail()"/>,
        /// <see cref="Assert.Fail(string)"/>
        /// as well as test cases that throw on an
        /// assertion.
        /// </remarks>
        public override int FailCount
        {
            get
            {
                RwLock.EnterReadLock();
                try
                {
                    return _failCount;
                }
                finally
                {
                    RwLock.ExitReadLock();
                }
            }
        }

        /// <summary>
        /// Gets the number of test cases that had warnings
        /// when running the test and all its children.
        /// </summary>
        /// <remarks>
        /// Count reflects test cases that return with
        /// <see cref="Assert.Warn(string)"/>.
        /// </remarks>
        public override int WarningCount
        {
            get
            {
                RwLock.EnterReadLock();
                try
                {
                    return _warningCount;
                }
                finally
                {
                    RwLock.ExitReadLock();
                }
            }
        }

        /// <summary>
        /// Gets the number of test cases that passed
        /// when running the test and all its children.
        /// </summary>
        /// <remarks>
        /// Count reflects test cases that return with
        /// <see cref="Assert.Pass()"/>,
        /// <see cref="Assert.Pass(string)"/>,
        /// <see cref="Assert.Charlie()"/>,
        /// as well as test cases that return without an
        /// assertion.
        /// </remarks>
        public override int PassCount
        {
            get
            {
                RwLock.EnterReadLock();
                try
                {
                    return _passCount;
                }
                finally
                {
                    RwLock.ExitReadLock();
                }
            }
        }

        /// <summary>
        /// Gets the number of test cases that were skipped
        /// when running the test and all its children.
        /// </summary>
        /// <remarks>
        /// Count reflects test cases that return with
        /// <see cref="Assert.Ignore()"/> as well as test
        /// cases marked with <see cref="ExplicitAttribute"/>,
        /// unless explicitly executed.
        /// </remarks>
        public override int SkipCount
        {
            get
            {
                RwLock.EnterReadLock();
                try
                {
                    return _skipCount;
                }
                finally
                {
                    RwLock.ExitReadLock();
                }
            }
        }

        /// <summary>
        /// Gets the number of test cases that were inconclusive
        /// when running the test and all its children.
        /// </summary>
        /// <remarks>
        /// Count reflects test cases that return with
        /// <see cref="Assert.Inconclusive()"/>.
        /// </remarks>
        public override int InconclusiveCount
        {
            get
            {
                RwLock.EnterReadLock();
                try
                {
                    return _inconclusiveCount;
                }
                finally
                {
                    RwLock.ExitReadLock();
                }
            }
        }

        /// <summary>
        /// Indicates whether this result has any child results.
        /// </summary>
        public override bool HasChildren => !_children.IsEmpty;

        /// <summary>
        /// Gets the collection of child results.
        /// </summary>
        public override IEnumerable<ITestResult> Children => _children;

        #endregion

        #region AddResult Method

        /// <summary>
        /// Adds a child result to this result, setting this result's
        /// ResultState to Failure if the child result failed.
        /// </summary>
        /// <param name="result">The result to be added</param>
        public virtual void AddResult(ITestResult result)
        {
            _children.Enqueue(result);
            RwLock.EnterWriteLock();
            try
            {
                MergeChildResult(result);
            }
            finally
            {
                RwLock.ExitWriteLock();
            }
        }

        private void MergeChildResult(ITestResult childResult)
        {
            // If this result is marked cancelled, don't change it
            if (ResultState != ResultState.Cancelled)
                UpdateResultState(childResult.ResultState);

            InternalAssertCount += childResult.AssertCount;
            _passCount += childResult.PassCount;
            _failCount += childResult.FailCount;
            _warningCount += childResult.WarningCount;
            _skipCount += childResult.SkipCount;
            _inconclusiveCount += childResult.InconclusiveCount;
            _totalCount += childResult.PassCount + childResult.FailCount + childResult.SkipCount + childResult.InconclusiveCount + childResult.WarningCount;
        }

        private void UpdateResultState(ResultState childResultState)
        {
            switch (childResultState.Status)
            {
                case TestStatus.Passed:
                    if (ResultState.Status == TestStatus.Inconclusive)
                        SetResult(ResultState.Success);
                    break;

                case TestStatus.Warning:
                    if (ResultState.Status == TestStatus.Inconclusive || ResultState.Status == TestStatus.Passed)
                        SetResult(ResultState.ChildWarning, CHILD_WARNINGS_MESSAGE);
                    break;

                case TestStatus.Failed:
                    if (childResultState.Label == "Cancelled")
                        SetResult(ResultState.Cancelled, USER_CANCELLED_MESSAGE);
                    else if (ResultState.Status != TestStatus.Failed)
                        SetResult(ResultState.ChildFailure, CHILD_ERRORS_MESSAGE);
                    break;

                case TestStatus.Skipped:
                    if (childResultState.Label == "Ignored")
                    {
                        if (ResultState.Status == TestStatus.Inconclusive || ResultState.Status == TestStatus.Passed)
                            SetResult(ResultState.ChildIgnored, CHILD_IGNORE_MESSAGE);
                    }

                    break;
            }
        }

        #endregion

        /// <inheritdoc />
        protected internal override TestResult CalculateDeltaResult(TestResult previous, Exception? exception = null)
        {
            var deltaResult = new TestSuiteResult(this, previous);

            CalculateDeltaResult(deltaResult, previous, exception);

            return deltaResult;
        }
    }
}
