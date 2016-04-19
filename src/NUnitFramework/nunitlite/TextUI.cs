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
using System.Reflection;
using NUnit.Common;
using NUnit.Framework.Compatibility;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnitLite
{
    public class TextUI
    {
        private ExtendedTextWriter _writer;
        private TextReader _reader;
        private NUnitLiteOptions _options;

        #region Constructors

        public TextUI(ExtendedTextWriter writer, TextReader reader, NUnitLiteOptions options)
        {
            _options = options;
            _writer = writer;
            _reader = reader;
        }

        public TextUI(ExtendedTextWriter writer, TextReader reader)
            : this(writer, reader, new NUnitLiteOptions()) { }

        public TextUI(ExtendedTextWriter writer)
#if SILVERLIGHT || PORTABLE
            : this(writer, null, new NUnitLiteOptions()) { }
#else
            : this(writer, Console.In, new NUnitLiteOptions()) { }
#endif

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
            string copyright = "Copyright (C) 2016, Charlie Poole";
            string build = "";

            var copyrightAttr = executingAssembly.GetCustomAttribute<AssemblyCopyrightAttribute>();
            if (copyrightAttr != null)
                copyright = copyrightAttr.Copyright;

            var configAttr = executingAssembly.GetCustomAttribute<AssemblyConfigurationAttribute>();
            if (configAttr != null)
                build = string.Format("({0})", configAttr.Configuration);

            WriteHeader(String.Format("NUnitLite {0} {1}", version.ToString(3), build));
            WriteSubHeader(copyright);
            _writer.WriteLine();
        }

        #endregion

        #region DisplayTestFiles

        public void DisplayTestFiles(IEnumerable<string> testFiles)
        {
            WriteSectionHeader("Test Files");

            foreach (string testFile in testFiles)
                _writer.WriteLine(ColorStyle.Default, "    " + testFile);

            _writer.WriteLine();
        }

        #endregion

        #region DisplayHelp

        public void DisplayHelp()
        {
            WriteHeader("Usage: NUNITLITE [assembly] [options]");
            _writer.WriteLine();
            WriteHelpLine("Runs a set of NUnitLite tests from the console.");
            _writer.WriteLine();

            WriteSectionHeader("Assembly:");
            WriteHelpLine("      An alternate assembly from which to execute tests. Normally, the tests");
            WriteHelpLine("      contained in the executable test assembly itself are run. An alternate");
            WriteHelpLine("      assembly is specified using the assembly name, without any path or.");
            WriteHelpLine("      extension. It must be in the same in the same directory as the executable");
            WriteHelpLine("      or on the probing path.");
            _writer.WriteLine();

            WriteSectionHeader("Options:");
            using (var sw = new StringWriter())
            {
                _options.WriteOptionDescriptions(sw);
                _writer.Write(ColorStyle.Help, sw.ToString());
            }

            WriteSectionHeader("Notes:");
            WriteHelpLine("    * File names may be listed by themselves, with a relative path or ");
            WriteHelpLine("      using an absolute path. Any relative path is based on the current ");
            WriteHelpLine("      directory or on the Documents folder if running on a under the ");
            WriteHelpLine("      compact framework.");
            _writer.WriteLine();
            WriteHelpLine("    * On Windows, options may be prefixed by a '/' character if desired");
            _writer.WriteLine();
            WriteHelpLine("    * Options that take values may use an equal sign or a colon");
            WriteHelpLine("      to separate the option from its value.");
            _writer.WriteLine();
            WriteHelpLine("    * Several options that specify processing of XML output take");
            WriteHelpLine("      an output specification as a value. A SPEC may take one of");
            WriteHelpLine("      the following forms:");
            WriteHelpLine("          --OPTION:filename");
            WriteHelpLine("          --OPTION:filename;format=formatname");
            WriteHelpLine("          --OPTION:filename;transform=xsltfile");
            _writer.WriteLine();
            WriteHelpLine("      The --result option may use any of the following formats:");
            WriteHelpLine("          nunit3 - the native XML format for NUnit 3.0");
            WriteHelpLine("          nunit2 - legacy XML format used by earlier releases of NUnit");
            _writer.WriteLine();
            WriteHelpLine("      The --explore option may use any of the following formats:");
            WriteHelpLine("          nunit3 - the native XML format for NUnit 3.0");
            WriteHelpLine("          cases  - a text file listing the full names of all test cases.");
            WriteHelpLine("      If --explore is used without any specification following, a list of");
            WriteHelpLine("      test cases is output to the console.");
            _writer.WriteLine();
        }

        #endregion

        #region DisplayRuntimeEnvironment

        /// <summary>
        /// Displays info about the runtime environment.
        /// </summary>
        public void DisplayRuntimeEnvironment()
        {
#if !PORTABLE
            WriteSectionHeader("Runtime Environment");
            _writer.WriteLabelLine("   OS Version: ", Environment.OSVersion);
            _writer.WriteLabelLine("  CLR Version: ", Environment.Version);
            _writer.WriteLine();
#endif
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
                        _writer.WriteLabelLine("    Test: ", testName);

                if (_options.WhereClauseSpecified)
                    _writer.WriteLabelLine("    Where: ", _options.WhereClause.Trim());

                _writer.WriteLine();
            }
        }

        #endregion

        #region DisplayRunSettings

        public void DisplayRunSettings()
        {
            WriteSectionHeader("Run Settings");

            if (_options.DefaultTimeout >= 0)
                _writer.WriteLabelLine("    Default timeout: ", _options.DefaultTimeout);

#if PARALLEL
            _writer.WriteLabelLine(
                "    Number of Test Workers: ",
                _options.NumberOfTestWorkers >= 0
                    ? _options.NumberOfTestWorkers
#if NETCF
                    : 2);
#else
                    : Math.Max(Environment.ProcessorCount, 2));
