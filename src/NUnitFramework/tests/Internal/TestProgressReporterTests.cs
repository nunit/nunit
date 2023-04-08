// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.TestUtilities;
using NUnit.TestData.TestFixtureTests;

namespace NUnit.Framework.Internal
{
	public class TestProgressReporterTests
	{
		private ReportCollector _listener;
		private TestProgressReporter _reporter;

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
            Assert.NotNull(startReport);
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
            Assert.NotNull(startReport);
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
            Assert.NotNull(endReport);
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
			Assert.NotNull(startReport);
			StringAssert.StartsWith("<start-suite", startReport);
            Assert.That(startReport, Contains.Substring("type=\"TestFixture\""));
		}

		[Test]
		public void TestStarted_TestMethodEmitsStartTestElement()
		{
			var work = TestBuilder.CreateWorkItem(typeof(FixtureWithTestFixtureAttribute), "SomeTest");
			work.Context.Listener = _reporter;

			TestBuilder.ExecuteWorkItem(work);

			var startReport = _listener.Reports.FirstOrDefault();
			Assert.NotNull(startReport);
			StringAssert.StartsWith("<start-test", startReport);
		}

		[Test]
		public void TestStarted_ReportAttributes()
		{
			var work = TestBuilder.CreateWorkItem(typeof(FixtureWithTestFixtureAttribute), "SomeTest");
			work.Context.Listener = _reporter;

			TestBuilder.ExecuteWorkItem(work);

			var startReport = _listener.Reports.FirstOrDefault();
			Assert.NotNull(startReport);
			StringAssert.StartsWith("<start-test", startReport);
			StringAssert.Contains($"id=\"{work.Test.Id}\"", startReport);
			StringAssert.Contains($"parentId=\"{work.Test.Parent?.Id}\"", startReport);
			StringAssert.Contains($"name=\"{work.Test.Name}\"", startReport);
			StringAssert.Contains($"fullname=\"{work.Test.FullName}\"", startReport);
			StringAssert.Contains($"type=\"{work.Test.TestType}\"", startReport);
		}

		[Test]
		public void TestFinished_AdditionalReportAttributes()
		{
			var work = TestBuilder.CreateWorkItem(typeof(FixtureWithTestFixtureAttribute), "SomeTest");
			work.Context.Listener = _reporter;

			TestBuilder.ExecuteWorkItem(work);

			var endReport = _listener.Reports.LastOrDefault();
			Assert.NotNull(endReport);
			StringAssert.DoesNotStartWith("<start", endReport);
			StringAssert.Contains($"parentId=\"{work.Test.Parent?.Id}\"", endReport);
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
