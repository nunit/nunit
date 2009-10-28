// ***********************************************************************
// Copyright (c) 2009 Charlie Poole
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
using NUnit.Core;

namespace NUnit.AdhocTestRunner
{
    public class ResultReporter
    {
        TestResult result;
        ResultSummary summary;

        int reportIndex = 0;

        public ResultReporter(TestResult result)
        {
            this.result = result;
            this.summary = new ResultSummary(result);
        }

        /// <summary>
        /// Reports the results.
        /// </summary>
        /// <param name="result">The result.</param>
        public void ReportResults()
        {
            WriteSummaryReport();

            if (summary.ErrorsAndFailures > 0)
                WriteErrorsAndFailuresReport();

            if (summary.TestsNotRun > 0)
                WriteNotRunReport();
        }

        private void WriteSummaryReport()
        {
            Console.WriteLine();
            Console.WriteLine(
                "   Tests run: {0}, Errors: {1}, Failures: {2}, Inconclusive: {3}",
                summary.TestsRun, summary.Errors, summary.Failures, summary.Inconclusive);
            Console.WriteLine(
                "     Not run: {0}, Invalid: {1}, Ignored: {2}, Skipped: {3}",
                summary.TestsNotRun, summary.NotRunnable, summary.Ignored, summary.Skipped);
            Console.WriteLine(
                "        Time: {0} seconds", summary.Time);
            Console.WriteLine();
        }

        private void WriteErrorsAndFailuresReport()
        {
            this.reportIndex = 0;
            Console.WriteLine("Errors and Failures:");
            WriteErrorsAndFailures(result);
            Console.WriteLine();
        }

        private void WriteErrorsAndFailures(TestResult result)
        {
            if (result.Executed)
            {
                if (result.HasResults)
                {
                    if ((result.IsFailure || result.IsError) && result.FailureSite == FailureSite.SetUp)
                        WriteSingleResult(result);

                    foreach (TestResult childResult in result.Results)
                        WriteErrorsAndFailures(childResult);
                }
                else if (result.IsFailure || result.IsError)
                {
                    WriteSingleResult(result);
                }
            }
        }

        public void WriteNotRunReport()
        {
            this.reportIndex = 0;
            Console.WriteLine("Tests Not Run:");
            WriteNotRunResults(result);
            Console.WriteLine();
        }

        private void WriteNotRunResults(TestResult result)
        {
            if (result.HasResults)
                foreach (TestResult childResult in result.Results)
                    WriteNotRunResults(childResult);
            else if (!result.Executed)
                WriteSingleResult(result);
        }

        private void WriteSingleResult(TestResult result)
        {
            string status = result.IsFailure || result.IsError
                ? string.Format("{0} {1}", result.FailureSite, result.ResultState)
                : result.ResultState.ToString();

            Console.WriteLine("{0}) {1} : {2}", ++reportIndex, status, result.FullName);

            if (result.Message != null && result.Message != string.Empty)
                Console.WriteLine(result.Message);

            if (result.StackTrace != null && result.StackTrace != string.Empty)
                Console.WriteLine(result.IsFailure
                    ? StackTraceFilter.Filter(result.StackTrace)
                    : result.StackTrace + Environment.NewLine);
        }
    }
}
