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
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.TestUtilities
{
    /// <summary>
    /// Summary description for ResultSummary.
    /// </summary>
    public class ResultSummary
    {
        private int resultCount = 0;
        private int failureCount = 0;
        private int successCount = 0;
        private int inconclusiveCount = 0;
        private int skipCount = 0;

        private DateTime startTime = DateTime.MinValue;
        private DateTime endTime = DateTime.MaxValue;
        private double duration = 0.0d;
        private string name;

        public ResultSummary() { }

        public ResultSummary(ITestResult result)
        {
            Summarize(result);
        }

        private void Summarize(ITestResult result)
        {
            if (name == null)
            {
                name = result.Name;
                startTime = result.StartTime;
                endTime = result.EndTime;
                duration = result.Duration;
            }

            if (result.HasChildren)
            {
                foreach (TestResult childResult in result.Children)
                    Summarize(childResult);
            }
            else
            {
                resultCount++;

                switch (result.ResultState.Status)
                {
                    case TestStatus.Passed:
                        successCount++;
                        break;
                    case TestStatus.Failed:
                        failureCount++;
                        break;
                    case TestStatus.Inconclusive:
                        inconclusiveCount++;
                        break;
                    case TestStatus.Skipped:
                    default:
                        skipCount++;
                        break;
                }
            }
        }

        public string Name
        {
            get { return name; }
        }

        public bool Success
        {
            get { return failureCount == 0; }
        }

        /// <summary>
        /// Returns the number of test cases for which results
        /// have been summarized. Any tests excluded by use of
        /// Category or Explicit attributes are not counted.
        /// </summary>
        public int ResultCount
        {
            get { return resultCount; }
        }

        /// <summary>
        /// Returns the number of test cases actually run, which
        /// is the same as ResultCount, less any Skipped, Ignored
        /// or NonRunnable tests.
        /// </summary>
        public int TestsRun
        {
            get { return Passed + Failed + Inconclusive; }
        }

        /// <summary>
        /// Returns the number of tests that passed
        /// </summary>
        public int Passed
        {
            get { return successCount; }
        }

        /// <summary>
        /// Returns the number of test cases that failed.
        /// </summary>
        public int Failed
        {
            get { return failureCount; }
        }

        /// <summary>
        /// Returns the number of test cases that failed.
        /// </summary>
        public int Inconclusive
        {
            get { return inconclusiveCount; }
        }

        /// <summary>
        /// Returns the number of test cases that were skipped.
        /// </summary>
        public int Skipped
        {
            get { return skipCount; }
        }

        /// <summary>
        /// Gets the start time of the test run.
        /// </summary>
        public DateTime StartTime
        {
            get { return startTime; }
        }

        /// <summary>
        /// Gets the end time of the test run.
        /// </summary>
        public DateTime EndTime
        {
            get { return endTime; }
        }

        /// <summary>
        /// Gets the duration of the test run in seconds.
        /// </summary>
        public double Duration
        {
            get { return duration; }
        }
    }
}
