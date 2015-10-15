// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Common;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnitLite.Runner
{
    public class TextUI
    {
        private TextWriter _writer;
        private TextReader _reader;
        private CommandLineOptions _options;

        #region Constructors

        public TextUI(TextWriter writer, TextReader reader, CommandLineOptions options)
        {
            _options = options;
            _writer = writer;
            _reader = reader;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Writes the header.
        /// </summary>
        public void DisplayHeader()
        {
            Assembly executingAssembly = typeof(TextUI).GetTypeInfo().Assembly;
            AssemblyName assemblyName = AssemblyHelper.GetAssemblyName(executingAssembly);
            Version version = assemblyName.Version;
            string copyright = "Copyright (C) 2015, Charlie Poole";
            string build = "";

            object[] attrs = executingAssembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute)).ToArray();
            if (attrs.Length > 0)
            {
                var copyrightAttr = (AssemblyCopyrightAttribute)attrs[0];
                copyright = copyrightAttr.Copyright;
            }

            attrs = executingAssembly.GetCustomAttributes(typeof(AssemblyConfigurationAttribute)).ToArray();
            if (attrs.Length > 0)
            {
                var configAttr = (AssemblyConfigurationAttribute)attrs[0];
                build = string.Format("({0})", configAttr.Configuration);
            }

            _writer.WriteLine(String.Format("NUnitLite {0} {1}", version.ToString(3), build));
            _writer.WriteLine(copyright);
            _writer.WriteLine();
        }

        public void DisplayTestFiles(IEnumerable<string> testFiles)
        {
            _writer.WriteLine("Test Files");

            foreach (string testFile in testFiles)
                _writer.WriteLine("    " + testFile);

            _writer.WriteLine();
        }

        public void DisplayHelp()
        {
            _writer.WriteLine("Usage: NUNITLITE [options]");
            _writer.WriteLine();
            _writer.WriteLine("Runs a set of NUnitLite tests from the console.");
            _writer.WriteLine();

            _writer.WriteLine("Assembly:");
            _writer.WriteLine("      An alternate assembly from which to execute tests. Normally, the tests");
            _writer.WriteLine("      contained in the executable test assembly itself are run. An alternate");
            _writer.WriteLine("      assembly is specified using the assembly name, without any path or.");
            _writer.WriteLine("      extension. It must be in the same in the same directory as the executable");
            _writer.WriteLine("      or on the probing path.");
            _writer.WriteLine();

            _writer.WriteLine("Options:");
            using (var sw = new StringWriter())
            {
                _options.WriteOptionDescriptions(sw);
                _writer.Write(sw.ToString());
            }

            _writer.WriteLine("Notes:");
            _writer.WriteLine("    * File names may be listed by themselves, with a relative path or ");
            _writer.WriteLine("      using an absolute path. Any relative path is based on the current ");
            _writer.WriteLine("      directory or on the Documents folder if running on a under the ");
            _writer.WriteLine("      compact framework.");
            _writer.WriteLine();
            if (System.IO.Path.DirectorySeparatorChar != '/')
            {
                _writer.WriteLine("    * On Windows, options may be prefixed by a '/' character if desired");
                _writer.WriteLine();
            }
            _writer.WriteLine("    * Options that take values may use an equal sign or a colon");
            _writer.WriteLine("      to separate the option from its value.");
            _writer.WriteLine();
        }

        public void DisplayRequestedOptions()
        {
            _writer.WriteLine("Options");

            if (_options.DefaultTimeout >= 0)
                WriteLabelLine("    Default timeout: ", _options.DefaultTimeout);

            _writer.WriteLine();

            if (_options.TestList.Count > 0)
            {
                _writer.WriteLine("Selected test(s) -");
                foreach (string testName in _options.TestList)
                    _writer.WriteLine("    " + testName);
                _writer.WriteLine();
            }

            // TODO: Add where clause here
        }

        private bool _testCreatedOutput = false;

        public void TestFinished(ITestResult result)
        {
            bool isSuite = result.Test.IsSuite;

            var labels = "ON";

            if (_options.DisplayTestLabels != null)
                labels = _options.DisplayTestLabels.ToUpperInvariant();

            if (!isSuite && labels == "ALL" || !isSuite && labels == "ON" && result.Output.Length > 0)
            {
                _writer.WriteLine("=> " + result.Test.Name);
                _testCreatedOutput = true;
            }

            if (result.Output.Length > 0)
            {
                _writer.Write(result.Output);
                _testCreatedOutput = true;
                if (!result.Output.EndsWith("\n"))
                    _writer.WriteLine();
            }
        }

        public void WaitForUser(string message)
        {
            _writer.WriteLine(message);
            _reader.ReadLine();
        }

        #region Test Result Reports

        public void DisplaySummaryReport(ResultSummary summary)
        {
            var status = summary.ResultState.Status;

            var overallResult = status.ToString();
            if (overallResult == "Skipped")
                overallResult = "Warning";

            if (_testCreatedOutput)
                _writer.WriteLine();

            _writer.WriteLine("Test Run Summary");
            WriteLabelLine("   Overall result: ", overallResult);

            WriteSummaryCount("   Tests run: ", summary.RunCount);
            WriteSummaryCount(", Passed: ", summary.PassCount);
            WriteSummaryCount(", Errors: ", summary.ErrorCount);
            WriteSummaryCount(", Failures: ", summary.FailureCount);
            WriteSummaryCount(", Inconclusive: ", summary.InconclusiveCount);
            _writer.WriteLine();

            WriteSummaryCount("     Not run: ", summary.NotRunCount);
            WriteSummaryCount(", Invalid: ", summary.InvalidCount);
            WriteSummaryCount(", Ignored: ", summary.IgnoreCount);
            WriteSummaryCount(", Explicit: ", summary.ExplicitCount);
            WriteSummaryCount(", Skipped: ", summary.SkipCount);
            _writer.WriteLine();

            WriteLabelLine("  Start time: ", summary.StartTime.ToString("u"));
            WriteLabelLine("    End time: ", summary.EndTime.ToString("u"));
            WriteLabelLine("    Duration: ", summary.Duration.ToString("0.000") + " seconds");
            _writer.WriteLine();
        }

        private void WriteSummaryCount(string label, int count)
        {
            WriteLabel(label, count.ToString(CultureInfo.CurrentUICulture));
        }

        public void DisplayErrorsAndFailuresReport(ITestResult result)
        {
            _reportIndex = 0;
            _writer.WriteLine("Errors and Failures");
            DisplayErrorsAndFailures(result);
            _writer.WriteLine();

            if (_options.StopOnError)
            {
                _writer.WriteLine("Execution terminated after first error");
                _writer.WriteLine();
            }
        }

        public void DisplayNotRunReport(ITestResult result)
        {
            _reportIndex = 0;
            _writer.WriteLine("Tests Not Run");

            DisplayNotRunResults(result);

            _writer.WriteLine();
        }

        #endregion

        #endregion

        #region Helper Methods

        public void WriteLine(string message)
        {
            _writer.WriteLine(message);
        }

        public void DisplayError(string message)
        {
            _writer.WriteLine(message);
        }

        public void DisplayErrors(IList<string> messages)
        {
            foreach (string message in messages)
                _writer.WriteLine(message);
        }

        private void DisplayErrorsAndFailures(ITestResult result)
        {
            if (result.Test.IsSuite)
            {
                if (result.ResultState.Status == TestStatus.Failed)
                {
                    var suite = result.Test as TestSuite;
                    var site = result.ResultState.Site;
                    if (suite.TestType == "Theory" || site == FailureSite.SetUp || site == FailureSite.TearDown)
                        DisplayTestResult(result);
                    if (site == FailureSite.SetUp) return;
                }

                foreach (ITestResult childResult in result.Children)
                    DisplayErrorsAndFailures(childResult);
            }
            else if (result.ResultState.Status == TestStatus.Failed)
                DisplayTestResult(result);
        }

        private void DisplayNotRunResults(ITestResult result)
        {
            if (result.HasChildren)
                foreach (ITestResult childResult in result.Children)
                    DisplayNotRunResults(childResult);
            else if (result.ResultState.Status == TestStatus.Skipped)
                DisplayTestResult(result);
        }

        private static readonly char[] TRIM_CHARS = { '\r', '\n' };
        private int _reportIndex;

        private void DisplayTestResult(ITestResult result)
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
            _writer.WriteLine("{0}) {1} : {2}", ++_reportIndex, status, result.FullName);

            if (!string.IsNullOrEmpty(result.Message))
                _writer.WriteLine(result.Message.TrimEnd(TRIM_CHARS));

            if (!string.IsNullOrEmpty(result.StackTrace))
                _writer.WriteLine(result.StackTrace.TrimEnd(TRIM_CHARS));
        }

        private void WriteLabel(string label, object option)
        {
            _writer.Write(label);
            _writer.Write(option.ToString());
        }

        private void WriteLabelLine(string label, object option)
        {
            WriteLabel(label, option);
            _writer.WriteLine();
        }

        #endregion
    }
}