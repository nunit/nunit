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
using System.Globalization;
using System.Xml;

namespace NUnit.ConsoleRunner
{
    using Options;
    using Utilities;

    public class ResultReporter
    {
        XmlNode _result;
        string _testRunResult;
        ConsoleOptions _options;
        ResultSummary _summary;

        int _reportIndex = 0;

        public ResultReporter(XmlNode result, ConsoleOptions options)
        {
            _result = result;
            _testRunResult = result.GetAttribute("result");
            if (_testRunResult == "Skipped")
                _testRunResult = "Warning";
            _options = options;
            _summary = new ResultSummary(result);
        }

        public ResultSummary Summary
        {
            get { return _summary; }
        }

        /// <summary>
        /// Reports the results to the console
        /// </summary>
        public void ReportResults()
        {
            if (_options.StopOnError && _summary.ErrorsAndFailures > 0)
            {
                ColorConsole.WriteLine(ColorStyle.Failure, "Execution terminated after first error");
                Console.WriteLine();
            }

            WriteSummaryReport();

            WriteAssemblyErrorsAndWarnings();

            if (_testRunResult == "Failed")
                WriteErrorsAndFailuresReport();

            if (_summary.TestsNotRun > 0)
                WriteNotRunReport();
        }

        private void WriteSummaryReport()
        {
            ColorStyle overall = _testRunResult == "Passed"
                ? ColorStyle.Pass
                : ( _testRunResult == "Failed" ? ColorStyle.Failure : ColorStyle.Warning );
            Console.WriteLine();
            ColorConsole.WriteLine(ColorStyle.SectionHeader, "Test Run Summary");
            ColorConsole.WriteLabel("    Overall result: ", _testRunResult, overall, true);

            ColorConsole.WriteLabel("   Tests run: ", _summary.TestsRun.ToString(CultureInfo.CurrentUICulture), false);
            ColorConsole.WriteLabel(", Errors: ", _summary.Errors.ToString(CultureInfo.CurrentUICulture), false);
            ColorConsole.WriteLabel(", Failures: ", _summary.Failures.ToString(CultureInfo.CurrentUICulture), false);
            ColorConsole.WriteLabel(", Inconclusive: ", _summary.Inconclusive.ToString(CultureInfo.CurrentUICulture), true);

            ColorConsole.WriteLabel("     Not run: ", _summary.TestsNotRun.ToString(CultureInfo.CurrentUICulture), false);
            ColorConsole.WriteLabel(", Invalid: ", _summary.NotRunnable.ToString(CultureInfo.CurrentUICulture), false);
            ColorConsole.WriteLabel(", Ignored: ", _summary.Ignored.ToString(CultureInfo.CurrentUICulture), false);
            ColorConsole.WriteLabel(", Skipped: ", _summary.Skipped.ToString(CultureInfo.CurrentUICulture), true);

            ColorConsole.WriteLabel("  Start time: ", _summary.StartTime.ToString("u"), true);
            ColorConsole.WriteLabel("    End time: ", _summary.EndTime.ToString("u"), true);
            ColorConsole.WriteLabel("    Duration: ", string.Format("{0} seconds", _summary.Duration.ToString("0.000")), true);
            Console.WriteLine();
        }

        private void WriteAssemblyErrorsAndWarnings()
        {
            foreach (XmlNode node in _result.SelectNodes("test-suite[@type='Assembly']"))
            {
                if (node.GetAttribute("runstate") == "NotRunnable")
                    WriteAssemblyMessage(ColorStyle.Error, node.SelectSingleNode("properties/property[@name='_SKIPREASON']").GetAttribute("value"));
                else if (node.GetAttribute("total") == "0" || node.GetAttribute("testcasecount") == "0")
                    WriteAssemblyMessage(ColorStyle.Warning, "Warning: No tests found in " + node.GetAttribute("name"));
            }
        }

