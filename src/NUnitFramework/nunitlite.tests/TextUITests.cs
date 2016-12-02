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

namespace NUnitLite.Tests
{
    public class TextUITests
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
        public void DisplayWarning()
        {
            _textUI.DisplayWarning("This is a warning");
            Assert.That(Report, Is.EqualTo("This is a warning" + NL));
        }

        [Test]
        public void DisplayError()
        {
            _textUI.DisplayError("This is an error");
            Assert.That(Report, Is.EqualTo("This is an error" + NL));
        }

        [Test]
        public void DisplayErrors()
        {
            _textUI.DisplayErrors(new string[] { "This is an error", "Another error", "Final error" });
            Assert.That(Report, Is.EqualTo("This is an error" + NL + "Another error" + NL + "Final error" + NL));
        }

        [Test]
        public void DisplayHeader()
        {
            _textUI.DisplayHeader();

            var lines = GetReportLines();
            Assert.That(lines.Count, Is.EqualTo(3));
            Assert.That(lines[0], Does.StartWith("NUnitLite"));
            Assert.That(lines[1], Does.StartWith("Copyright"));
            Assert.That(lines[2], Is.EqualTo(""));
        }

        [Test]
        public void DisplayTestFiles()
        {
            _textUI.DisplayTestFiles(new string[] { "test1.dll", "another.test.dll", "final.test.dll" });
            Assert.That(GetReportLines(), Is.EqualTo(new string[]{
                "Test Files",
                "    test1.dll",
                "    another.test.dll",
                "    final.test.dll",
                ""
            }));
        }

        [Test]
        public void DisplayRuntimeEnvironment()
        {
            _textUI.DisplayRuntimeEnvironment();

            var lines = GetReportLines();
            Assert.That(lines.Count, Is.EqualTo(4));
            Assert.That(lines[1], Contains.Substring("OS Version"));
            Assert.That(lines[2], Contains.Substring("CLR Version"));
            Assert.That(lines[3], Is.Empty);
        }

        [Test]
        public void TestFinished_NoOutput()
        {
            var result = new TestCaseResult(Fakes.GetTestMethod(this, "MyFakeMethod"));
            _textUI.TestFinished(result);
            Assert.That(Report, Is.EqualTo(""));
        }

        [Test]
        public void TestFinished_WithOutput()
        {
            var result = new TestCaseResult(Fakes.GetTestMethod(this, "MyFakeMethod"));
            result.OutWriter.WriteLine("First line of output");
            result.OutWriter.WriteLine("Another line of output");

            _textUI.TestFinished(result);

            Assert.That(GetReportLines(), Is.EqualTo(new string[] {
                "=> NUnitLite.Tests.TextUITests.MyFakeMethod",
                "First line of output",
                "Another line of output"
            }));
        }

        private void MyFakeMethod() { }

        [Test]
        public void DisplayHelp()
        {
            _textUI.DisplayHelp();
            Assert.That(Report, Does.StartWith("Usage: NUNITLITE"));
            // TODO: Further assertions?
        }

        [Test][Ignore("Test needs modification")]
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
            var expected = new string[] {
                "Tests Not Run",
                "",
                //"1) Ignored : NUnit.Tests.Assemblies.MockTestFixture.IgnoreTest",
                //"Ignore Message",
                //"",
                "1) Explicit : NUnit.Tests.Assemblies.MockTestFixture.ExplicitTest",
                ""
            };

            _textUI.DisplayNotRunReport(_result);
            Assert.That(GetReportLines(), Is.EqualTo(expected));
        }

        [Test]
        public void SummaryReportTest()
        {
            var expected = new string[] {
                "Test Run Summary",
                "  Overall result: Failed",
                "  Test Count: 8, Passed: 1, Failed: 4, Warnings: 1, Inconclusive: 1, Skipped: 1",
                "    Failed Tests - Failures: 1, Errors: 1, Invalid: 2",
                "    Skipped Tests - Explicit: 1, Other: 0",
                "  Start time: 2014-12-02 12:34:56Z",
                "    End time: 2014-12-02 12:34:56Z",
                "    Duration: 0.123 seconds",
                ""
            };

            _textUI.DisplaySummaryReport(new ResultSummary(_result));
            Assert.That(GetReportLines(), Is.EqualTo(expected));
        }

        [Test]
        public void ErrorsAndFailuresReportTest()
        {
            _textUI.DisplayErrorsAndFailuresReport(_result);
            var lines = GetReportLines();

            Assert.That(lines[0], Is.EqualTo("Errors and Failures"));
            Assert.That(lines[2], Is.EqualTo("1) Invalid : NUnit.Tests.Assemblies.MockTestFixture.NonPublicTest"));
            Assert.That(lines[3], Is.EqualTo("Method is not public"));
            Assert.That(lines[5], Is.EqualTo("2) Failed : NUnit.Tests.Assemblies.MockTestFixture.FailingTest"));
            Assert.That(lines[6], Is.EqualTo("Intentional failure"));
            Assert.That(lines[9], Is.EqualTo("3) Invalid : NUnit.Tests.Assemblies.MockTestFixture.NotRunnableTest"));
            Assert.That(lines[10], Is.EqualTo("No arguments were provided"));
            Assert.That(lines[12], Is.EqualTo("4) Error : NUnit.Tests.Assemblies.MockTestFixture.TestWithException"));
            Assert.That(lines[13], Is.EqualTo("System.Exception : Intentional Exception"));
        }

#region Private Properties and Methods

        private string Report
        {
            get { return _reportBuilder.ToString(); }
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