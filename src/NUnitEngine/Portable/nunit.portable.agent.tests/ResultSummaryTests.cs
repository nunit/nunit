// ***********************************************************************
// Copyright (c) 2016 Charlie Poole
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

using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using NUnit.Framework;

namespace NUnit.Engine.Tests
{
    [TestFixture]
    public class ResultSummaryTests
    {
        XDocument _testResults;
        XElement _testRun;
        IEnumerable<XElement> _testSuites;
        ResultSummary _summary;
        XDocument _summaryResults;
        XElement _summaryTestRun;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            string testResultsFile = Path.Combine(TestContext.CurrentContext.TestDirectory, "MockTestResult.xml");
            using (var reader = new StreamReader(testResultsFile))
                _testResults = XDocument.Load(reader);

            _testRun = _testResults.Element("test-run");
            _testSuites = _testRun.Elements("test-suite");

            _summary = new ResultSummary();
            foreach (var element in _testSuites)
                _summary.AddResult(element);

            _summaryResults = _summary.GetTestResults();
            _summaryTestRun = _summaryResults.Element("test-run");
        }

        [Test]
        public void ResultCountsMatchOriginalResults()
        {
            var expectedTestcasecount = _testRun.Attribute("testcasecount").ToInt();
            var expectedPassed = _testRun.Attribute("passed").ToInt();
            var expectedFailed = _testRun.Attribute("failed").ToInt();
            var expectedInconclusive = _testRun.Attribute("inconclusive").ToInt();
            var expectedSkipped = _testRun.Attribute("skipped").ToInt();
            var expectedAsserts = _testRun.Attribute("asserts").ToInt();

            Assert.That(_summary.TestCount, Is.EqualTo(expectedTestcasecount));
            Assert.That(_summary.PassCount, Is.EqualTo(expectedPassed));
            Assert.That(_summary.FailedCount, Is.EqualTo(expectedFailed));
            Assert.That(_summary.InconclusiveCount, Is.EqualTo(expectedInconclusive));
            Assert.That(_summary.TotalSkipCount, Is.EqualTo(expectedSkipped));
            Assert.That(_summary.AssertCount, Is.EqualTo(expectedAsserts));
        }

        [Test]
        public void ResultTestRunMatchOriginalTestRun()
        {
            var expectedTestcasecount = _testRun.Attribute("testcasecount").Value;
            var expectedResult = _testRun.Attribute("result").Value;
            var expectedTotal = _testRun.Attribute("total").Value;
            var expectedPassed = _testRun.Attribute("passed").Value;
            var expectedFailed = _testRun.Attribute("failed").Value;
            var expectedInconclusive = _testRun.Attribute("inconclusive").Value;
            var expectedSkipped = _testRun.Attribute("skipped").Value;
            var expectedAsserts = _testRun.Attribute("asserts").Value;

            var actualTestcasecount = _summaryTestRun.Attribute("testcasecount").Value;
            var actualResult = _summaryTestRun.Attribute("result").Value;
            var actualTotal = _summaryTestRun.Attribute("total").Value;
            var actualPassed = _summaryTestRun.Attribute("passed").Value;
            var actualFailed = _summaryTestRun.Attribute("failed").Value;
            var actualInconclusive = _summaryTestRun.Attribute("inconclusive").Value;
            var actualSkipped = _summaryTestRun.Attribute("skipped").Value;
            var actualAsserts = _summaryTestRun.Attribute("asserts").Value;

            Assert.That(actualTestcasecount, Is.EqualTo(expectedTestcasecount));
            Assert.That(actualResult, Is.EqualTo(expectedResult));
            Assert.That(actualTotal, Is.EqualTo(expectedTotal));
            Assert.That(actualPassed, Is.EqualTo(expectedPassed));
            Assert.That(actualFailed, Is.EqualTo(expectedFailed));
            Assert.That(actualInconclusive, Is.EqualTo(expectedInconclusive));
            Assert.That(actualSkipped, Is.EqualTo(expectedSkipped));
            Assert.That(actualAsserts, Is.EqualTo(expectedAsserts));
        }
    }
}
