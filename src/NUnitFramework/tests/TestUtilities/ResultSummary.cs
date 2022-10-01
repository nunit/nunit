// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
                foreach (var childResult in result.Children)
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

        public string Name => name;

        public bool Success => failureCount == 0;

        /// <summary>
        /// Returns the number of test cases for which results
        /// have been summarized. Any tests excluded by use of
        /// Category or Explicit attributes are not counted.
        /// </summary>
        public int ResultCount => resultCount;

        /// <summary>
        /// Returns the number of test cases actually run, which
        /// is the same as ResultCount, less any Skipped, Ignored
        /// or NonRunnable tests.
        /// </summary>
        public int TestsRun => Passed + Failed + Inconclusive;

        /// <summary>
        /// Returns the number of tests that passed
        /// </summary>
        public int Passed => successCount;

        /// <summary>
        /// Returns the number of test cases that failed.
        /// </summary>
        public int Failed => failureCount;

        /// <summary>
        /// Returns the number of test cases that failed.
        /// </summary>
        public int Inconclusive => inconclusiveCount;

        /// <summary>
        /// Returns the number of test cases that were skipped.
        /// </summary>
        public int Skipped => skipCount;

        /// <summary>
        /// Gets the start time of the test run.
        /// </summary>
        public DateTime StartTime => startTime;

        /// <summary>
        /// Gets the end time of the test run.
        /// </summary>
        public DateTime EndTime => endTime;

        /// <summary>
        /// Gets the duration of the test run in seconds.
        /// </summary>
        public double Duration => duration;
    }
}
