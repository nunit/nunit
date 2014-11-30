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
        private ITestResult _result;
        private ResultSummary _summary;
        private int _reportCount = 0;

        /// <summary>
        /// Constructs an instance of ResultReporter
        /// </summary>
        /// <param name="result">The top-level result being reported</param>
        /// <param name="writer">A TextWriter to which the report is written</param>
        public ResultReporter(ITestResult result, ExtendedTextWriter writer)
        {
            _result = result;
            _writer = writer;

            _summary = new ResultSummary(_result);
        }

        /// <summary>
        /// Gets the ResultSummary created by the ResultReporter
        /// </summary>
        public ResultSummary Summary
        {
            get { return _summary; }
        }

        /// <summary>
        /// Produces the standard output reports.
        /// </summary>
        public void ReportResults()
        {
            if (_summary.TestCount == 0)
                _writer.WriteLine(ColorStyle.Warning, "Warning: No tests found");

            PrintSummaryReport();

            if (_result.ResultState.Status == TestStatus.Failed)
                PrintErrorReport();

            if (_summary.NotRunCount > 0)
                PrintNotRunReport();

            //if (commandLineOptions.Full)
            //    PrintFullReport(result);
        }

        /// <summary>
        /// Prints the Summary Report
        /// </summary>
        public void PrintSummaryReport()
        {
            var status = _result.ResultState.Status;

            string overallResult = status == TestStatus.Skipped
                ? "Warning"
                : status.ToString();

            ColorStyle overallStyle = status == TestStatus.Passed
                ? ColorStyle.Pass
                : status == TestStatus.Failed
                    ? ColorStyle.Failure
                    : status == TestStatus.Skipped
                        ? ColorStyle.Warning
                        : ColorStyle.Output;

            _writer.WriteLine();
            _writer.WriteLine(ColorStyle.SectionHeader, "Test Run Result -");
            _writer.WriteLabelLine("   Overall result: ", overallResult, overallStyle);

            _writer.WriteLabel("   Tests run: ", _summary.TestCount.ToString(CultureInfo.CurrentUICulture));
            _writer.WriteLabel(", Passed: ", _summary.PassCount.ToString(CultureInfo.CurrentUICulture));
            _writer.WriteLabel(", Errors: ", _summary.ErrorCount.ToString(CultureInfo.CurrentUICulture));
            _writer.WriteLabel(", Failures: ", _summary.FailureCount.ToString(CultureInfo.CurrentUICulture));
            _writer.WriteLabelLine(", Inconclusive: ", _summary.InconclusiveCount.ToString(CultureInfo.CurrentUICulture));
            
            _writer.WriteLabel("     Not run: ", _summary.NotRunCount.ToString(CultureInfo.CurrentUICulture));
            _writer.WriteLabel(", Invalid: ",_summary.InvalidCount.ToString(CultureInfo.CurrentUICulture));
            _writer.WriteLabel(", Ignored: ", _summary.IgnoreCount.ToString(CultureInfo.CurrentUICulture));
            _writer.WriteLabelLine(", Skipped: ", _summary.SkipCount.ToString(CultureInfo.CurrentUICulture));

            _writer.WriteLabelLine("     Start time: ", _result.StartTime.ToString("u"));
            _writer.WriteLabelLine("       End time: ", _result.EndTime.ToString("u"));
            _writer.WriteLabelLine("       Duration: ", _result.Duration.TotalSeconds.ToString("0.000") + " seconds");
        }

        /// <summary>
        /// Prints the Error Report
        /// </summary>
        public void PrintErrorReport()
        {
            _reportCount = 0;
            _writer.WriteLine();
            _writer.WriteLine(ColorStyle.SectionHeader, "Errors and Failures -");
            PrintErrorResults(_result);
        }

        /// <summary>
        /// Prints the Not Run Report
        /// </summary>
        public void PrintNotRunReport()
        {
            _reportCount = 0;
            _writer.WriteLine();
            _writer.WriteLine(ColorStyle.SectionHeader, "Tests Not Run -");
            PrintNotRunResults(_result);
        }

        /// <summary>
        /// Prints a full report of all results
        /// </summary>
        public void PrintFullReport()
        {
            _writer.WriteLine();
            _writer.WriteLine(ColorStyle.SectionHeader, "All Test Results -");
            PrintAllResults(_result, " ");
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
                    if (site == FailureSite.SetUp) return;
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
            {
                var colorStyle = result.ResultState == ResultState.Ignored
                    ? ColorStyle.Warning
                    : ColorStyle.Output;

                using (new ColorConsole(colorStyle))
                    WriteSingleResult(result);
            }
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

            if (status == "Failed" || status == "Error")
            {
                var site = result.ResultState.Site.ToString();
                if (site == "SetUp" || site == "TearDown")
                    status = site + " " + status;
            }

            _writer.WriteLine();
            _writer.WriteLine("{0}) {1} : {2}", ++_reportCount, status, result.FullName);

            if (result.Message != null && result.Message != string.Empty)
                _writer.WriteLine(result.Message);

            if (result.StackTrace != null && result.StackTrace != string.Empty)
                _writer.WriteLine(result.StackTrace);
        }

        #endregion
    }
}