#endif
#endif

#if !PORTABLE
            _writer.WriteLabelLine("    Work Directory: ", _options.WorkDirectory ?? NUnit.Env.DefaultWorkDirectory);
#endif

            _writer.WriteLabelLine("    Internal Trace: ", _options.InternalTraceLevel ?? "Off");

            if (_options.TeamCity)
                _writer.WriteLine(ColorStyle.Value, "    Display TeamCity Service Messages");

            _writer.WriteLine();
        }

        #endregion

        #region TestFinished

        private bool _testCreatedOutput = false;

        public void TestFinished(ITestResult result)
        {
            bool isSuite = result.Test.IsSuite;

            var labels = "ON";

#if !SILVERLIGHT
            if (_options.DisplayTestLabels != null)
                labels = _options.DisplayTestLabels.ToUpperInvariant();
#endif

            if (!isSuite && labels == "ALL" || !isSuite && labels == "ON" && result.Output.Length > 0)
            {
                _writer.WriteLine(ColorStyle.SectionHeader, "=> " + result.Test.FullName);
                _testCreatedOutput = true;
            }

            if (result.Output.Length > 0)
            {
                _writer.Write(ColorStyle.Output, result.Output);
                _testCreatedOutput = true;
                if (!result.Output.EndsWith("\n"))
                    _writer.WriteLine();
            }

            if (result.Test is TestAssembly && _testCreatedOutput)
            {
                _writer.WriteLine();
                _testCreatedOutput = false;
            }
        }

        #endregion

        #region WaitForUser

        public void WaitForUser(string message)
        {
            // Ignore if we don't have a TextReader
            if (_reader != null)
            {
                _writer.WriteLine(ColorStyle.Label, message);
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
                _writer.WriteLine();

            WriteSectionHeader("Test Run Summary");
            _writer.WriteLabelLine("  Overall result: ", overallResult, overallStyle);

            WriteSummaryCount("  Test Count: ", summary.TestCount);
            WriteSummaryCount(", Passed: ", summary.PassCount);
            WriteSummaryCount(", Failed: ", summary.FailedCount, ColorStyle.Failure);
            WriteSummaryCount(", Inconclusive: ", summary.InconclusiveCount);
            WriteSummaryCount(", Skipped: ", summary.TotalSkipCount);
            _writer.WriteLine();

            if (summary.FailedCount > 0)
            {
                WriteSummaryCount("    Failed Tests - Failures: ", summary.FailureCount, ColorStyle.Failure);
                WriteSummaryCount(", Errors: ", summary.ErrorCount, ColorStyle.Error);
                WriteSummaryCount(", Invalid: ", summary.InvalidCount, ColorStyle.Error);
                _writer.WriteLine();
            }
            if (summary.TotalSkipCount > 0)
            {
                WriteSummaryCount("    Skipped Tests - Ignored: ", summary.IgnoreCount, ColorStyle.Warning);
                WriteSummaryCount(", Explicit: ", summary.ExplicitCount);
                WriteSummaryCount(", Other: ", summary.SkipCount);
                _writer.WriteLine();
            }

            _writer.WriteLabelLine("  Start time: ", summary.StartTime.ToString("u"));
            _writer.WriteLabelLine("    End time: ", summary.EndTime.ToString("u"));
            _writer.WriteLabelLine("    Duration: ", summary.Duration.ToString("0.000") + " seconds");
            _writer.WriteLine();
        }

        private void WriteSummaryCount(string label, int count)
        {
            _writer.WriteLabel(label, count.ToString(CultureInfo.CurrentUICulture));
        }

        private void WriteSummaryCount(string label, int count, ColorStyle color)
        {
            _writer.WriteLabel(label, count.ToString(CultureInfo.CurrentUICulture), count > 0 ? color : ColorStyle.Value);
        }

        #endregion

        #region DisplayErrorsAndFailuresReport

        public void DisplayErrorsAndFailuresReport(ITestResult result)
        {
            _reportIndex = 0;
            WriteSectionHeader("Errors and Failures");
            DisplayErrorsAndFailures(result);
            _writer.WriteLine();

#if !SILVERLIGHT
            if (_options.StopOnError)
            {
                _writer.WriteLine(ColorStyle.Failure, "Execution terminated after first error");
                _writer.WriteLine();
            }
#endif
        }

        #endregion

        #region DisplayNotRunReport

        public void DisplayNotRunReport(ITestResult result)
        {
            _reportIndex = 0;
            WriteSectionHeader("Tests Not Run");

            DisplayNotRunResults(result);

            _writer.WriteLine();
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
            _writer.WriteLine(ColorStyle.Warning, text);
        }

        #endregion

        #region DisplayError

        public void DisplayError(string text)
        {
            _writer.WriteLine(ColorStyle.Error, text);
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

        private static readonly char[] TRIM_CHARS = new char[] { '\r', '\n' };
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

            ColorStyle style = ColorStyle.Output;
            switch (result.ResultState.Status)
            {
                case TestStatus.Failed:
                    style = ColorStyle.Failure;
                    break;
                case TestStatus.Skipped:
                    style = status == "Ignored" ? ColorStyle.Warning : ColorStyle.Output;
                    break;
                case TestStatus.Passed:
                    style = ColorStyle.Pass;
                    break;
            }

            _writer.WriteLine();
            _writer.WriteLine(
                style, string.Format("{0}) {1} : {2}", ++_reportIndex, status, result.FullName));

            if (!string.IsNullOrEmpty(result.Message))
                _writer.WriteLine(style, result.Message.TrimEnd(TRIM_CHARS));

            if (!string.IsNullOrEmpty(result.StackTrace))
                _writer.WriteLine(style, result.StackTrace.TrimEnd(TRIM_CHARS));
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
            _writer.WriteLine(ColorStyle.Header, text);
        }

        private void WriteSubHeader(string text)
        {
            _writer.WriteLine(ColorStyle.SubHeader, text);
        }

        private void WriteSectionHeader(string text)
        {
            _writer.WriteLine(ColorStyle.SectionHeader, text);
        }

        private void WriteHelpLine(string text)
        {
            _writer.WriteLine(ColorStyle.Help, text);
        }

        #endregion
    }
}
