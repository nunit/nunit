// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
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

#if !PORTABLE && !SILVERLIGHT // Mock assembly is needed
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using NUnit.Common.ColorConsole;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Tests.Assemblies;

namespace NUnitLite.Runner.Tests
{
    [Explicit("Not reliable in the CI build because of differences in  the stack traces")]
    public class ResultReporterTests
    {
        private static readonly string[] REPORT_SEQUENCE = new string[] {
            "Test Run Summary",
            "Errors and Failures",
            "Tests Not Run"
        };

        private TestResult _result;
        private ResultReporter _reporter;
        private StringBuilder _report;

        [OneTimeSetUp]
        public void CreateResult()
        {
            _result = NUnit.TestUtilities.TestBuilder.RunTestFixture(typeof(MockTestFixture)) as TestResult;
            Assert.NotNull(_result, "Unable to run fixture");

            _result.StartTime = _result.EndTime = new DateTime(2014, 12, 2, 12, 34, 56, DateTimeKind.Utc);
            _result.Duration = TimeSpan.FromSeconds(0.123);
        }

        [SetUp]
        public void CreateReporter()
        {
            _report = new StringBuilder();
            var writer = new ExtendedTextWriter(new StringWriter(_report));

            _reporter = new ResultReporter(_result, writer);
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
                "   Overall result: Failed",
                "   Tests run: 9, Passed: 4, Errors: 1, Failures: 1, Inconclusive: 1",
                "     Not run: 3, Invalid: 2, Ignored: 1, Skipped: 0",
                "  Start time: 2014-12-02 12:34:56Z",
                "    End time: 2014-12-02 12:34:56Z",
                "    Duration: 0.123 seconds",
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
                "1) Invalid : NUnit.Tests.Assemblies.MockTestFixture.MockTest5",
                "Method is not public",
                "",
                "2) Failed : NUnit.Tests.Assemblies.MockTestFixture.FailingTest",
                "Intentional failure",
#if DEBUG
                "at NUnit.Tests.Assemblies.MockTestFixture.FailingTest() in d:\\Dev\\NUnit\\nunit-3.0\\src\\NUnitFramework\\mock-assembly\\MockAssembly.cs:line 134",
#else
                "at NUnit.Tests.Assemblies.MockTestFixture.FailingTest()",
#endif
                "",
                "3) Invalid : NUnit.Tests.Assemblies.MockTestFixture.NotRunnableTest",
                "No arguments were provided",
                "",
                "4) Error : NUnit.Tests.Assemblies.MockTestFixture.TestWithException",
                "System.Exception : Intentional Exception",
#if DEBUG
                "   at NUnit.Tests.Assemblies.MockTestFixture.MethodThrowsException() in d:\\Dev\\NUnit\\nunit-3.0\\src\\NUnitFramework\\mock-assembly\\MockAssembly.cs:line 171",
                "   at NUnit.Tests.Assemblies.MockTestFixture.TestWithException() in d:\\Dev\\NUnit\\nunit-3.0\\src\\NUnitFramework\\mock-assembly\\MockAssembly.cs:line 166",
#else
                "   at NUnit.Tests.Assemblies.MockTestFixture.MethodThrowsException()",
                "   at NUnit.Tests.Assemblies.MockTestFixture.TestWithException()",
#endif
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
#endif
