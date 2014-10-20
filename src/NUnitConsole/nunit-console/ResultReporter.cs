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
        /// <summary>
        /// Error message for when child tests have errors
        /// </summary>
        private static readonly string CHILD_ERRORS_MESSAGE = "One or more child tests had errors";

        XmlNode result;
        string testRunResult;
        ConsoleOptions options;
        ResultSummary summary;

        int reportIndex = 0;

        public ResultReporter(XmlNode result, ConsoleOptions options)
        {
            this.result = result;
            this.testRunResult = result.GetAttribute("result");
            this.options = options;
            this.summary = new ResultSummary(result);
        }

        public ResultSummary Summary
        {
            get { return summary; }
        }

        /// <summary>
        /// Reports the results to the console
        /// </summary>
        public void ReportResults()
        {
            if (options.StopOnError && summary.ErrorsAndFailures > 0)
            {
                ColorConsole.WriteLine(ColorStyle.Failure, "Execution terminated after first error");
                Console.WriteLine();
            }

            WriteSummaryReport();

            WriteAssemblyErrorsAndWarnings();

            if (testRunResult == "Failed")
                WriteErrorsAndFailuresReport();

            if (summary.TestsNotRun > 0)
                WriteNotRunReport();
        }

        private void WriteSummaryReport()
        {
            ColorStyle overall = testRunResult == "Passed"
                ? ColorStyle.Pass
                : ( testRunResult == "Failed" ? ColorStyle.Failure : ColorStyle.Warning );
            Console.WriteLine();
            ColorConsole.WriteLine(ColorStyle.SectionHeader, "Test Run Summary");
            ColorConsole.WriteLabel("    Overall result: ", testRunResult, overall, true);

            ColorConsole.WriteLabel("   Tests run: ", summary.TestsRun.ToString(CultureInfo.CurrentUICulture), false);
            ColorConsole.WriteLabel(", Errors: ", summary.Errors.ToString(CultureInfo.CurrentUICulture), false);
            ColorConsole.WriteLabel(", Failures: ", summary.Failures.ToString(CultureInfo.CurrentUICulture), false);
            ColorConsole.WriteLabel(", Inconclusive: ", summary.Inconclusive.ToString(CultureInfo.CurrentUICulture), true);

            ColorConsole.WriteLabel("     Not run: ", summary.TestsNotRun.ToString(CultureInfo.CurrentUICulture), false);
            ColorConsole.WriteLabel(", Invalid: ", summary.NotRunnable.ToString(CultureInfo.CurrentUICulture), false);
            ColorConsole.WriteLabel(", Ignored: ", summary.Ignored.ToString(CultureInfo.CurrentUICulture), false);
            ColorConsole.WriteLabel(", Skipped: ", summary.Skipped.ToString(CultureInfo.CurrentUICulture), true);

            ColorConsole.WriteLabel("  Start time: ", summary.StartTime.ToString("u"), true);
            ColorConsole.WriteLabel("    End time: ", summary.EndTime.ToString("u"), true);
            ColorConsole.WriteLabel("    Duration: ", string.Format("{0} seconds", summary.Duration.ToString("0.000")), true);
            Console.WriteLine();
        }

        private void WriteAssemblyErrorsAndWarnings()
        {
            foreach (XmlNode node in this.result.SelectNodes("test-suite[@type='Assembly']"))
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
            this.reportIndex = 0;
            ColorConsole.WriteLine(ColorStyle.SectionHeader, "Errors and Failures");
            WriteErrorsAndFailures(result);
            Console.WriteLine();
        }

        private void WriteErrorsAndFailures(XmlNode result)
        {
            switch(result.Name)
            {
                case "test-case":
                    string resultState = result.GetAttribute("result");
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
                    break;

                case "test-suite":
                    if (result.GetAttribute("result") == "Failed")
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
                        }
                    }
                    
                    break;
            }

            // TODO: Display failures in fixture setup or teardown
            foreach (XmlNode childResult in result.ChildNodes)
                WriteErrorsAndFailures(childResult);
        }


        public void WriteNotRunReport()
        {
            this.reportIndex = 0;
            ColorConsole.WriteLine(ColorStyle.SectionHeader, "Tests Not Run");
            WriteNotRunResults(result);
            Console.WriteLine();
        }

        private void WriteNotRunResults(XmlNode result)
        {
            if (result.Name == "test-case")
            {
                string resultState = result.GetAttribute("result");
                if (resultState == "Skipped" || resultState == "Ignored" || resultState == "NotRunnable")
                {
                    using (new ColorConsole(ColorStyle.Warning))
                        WriteSingleResult(result);
                }
            }
            else
            {
                foreach (XmlNode childResult in result.ChildNodes)
                    WriteNotRunResults(childResult);
            }
        }

        private void WriteSingleResult(XmlNode result)
        {
            string status = result.GetAttribute("label");
            if (status == null)
                status = result.GetAttribute("result");
            string fullName = result.GetAttribute("fullname");

            Console.WriteLine("{0}) {1} : {2}", ++reportIndex, status, fullName);

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
