// *****************************************************
// Copyright 2007, Charlie Poole
//
// Licensed under the Open Software License version 3.0
// *****************************************************

using System;
using NUnit.Framework;

namespace NUnitLite
{
    public class ResultSummary
    {
        private int testCount;
        private int errorCount;
        private int failureCount;
        private int notRunCount;

        public ResultSummary(TestResult result)
        {
            Visit(result);
        }

        public int TestCount
        {
            get { return testCount; }
        }

        public int ErrorCount
        {
            get { return errorCount; }
        }

        public int FailureCount
        {
            get { return failureCount; }
        }

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
