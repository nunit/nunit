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
using System.Xml;
using NUnit.Util;

namespace NUnit.ConsoleRunner
{
    public class ResultReporter
    {
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
        /// Reports the results.
        /// </summary>
        /// <param name="result">The result.</param>
        public void ReportResults()
        {
            if (options.StopOnError && summary.ErrorsAndFailures > 0)
            {
                ColorSetter.WriteLine(ColorStyle.Failure, "Execution terminated after first error");
                Console.WriteLine();
            }

            WriteSummaryReport();

            if (ListAssembliesWithNoTests(result) > 0)
                Console.WriteLine();

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
            ColorSetter.WriteLine(ColorStyle.SectionHeader, "Test Run Summary -");
            ColorSetter.Write(ColorStyle.Label, "   Overall result: ");
            ColorSetter.WriteLine(overall, testRunResult);
            ColorSetter.WriteLine(ColorStyle.Default, String.Format( 
                "   Tests run: {0}, Errors: {1}, Failures: {2}, Inconclusive: {3}",
                summary.TestsRun, summary.Errors, summary.Failures, summary.Inconclusive));
            ColorSetter.WriteLine(ColorStyle.Warning, string.Format( 
                "     Not run: {0}, Invalid: {1}, Ignored: {2}, Skipped: {3}",
                summary.TestsNotRun, summary.NotRunnable, summary.Ignored, summary.Skipped));
            ColorSetter.WriteLine(ColorStyle.Default, string.Format(
                "        Time: {0} seconds", summary.Time));
            Console.WriteLine();
        }

        private int ListAssembliesWithNoTests(XmlNode result)
        {
            int count = 0;

            switch (result.Name)
            {
                case "test-run":
                    foreach (XmlNode child in result.ChildNodes)
                        count += ListAssembliesWithNoTests(child);
                    break;

                case "test-suite":
                    if (result.GetAttribute("type") == "Assembly")
                    {
                        if (result.GetAttribute("total") == "0")
                        {
                            using (new ColorSetter(ColorStyle.Warning))
                                Console.WriteLine("Warning: No tests found in " + result.GetAttribute("name"));
                        }
                        count++;
                        break;
                    }

                    foreach (XmlNode child in result.ChildNodes)
                        count += ListAssembliesWithNoTests(child);
                    break;
            }

            return count;
        }

        private void WriteErrorsAndFailuresReport()
        {
            this.reportIndex = 0;
            Console.WriteLine("Errors and Failures -");
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
                        using (new ColorSetter(ColorStyle.Failure))
                            WriteSingleResult(result);
                    }
                    else if (resultState == "Error")
                    {
                        using (new ColorSetter(ColorStyle.Error))
                            WriteSingleResult(result);
                    }
                    else if (resultState == "Cancelled")
                    {
                        using (new ColorSetter(ColorStyle.Warning))
                            WriteSingleResult(result);
                    }
                    return;

                case "test-run":
                    break;

                case "test-suite":
                    string resultType = result.GetAttribute("type");
                    if (resultType == "Theory")
                    {
                        resultState = result.GetAttribute("result");
                        if (resultState == "Failed")
                        {
                            using (new ColorSetter(ColorStyle.Failure))
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
            Console.WriteLine("Tests Not Run -");
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
                    using (new ColorSetter(ColorStyle.Warning))
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
                    Console.WriteLine(status == "Failed"
                        ? StackTraceFilter.Filter(stacktrace.InnerText)
                        : stacktrace.InnerText + Environment.NewLine);
                    //Console.WriteLine(stacktrace.InnerText + Environment.NewLine);
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
