// ***********************************************************************
// Copyright (c) 2015 Charlie Poole, Rob Prouse
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
using System.Reflection;
using NUnit.Common;
using NUnit.Compatibility;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnitLite
{
    public class TextUI
    {
        public ExtendedTextWriter Writer { get; }

        private readonly TextReader _reader;
        private readonly NUnitLiteOptions _options;

        private readonly bool _displayBeforeTest;
        private readonly bool _displayAfterTest;
        private readonly bool _displayBeforeOutput;

        #region Constructor

        public TextUI(ExtendedTextWriter writer, TextReader reader, NUnitLiteOptions options)
        {
            Writer = writer;
            _reader = reader;
            _options = options;

            string labelsOption = options.DisplayTestLabels?.ToUpperInvariant() ?? "ON";

            _displayBeforeTest = labelsOption == "ALL" || labelsOption == "BEFORE";
            _displayAfterTest = labelsOption == "AFTER";
            _displayBeforeOutput = _displayBeforeTest || _displayAfterTest || labelsOption == "ON";
        }

        #endregion

        #region Public Methods

        #region DisplayHeader

        /// <summary>
        /// Writes the header.
        /// </summary>
        public void DisplayHeader()
        {
            Assembly executingAssembly = GetType().GetTypeInfo().Assembly;
            AssemblyName assemblyName = AssemblyHelper.GetAssemblyName(executingAssembly);
            Version version = assemblyName.Version;
            string copyright = "Copyright (C) 2018 Charlie Poole, Rob Prouse";
            string build = "";

            var copyrightAttr = executingAssembly.GetCustomAttribute<AssemblyCopyrightAttribute>();
            if (copyrightAttr != null)
                copyright = copyrightAttr.Copyright;

            var configAttr = executingAssembly.GetCustomAttribute<AssemblyConfigurationAttribute>();
            if (configAttr != null)
                build = string.Format("({0})", configAttr.Configuration);

            WriteHeader(String.Format("NUnitLite {0} {1}", version.ToString(3), build));
            WriteSubHeader(copyright);
            Writer.WriteLine();
        }

        #endregion

        #region DisplayTestFiles

        public void DisplayTestFiles(IEnumerable<string> testFiles)
        {
            WriteSectionHeader("Test Files");

            foreach (string testFile in testFiles)
                Writer.WriteLine(ColorStyle.Default, "    " + testFile);

            Writer.WriteLine();
        }

        #endregion

        #region DisplayHelp

        public void DisplayHelp()
        {
            WriteHeader("Usage: NUNITLITE-RUNNER assembly [options]");
            WriteHeader("       USER-EXECUTABLE [options]");
            Writer.WriteLine();
            WriteHelpLine("Runs a set of NUnitLite tests from the console.");
            Writer.WriteLine();

            WriteSectionHeader("Assembly:");
            WriteHelpLine("      File name or path of the assembly from which to execute tests. Required");
            WriteHelpLine("      when using the nunitlite-runner executable to run the tests. Not allowed");
            WriteHelpLine("      when running a self-executing user test assembly.");
            Writer.WriteLine();

            WriteSectionHeader("Options:");
            using (var sw = new StringWriter())
            {
                _options.WriteOptionDescriptions(sw);
                Writer.Write(ColorStyle.Help, sw.ToString());
            }

            WriteSectionHeader("Notes:");
            WriteHelpLine("    * File names may be listed by themselves, with a relative path or ");
            WriteHelpLine("      using an absolute path. Any relative path is based on the current ");
            WriteHelpLine("      directory.");
            Writer.WriteLine();
            WriteHelpLine("    * On Windows, options may be prefixed by a '/' character if desired");
            Writer.WriteLine();
            WriteHelpLine("    * Options that take values may use an equal sign or a colon");
            WriteHelpLine("      to separate the option from its value.");
            Writer.WriteLine();
            WriteHelpLine("    * Several options that specify processing of XML output take");
            WriteHelpLine("      an output specification as a value. A SPEC may take one of");
            WriteHelpLine("      the following forms:");
            WriteHelpLine("          --OPTION:filename");
            WriteHelpLine("          --OPTION:filename;format=formatname");
            Writer.WriteLine();
            WriteHelpLine("      The --result option may use any of the following formats:");
            WriteHelpLine("          nunit3 - the native XML format for NUnit 3");
            WriteHelpLine("          nunit2 - legacy XML format used by earlier releases of NUnit");
            Writer.WriteLine();
            WriteHelpLine("      The --explore option may use any of the following formats:");
            WriteHelpLine("          nunit3 - the native XML format for NUnit 3");
            WriteHelpLine("          cases  - a text file listing the full names of all test cases.");
            WriteHelpLine("      If --explore is used without any specification following, a list of");
            WriteHelpLine("      test cases is output to the console.");
            Writer.WriteLine();
        }

        #endregion

        #region DisplayRuntimeEnvironment

        /// <summary>
        /// Displays info about the runtime environment.
        /// </summary>
        public void DisplayRuntimeEnvironment()
        {
            WriteSectionHeader("Runtime Environment");
#if !PLATFORM_DETECTION
            Writer.WriteLabelLine("   OS Version: ", System.Runtime.InteropServices.RuntimeInformation.OSDescription);
#else
            Writer.WriteLabelLine("   OS Version: ", OSPlatform.CurrentPlatform);
#endif
#if NETSTANDARD1_6
            Writer.WriteLabelLine("  CLR Version: ", System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription);
#else
            Writer.WriteLabelLine("  CLR Version: ", Environment.Version);
#endif
            Writer.WriteLine();
        }

        #endregion

        #region DisplayTestFilters

        public void DisplayTestFilters()
        {
            if (_options.TestList.Count > 0 || _options.WhereClauseSpecified)
            {
                WriteSectionHeader("Test Filters");

                if (_options.TestList.Count > 0)
                    foreach (string testName in _options.TestList)
                        Writer.WriteLabelLine("    Test: ", testName);

                if (_options.WhereClauseSpecified)
                    Writer.WriteLabelLine("    Where: ", _options.WhereClause.Trim());

                Writer.WriteLine();
            }
        }

        #endregion

        #region DisplayRunSettings

        public void DisplayRunSettings()
        {
            WriteSectionHeader("Run Settings");

            if (_options.DefaultTimeout >= 0)
                Writer.WriteLabelLine("    Default timeout: ", _options.DefaultTimeout);

#if PARALLEL
            Writer.WriteLabelLine(
                "    Number of Test Workers: ",
                _options.NumberOfTestWorkers >= 0
                    ? _options.NumberOfTestWorkers
                    : Math.Max(Environment.ProcessorCount, 2));
#endif

            Writer.WriteLabelLine("    Work Directory: ", _options.WorkDirectory ?? Directory.GetCurrentDirectory());
            Writer.WriteLabelLine("    Internal Trace: ", _options.InternalTraceLevel ?? "Off");

            if (_options.TeamCity)
                Writer.WriteLine(ColorStyle.Value, "    Display TeamCity Service Messages");

            Writer.WriteLine();
        }

        #endregion

        #region TestStarted

        public void TestStarted(ITest test)
        {
            if (_displayBeforeTest && !test.IsSuite)
                WriteLabelLine(test.FullName);
        }

        #endregion

        #region TestFinished

        private bool _testCreatedOutput = false;
        private bool _needsNewLine = false;

        public void TestFinished(ITestResult result)
        {
            if (result.Output.Length > 0)
            {
                if (_displayBeforeOutput)
                    WriteLabelLine(result.Test.FullName);

                WriteOutput(result.Output);

                if (!result.Output.EndsWith("\n"))
                    Writer.WriteLine();
            }

            if (!result.Test.IsSuite)
            {
                if (_displayAfterTest)
                    WriteLabelLineAfterTest(result.Test.FullName, result.ResultState);
            }

            if (result.Test is TestAssembly && _testCreatedOutput)
            {
                Writer.WriteLine();
                _testCreatedOutput = false;
            }
        }

        #endregion

        #region TestOutput

        public void TestOutput(TestOutput output)
        {
            if (_displayBeforeOutput && output.TestName != null)
                WriteLabelLine(output.TestName);

            WriteOutput(output.Stream == "Error" ? ColorStyle.Error : ColorStyle.Output, output.Text);
        }

        #endregion

        #region WaitForUser

        public void WaitForUser(string message)
        {
            // Ignore if we don't have a TextReader
            if (_reader != null)
            {
                Writer.WriteLine(ColorStyle.Label, message);
                _reader.ReadLine();
            }
        }

        #endregion

        #region Test Result Reports

        #region DisplaySummaryReport

        public void DisplaySummaryReport(ResultSummary summary)
        {
            var status = summary.ResultState.Status;

            var overallResult = status.ToString();
            if (overallResult == "Skipped")
                overallResult = "Warning";

            ColorStyle overallStyle = status == TestStatus.Passed
                ? ColorStyle.Pass
                : status == TestStatus.Failed
                    ? ColorStyle.Failure
                    : status == TestStatus.Skipped
                        ? ColorStyle.Warning
                        : ColorStyle.Output;

            if (_testCreatedOutput)
                Writer.WriteLine();

            WriteSectionHeader("Test Run Summary");
            Writer.WriteLabelLine("  Overall result: ", overallResult, overallStyle);

            WriteSummaryCount("  Test Count: ", summary.TestCount);
            WriteSummaryCount(", Passed: ", summary.PassCount);
            WriteSummaryCount(", Failed: ", summary.FailedCount, ColorStyle.Failure);
            WriteSummaryCount(", Warnings: ", summary.WarningCount, ColorStyle.Warning);
            WriteSummaryCount(", Inconclusive: ", summary.InconclusiveCount);
            WriteSummaryCount(", Skipped: ", summary.TotalSkipCount);
            Writer.WriteLine();

            if (summary.FailedCount > 0)
            {
                WriteSummaryCount("    Failed Tests - Failures: ", summary.FailureCount, ColorStyle.Failure);
                WriteSummaryCount(", Errors: ", summary.ErrorCount, ColorStyle.Error);
                WriteSummaryCount(", Invalid: ", summary.InvalidCount, ColorStyle.Error);
                Writer.WriteLine();
            }
            if (summary.TotalSkipCount > 0)
            {
                WriteSummaryCount("    Skipped Tests - Ignored: ", summary.IgnoreCount, ColorStyle.Warning);
                WriteSummaryCount(", Explicit: ", summary.ExplicitCount);
                WriteSummaryCount(", Other: ", summary.SkipCount);
                Writer.WriteLine();
            }

            Writer.WriteLabelLine("  Start time: ", summary.StartTime.ToString("u"));
            Writer.WriteLabelLine("    End time: ", summary.EndTime.ToString("u"));
            Writer.WriteLabelLine("    Duration: ", string.Format(NumberFormatInfo.InvariantInfo, "{0:0.000} seconds", summary.Duration));
            Writer.WriteLine();
        }

        private void WriteSummaryCount(string label, int count)
        {
            Writer.WriteLabel(label, count.ToString(CultureInfo.CurrentUICulture));
        }

        private void WriteSummaryCount(string label, int count, ColorStyle color)
        {
            Writer.WriteLabel(label, count.ToString(CultureInfo.CurrentUICulture), count > 0 ? color : ColorStyle.Value);
        }

        #endregion

        #region DisplayErrorsAndFailuresReport

        public void DisplayErrorsFailuresAndWarningsReport(ITestResult result)
        {
            _reportIndex = 0;
            WriteSectionHeader("Errors, Failures and Warnings");
            DisplayErrorsFailuresAndWarnings(result);
            Writer.WriteLine();

            if (_options.StopOnError)
            {
                Writer.WriteLine(ColorStyle.Failure, "Execution terminated after first error");
                Writer.WriteLine();
            }
        }

        #endregion

        #region DisplayNotRunReport

        public void DisplayNotRunReport(ITestResult result)
        {
            _reportIndex = 0;
            WriteSectionHeader("Tests Not Run");

            DisplayNotRunResults(result);

            Writer.WriteLine();
        }

        #endregion

        #region DisplayFullReport

#if FULL    // Not currently used, but may be reactivated
        /// <summary>
        /// Prints a full report of all results
        /// </summary>
        public void DisplayFullReport(ITestResult result)
        {
            WriteLine(ColorStyle.SectionHeader, "All Test Results -");
            _writer.WriteLine();

            DisplayAllResults(result, " ");

            _writer.WriteLine();
        }
#endif

        #endregion

        #endregion

        #region DisplayWarning

        public void DisplayWarning(string text)
        {
            Writer.WriteLine(ColorStyle.Warning, text);
        }

        #endregion

        #region DisplayError

        public void DisplayError(string text)
        {
            Writer.WriteLine(ColorStyle.Error, text);
        }

        #endregion

        #region DisplayErrors

        public void DisplayErrors(IList<string> messages)
        {
            foreach (string message in messages)
                DisplayError(message);
        }

        #endregion

        #endregion

        #region Helper Methods

        private void DisplayErrorsFailuresAndWarnings(ITestResult result)
        {
            bool display =
                result.ResultState.Status == TestStatus.Failed ||
                result.ResultState.Status == TestStatus.Warning;

            if (result.Test.IsSuite)
            {
                if (display)
                {
                    var suite = result.Test as TestSuite;
                    var site = result.ResultState.Site;
                    if (suite.TestType == "Theory" || site == FailureSite.SetUp || site == FailureSite.TearDown)
                        DisplayTestResult(result);
                    if (site == FailureSite.SetUp) return;
                }

                foreach (ITestResult childResult in result.Children)
                    DisplayErrorsFailuresAndWarnings(childResult);
            }
            else if (display)
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

        private static readonly char[] TRIM_CHARS = new char[] { '\r', '\n' };
        private int _reportIndex;

        private void DisplayTestResult(ITestResult result)
        {
            ResultState resultState = result.ResultState;
            string fullName = result.FullName;
            string message = result.Message;
            string stackTrace = result.StackTrace;
            string reportID = (++_reportIndex).ToString();
            int numAsserts = result.AssertionResults.Count;

            if (numAsserts > 0)
            {
                int assertionCounter = 0;
                string assertID = reportID;
                foreach (var assertion in result.AssertionResults)
                {
                    if (numAsserts > 1)
                        assertID = string.Format("{0}-{1}", reportID, ++assertionCounter);
                    ColorStyle style = GetColorStyle(resultState);
                    string status = assertion.Status.ToString();
                    DisplayTestResult(style, assertID, status, fullName, assertion.Message, assertion.StackTrace);
                }
            }
            else
            {
                ColorStyle style = GetColorStyle(resultState);
                string status = GetResultStatus(resultState);
                DisplayTestResult(style, reportID, status, fullName, message, stackTrace);
            }
        }

        private void DisplayTestResult(ColorStyle style, string prefix, string status, string fullName, string message, string stackTrace)
        {
            Writer.WriteLine();
            Writer.WriteLine(
                style, string.Format("{0}) {1} : {2}", prefix, status, fullName));

            if (!string.IsNullOrEmpty(message))
                Writer.WriteLine(style, message.TrimEnd(TRIM_CHARS));

            if (!string.IsNullOrEmpty(stackTrace))
                Writer.WriteLine(style, stackTrace.TrimEnd(TRIM_CHARS));
        }

        private static ColorStyle GetColorStyle(ResultState resultState)
        {
            ColorStyle style = ColorStyle.Output;
            switch (resultState.Status)
            {
                case TestStatus.Failed:
                    style = ColorStyle.Failure;
                    break;
                case TestStatus.Warning:
                    style = ColorStyle.Warning;
                    break;
                case TestStatus.Skipped:
                    style = resultState.Label == "Ignored" ? ColorStyle.Warning : ColorStyle.Output;
                    break;
                case TestStatus.Passed:
                    style = ColorStyle.Pass;
                    break;
            }

            return style;
        }

        private static string GetResultStatus(ResultState resultState)
        {
            string status = resultState.Label;
            if (string.IsNullOrEmpty(status))
                status = resultState.Status.ToString();

            if (status == "Failed" || status == "Error")
            {
                var site = resultState.Site.ToString();
                if (site == "SetUp" || site == "TearDown")
                    status = site + " " + status;
            }

            return status;
        }

#if FULL
        private void DisplayAllResults(ITestResult result, string indent)
        {
            string status = null;
            ColorStyle style = ColorStyle.Output;
            switch (result.ResultState.Status)
            {
                case TestStatus.Failed:
                    status = "FAIL";
                    style = ColorStyle.Failure;
                    break;
                case TestStatus.Skipped:
                    if (result.ResultState.Label == "Ignored")
                    {
                        status = "IGN ";
                        style = ColorStyle.Warning;
                    }
                    else
                    {
                        status = "SKIP";
                        style = ColorStyle.Output;
                    }
                    break;
                case TestStatus.Inconclusive:
                    status = "INC ";
                    style = ColorStyle.Output;
                    break;
                case TestStatus.Passed:
                    status = "OK  ";
                    style = ColorStyle.Pass;
                    break;
            }

            WriteLine(style, status + indent + result.Name);

            if (result.HasChildren)
                foreach (ITestResult childResult in result.Children)
                    PrintAllResults(childResult, indent + "  ");
        }
#endif

        private void WriteHeader(string text)
        {
            Writer.WriteLine(ColorStyle.Header, text);
        }

        private void WriteSubHeader(string text)
        {
            Writer.WriteLine(ColorStyle.SubHeader, text);
        }

        private void WriteSectionHeader(string text)
        {
            Writer.WriteLine(ColorStyle.SectionHeader, text);
        }

        private void WriteHelpLine(string text)
        {
            Writer.WriteLine(ColorStyle.Help, text);
        }

        private string _currentLabel;

        private void WriteLabelLine(string label)
        {
            if (label != _currentLabel)
            {
                WriteNewLineIfNeeded();

                Writer.WriteLine(ColorStyle.SectionHeader, "=> " + label);

                _testCreatedOutput = true;
                _currentLabel = label;
            }
        }

        private void WriteLabelLineAfterTest(string label, ResultState resultState)
        {
            WriteNewLineIfNeeded();

            string status = string.IsNullOrEmpty(resultState.Label)
                ? resultState.Status.ToString()
                : resultState.Label;

            Writer.Write(GetColorForResultStatus(status), status);
            Writer.WriteLine(ColorStyle.SectionHeader, " => " + label);

            _currentLabel = label;
        }

        private void WriteNewLineIfNeeded()
        {
            if (_needsNewLine)
            {
                Writer.WriteLine();
                _needsNewLine = false;
            }
        }

        private void WriteOutput(string text)
        {
            WriteOutput(ColorStyle.Output, text);
        }

        private void WriteOutput(ColorStyle color, string text)
        {
            Writer.Write(color, text);

            _testCreatedOutput = true;
            _needsNewLine = !text.EndsWith("\n");
        }

         private static ColorStyle GetColorForResultStatus(string status)
        {
            switch (status)
            {
                case "Passed":
                    return ColorStyle.Pass;
                case "Failed":
                    return ColorStyle.Failure;
                case "Error":
                case "Invalid":
                case "Cancelled":
                    return ColorStyle.Error;
                case "Warning":
                case "Ignored":
                    return ColorStyle.Warning;
                default:
                    return ColorStyle.Output;
            }
        }

        #endregion
    }
}
