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
using NUnit.Framework;
using NUnit.Framework.Api;

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
            if (result.Test.IsTestCase)
            {
                testCount++;
                switch (result.ResultState)
                {
                    case ResultState.Ignored:
                    case ResultState.Skipped:
                        notRunCount++;
                        break;
                    case ResultState.Error:
                    case ResultState.NotRunnable:
                    case ResultState.Cancelled:
                        errorCount++;
                        break;
                    case ResultState.Failure:
                        failureCount++;
                        break;
                    default:
                        break;
                }

                return;
            }

            if (result.Results != null)
                foreach (TestResult r in result.Results)
                    Visit(r);
        }
    }
}
