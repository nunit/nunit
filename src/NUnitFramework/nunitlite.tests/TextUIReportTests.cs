// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Common;
using NUnit.Framework;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.Tests.Assemblies;

namespace NUnitLite.Tests
{
    public class TextUIReportTests
    {
        private static readonly string NL = Environment.NewLine;

        private TextUI _textUI;
        private StringBuilder _reportBuilder;
        private TestResult _result;

        private static readonly string[] REPORT_SEQUENCE = new string[]
        {
            "Test Run Summary",
            "Errors and Failures",
            "Tests Not Run"
        };

        [OneTimeSetUp]
        public void CreateResult()
        {
            _result = TestBuilder.RunTestFixture(typeof(MockTestFixture)) as TestResult;
            Assert.That(_result, Is.Not.Null, "Unable to run fixture");

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
                "2) Ignored : NUnit.Tests.Assemblies.MockTestFixture.SkippedTest(1)\n" +
                "Ignore testcase\n\n" +
                "3) Ignored : NUnit.Tests.Assemblies.MockTestFixture.SkippedTest(2)\n" +
                "Ignore testcase\n\n" +
                "4) Ignored : NUnit.Tests.Assemblies.MockTestFixture.SkippedTest(3)\n" +
                "Ignore testcase\n\n" +
                "5) Explicit : NUnit.Tests.Assemblies.MockTestFixture.ExplicitTest\n\n"));
        }

        [Test]
        public void SummaryReportTest()
        {
            _textUI.DisplaySummaryReport(new ResultSummary(_result));

            Assert.That(Report, Is.EqualTo(
                "Test Run Summary\n" +
                "  Overall result: Failed\n" +
                "  Test Count: 13, Passed: 2, Failed: 4, Warnings: 1, Inconclusive: 1, Skipped: 5\n" +
                "    Failed Tests - Failures: 1, Errors: 1, Invalid: 2\n" +
                "    Skipped Tests - Ignored: 4, Explicit: 1, Other: 0\n" +
                "  Start time: 2014-12-02 12:34:56Z\n" +
                "    End time: 2014-12-02 12:34:56Z\n" +
                "    Duration: 0.123 seconds\n\n"));
        }

        // Requires mock-assembly to be compiled with /optimize-
        // or else the test methods do tail calls and the stack frame disappears
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

        private string Report => _reportBuilder.ToString().Replace(NL, "\n");

        private IList<string> GetReportLines()
        {
            var rdr = new StringReader(Report);

            string line;
            var lines = new List<string>();
            while ((line = rdr.ReadLine()) is not null)
            {
                if (!line.Contains("InvokeStub_"))
                {
                    lines.Add(line);
                }
            }

            return lines;
        }
        #endregion
    }
}
