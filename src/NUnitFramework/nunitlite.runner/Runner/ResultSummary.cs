// ***********************************************************************
// Copyright (c) 2014-2015 Charlie Poole
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
using NUnit.Framework.Interfaces;

namespace NUnitLite.Runner
{
    /// <summary>
    /// Helper class used to summarize the result of a test run
    /// </summary>
    public class ResultSummary
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultSummary"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        public ResultSummary(ITestResult result)
        {
            InitializeCounters();

            ResultState = result.ResultState;
            StartTime = result.StartTime;
            EndTime = result.EndTime;
            Duration = result.Duration;

            Summarize(result);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of test cases for which results
        /// have been summarized. Any tests excluded by use of
        /// Category or Explicit attributes are not counted.
        /// </summary>
        public int TestCount { get; private set; }

        /// <summary>
        /// Returns the number of test cases actually run, which
        /// is the same as TestCount, less any Skipped, Ignored
        /// or NonRunnable tests.
        /// </summary>
        public int RunCount { get; private set; }

        /// <summary>
        /// Gets the count of passed tests
        /// </summary>
        public int PassCount { get; private set; }

        /// <summary>
        /// Gets count of failed tests, excluding errors and invalid tests
        /// </summary>
        public int FailureCount { get; private set; }

        /// <summary>
        /// Gets the error count
        /// </summary>
        public int ErrorCount { get; private set; }

        /// <summary>
        /// Gets the count of inconclusive tests
        /// </summary>
        public int InconclusiveCount { get; private set; }

        /// <summary>
        /// Returns the number of test cases that were not runnable
        /// due to errors in the signature of the class or method.
        /// Such tests are also counted as Errors.
        /// </summary>
        public int InvalidCount { get; private set; }

        /// <summary>
        /// Gets the count of skipped tests, excluding ignored tests
        /// </summary>
        public int SkipCount { get; private set; }

        /// <summary>
        /// Gets the ignore count
        /// </summary>
        public int IgnoreCount { get; private set; }

        public ResultState ResultState { get; private set; }

        public DateTime StartTime { get; private set; }

        public DateTime EndTime { get; private set; }

        public TimeSpan Duration { get; private set; }

        #endregion

        #region Helper Methods

        private void InitializeCounters()
        {
            TestCount = 0;
            RunCount = 0;
            PassCount = 0;
            FailureCount = 0;
            ErrorCount = 0;
            InconclusiveCount = 0;
            SkipCount = 0;
            IgnoreCount = 0;
            InvalidCount = 0;
        }

        private void Summarize(ITestResult result)
        {
            if (result.Test.IsSuite)
            {
                foreach (ITestResult r in result.Children)
                    Summarize(r);
            }
            else
            {
                TestCount++;
                switch (result.ResultState.Status)
                {
                    case TestStatus.Passed:
                        PassCount++;
                        RunCount++;
                        break;
                    case TestStatus.Skipped:
                        if (result.ResultState == ResultState.Ignored)
                            IgnoreCount++;
                        else if (result.ResultState == ResultState.Skipped)
                            SkipCount++;
                        break;
                    case TestStatus.Failed:
                        RunCount++;
                        if (result.ResultState == ResultState.Failure)
                            FailureCount++;
                        else if (result.ResultState == ResultState.NotRunnable)
                            InvalidCount++;
                        else
                            ErrorCount++;
                        break;
                    case TestStatus.Inconclusive:
                        RunCount++;
                        InconclusiveCount++;
                        break;
                }

                return;
            }
        }

        #endregion
    }
}
