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
using System.Collections.Concurrent;
using NUnit.Framework.Interfaces;
using System.Threading;
#if NETCF
using Interlocked = System.Threading.InterlockedEx;
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
            get { return _failCount; }
        }

        /// <summary>
        /// Gets the number of test cases that passed
        /// when running the test and all its children.
        /// </summary>
        public override int PassCount
        {
            get { return _passCount; }
        }

        /// <summary>
        /// Gets the number of test cases that were skipped
        /// when running the test and all its children.
        /// </summary>
        public override int SkipCount
        {
            get { return _skipCount; }
        }

        /// <summary>
        /// Gets the number of test cases that were inconclusive
        /// when running the test and all its children.
        /// </summary>
        public override int InconclusiveCount
        {
            get { return _inconclusiveCount; }
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
            _children.Enqueue(result);
#else
            _children.Add(result);
#endif

            //AssertCount += result.AssertCount;

            bool stateSet;
            do
            {
                stateSet = true;

                var resultState = ResultState;

                // If this result is marked cancelled, don't change it
                if (resultState == ResultState.Cancelled)
                    break;

                switch (result.ResultState.Status)
                {
                    case TestStatus.Passed:

                        if (resultState.Status == TestStatus.Inconclusive)
                            stateSet = SetResultIf(resultState, ResultState.Success);

                        break;

                    case TestStatus.Failed:


                        if (resultState.Status != TestStatus.Failed)
                            stateSet = SetResultIf(resultState, ResultState.ChildFailure, CHILD_ERRORS_MESSAGE);

                        break;

                    case TestStatus.Skipped:

                        if (result.ResultState.Label == "Ignored")
                            if (resultState.Status == TestStatus.Inconclusive || resultState.Status == TestStatus.Passed)
                                stateSet = SetResultIf(resultState, ResultState.Ignored, CHILD_IGNORE_MESSAGE);

                        break;
                }
            } while (!stateSet);

            Interlocked.Add (ref InternalAssertCount, result.AssertCount);
            Interlocked.Add (ref _passCount, result.PassCount);
            Interlocked.Add (ref _failCount, result.FailCount);
            Interlocked.Add (ref _skipCount, result.SkipCount);
            Interlocked.Add (ref _inconclusiveCount, result.InconclusiveCount);
        }

#endregion
    }
}
