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

#if !PORTABLE
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Common;
using NUnit.Framework;
using NUnit.Framework.Internal;
using NUnit.TestUtilities;
using NUnit.Tests.Assemblies;
using NUnit.Framework.Interfaces;

namespace NUnitLite.Tests
{
    public class TextUITests
    {
        private static readonly string NL = Environment.NewLine;

        private StringBuilder _reportBuilder;

        [SetUp]
        public void SetUp()
        {
            _reportBuilder = new StringBuilder();
        }

        private TextUI CreateTextUI()
        {
            return CreateTextUI("Off");
        }

        private TextUI CreateTextUI(string labelSetting)
        {
            var writer = new ExtendedTextWrapper(new StringWriter(_reportBuilder));
            var options = new NUnitLiteOptions("--labels:" + labelSetting);
            return new TextUI(writer, null, options);
        }

        [Test]
        public void DisplayWarning()
        {
            CreateTextUI().DisplayWarning("This is a warning");

            Assert.That(Report, Is.EqualTo("This is a warning\n"));
        }

        [Test]
        public void DisplayError()
        {
            CreateTextUI().DisplayError("This is an error");

            Assert.That(Report, Is.EqualTo("This is an error\n"));
        }

        [Test]
        public void DisplayErrors()
        {
            CreateTextUI().DisplayErrors(new string[] { "This is an error", "Another error", "Final error" });

            Assert.That(Report, Is.EqualTo("This is an error\nAnother error\nFinal error\n"));
        }

        [Test]
        public void DisplayHeader()
        {
            CreateTextUI().DisplayHeader();

            Assert.That(Report, Does.Match("^NUnitLite.*\nCopyright.*\n\n$"));
        }

        [Test]
        public void DisplayTestFiles()
        {
            CreateTextUI().DisplayTestFiles(new string[] { "test1.dll", "another.test.dll", "final.test.dll" });

            Assert.That(Report, Is.EqualTo("Test Files\n    test1.dll\n    another.test.dll\n    final.test.dll\n\n"));
        }

        [Test]
        public void DisplayRuntimeEnvironment()
        {
            CreateTextUI().DisplayRuntimeEnvironment();

            Assert.That(Report, Does.Match("^Runtime Environment\n   OS Version:.*\n  CLR Version:.*\n\n$"));
        }

        [TestCase("Off", "")]
        [TestCase("On", "")]
        [TestCase("All", "=> NUnitLite.Tests.TextUITests.MyFakeMethod\n")]
        [TestCase("Before", "=> NUnitLite.Tests.TextUITests.MyFakeMethod\n")]
        [TestCase("After", "")]
        public void TestStarted(string labels, string expected)
        {
            var test = Fakes.GetTestMethod(this, "MyFakeMethod");
            CreateTextUI(labels).TestStarted(test);

            Assert.That(Report, Is.EqualTo(expected));
        }

        [TestCase("Off", "")]
        [TestCase("On", "")]
        [TestCase("All", "")]
        [TestCase("Before", "")]
        [TestCase("After", "")]
        public void SuiteStarted(string labels, string expected)
        {
            var suite = new TestSuite("DummySuite");
            CreateTextUI(labels).TestStarted(suite);

            Assert.That(Report, Is.EqualTo(expected));
        }

