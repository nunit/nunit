// ***********************************************************************
// Copyright (c) 2012 Charlie Poole
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

using System.IO;
using System.Globalization;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnitLite.Runner
{
    /// <summary>
    /// ResultReporter writes the NUnitLite results to a TextWriter.
    /// </summary>
    public class ResultReporter
    {
        private ExtendedTextWriter _writer;
        private ITestResult result;
        private ResultSummary summary;
        private int reportCount = 0;

        /// <summary>
        /// Constructs an instance of ResultReporter
        /// </summary>
        /// <param name="result">The top-level result being reported</param>
        /// <param name="writer">A TextWriter to which the report is written</param>
        public ResultReporter(ITestResult result, ExtendedTextWriter writer)
        {
            this.result = result;
            this._writer = writer;

            this.summary = new ResultSummary(this.result);
        }

        /// <summary>
        /// Gets the ResultSummary created by the ResultReporter
        /// </summary>
        public ResultSummary Summary
        {
            get { return summary; }
        }

        /// <summary>
        /// Produces the standard output reports.
        /// </summary>
        public void ReportResults()
        {
            if (summary.TestCount == 0)
                _writer.WriteLine(ColorStyle.Warning, "Warning: No tests found");

            PrintSummaryReport();

            if (this.result.ResultState.Status == TestStatus.Failed)
            //if (summary.FailureCount > 0 || summary.ErrorCount > 0)
                PrintErrorReport();

            if (summary.NotRunCount > 0)
                PrintNotRunReport();

            //if (commandLineOptions.Full)
            //    PrintFullReport(result);
        }

        /// <summary>
        /// Prints the Summary Report
        /// </summary>
        public void PrintSummaryReport()
        {
            var status = result.ResultState.Status;
            ColorStyle overall = status == TestStatus.Passed
                ? ColorStyle.Pass
                : status == TestStatus.Failed
                    ? ColorStyle.Failure
                    : ColorStyle.Warning;

            _writer.WriteLine();
            _writer.WriteLine(ColorStyle.SectionHeader, "Test Run Result -");
            _writer.WriteLabelLine("   Overall result: ", result.ResultState.Status, overall);

            _writer.WriteLabel("   Tests run: ", summary.TestCount.ToString(CultureInfo.CurrentUICulture));
            _writer.WriteLabel(", Passed: ", summary.PassCount.ToString(CultureInfo.CurrentUICulture));
            _writer.WriteLabel(", Errors: ", summary.ErrorCount.ToString(CultureInfo.CurrentUICulture));
            _writer.WriteLabel(", Failures: ", summary.FailureCount.ToString(CultureInfo.CurrentUICulture));
            _writer.WriteLabelLine(", Inconclusive: ", summary.InconclusiveCount.ToString(CultureInfo.CurrentUICulture));
            
            _writer.WriteLabel("     Not run: ", summary.NotRunCount.ToString(CultureInfo.CurrentUICulture));
            _writer.WriteLabel(", Invalid: ",summary.InvalidCount.ToString(CultureInfo.CurrentUICulture));
            _writer.WriteLabel(", Ignored: ", summary.IgnoreCount.ToString(CultureInfo.CurrentUICulture));
            _writer.WriteLabelLine(", Skipped: ", summary.SkipCount.ToString(CultureInfo.CurrentUICulture));

            _writer.WriteLabelLine("     Start time: ", result.StartTime.ToString("u"));
            _writer.WriteLabelLine("       End time: ", result.EndTime.ToString("u"));
            _writer.WriteLabelLine("       Duration: ", result.Duration.TotalSeconds.ToString("0.000") + " seconds");
        }

        /// <summary>
        /// Prints the Error Report
        /// </summary>
        public void PrintErrorReport()
        {
            reportCount = 0;
            _writer.WriteLine();
            _writer.WriteLine(ColorStyle.SectionHeader, "Errors and Failures -");
            PrintErrorResults(this.result);
        }

        /// <summary>
        /// Prints the Not Run Report
        /// </summary>
        public void PrintNotRunReport()
        {
            reportCount = 0;
            _writer.WriteLine();
            _writer.WriteLine(ColorStyle.SectionHeader, "Tests Not Run -");
            PrintNotRunResults(this.result);
        }

        /// <summary>
        /// Prints a full report of all results
        /// </summary>
        public void PrintFullReport()
        {
            _writer.WriteLine();
            _writer.WriteLine(ColorStyle.SectionHeader, "All Test Results -");
            PrintAllResults(this.result, " ");
        }

        #region Helper Methods

        private void PrintErrorResults(ITestResult result)
        {
            if (result.Test.IsSuite)
            {
                if (result.ResultState.Status == TestStatus.Failed)
                {
                    var suite = result.Test as TestSuite;
                    var site = result.ResultState.Site;
                    if (suite.TestType == "Theory" || site == FailureSite.SetUp || site == FailureSite.TearDown)
                        using (new ColorConsole(ColorStyle.Failure))
                        WriteSingleResult(result);
                }

                foreach (ITestResult childResult in result.Children)
                    PrintErrorResults(childResult);
            }
            else if (result.ResultState.Status == TestStatus.Failed || result.ResultState == ResultState.NotRunnable)
                using (new ColorConsole(ColorStyle.Failure))
                    WriteSingleResult(result);
        }

        private void PrintNotRunResults(ITestResult result)
        {
            if (result.HasChildren)
                foreach (ITestResult childResult in result.Children)
                    PrintNotRunResults(childResult);
            else if (result.ResultState.Status == TestStatus.Skipped)
                using (new ColorConsole(ColorStyle.Warning))
                    WriteSingleResult(result);
        }

        private void PrintTestProperties(ITest test)
        {
            foreach (string key in test.Properties.Keys)
                foreach (object value in test.Properties[key])
                    _writer.WriteLabelLine(string.Format("  {0}: ", key), value);
        }

        private void PrintAllResults(ITestResult result, string indent)
        {
            string status = null;
            ColorStyle style = ColorStyle.Default;
            switch (result.ResultState.Status)
            {
                case TestStatus.Failed:
                    status = "FAIL";
                    style = ColorStyle.Failure;
                    break;
                case TestStatus.Skipped:
                    status = "SKIP";
                    style = ColorStyle.Warning;
                    break;
                case TestStatus.Inconclusive:
                    status = "INC ";
                    break;
                case TestStatus.Passed:
                    status = "OK  ";
                    style = ColorStyle.Pass;
                    break;
            }

            _writer.Write(style, status);
            _writer.Write(indent);
            _writer.WriteLine(status, result.Name);

            if (result.HasChildren)
                foreach (ITestResult childResult in result.Children)
                    PrintAllResults(childResult, indent + "  ");
        }

        private void WriteSingleResult(ITestResult result)
        {
            string status = result.ResultState.Label;
            if (string.IsNullOrEmpty(status))
                status = result.ResultState.Status.ToString();

            _writer.WriteLine();
            _writer.WriteLine("{0}) {1} : {2}", ++reportCount, status, result.FullName);

            if (result.Message != null && result.Message != string.Empty)
                _writer.WriteLine(result.Message);

            if (result.StackTrace != null && result.StackTrace != string.Empty)
                _writer.WriteLine(result.StackTrace);
        }

        #endregion
    }
}
