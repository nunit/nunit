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

namespace NUnit.Framework.TestHarness
{
    public class ResultReporter
    {
        XmlNode result;
        string testRunResult;
        ResultSummary summary;

        int reportIndex = 0;

        public ResultReporter(XmlNode result)
        {
            this.result = result;
            this.testRunResult = GetStatus(result);
            this.summary = new ResultSummary(result);
            if (summary.ResultCount == 0)
                this.testRunResult += " - No tests found.";
        }

        /// <summary>
        /// Reports the results.
        /// </summary>
        /// <param name="result">The result.</param>
        public void ReportResults()
        {
            if (summary.ResultCount == 0)
                Console.WriteLine("Warning: No tests found");

            WriteSummaryReport();

            if (testRunResult == "Failed" || testRunResult == "Error")
            //if (summary.ErrorsAndFailures > 0)
                WriteErrorsAndFailuresReport();

            if (summary.TestsNotRun > 0)
                WriteNotRunReport();
        }

        private void WriteSummaryReport()
        {
            Console.WriteLine();
            Console.WriteLine("Test Run Summary -");
            Console.WriteLine(
                "   Overall result: {0}", testRunResult);
            Console.WriteLine(
                "   Tests run: {0}, Errors: {1}, Failures: {2}, Inconclusive: {3}",
                summary.TestsRun, summary.Errors, summary.Failures, summary.Inconclusive);
            Console.WriteLine(
                "     Not run: {0}, Invalid: {1}, Ignored: {2}, Skipped: {3}",
                summary.TestsNotRun, summary.NotRunnable, summary.Ignored, summary.Skipped);
            Console.WriteLine(
                "  Start time: {0}", summary.StartTime.ToString("u"));
            Console.WriteLine(
                "    End time: {0}", summary.EndTime.ToString("u"));
            Console.WriteLine(
                "    Duration: {0} seconds", summary.Duration.ToString("0.000"));
            Console.WriteLine();
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
            if (result.Name=="test-case")
            {
                string resultState = result.Attributes["result"].Value;
                if (resultState == "Failed" || resultState == "Error" || resultState == "Cancelled")
                    WriteSingleResult(result);
            }
            else
            {
                if (result.Name == "test-suite")
                {
                    string resultType = result.Attributes["type"].Value;
                    if (resultType == "Theory")
                    {
                        string resultState = result.Attributes["result"].Value;
                        if (resultState == "Failed")
                            WriteSingleResult(result);
                    }
                }

                foreach (XmlNode childResult in result.ChildNodes)
                    WriteErrorsAndFailures(childResult);
            }
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
                string resultState = result.Attributes["result"].Value;
                if (resultState == "Skipped" || resultState == "Ignored" || resultState == "NotRunnable")
                    WriteSingleResult(result);
            }
            else
            {
                foreach (XmlNode childResult in result.ChildNodes)
                    WriteNotRunResults(childResult);
            }
        }

        private void WriteSingleResult(XmlNode result)
        {
            string status = GetStatus(result);
            string fullName = result.Attributes["fullname"].Value;

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

        private string GetStatus(XmlNode result)
        {
            if (result.Attributes["label"] != null)
                return result.Attributes["label"].Value;

            return result.Attributes["result"].Value;
        }
    }
}