        [TestCase("Off", TestStatus.Passed, "", "", "")]
        [TestCase("On", TestStatus.Passed, "", "", "")]
        [TestCase("All", TestStatus.Passed, "", "", "")]
        [TestCase("Before", TestStatus.Passed, "", "", "")]
        [TestCase("After", TestStatus.Passed, "", "", "Passed => NUnitLite.Tests.TextUITests.MyFakeMethod\n")]
        [TestCase("After", TestStatus.Failed, "", "", "Failed => NUnitLite.Tests.TextUITests.MyFakeMethod\n")]
        [TestCase("After", TestStatus.Failed, "Invalid", "", "Invalid => NUnitLite.Tests.TextUITests.MyFakeMethod\n")]
        [TestCase("After", TestStatus.Failed, "Cancelled", "", "Cancelled => NUnitLite.Tests.TextUITests.MyFakeMethod\n")]
        [TestCase("After", TestStatus.Failed, "Error", "", "Error => NUnitLite.Tests.TextUITests.MyFakeMethod\n")]
        [TestCase("After", TestStatus.Inconclusive, "", "", "Inconclusive => NUnitLite.Tests.TextUITests.MyFakeMethod\n")]
        [TestCase("After", TestStatus.Skipped, "", "", "Skipped => NUnitLite.Tests.TextUITests.MyFakeMethod\n")]
        [TestCase("After", TestStatus.Skipped, "Ignored", "", "Ignored => NUnitLite.Tests.TextUITests.MyFakeMethod\n")]
        [TestCase("After", TestStatus.Warning, "", "", "Warning => NUnitLite.Tests.TextUITests.MyFakeMethod\n")]
        [TestCase("Off", TestStatus.Passed, "",
            "First line of output\nAnother line of output",
            "First line of output\nAnother line of output\n")]
        [TestCase("On", TestStatus.Passed, "",
            "First line of output\nAnother line of output",
            "=> NUnitLite.Tests.TextUITests.MyFakeMethod\nFirst line of output\nAnother line of output\n")]
        [TestCase("All", TestStatus.Passed, "",
            "First line of output\nAnother line of output",
            "=> NUnitLite.Tests.TextUITests.MyFakeMethod\nFirst line of output\nAnother line of output\n")]
        [TestCase("Before", TestStatus.Passed, "",
            "First line of output\nAnother line of output",
            "=> NUnitLite.Tests.TextUITests.MyFakeMethod\nFirst line of output\nAnother line of output\n")]
        [TestCase("After", TestStatus.Passed, "",
            "First line of output\nAnother line of output",
            "=> NUnitLite.Tests.TextUITests.MyFakeMethod\nFirst line of output\nAnother line of output\nPassed => NUnitLite.Tests.TextUITests.MyFakeMethod\n")]
        [TestCase("After", TestStatus.Failed, "",
            "First line of output\nAnother line of output",
            "=> NUnitLite.Tests.TextUITests.MyFakeMethod\nFirst line of output\nAnother line of output\nFailed => NUnitLite.Tests.TextUITests.MyFakeMethod\n")]
        [TestCase("After", TestStatus.Failed, "Invalid",
            "First line of output\nAnother line of output",
            "=> NUnitLite.Tests.TextUITests.MyFakeMethod\nFirst line of output\nAnother line of output\nInvalid => NUnitLite.Tests.TextUITests.MyFakeMethod\n")]
        [TestCase("After", TestStatus.Failed, "Cancelled",
            "First line of output\nAnother line of output",
            "=> NUnitLite.Tests.TextUITests.MyFakeMethod\nFirst line of output\nAnother line of output\nCancelled => NUnitLite.Tests.TextUITests.MyFakeMethod\n")]
        [TestCase("After", TestStatus.Failed, "Error",
            "First line of output\nAnother line of output",
            "=> NUnitLite.Tests.TextUITests.MyFakeMethod\nFirst line of output\nAnother line of output\nError => NUnitLite.Tests.TextUITests.MyFakeMethod\n")]
        [TestCase("After", TestStatus.Inconclusive, "",
            "First line of output\nAnother line of output",
            "=> NUnitLite.Tests.TextUITests.MyFakeMethod\nFirst line of output\nAnother line of output\nInconclusive => NUnitLite.Tests.TextUITests.MyFakeMethod\n")]
        [TestCase("After", TestStatus.Skipped, "",
            "First line of output\nAnother line of output",
            "=> NUnitLite.Tests.TextUITests.MyFakeMethod\nFirst line of output\nAnother line of output\nSkipped => NUnitLite.Tests.TextUITests.MyFakeMethod\n")]
        [TestCase("After", TestStatus.Skipped, "Ignored",
            "First line of output\nAnother line of output",
            "=> NUnitLite.Tests.TextUITests.MyFakeMethod\nFirst line of output\nAnother line of output\nIgnored => NUnitLite.Tests.TextUITests.MyFakeMethod\n")]
        [TestCase("After", TestStatus.Warning, "",
            "First line of output\nAnother line of output",
            "=> NUnitLite.Tests.TextUITests.MyFakeMethod\nFirst line of output\nAnother line of output\nWarning => NUnitLite.Tests.TextUITests.MyFakeMethod\n")]
        public void TestFinished(string labelsOption, TestStatus resultStatus, string resultLabel, string output, string expected)
        {
            var result = new TestCaseResult(Fakes.GetTestMethod(this, "MyFakeMethod"));
            result.SetResult(new ResultState(resultStatus, resultLabel));

            foreach (var line in output.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))
                result.OutWriter.WriteLine(line);

