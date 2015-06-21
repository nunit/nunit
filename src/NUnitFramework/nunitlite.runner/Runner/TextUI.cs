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
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnitLite.Runner
{
    public class TextUI
    {
        private ExtendedTextWriter _outWriter;
#if !SILVERLIGHT
        private ConsoleOptions _options;
#endif

        #region Constructors

#if SILVERLIGHT
        public TextUI(ExtendedTextWriter writer)
        {
            _outWriter = writer;
        }
#else
        public TextUI(ConsoleOptions options) : this(null, options) { }

        public TextUI(ExtendedTextWriter writer, ConsoleOptions options)
        {
            _options = options;
            _outWriter = writer ?? new ColorConsoleWriter(!options.NoColor);
        }
#endif

        #endregion

        #region Public Methods

        #region DisplayHeader

        /// <summary>
        /// Writes the header.
        /// </summary>
        public void DisplayHeader()
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            AssemblyName assemblyName = AssemblyHelper.GetAssemblyName(executingAssembly);
            Version version = assemblyName.Version;
            string copyright = "Copyright (C) 2015, Charlie Poole";
            string build = "";

            object[] attrs = executingAssembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            if (attrs.Length > 0)
            {
                var copyrightAttr = (AssemblyCopyrightAttribute)attrs[0];
                copyright = copyrightAttr.Copyright;
            }

            attrs = executingAssembly.GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false);
            if (attrs.Length > 0)
            {
                var configAttr = (AssemblyConfigurationAttribute)attrs[0];
                build = string.Format("({0})", configAttr.Configuration);
            }

            WriteHeader(String.Format("NUnitLite {0} {1}", version.ToString(3), build));
            WriteSubHeader(copyright);
            SkipLine();
        }

        #endregion

        #region DisplayTestFiles

        public void DisplayTestFiles(string[] testFiles)
        {
            WriteSectionHeader("Test Files");

            foreach (string testFile in testFiles)
                WriteLine(ColorStyle.Default, "    " + testFile);

            SkipLine();
        }

        #endregion

        #region DisplayHelp

#if !SILVERLIGHT
        public void DisplayHelp()
        {
            // TODO: The NETCF code is just a placeholder. Figure out how to do it correctly.
            WriteHeader("Usage: NUNITLITE [assembly] [options]");
            SkipLine();
            WriteHelpLine("Runs a set of NUnitLite tests from the console.");
            SkipLine();

            WriteSectionHeader("Assembly:");
            WriteHelpLine("      An alternate assembly from which to execute tests. Normally, the tests");
            WriteHelpLine("      contained in the executable test assembly itself are run. An alternate");
            WriteHelpLine("      assembly is specified using the assembly name, without any path or.");
            WriteHelpLine("      extension. It must be in the same in the same directory as the executable");
            WriteHelpLine("      or on the probing path.");
            SkipLine();

            WriteSectionHeader("Options:");
            using (var sw = new StringWriter())
            {
                _options.WriteOptionDescriptions(sw);
                _outWriter.Write(ColorStyle.Help, sw.ToString());
            }

            WriteSectionHeader("Notes:");
            WriteHelpLine("    * File names may be listed by themselves, with a relative path or ");
            WriteHelpLine("      using an absolute path. Any relative path is based on the current ");
            WriteHelpLine("      directory or on the Documents folder if running on a under the ");
            WriteHelpLine("      compact framework.");
            SkipLine();
            if (System.IO.Path.DirectorySeparatorChar != '/')
            {
                WriteHelpLine("    * On Windows, options may be prefixed by a '/' character if desired");
                SkipLine();
            }
            WriteHelpLine("    * Options that take values may use an equal sign or a colon");
            WriteHelpLine("      to separate the option from its value.");
            SkipLine();
            WriteHelpLine("    * Several options that specify processing of XML output take");
            WriteHelpLine("      an output specification as a value. A SPEC may take one of");
            WriteHelpLine("      the following forms:");
            WriteHelpLine("          --OPTION:filename");
            WriteHelpLine("          --OPTION:filename;format=formatname");
            WriteHelpLine("          --OPTION:filename;transform=xsltfile");
            SkipLine();
            WriteHelpLine("      The --result option may use any of the following formats:");
            WriteHelpLine("          nunit3 - the native XML format for NUnit 3.0");
            WriteHelpLine("          nunit2 - legacy XML format used by earlier releases of NUnit");
            SkipLine();
            WriteHelpLine("      The --explore option may use any of the following formats:");
            WriteHelpLine("          nunit3 - the native XML format for NUnit 3.0");
            WriteHelpLine("          cases  - a text file listing the full names of all test cases.");
            WriteHelpLine("      If --explore is used without any specification following, a list of");
            WriteHelpLine("      test cases is output to the console.");
            SkipLine();
        }
#endif

        #endregion

        #region DisplayRequestedOptions

#if !SILVERLIGHT
        public void DisplayRequestedOptions()
        {
            WriteSectionHeader("Options");

            if (_options.DefaultTimeout >= 0)
                WriteLabelLine("    Default timeout: ", _options.DefaultTimeout);

            WriteLabelLine("    Work Directory: ", _options.WorkDirectory ?? NUnit.Env.DefaultWorkDirectory);

            WriteLabelLine("    Internal Trace: ", _options.InternalTraceLevel ?? "Off");

            if (_options.TeamCity)
                _outWriter.WriteLine(ColorStyle.Value, "    Display TeamCity Service Messages");

            SkipLine();

            if (_options.TestList.Count > 0)
            {
                WriteSectionHeader("Selected test(s) -");
                foreach (string testName in _options.TestList)
                    _outWriter.WriteLine(ColorStyle.Value, "    " + testName);
                SkipLine();
            }

            if (!string.IsNullOrEmpty(_options.Include))
            {
                WriteLabelLine("Included categories: ", _options.Include);
                SkipLine();
            }

            if (!string.IsNullOrEmpty(_options.Exclude))
            {
                WriteLabelLine("Excluded categories: ", _options.Exclude);
                SkipLine();
            }
        }
