// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Tests.TestUtilities
{
    /// <summary>
    /// Summary description for ResultSummary.
    /// </summary>
    public class ResultSummary
    {
        private int _resultCount = 0;
        private int _failureCount = 0;
        private int _successCount = 0;
        private int _inconclusiveCount = 0;
        private int _skipCount = 0;

        private readonly DateTime _startTime = DateTime.MinValue;
        private readonly DateTime _endTime = DateTime.MaxValue;
        private readonly double _duration = 0.0d;
        private readonly string _name;

        public ResultSummary(ITestResult result)
        {
            _name = result.Name;
            _startTime = result.StartTime;
            _endTime = result.EndTime;
            _duration = result.Duration;
            Summarize(result);
        }

        private void Summarize(ITestResult result)
        {
            if (result.HasChildren)
            {
                foreach (var childResult in result.Children)
                    Summarize(childResult);
            }
            else
            {
                _resultCount++;

                switch (result.ResultState.Status)
                {
                    case TestStatus.Passed:
                        _successCount++;
                        break;
                    case TestStatus.Failed:
                        _failureCount++;
                        break;
                    case TestStatus.Inconclusive:
                        _inconclusiveCount++;
                        break;
                    case TestStatus.Skipped:
                    default:
                        _skipCount++;
                        break;
                }
            }
        }

        public string Name => _name;

        public bool Success => _failureCount == 0;

        /// <summary>
        /// Returns the number of test cases for which results
        /// have been summarized. Any tests excluded by use of
        /// Category or Explicit attributes are not counted.
        /// </summary>
        public int ResultCount => _resultCount;

        /// <summary>
        /// Returns the number of test cases actually run, which
        /// is the same as ResultCount, less any Skipped, Ignored
        /// or NonRunnable tests.
        /// </summary>
        public int TestsRun => Passed + Failed + Inconclusive;

        /// <summary>
        /// Returns the number of tests that passed
        /// </summary>
        public int Passed => _successCount;

        /// <summary>
        /// Returns the number of test cases that failed.
        /// </summary>
        public int Failed => _failureCount;

        /// <summary>
        /// Returns the number of test cases that failed.
        /// </summary>
        public int Inconclusive => _inconclusiveCount;

        /// <summary>
        /// Returns the number of test cases that were skipped.
        /// </summary>
        public int Skipped => _skipCount;

        /// <summary>
        /// Gets the start time of the test run.
        /// </summary>
        public DateTime StartTime => _startTime;

        /// <summary>
        /// Gets the end time of the test run.
        /// </summary>
        public DateTime EndTime => _endTime;

        /// <summary>
        /// Gets the duration of the test run in seconds.
        /// </summary>
        public double Duration => _duration;
    }
}