            CreateTextUI(labelsOption).TestFinished(result);

            Assert.That(Report, Is.EqualTo(expected));
        }

        [TestCase("Off", TestStatus.Passed, "", "", "")]
        [TestCase("On", TestStatus.Passed, "", "", "")]
        [TestCase("All", TestStatus.Passed, "", "", "")]
        [TestCase("Before", TestStatus.Passed, "", "", "")]
        [TestCase("After", TestStatus.Passed, "", "", "")]
        [TestCase("Off", TestStatus.Passed, "",
            "First line of output\nAnother line of output",
            "First line of output\nAnother line of output\n")]
        [TestCase("On", TestStatus.Passed, "",
            "First line of output\nAnother line of output",
            "=> DummySuite\nFirst line of output\nAnother line of output\n")]
        [TestCase("All", TestStatus.Passed, "",
            "First line of output\nAnother line of output",
            "=> DummySuite\nFirst line of output\nAnother line of output\n")]
        [TestCase("Before", TestStatus.Passed, "",
            "First line of output\nAnother line of output",
            "=> DummySuite\nFirst line of output\nAnother line of output\n")]
        [TestCase("After", TestStatus.Passed, "",
            "First line of output\nAnother line of output",
            "=> DummySuite\nFirst line of output\nAnother line of output\n")]
        public void SuiteFinished(string labelsOption, TestStatus resultStatus, string resultLabel, string output, string expected)
        {
            var result = new TestSuiteResult(new TestSuite("DummySuite"));
            result.SetResult(new ResultState(resultStatus, resultLabel));

            foreach (var line in output.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))
                result.OutWriter.WriteLine(line);

            CreateTextUI(labelsOption).TestFinished(result);

