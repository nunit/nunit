// ***********************************************************************
// Copyright (c) 2017 Charlie Poole, Rob Prouse
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
		public void TestStarted_FixtureEmitsStartSuiteElement()
		{
			var work = TestBuilder.CreateWorkItem(typeof(FixtureWithTestFixtureAttribute));
			work.Context.Listener = _reporter;

			TestBuilder.ExecuteWorkItem(work);

			var startReport = _listener.Reports.FirstOrDefault();
			Assert.NotNull(startReport);
			StringAssert.StartsWith("<start-suite", startReport);
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