        private void WriteAssemblyMessage(ColorStyle style, string message)
        {
            ColorConsole.WriteLine(style, message);
            Console.WriteLine();
        }

        private void WriteErrorsAndFailuresReport()
        {
            _reportIndex = 0;
            ColorConsole.WriteLine(ColorStyle.SectionHeader, "Errors and Failures");
            WriteErrorsAndFailures(_result);
            Console.WriteLine();
        }

        private void WriteErrorsAndFailures(XmlNode result)
        {
            string resultState = result.GetAttribute("result");

            switch (result.Name)
            {
                case "test-case":
                    if (resultState == "Failed")
                    {
                        using (new ColorConsole(ColorStyle.Failure))
                            WriteSingleResult(result);
                    }
                    else if (resultState == "Error")
                    {
                        using (new ColorConsole(ColorStyle.Error))
                            WriteSingleResult(result);
                    }
                    else if (resultState == "Cancelled")
                    {
                        using (new ColorConsole(ColorStyle.Warning))
                            WriteSingleResult(result);
                    }
                    return;

                case "test-run":
                    foreach (XmlNode childResult in result.ChildNodes)
                        WriteErrorsAndFailures(childResult);
                    break;

                case "test-suite":
                    if (resultState == "Failed")
                    {
                        if (result.GetAttribute("type") == "Theory")
                        {
                            using (new ColorConsole(ColorStyle.Failure))
                                WriteSingleResult(result);
                        }
                        else
                        {
                            var site = result.GetAttribute("site");
                            if (site == "SetUp" || site == "TearDown")
                                using (new ColorConsole(ColorStyle.Failure))
                                    WriteSingleResult(result);
                            if (site == "SetUp") return;
                        }
                    }
                    
                    foreach (XmlNode childResult in result.ChildNodes)
                        WriteErrorsAndFailures(childResult);

                    break;
            }
        }

        public void WriteNotRunReport()
        {
            _reportIndex = 0;
            ColorConsole.WriteLine(ColorStyle.SectionHeader, "Tests Not Run");
            WriteNotRunResults(_result);
            Console.WriteLine();
        }

        private void WriteNotRunResults(XmlNode result)
        {
            switch (result.Name)
            {
                case "test-case":
                    string resultState = result.GetAttribute("result");

                    if (resultState == "Skipped")
                    {
                        string label = result.GetAttribute("label");

                        ColorStyle style = label == "Ignored" ? ColorStyle.Warning : ColorStyle.Output;

                        using (new ColorConsole(style))
                            WriteSingleResult(result);
                    }

                    break;

                case "test-suite":
                case "test-run":
                    foreach (XmlNode childResult in result.ChildNodes)
                        WriteNotRunResults(childResult);

                    break;
            }
        }

        private void WriteSingleResult(XmlNode result)
        {
            string status = result.GetAttribute("label");
            if (status == null)
                status = result.GetAttribute("result");

            if (status == "Failed" || status == "Error")
            {
                var site = result.GetAttribute("site");
                if (site == "SetUp" || site == "TearDown")
                    status = site + " " + status;
            }

            string fullName = result.GetAttribute("fullname");

            Console.WriteLine("{0}) {1} : {2}", ++_reportIndex, status, fullName);

            XmlNode failureNode = result.SelectSingleNode("failure");
            if (failureNode != null)
            {
                XmlNode message = failureNode.SelectSingleNode("message");
                XmlNode stacktrace = failureNode.SelectSingleNode("stack-trace");

                if (message != null)
                    Console.WriteLine(message.InnerText);

                if (stacktrace != null)
                    Console.WriteLine(stacktrace.InnerText + Environment.NewLine);
            }

            XmlNode reasonNode = result.SelectSingleNode("reason");
            if (reasonNode != null)
            {
                XmlNode message = reasonNode.SelectSingleNode("message");

                if (message != null)
                    Console.WriteLine(message.InnerText);
            }
        }
    }
}
