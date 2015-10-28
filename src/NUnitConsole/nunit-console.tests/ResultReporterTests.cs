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

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using NUnit.Common;
using NUnit.Engine;
using NUnit.Framework;
using NUnit.Framework.Api;
using NUnit.Framework.Internal;
using NUnit.Tests.Assemblies;
using NUnit.Engine.Internal;

namespace NUnit.ConsoleRunner.Tests
{

    public class ResultReporterTests
    {
        private XmlNode _result;
        private ResultReporter _reporter;
        private StringBuilder _report;

        [OneTimeSetUp]
        public void CreateResult()
        {
            var mockAssembly = typeof (MockAssembly).Assembly;
            var emptySettings = new Dictionary<string, object>();

            var runner = new NUnitTestAssemblyRunner(new DefaultTestAssemblyBuilder());
            runner.Load(mockAssembly, emptySettings);
            var xmlText = runner.Run(TestListener.NULL, Framework.Internal.TestFilter.Empty).ToXml(true).OuterXml;
            var engineResult = AddMetadata(new TestEngineResult(xmlText));
            _result = engineResult.Xml;

            Assert.NotNull(_result, "Unable to create report result.");
        }

        [SetUp]
        public void CreateReporter()
        {
            _report = new StringBuilder();
            var writer = new ExtendedTextWrapper(new StringWriter(_report));
            _reporter = new ResultReporter(_result, writer, new ConsoleOptions());
        }

        [Test]
        public void ReportSequenceTest()
        {
            var report = GetReport(_reporter.ReportResults);

            var reportSequence = new[]
            {
                "Tests Not Run",
                "Errors and Failures",
                "Run Settings",
                "Test Run Summary"
            };

            int last = -1;

            foreach (string title in reportSequence)
            {
                var index = report.IndexOf(title);
                Assert.That(index > 0, "Report not found: " + title);
                Assert.That(index > last, "Report out of sequence: " + title);
                last = index;
            }
        }

        [Test]
        public void CheckReportSettingsTest()
        {
            var expected = new [] {
                "Run Settings",
                "Setting1Name: Setting1Value",
                "Setting2Name: Setting2Value"
            };

            var actualSummary = GetReportLines(_reporter.WriteRunSettingsReport);
            this.VerifyOrderAndContent(expected, actualSummary);
        }

        [Test]
        public void SummaryReportTest()
        {
            var expected = new [] {
                "Test Run Summary",
                "Overall result: Failed",
                "Tests run: 35, Passed: 30, Errors: 1, Failures: 3, Inconclusive: 1",
                "Not run: 10, Invalid: 3, Ignored: 4, Explicit: 3, Skipped: 0",
                "Start time: 2015-10-19 02:12:28Z",
                "End time: 2015-10-19 02:12:29Z",
                "Duration: 0.349 seconds"
            };

            var actualSummary = GetReportLines(_reporter.WriteSummaryReport);
            this.VerifyOrderAndContent(expected, actualSummary);
        }

        [Test]
        public void ErrorsAndFailuresReportTest()
        {
            var expected = new List<string> {
                "Errors and Failures",
                "",
                "1) Failed : NUnit.Tests.Assemblies.MockTestFixture.FailingTest",
                "Intentional failure",
                "at NUnit.Tests.Assemblies.MockTestFixture.FailingTest()",
                "",
                "2) Invalid : NUnit.Tests.Assemblies.MockTestFixture.MockTest5",
                "Method is not public",
                "",
                "3) Invalid : NUnit.Tests.Assemblies.MockTestFixture.NotRunnableTest",
                "No arguments were provided",
                "",
                "4) Error : NUnit.Tests.Assemblies.MockTestFixture.TestWithException",
                "System.Exception : Intentional Exception",
                "at NUnit.Tests.Assemblies.MockTestFixture.MethodThrowsException()",
                "at NUnit.Tests.Assemblies.MockTestFixture.TestWithException()",
                "",
                "5) Invalid : NUnit.Tests.BadFixture",
                "No suitable constructor was found",
                "",
                "6) Failed : NUnit.Tests.CDataTestFixure.DemonstrateIllegalSequenceAtEndOfFailureMessage",
                "The CDATA was: <![CDATA[ My <xml> ]]>",
                "at NUnit.Tests.CDataTestFixure.DemonstrateIllegalSequenceAtEndOfFailureMessage()",
                "",
                "7) Failed : NUnit.Tests.CDataTestFixure.DemonstrateIllegalSequenceInFailureMessage",
                "Deliberate failure to illustrate ]]> in message ",
                "at NUnit.Tests.CDataTestFixure.DemonstrateIllegalSequenceInFailureMessage()",
                ""
            };

            var report = GetReportLines(_reporter.WriteErrorsAndFailuresReport);
            this.VerifyOrderAndContent(expected, report);
        }

        [Test]
        public void TestsNotRunTest()
        {
            var expected = new [] {
                "Tests Not Run",
                "",
                "1) Explicit : NUnit.Tests.Assemblies.MockTestFixture.ExplicitlyRunTest",
                "",
                "",
                "2) Ignored : NUnit.Tests.Assemblies.MockTestFixture.MockTest4",
                "ignoring this test method for now",
                "",
                "3) Explicit : NUnit.Tests.ExplicitFixture.Test1",
                "OneTimeSetUp: ",
                "",
                "4) Explicit : NUnit.Tests.ExplicitFixture.Test2",
                "OneTimeSetUp: ",
                "",
                "5) Ignored : NUnit.Tests.IgnoredFixture.Test1",
                "OneTimeSetUp: BECAUSE",
                "",
                "6) Ignored : NUnit.Tests.IgnoredFixture.Test2",
                "OneTimeSetUp: BECAUSE",
                "",
                "7) Ignored : NUnit.Tests.IgnoredFixture.Test3",
                "OneTimeSetUp: BECAUSE",
                ""
            };

            var report = GetReportLines(_reporter.WriteNotRunReport);
            this.VerifyOrderAndContent(expected, report);
        }

        private void VerifyOrderAndContent(IList<string> expected, IList<string> actual)
        {
            for (int i = 0; i < expected.Count; i++)
            {
                var expectedLine = expected[i];
                var actualLine = actual[i];

                StringAssert.Contains(expectedLine.Trim(), actualLine.Trim());
            }
        }

        #region Helper Methods

        private TestEngineResult AddMetadata(TestEngineResult input)
        {
            input.Add("<settings><setting name=\"Setting1Name\" value=\"Setting1Value\"></setting><setting name=\"Setting2Name\" value=\"Setting2Value\"></setting></settings>");
            return input.Aggregate("test-run start-time=\"2015-10-19 02:12:28Z\" end-time=\"2015-10-19 02:12:29Z\" duration=\"0.348616\"", string.Empty, string.Empty);
        }

        private string GetReport(TestDelegate del)
        {
            del();
            return _report.ToString();
        }

        private IList<string> GetReportLines(TestDelegate del)
        {
            var rdr = new StringReader(GetReport(del));

            string line;
            var lines = new List<string>();
            while ((line = rdr.ReadLine()) != null)
                lines.Add(line);

            return lines;
        }

        #endregion
    }
}