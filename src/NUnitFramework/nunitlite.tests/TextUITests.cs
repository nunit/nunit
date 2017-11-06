// ***********************************************************************
// Copyright (c) 2015-2017 Charlie Poole, Rob Prouse
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
using System.IO;
using System.Text;
using NUnit.Common;
using NUnit.Framework;
using NUnit.Framework.Internal;
using NUnit.TestUtilities;
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

        [TestCase("Off", "ABC+XYZ\n")]
        [TestCase("On", "=> NUnitLite.Tests.TextUITests.MyFakeMethod\nABC+XYZ\n")]
        [TestCase("All", "=> NUnitLite.Tests.TextUITests.MyFakeMethod\nABC+XYZ\n")]
        [TestCase("Before", "=> NUnitLite.Tests.TextUITests.MyFakeMethod\nABC+XYZ\n")]
        [TestCase("After", "=> NUnitLite.Tests.TextUITests.MyFakeMethod\nABC+XYZ\nPassed => NUnitLite.Tests.TextUITests.MyFakeMethod\n")]
        public void TestWritesSingleCharacters(string labels, string expected)
        {
            var result = new TestCaseResult(Fakes.GetTestMethod(this, "MyFakeMethod"));
            result.SetResult(ResultState.Success);

            result.OutWriter.Write(new [] { 'A', 'B', 'C' });
            result.OutWriter.Write('+');
            result.OutWriter.WriteLine("XYZ");
            
            CreateTextUI(labels).TestFinished(result);

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

#pragma warning disable 414
        static TestCaseData[] ImmediateOutputData = new[] {
            new TestCaseData("Off",
                new [] { new TestOutput("OUTPUT\n", "", null, "SomeMethod") },
                "OUTPUT\n"),
            new TestCaseData("On",
                new [] { new TestOutput("OUTPUT\n", "", null, "SomeMethod") },
                "=> SomeMethod\nOUTPUT\n"),
            new TestCaseData("All",
                new [] { new TestOutput("OUTPUT\n", "", null, "SomeMethod") },
                "=> SomeMethod\nOUTPUT\n"),
            new TestCaseData("Before",
                new [] { new TestOutput("OUTPUT\n", "", null, "SomeMethod") },
                "=> SomeMethod\nOUTPUT\n"),
            new TestCaseData("After",
                new [] { new TestOutput("OUTPUT\n", "", null, "SomeMethod") },
                "=> SomeMethod\nOUTPUT\n"),
            new TestCaseData("Off",
                new [] { new TestOutput("OUTPUT", "", null, "SomeMethod") },
                "OUTPUT"),
            new TestCaseData("On",
                new [] { new TestOutput("OUTPUT", "", null, "SomeMethod") },
                "=> SomeMethod\nOUTPUT"),
            new TestCaseData("All",
                new [] { new TestOutput("OUTPUT", "", null, "SomeMethod") },
                "=> SomeMethod\nOUTPUT"),
            new TestCaseData("Before",
                new [] { new TestOutput("OUTPUT", "", null, "SomeMethod") },
                "=> SomeMethod\nOUTPUT"),
            new TestCaseData("After",
                new [] { new TestOutput("OUTPUT", "", null, "SomeMethod") },
                "=> SomeMethod\nOUTPUT"),
            new TestCaseData("On",
                new [] { new TestOutput("A", "", null, "T1"), new TestOutput("B", "", null, "T1"), new TestOutput("C", "", null, "T1") },
                "=> T1\nABC"),
            new TestCaseData("On",
                new [] { new TestOutput("A", "", null, "T1"), new TestOutput("B", "", null, "T1"), new TestOutput("C", "", null, "T1"), new TestOutput("\n", "", null, "T1") },
                "=> T1\nABC\n"),
            new TestCaseData("On",
                new [] { new TestOutput("Hello\n", "", null, "EN"), new TestOutput("Ciao!\n", "", null, "IT"), new TestOutput("World!\n", "", null, "EN") },
                "=> EN\nHello\n=> IT\nCiao!\n=> EN\nWorld!\n"),
            new TestCaseData("On",
                new [] { new TestOutput("Hello", "", null, "EN"), new TestOutput("Ciao!", "", null, "IT"), new TestOutput("World!", "", null, "EN") },
                "=> EN\nHello\n=> IT\nCiao!\n=> EN\nWorld!"),
        };
#pragma warning restore 414

        [TestCaseSource(nameof(ImmediateOutputData))]
        public void ImmediateOutput(string labels, TestOutput[] outputs, string expected)
        {
            var textUI = CreateTextUI(labels);
            foreach (var output in outputs)
                textUI.TestOutput(output);
            
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
            textUI.TestOutput(new TestOutput("IMMEDIATE OUTPUT\n", "", null, test.FullName));
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
            textUI.TestOutput(new TestOutput("Immediate output from first test\n", "", null, test1.FullName));
            textUI.TestStarted(test2);
            textUI.TestOutput(new TestOutput("Another immediate output from first test\n", "", null, test1.FullName));
            textUI.TestOutput(new TestOutput("Immediate output from second test\n", "", null, test2.FullName));
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
            textUI.TestOutput(new TestOutput("Immediate output from first test\n", "", null, test1.FullName));
            textUI.TestStarted(test2);
            textUI.TestOutput(new TestOutput("Another immediate output from first test\n", "", null, test1.FullName));
            textUI.TestOutput(new TestOutput("Immediate output from second test\n", "", null, test2.FullName));
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
}