            Assert.That(Report, Is.EqualTo(expected));
        }

        [TestCase("Off", "OUTPUT\n")]
        [TestCase("On", "=> SomeMethod\nOUTPUT\n")]
        [TestCase("All", "=> SomeMethod\nOUTPUT\n")]
        [TestCase("Before", "=> SomeMethod\nOUTPUT\n")]
        [TestCase("After", "=> SomeMethod\nOUTPUT\n")]
        public void ImmediateOutput(string labels, string expected)
        {
            CreateTextUI(labels).TestOutput(new NUnit.Framework.Interfaces.TestOutput("OUTPUT", "", "SomeMethod"));

            Assert.That(Report, Is.EqualTo(expected));
        }

        [TestCase("Off", "OUTPUT\n")]
        [TestCase("On", "=> NUnitLite.Tests.TextUITests.MyFakeMethod\nOUTPUT\n")]
        [TestCase("All", "=> NUnitLite.Tests.TextUITests.MyFakeMethod\nOUTPUT\n")]
        [TestCase("Before", "=> NUnitLite.Tests.TextUITests.MyFakeMethod\nOUTPUT\n")]
        [TestCase("After", "=> NUnitLite.Tests.TextUITests.MyFakeMethod\nOUTPUT\nPassed => NUnitLite.Tests.TextUITests.MyFakeMethod\n")]
        public void SingleTest_StartAndFinish(string labels, string expected)
        {
            var test = Fakes.GetTestMethod(this, "MyFakeMethod");
            var result = new TestCaseResult(test);
            result.SetResult(ResultState.Success);
            var textUI = CreateTextUI(labels);

            textUI.TestStarted(test);
            result.OutWriter.WriteLine("OUTPUT");
            textUI.TestFinished(result);

            Assert.That(Report, Is.EqualTo(expected));
        }

        [TestCase("Off", "IMMEDIATE OUTPUT\nNORMAL OUTPUT\n")]
        [TestCase("On", "=> NUnitLite.Tests.TextUITests.MyFakeMethod\nIMMEDIATE OUTPUT\nNORMAL OUTPUT\n")]
        [TestCase("All", "=> NUnitLite.Tests.TextUITests.MyFakeMethod\nIMMEDIATE OUTPUT\nNORMAL OUTPUT\n")]
        [TestCase("Before", "=> NUnitLite.Tests.TextUITests.MyFakeMethod\nIMMEDIATE OUTPUT\nNORMAL OUTPUT\n")]
        [TestCase("After", "=> NUnitLite.Tests.TextUITests.MyFakeMethod\nIMMEDIATE OUTPUT\nNORMAL OUTPUT\nPassed => NUnitLite.Tests.TextUITests.MyFakeMethod\n")]
        public void SingleTest_ImmediateOutput(string labels, string expected)
        {
            var test = Fakes.GetTestMethod(this, "MyFakeMethod");
            var result = new TestCaseResult(test);
            result.SetResult(ResultState.Success);
            var textUI = CreateTextUI(labels);

            textUI.TestStarted(test);
            textUI.TestOutput(new TestOutput("IMMEDIATE OUTPUT", "", test.FullName));
            result.OutWriter.WriteLine("NORMAL OUTPUT");
            textUI.TestFinished(result);

            Assert.That(Report, Is.EqualTo(expected));
        }

        [TestCase("Off", "OUTPUT1\nOUTPUT2\n")]
        [TestCase("On",
            "=> NUnitLite.Tests.TextUITests.MyFakeMethod\n" +
            "OUTPUT1\n" +
            "=> NUnitLite.Tests.TextUITests.AnotherFakeMethod\n" +
            "OUTPUT2\n")]
        [TestCase("All",
            "=> NUnitLite.Tests.TextUITests.MyFakeMethod\n" +
            "OUTPUT1\n" +
            "=> NUnitLite.Tests.TextUITests.AnotherFakeMethod\n" +
            "OUTPUT2\n")]
        [TestCase("Before",
            "=> NUnitLite.Tests.TextUITests.MyFakeMethod\n" +
            "OUTPUT1\n" +
            "=> NUnitLite.Tests.TextUITests.AnotherFakeMethod\n" +
            "OUTPUT2\n")]
        [TestCase("After",
            "=> NUnitLite.Tests.TextUITests.MyFakeMethod\n" +
            "OUTPUT1\n" +
            "Failed => NUnitLite.Tests.TextUITests.MyFakeMethod\n" +
            "=> NUnitLite.Tests.TextUITests.AnotherFakeMethod\n" +
            "OUTPUT2\n" +
            "Passed => NUnitLite.Tests.TextUITests.AnotherFakeMethod\n")]
        public void TwoTests_SequentialExecution(string labels, string expected)
        {
            var test1 = Fakes.GetTestMethod(this, "MyFakeMethod");
            var result1 = new TestCaseResult(test1);
            result1.SetResult(ResultState.Failure);

            var test2 = Fakes.GetTestMethod(this, "AnotherFakeMethod");
            var result2 = new TestCaseResult(test2);
            result2.SetResult(ResultState.Success);

            var textUI = CreateTextUI(labels);

            textUI.TestStarted(test1);
            result1.OutWriter.WriteLine("OUTPUT1");
            textUI.TestFinished(result1);
            textUI.TestStarted(test2);
            result2.OutWriter.WriteLine("OUTPUT2");
            textUI.TestFinished(result2);

            Assert.That(Report, Is.EqualTo(expected));
        }

        [TestCase("Off",
            "Immediate output from first test\n" +
            "Another immediate output from first test\n" +
            "Immediate output from second test\n" +
            "OUTPUT1\n" +
            "OUTPUT2\n")]
        [TestCase("On",
            "=> NUnitLite.Tests.TextUITests.MyFakeMethod\n" +
            "Immediate output from first test\n" +
            "Another immediate output from first test\n" +
            "=> NUnitLite.Tests.TextUITests.AnotherFakeMethod\n" +
            "Immediate output from second test\n" +
            "=> NUnitLite.Tests.TextUITests.MyFakeMethod\n" +
            "OUTPUT1\n" +
            "=> NUnitLite.Tests.TextUITests.AnotherFakeMethod\n" +
            "OUTPUT2\n")]
        [TestCase("All",
            "=> NUnitLite.Tests.TextUITests.MyFakeMethod\n" +
            "Immediate output from first test\n" +
            "=> NUnitLite.Tests.TextUITests.AnotherFakeMethod\n" +
            "=> NUnitLite.Tests.TextUITests.MyFakeMethod\n" +
            "Another immediate output from first test\n" +
            "=> NUnitLite.Tests.TextUITests.AnotherFakeMethod\n" +
            "Immediate output from second test\n" +
            "=> NUnitLite.Tests.TextUITests.MyFakeMethod\n" +
            "OUTPUT1\n" +
            "=> NUnitLite.Tests.TextUITests.AnotherFakeMethod\n" +
            "OUTPUT2\n")]
        [TestCase("Before",
            "=> NUnitLite.Tests.TextUITests.MyFakeMethod\n" +
            "Immediate output from first test\n" +
            "=> NUnitLite.Tests.TextUITests.AnotherFakeMethod\n" +
            "=> NUnitLite.Tests.TextUITests.MyFakeMethod\n" +
            "Another immediate output from first test\n" +
            "=> NUnitLite.Tests.TextUITests.AnotherFakeMethod\n" +
            "Immediate output from second test\n" +
            "=> NUnitLite.Tests.TextUITests.MyFakeMethod\n" +
            "OUTPUT1\n" +
            "=> NUnitLite.Tests.TextUITests.AnotherFakeMethod\n" +
            "OUTPUT2\n")]
        [TestCase("After",
            "=> NUnitLite.Tests.TextUITests.MyFakeMethod\n" +
            "Immediate output from first test\n" +
            "Another immediate output from first test\n" +
            "=> NUnitLite.Tests.TextUITests.AnotherFakeMethod\n" +
            "Immediate output from second test\n" +
            "=> NUnitLite.Tests.TextUITests.MyFakeMethod\n" +
            "OUTPUT1\n" +
            "Failed => NUnitLite.Tests.TextUITests.MyFakeMethod\n" +
            "=> NUnitLite.Tests.TextUITests.AnotherFakeMethod\n" +
            "OUTPUT2\n" +
            "Passed => NUnitLite.Tests.TextUITests.AnotherFakeMethod\n")]
        public void TwoTests_InterleavedExecution(string labels, string expected)
        {
            var test1 = Fakes.GetTestMethod(this, "MyFakeMethod");
            var result1 = new TestCaseResult(test1);
            result1.SetResult(ResultState.Failure);

            var test2 = Fakes.GetTestMethod(this, "AnotherFakeMethod");
            var result2 = new TestCaseResult(test2);
            result2.SetResult(ResultState.Success);

            var textUI = CreateTextUI(labels);

            textUI.TestStarted(test1);
            textUI.TestOutput(new TestOutput("Immediate output from first test", "", test1.FullName));
            textUI.TestStarted(test2);
            textUI.TestOutput(new TestOutput("Another immediate output from first test", "", test1.FullName));
            textUI.TestOutput(new TestOutput("Immediate output from second test", "", test2.FullName));
            result1.OutWriter.WriteLine("OUTPUT1");
            textUI.TestFinished(result1);
            result2.OutWriter.WriteLine("OUTPUT2");
            textUI.TestFinished(result2);

            Assert.That(Report, Is.EqualTo(expected));
        }

        [TestCase("Off",
            "Immediate output from first test\n" +
            "Another immediate output from first test\n" +
            "Immediate output from second test\n" +
            "OUTPUT2\n" +
            "OUTPUT1\n")]
        [TestCase("On",
            "=> NUnitLite.Tests.TextUITests.MyFakeMethod\n" +
            "Immediate output from first test\n" +
            "Another immediate output from first test\n" +
            "=> NUnitLite.Tests.TextUITests.AnotherFakeMethod\n" +
            "Immediate output from second test\n" +
            "OUTPUT2\n" +
            "=> NUnitLite.Tests.TextUITests.MyFakeMethod\n" +
            "OUTPUT1\n")]
        [TestCase("All",
            "=> NUnitLite.Tests.TextUITests.MyFakeMethod\n" +
            "Immediate output from first test\n" +
            "=> NUnitLite.Tests.TextUITests.AnotherFakeMethod\n" +
            "=> NUnitLite.Tests.TextUITests.MyFakeMethod\n" +
            "Another immediate output from first test\n" +
            "=> NUnitLite.Tests.TextUITests.AnotherFakeMethod\n" +
            "Immediate output from second test\n" +
            "OUTPUT2\n" +
            "=> NUnitLite.Tests.TextUITests.MyFakeMethod\n" +
            "OUTPUT1\n")]
        [TestCase("Before",
            "=> NUnitLite.Tests.TextUITests.MyFakeMethod\n" +
            "Immediate output from first test\n" +
            "=> NUnitLite.Tests.TextUITests.AnotherFakeMethod\n" +
            "=> NUnitLite.Tests.TextUITests.MyFakeMethod\n" +
            "Another immediate output from first test\n" +
            "=> NUnitLite.Tests.TextUITests.AnotherFakeMethod\n" +
            "Immediate output from second test\n" +
            "OUTPUT2\n" +
            "=> NUnitLite.Tests.TextUITests.MyFakeMethod\n" +
            "OUTPUT1\n")]
        [TestCase("After",
            "=> NUnitLite.Tests.TextUITests.MyFakeMethod\n" +
            "Immediate output from first test\n" +
            "Another immediate output from first test\n" +
            "=> NUnitLite.Tests.TextUITests.AnotherFakeMethod\n" +
            "Immediate output from second test\n" +
            "OUTPUT2\n" +
            "Passed => NUnitLite.Tests.TextUITests.AnotherFakeMethod\n" +
            "=> NUnitLite.Tests.TextUITests.MyFakeMethod\n" +
            "OUTPUT1\n" +
            "Failed => NUnitLite.Tests.TextUITests.MyFakeMethod\n")]
        public void TwoTests_NestedExecution(string labels, string expected)
        {
            var test1 = Fakes.GetTestMethod(this, "MyFakeMethod");
            var result1 = new TestCaseResult(test1);
            result1.SetResult(ResultState.Failure);

            var test2 = Fakes.GetTestMethod(this, "AnotherFakeMethod");
            var result2 = new TestCaseResult(test2);
            result2.SetResult(ResultState.Success);

            var textUI = CreateTextUI(labels);

            textUI.TestStarted(test1);
            textUI.TestOutput(new TestOutput("Immediate output from first test", "", test1.FullName));
            textUI.TestStarted(test2);
            textUI.TestOutput(new TestOutput("Another immediate output from first test", "", test1.FullName));
            textUI.TestOutput(new TestOutput("Immediate output from second test", "", test2.FullName));
            result2.OutWriter.WriteLine("OUTPUT2");
            textUI.TestFinished(result2);
            result1.OutWriter.WriteLine("OUTPUT1");
            textUI.TestFinished(result1);

            Assert.That(Report, Is.EqualTo(expected));
        }

        #region Private Properties and Methods

        private void MyFakeMethod() { }

        private void AnotherFakeMethod() { }

        private string Report
        {
            get { return _reportBuilder.ToString().Replace(NL, "\n"); }
        }

        private IList<string> GetReportLines()
        {
            var rdr = new StringReader(Report);

            string line;
            var lines = new List<string>();
            while ((line = rdr.ReadLine()) != null)
                lines.Add(line);

            return lines;
        }

        #endregion
    }

    public class TextUIReportTests
    {
        private static readonly string NL = Environment.NewLine;

        private TextUI _textUI;
        private StringBuilder _reportBuilder;
        private TestResult _result;

        private static readonly string[] REPORT_SEQUENCE = new string[] {
            "Test Run Summary",
            "Errors and Failures",
            "Tests Not Run"
        };

        [OneTimeSetUp]
        public void CreateResult()
        {
            _result = NUnit.TestUtilities.TestBuilder.RunTestFixture(typeof(MockTestFixture)) as TestResult;
            Assert.NotNull(_result, "Unable to run fixture");

            _result.StartTime = _result.EndTime = new DateTime(2014, 12, 2, 12, 34, 56, DateTimeKind.Utc);
            _result.Duration = 0.123;
        }

        [SetUp]
        public void CreateTextUI()
        {
            _reportBuilder = new StringBuilder();
            var writer = new ExtendedTextWrapper(new StringWriter(_reportBuilder));
            var options = new NUnitLiteOptions();
            _textUI = new TextUI(writer, null, options);
        }

        [Test]
        public void DisplayHelp()
        {
            _textUI.DisplayHelp();
            Assert.That(Report, Does.StartWith("Usage: NUNITLITE"));
            // TODO: Further assertions?
        }

        [Test]
        [Ignore("Test needs modification")]
        public void ReportSequenceTest()
        {
            var textRunner = new TextRunner();
            textRunner.ReportResults(_result);

            int last = -1;

            foreach (string title in REPORT_SEQUENCE)
            {
                var index = Report.IndexOf(title);
                Assert.That(index >= 0, "Report not found: " + title);
                Assert.That(index > last, "Report out of sequence: " + title);
            }
        }

        [Test]
        public void TestsNotRunTest()
        {
            _textUI.DisplayNotRunReport(_result);

            Assert.That(Report, Is.EqualTo(
                "Tests Not Run\n\n" +
                "1) Ignored : NUnit.Tests.Assemblies.MockTestFixture.IgnoreTest\n" +
                "Ignore Message\n\n" +
                "2) Explicit : NUnit.Tests.Assemblies.MockTestFixture.ExplicitTest\n\n"));
        }

        [Test]
        public void SummaryReportTest()
        {
            _textUI.DisplaySummaryReport(new ResultSummary(_result));

            Assert.That(Report, Is.EqualTo(
                "Test Run Summary\n" +
                "  Overall result: Failed\n" +
                "  Test Count: 10, Passed: 2, Failed: 4, Warnings: 1, Inconclusive: 1, Skipped: 2\n" +
                "    Failed Tests - Failures: 1, Errors: 1, Invalid: 2\n" +
                "    Skipped Tests - Ignored: 1, Explicit: 1, Other: 0\n" +
                "  Start time: 2014-12-02 12:34:56Z\n" +
                "    End time: 2014-12-02 12:34:56Z\n" +
                "    Duration: 0.123 seconds\n\n"));
        }

        [Test]
        public void ErrorsAndFailuresReportTest()
        {
            _textUI.DisplayErrorsFailuresAndWarningsReport(_result);
            var lines = GetReportLines();

            // NOTE: Although this could be done by a very long regular expression, 
            // any error messages would be hard to interpret.

            Assert.That(lines[0], Is.EqualTo("Errors, Failures and Warnings"));
            Assert.That(lines[2], Is.EqualTo("1) Invalid : NUnit.Tests.Assemblies.MockTestFixture.NonPublicTest"));
            Assert.That(lines[3], Is.EqualTo("Method is not public"));
            Assert.That(lines[5], Is.EqualTo("2) Failed : NUnit.Tests.Assemblies.MockTestFixture.FailingTest"));
            Assert.That(lines[6], Is.EqualTo("Intentional failure"));
            Assert.That(lines[9], Is.EqualTo("3) Warning : NUnit.Tests.Assemblies.MockTestFixture.WarningTest"));
            Assert.That(lines[10], Is.EqualTo("Warning Message"));
            Assert.That(lines[13], Is.EqualTo("4) Invalid : NUnit.Tests.Assemblies.MockTestFixture.NotRunnableTest"));
            Assert.That(lines[14], Is.EqualTo("No arguments were provided"));
            Assert.That(lines[16], Is.EqualTo("5) Error : NUnit.Tests.Assemblies.MockTestFixture.TestWithException"));
            Assert.That(lines[17], Is.EqualTo("System.Exception : Intentional Exception"));
        }

        #region Private Properties and Methods

        private string Report
        {
            get { return _reportBuilder.ToString().Replace(NL, "\n"); }
        }

        private IList<string> GetReportLines()
        {
            var rdr = new StringReader(Report);

            string line;
            var lines = new List<string>();
            while ((line = rdr.ReadLine()) != null)
                lines.Add(line);

            return lines;
        }

        #endregion
    }
}
#endif