// *****************************************************
// Copyright 2007, Charlie Poole
//
// Licensed under the Open Software License version 3.0
// *****************************************************

using System;
using NUnit.Framework;

namespace NUnitLite
{
    /// <summary>
    /// Helper class used to summarize the result of a test run
    /// </summary>
    public class ResultSummary
    {
        private int testCount;
        private int errorCount;
        private int failureCount;
        private int notRunCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultSummary"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        public ResultSummary(TestResult result)
        {
            Visit(result);
        }

        /// <summary>
        /// Gets the test count.
        /// </summary>
        /// <value>The test count.</value>
        public int TestCount
        {
            get { return testCount; }
        }

        /// <summary>
        /// Gets the error count.
        /// </summary>
        /// <value>The error count.</value>
        public int ErrorCount
        {
            get { return errorCount; }
        }

        /// <summary>
        /// Gets the failure count.
        /// </summary>
        /// <value>The failure count.</value>
        public int FailureCount
        {
            get { return failureCount; }
        }

        /// <summary>
        /// Gets the not run count.
        /// </summary>
        /// <value>The not run count.</value>
        public int NotRunCount
        {
            get { return notRunCount; }
        }

        private void Visit(TestResult result)
        {
            if (result.Test is TestSuite)
            {
                if (result.Results != null)
                    foreach (TestResult r in result.Results)
                        Visit(r);
                return;
            }
 
            // We only count non-suites
            testCount++;
            switch (result.ResultState)
            {
                case ResultState.NotRun:
                    notRunCount++;
                    break;
                case ResultState.Error:
                    errorCount++;
                    break;
                case ResultState.Failure:
                    failureCount++;
                    break;
                default:
                    break;
            }
        }
    }
}
