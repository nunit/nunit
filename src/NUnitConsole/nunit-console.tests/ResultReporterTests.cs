using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using NUnit.Common;
using NUnit.Framework;

namespace NUnit.ConsoleRunner.Tests
{
    using Utilities;

    public class ResultReporterTests
    {
        private const string MOCK_TEST_RESULT = "NUnit.ConsoleRunner.Tests.MockTestResult.xml";
        private static readonly string[] REPORT_SEQUENCE = new string[] {
            "Test Run Summary",
            "Errors and Failures",
            "Tests Not Run"
        };

        private XmlNode _result;
        private ResultReporter _reporter;
        private StringBuilder _report;

        [OneTimeSetUp]
        public void CreateResult()
        {
            string xmlText;

            using (StreamReader rdr = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(MOCK_TEST_RESULT)))
            {
                xmlText = rdr.ReadToEnd();
            }

            Assert.That(xmlText.Length > 0, "Unable to read MockTestResult.xml resource");

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlText);
            _result = doc.SelectSingleNode("test-run");
            Assert.NotNull(_result, "Unable to create test result XmlNode");
        }

        [SetUp]
        public void CreateReporter()
        {
            _report = new StringBuilder();

            var writer = new ExtendedTextWrapper(new StringWriter(_report));
            var options = new ConsoleOptions();
            options.Parse(new string[] { "MockTestResult.xml" });

            _reporter = new ResultReporter(_result, writer, options);
        }

        [Test]
        public void ReportSequenceTest()
        {
            var report = GetReport(_reporter.ReportResults);
            //Console.WriteLine(report);

            int last = -1;

            foreach (string title in REPORT_SEQUENCE)
            {
                var index = report.IndexOf(title);
                Assert.That(index > 0, "Report not found: " + title);
                Assert.That(index > last, "Report out of sequence: " + title);
            }
        }

        [Test]
        public void SummaryReportTest()
        {
            var expected = new string[] {
                "Test Run Summary",
                "    Overall result: Failed",
                "   Tests run: 24, Passed: 18, Errors: 1, Failures: 1, Inconclusive: 1",
                "     Not run: 7, Invalid: 3, Ignored: 4, Skipped: 0",
                "  Start time: 2014-12-02 03:07:10Z",
                "    End time: 2014-12-02 03:07:10Z",
                "    Duration: 0.193 seconds",
                ""
            };

            var report = GetReportLines(_reporter.WriteSummaryReport);
            Assert.That(report, Is.EqualTo(expected));
        }

        [Test]
        public void ErrorsAndFailuresReportTest()
        {
            var expected = new string[] {
                "Errors and Failures",
                "",
                "1) Failed : NUnit.Tests.Assemblies.MockTestFixture.FailingTest",
                "Intentional failure",
                "at NUnit.Tests.Assemblies.MockTestFixture.FailingTest() in d:\\Dev\\NUnit\\nunit-3.0\\src\\NUnitFramework\\mock-assembly\\MockAssembly.cs:line 134",
                "",
                "2) Invalid : NUnit.Tests.Assemblies.MockTestFixture.MockTest5",
                "Method is not public",
                "",
                "3) Invalid : NUnit.Tests.Assemblies.MockTestFixture.NotRunnableTest",
                "No arguments were provided",
                "",
                "4) Error : NUnit.Tests.Assemblies.MockTestFixture.TestWithException",
                "System.Exception : Intentional Exception",
                "   at NUnit.Tests.Assemblies.MockTestFixture.MethodThrowsException() in d:\\Dev\\NUnit\\nunit-3.0\\src\\NUnitFramework\\mock-assembly\\MockAssembly.cs:line 171",
                "   at NUnit.Tests.Assemblies.MockTestFixture.TestWithException() in d:\\Dev\\NUnit\\nunit-3.0\\src\\NUnitFramework\\mock-assembly\\MockAssembly.cs:line 166",
                "",
                "5) Invalid : NUnit.Tests.BadFixture",
                "No suitable constructor was found",
                ""
            };

            var report = GetReportLines(_reporter.WriteErrorsAndFailuresReport);
            Assert.That(report, Is.EqualTo(expected));
        }

        [Test]
        public void TestsNotRunTest()
        {
            var expected = new string[] {
                "Tests Not Run",
                "",
                "1) Ignored : NUnit.Tests.Assemblies.MockTestFixture.MockTest4",
                "ignoring this test method for now",
                "",
                "2) Ignored : NUnit.Tests.IgnoredFixture.Test1",
                "OneTimeSetUp: BECAUSE",
                "",
                "3) Ignored : NUnit.Tests.IgnoredFixture.Test2",
                "OneTimeSetUp: BECAUSE",
                "",
                "4) Ignored : NUnit.Tests.IgnoredFixture.Test3",
                "OneTimeSetUp: BECAUSE",
                ""
            };

            var report = GetReportLines(_reporter.WriteNotRunReport);
            Assert.That(report, Is.EqualTo(expected));
        }

        #region Helper Methods

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