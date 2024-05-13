// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.TestFixtureTests;

namespace NUnit.Framework.Tests.Internal
{
    public class TestProgressReporterTests
    {
        private ReportCollector? _listener;
        private TestProgressReporter? _reporter;

        [OneTimeSetUp]
        public void SetupFixture()
        {
            _listener = new ReportCollector();
            _reporter = new TestProgressReporter(_listener);
        }

        [SetUp]
        public void Setup()
        {
            _listener.Reports.Clear();
        }

        [Test]
        public void TestStarted_AssemblyEmitsSingleStartSuiteElement()
        {
            var work = TestBuilder.CreateWorkItem(new TestAssembly("mytest.dll"));
            work.Context.Listener = _reporter;

            TestBuilder.ExecuteWorkItem(work);

            var startReport = _listener.Reports.FirstOrDefault();
            Assert.That(startReport, Is.Not.Null);
            Assert.That(startReport, Does.StartWith("<start-suite"));
            Assert.That(startReport, Contains.Substring("type=\"Assembly\""));

            Assert.That(_listener.Reports.Count(x => x.StartsWith("<start-suite") && x.Contains("type=\"Assembly\"")), Is.EqualTo(1), "More than one Assembly event");
        }

        [Test]
        public void TestStarted_AssemblyIncludesFrameworkVersion()
        {
            var work = TestBuilder.CreateWorkItem(new TestAssembly("mytest.dll"));
            work.Context.Listener = _reporter;

            TestBuilder.ExecuteWorkItem(work);

            var startReport = _listener.Reports.FirstOrDefault();
            Assert.That(startReport, Is.Not.Null);
            Assert.That(startReport, Does.StartWith("<start-suite"));
            Assert.That(startReport, Contains.Substring($"framework-version=\"{typeof(TestProgressReporter).Assembly.GetName().Version}\""));
        }

        [Test]
        public void TestFinished_AssemblyEmitsSingleTestSuiteElement()
        {
            var work = TestBuilder.CreateWorkItem(new TestAssembly("mytest.dll"));
            work.Context.Listener = _reporter;

            TestBuilder.ExecuteWorkItem(work);

            var endReport = _listener.Reports.LastOrDefault();
            Assert.That(endReport, Is.Not.Null);
            Assert.That(endReport, Does.StartWith("<test-suite"));
            Assert.That(endReport, Contains.Substring("type=\"Assembly\""));

            Assert.That(_listener.Reports.Count(x => x.StartsWith("<test-suite") && x.Contains("type=\"Assembly\"")), Is.EqualTo(1), "More than one Assembly event");
        }

        [Test]
        public void TestStarted_FixtureEmitsStartSuiteElement()
        {
            var work = TestBuilder.CreateWorkItem(typeof(FixtureWithTestFixtureAttribute));
            work.Context.Listener = _reporter;

            TestBuilder.ExecuteWorkItem(work);

            var startReport = _listener.Reports.FirstOrDefault();
            Assert.That(startReport, Is.Not.Null);
            Assert.That(startReport, Does.StartWith("<start-suite"));
            Assert.That(startReport, Contains.Substring("type=\"TestFixture\""));
        }

        [Test]
        public void TestStarted_TestMethodEmitsStartTestElement()
        {
            var work = TestBuilder.CreateWorkItem(typeof(FixtureWithTestFixtureAttribute), "SomeTest");
            work.Context.Listener = _reporter;

            TestBuilder.ExecuteWorkItem(work);

            var startReport = _listener.Reports.FirstOrDefault();
            Assert.That(startReport, Is.Not.Null);
            Assert.That(startReport, Does.StartWith("<start-test"));
        }

        [Test]
        public void TestStarted_ReportAttributes()
        {
            var work = TestBuilder.CreateWorkItem(typeof(FixtureWithTestFixtureAttribute), "SomeTest");
            work.Context.Listener = _reporter;

            TestBuilder.ExecuteWorkItem(work);

            var startReport = _listener.Reports.FirstOrDefault();
            Assert.That(startReport, Is.Not.Null);
            Assert.That(startReport, Does.StartWith("<start-test"));
            Assert.That(startReport, Contains.Substring($"id=\"{work.Test.Id}\""));
            Assert.That(startReport, Contains.Substring($"parentId=\"{work.Test.Parent?.Id}\""));
            Assert.That(startReport, Contains.Substring($"name=\"{work.Test.Name}\""));
            Assert.That(startReport, Contains.Substring($"fullname=\"{work.Test.FullName}\""));
            Assert.That(startReport, Contains.Substring($"type=\"{work.Test.TestType}\""));
        }

        [Test]
        public void TestFinished_AdditionalReportAttributes()
        {
            var work = TestBuilder.CreateWorkItem(typeof(FixtureWithTestFixtureAttribute), "SomeTest");
            work.Context.Listener = _reporter;

            TestBuilder.ExecuteWorkItem(work);

            var endReport = _listener.Reports.LastOrDefault();
            Assert.That(endReport, Is.Not.Null);
            Assert.That(endReport, Does.Not.StartWith("<start"));
            Assert.That(endReport, Contains.Substring($"parentId=\"{work.Test.Parent?.Id}\""));
        }

        #region Nested ReportCollector Class

        private class ReportCollector : System.Web.UI.ICallbackEventHandler
        {
            public List<string> Reports { get; } = new List<string>();

            public void RaiseCallbackEvent(string report)
            {
                Reports.Add(report);
            }

            public string GetCallbackResult()
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
