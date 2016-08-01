// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
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
using NUnit.Framework.Interfaces;
using System.Threading;
#if NETCF || NET_2_0
using NUnit.Compatibility;
#endif

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Represents the result of running a test suite
    /// </summary>
    public class TestSuiteResult : TestResult
    {
        private int _passCount = 0;
        private int _failCount = 0;
        private int _skipCount = 0;
        private int _inconclusiveCount = 0;
#if PARALLEL
        private ConcurrentQueue<ITestResult> _children;
#else
        private List<ITestResult> _children;
#endif

        /// <summary>
        /// Construct a TestSuiteResult base on a TestSuite
        /// </summary>
        /// <param name="suite">The TestSuite to which the result applies</param>
        public TestSuiteResult(TestSuite suite) : base(suite)
        {
#if PARALLEL
            _children = new ConcurrentQueue<ITestResult>();
#else
            _children = new List<ITestResult>();
#endif
        }

#region Overrides

        /// <summary>
        /// Gets the number of test cases that failed
        /// when running the test and all its children.
        /// </summary>
        public override int FailCount
        {
            get
            {
#if PARALLEL
                RwLock.EnterReadLock();
#endif
                try
                {
                    return _failCount;
                }
                finally
                {
#if PARALLEL
                    RwLock.ExitReadLock();
#endif
                }
            }
        }

        /// <summary>
        /// Gets the number of test cases that passed
        /// when running the test and all its children.
        /// </summary>
        public override int PassCount
        {
            get
            {
#if PARALLEL
                RwLock.EnterReadLock();
#endif
                try
                {
                    return _passCount;
                }
                finally
                {
#if PARALLEL
                    RwLock.ExitReadLock();
#endif
                }
            }
        }

        /// <summary>
        /// Gets the number of test cases that were skipped
        /// when running the test and all its children.
        /// </summary>
        public override int SkipCount
        {
            get
            {
#if PARALLEL
                RwLock.EnterReadLock();
#endif
                try
                {
                    return _skipCount;
                }
                finally
                {
#if PARALLEL
                    RwLock.ExitReadLock();
#endif
                }
           }
        }

        /// <summary>
        /// Gets the number of test cases that were inconclusive
        /// when running the test and all its children.
        /// </summary>
        public override int InconclusiveCount
        {
            get
            {
#if PARALLEL
                RwLock.EnterReadLock();
#endif
                try
                {
                    return _inconclusiveCount;
                }
                finally
                {
#if PARALLEL
                    RwLock.ExitReadLock();
#endif
                }
            }
        }

        /// <summary>
        /// Indicates whether this result has any child results.
        /// </summary>
        public override bool HasChildren
        {
            get
            {
#if PARALLEL
                return !_children.IsEmpty;
#else
                return _children.Count != 0;
#endif
            }
        }

        /// <summary>
        /// Gets the collection of child results.
        /// </summary>
        public override IEnumerable<ITestResult> Children
        {
            get { return _children; }
        }

        #endregion

        #region AddResult Method

        /// <summary>
        /// Adds a child result to this result, setting this result's
        /// ResultState to Failure if the child result failed.
        /// </summary>
        /// <param name="result">The result to be added</param>
        public virtual void AddResult(ITestResult result)
        {
#if PARALLEL
            var childrenAsConcurrentQueue = Children as ConcurrentQueue<ITestResult>;
            if (childrenAsConcurrentQueue != null)
                childrenAsConcurrentQueue.Enqueue(result);
            else
#endif
            {
                var childrenAsIList = Children as IList<ITestResult>;
                if (childrenAsIList != null)
                    childrenAsIList.Add(result);
                else
                    throw new NotSupportedException("cannot add results to Children");

            }

#if PARALLEL
            RwLock.EnterWriteLock();
#endif
            try
            {
                // If this result is marked cancelled, don't change it
                if (ResultState != ResultState.Cancelled)
                {
                    switch (result.ResultState.Status)
                    {
                        case TestStatus.Passed:

                            if (ResultState.Status == TestStatus.Inconclusive)
                                SetResult(ResultState.Success);

                            break;

                        case TestStatus.Failed:


                            if (ResultState.Status != TestStatus.Failed)
                                SetResult(ResultState.ChildFailure, CHILD_ERRORS_MESSAGE);

                            break;

                        case TestStatus.Skipped:

                            if (result.ResultState.Label == "Ignored")
                                if (ResultState.Status == TestStatus.Inconclusive || ResultState.Status == TestStatus.Passed)
                                    SetResult(ResultState.Ignored, CHILD_IGNORE_MESSAGE);

                            break;
                    }
                }

                InternalAssertCount += result.AssertCount;
                _passCount += result.PassCount;
                _failCount += result.FailCount;
                _skipCount += result.SkipCount;
                _inconclusiveCount += result.InconclusiveCount;
            }
            finally
            {
#if PARALLEL
                RwLock.ExitWriteLock();
#endif
            }
        }

        #endregion
    }
}