#endif

        #endregion

        #region DisplayRuntimeEnvironment

        /// <summary>
        /// Displays info about the runtime environment.
        /// </summary>
        public void DisplayRuntimeEnvironment()
        {
            WriteSectionHeader("Runtime Environment");
            WriteLabelLine("   OS Version: ", Environment.OSVersion);
            WriteLabelLine("  CLR Version: ", Environment.Version);
            SkipLine();
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
                labels = _options.DisplayTestLabels.ToUpper(CultureInfo.InvariantCulture);
#endif

            if (!isSuite && labels == "ALL" || !isSuite && labels == "ON" && result.Output.Length > 0)
            {
                _outWriter.WriteLine(ColorStyle.SectionHeader, "=> " + result.Test.Name);
                _testCreatedOutput = true;
            }

            if (result.Output.Length > 0)
            {
                _outWriter.Write(ColorStyle.Output, result.Output);
                _testCreatedOutput = true;
                if (!result.Output.EndsWith("\n"))
                    SkipLine();
            }
        }

        #endregion

        #region WaitForUser

#if !SILVERLIGHT
        public void WaitForUser(string message)
        {
            // Ignore if we are not using the console
            if (_outWriter is ColorConsoleWriter)
            {
                _outWriter.WriteLine(ColorStyle.Label, message);
                Console.ReadLine();
            }
        }
#endif

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
                SkipLine();

            WriteSectionHeader("Test Run Summary");
            WriteLabelLine("   Overall result: ", overallResult, overallStyle);

            WriteLabel("   Tests run: ", summary.RunCount.ToString(CultureInfo.CurrentUICulture));
            WriteLabel(", Passed: ", summary.PassCount.ToString(CultureInfo.CurrentUICulture));
            WriteLabel(", Errors: ", summary.ErrorCount.ToString(CultureInfo.CurrentUICulture));
            WriteLabel(", Failures: ", summary.FailureCount.ToString(CultureInfo.CurrentUICulture));
            WriteLabelLine(", Inconclusive: ", summary.InconclusiveCount.ToString(CultureInfo.CurrentUICulture));

            var notRunTotal = summary.SkipCount + summary.IgnoreCount + summary.InvalidCount;
            WriteLabel("     Not run: ", notRunTotal.ToString(CultureInfo.CurrentUICulture));
            WriteLabel(", Invalid: ", summary.InvalidCount.ToString(CultureInfo.CurrentUICulture));
            WriteLabel(", Ignored: ", summary.IgnoreCount.ToString(CultureInfo.CurrentUICulture));
            WriteLabelLine(", Skipped: ", summary.SkipCount.ToString(CultureInfo.CurrentUICulture));

            WriteLabelLine("  Start time: ", summary.StartTime.ToString("u"));
            WriteLabelLine("    End time: ", summary.EndTime.ToString("u"));
            WriteLabelLine("    Duration: ", summary.Duration.ToString("0.000") + " seconds");
            SkipLine();
        }

        #endregion

        #region DisplayErrorsAndFailuresReport

        public void DisplayErrorsAndFailuresReport(ITestResult result)
        {
            _reportIndex = 0;
            WriteSectionHeader("Errors and Failures");
            DisplayErrorsAndFailures(result);
            SkipLine();

#if !SILVERLIGHT
            if (_options.StopOnError)
            {
                WriteLine(ColorStyle.Failure, "Execution terminated after first error");
                SkipLine();
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

            SkipLine();
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
            SkipLine();

            DisplayAllResults(result, " ");

            SkipLine();
        }
#endif

        #endregion

        #endregion

        #region DisplayWarning

        public void DisplayWarning(string text)
        {
            WriteLine(ColorStyle.Warning, text);
        }

        #endregion

        #region DisplayError

        public void DisplayError(string text)
        {
            WriteLine(ColorStyle.Error, text);
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

            SkipLine();
            WriteLine(
                style, string.Format("{0}) {1} : {2}", ++_reportIndex, status, result.FullName));

            if (!string.IsNullOrEmpty(result.Message))
                WriteLine(style, result.Message.TrimEnd(TRIM_CHARS));

            if (!string.IsNullOrEmpty(result.StackTrace))
                WriteLine(style, result.StackTrace.TrimEnd(TRIM_CHARS));
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

        private void WriteLine(ColorStyle style, string text)
        {
            _outWriter.WriteLine(style, text);
        }

        private void WriteHeader(string text)
        {
            WriteLine(ColorStyle.Header, text);
        }

        private void WriteSubHeader(string text)
        {
            WriteLine(ColorStyle.SubHeader, text);
        }

        private void WriteSectionHeader(string text)
        {
            WriteLine(ColorStyle.SectionHeader, text);
        }

        private void WriteHelpLine(string text)
        {
            WriteLine(ColorStyle.Help, text);
        }

        private void WriteLabel(string label, object option)
        {
            _outWriter.WriteLabel(label, option);
        }

        private void WriteLabelLine(string label, object option)
        {
            _outWriter.WriteLabelLine(label, option);
        }

        private void WriteLabelLine(string label, object option, ColorStyle valueStyle)
        {
            _outWriter.WriteLabelLine(label, option, valueStyle);
        }

        private void SkipLine()
        {
            _outWriter.WriteLine();
        }

        #endregion
    }
}
