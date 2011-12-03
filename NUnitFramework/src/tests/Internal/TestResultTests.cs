// ***********************************************************************
// Copyright (c) 2010 Charlie Poole
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
using System.Xml;
using NUnit.Framework.Api;
using NUnit.TestUtilities;

namespace NUnit.Framework.Internal
{
	/// <summary>
	/// Summary description for TestResultTests.
	/// </summary>
	[TestFixture]
	public abstract class TestResultTests
	{
		protected TestCaseResult testResult;
        protected TestSuiteResult suiteResult;
        protected TestMethod test;

        protected string ignoredChildMessage = "One or more child tests were ignored";
        protected string failingChildMessage = "One or more child tests had errors";

		[SetUp]
		public void SetUp()
		{
            test = new TestMethod(typeof(DummySuite).GetMethod("DummyMethod"));
            test.Properties.Set(PropertyNames.Description, "Test description");
            test.Properties.Add(PropertyNames.Category, "Dubious");
            test.Properties.Set("Priority", "low");
			testResult = (TestCaseResult)test.MakeTestResult();

            TestSuite suite = new TestSuite(typeof(DummySuite));
            suite.Properties.Set(PropertyNames.Description, "Suite description");
            suite.Properties.Add(PropertyNames.Category, "Fast");
            suite.Properties.Add("Value", 3);
            suiteResult = (TestSuiteResult)suite.MakeTestResult();

            SimulateTestRun();
        }

        [Test]
        public void TestResultBasicInfo()
        {
            Assert.AreEqual("DummyMethod", testResult.Name);
            Assert.AreEqual("NUnit.Framework.Internal.TestResultTests+DummySuite.DummyMethod", testResult.FullName);
        }

        [Test]
        public void SuiteResultBasicInfo()
        {
            Assert.AreEqual("TestResultTests+DummySuite", suiteResult.Name);
            Assert.AreEqual("NUnit.Framework.Internal.TestResultTests+DummySuite", suiteResult.FullName);
        }

        [Test]
        public void TestResultXmlNodeBasicInfo()
        {
            XmlNode testNode = testResult.ToXml(true);

            Assert.True(testNode is XmlElement);
            Assert.NotNull(testNode.Attributes["id"]);
            Assert.AreEqual("test-case", testNode.Name);
            Assert.AreEqual("DummyMethod", testNode.Attributes["name"].Value);
            Assert.AreEqual("NUnit.Framework.Internal.TestResultTests+DummySuite.DummyMethod", testNode.Attributes["fullname"].Value);
            
            Assert.AreEqual("Test description", testNode.SelectSingleNode("properties/property[@name='Description']").Attributes["value"].Value);
            Assert.AreEqual("Dubious", testNode.SelectSingleNode("properties/property[@name='Category']").Attributes["value"].Value);
            Assert.AreEqual("low", testNode.SelectSingleNode("properties/property[@name='Priority']").Attributes["value"].Value);

            Assert.AreEqual(0, testNode.SelectNodes("test-case").Count);
        }

        [Test]
        public void SuiteResultXmlNodeBasicInfo()
        {
            XmlNode suiteNode = suiteResult.ToXml(true);

            Assert.True(suiteNode is XmlElement);
            Assert.NotNull(suiteNode.Attributes["id"]);
            Assert.AreEqual("test-suite", suiteNode.Name);
            Assert.AreEqual("TestResultTests+DummySuite", suiteNode.Attributes["name"].Value);
            Assert.AreEqual("NUnit.Framework.Internal.TestResultTests+DummySuite", suiteNode.Attributes["fullname"].Value);

            Assert.AreEqual("Suite description", suiteNode.SelectSingleNode("properties/property[@name='Description']").Attributes["value"].Value);
            Assert.AreEqual("Fast", suiteNode.SelectSingleNode("properties/property[@name='Category']").Attributes["value"].Value);
            Assert.AreEqual("3", suiteNode.SelectSingleNode("properties/property[@name='Value']").Attributes["value"].Value);
        }

        protected abstract void SimulateTestRun();

        public class DummySuite
        {
            public void DummyMethod() { }
        }
    }

    public class DefaultResultTests : TestResultTests
    {
        protected override void SimulateTestRun()
        {
            suiteResult.AddResult(testResult);
        }

        [Test]
        public void TestResultIsInconclusive()
        {
            Assert.AreEqual(ResultState.Inconclusive, testResult.ResultState);
            Assert.AreEqual(TestStatus.Inconclusive, testResult.ResultState.Status);
            Assert.That(testResult.ResultState.Label, Is.Empty);
            Assert.AreEqual(0.0, testResult.Time);
        }

        [Test]
        public void SuiteResultIsInconclusive()
        {
            Assert.AreEqual(ResultState.Inconclusive, suiteResult.ResultState);
            Assert.AreEqual(0, suiteResult.AssertCount);
        }

        [Test]
        public void TestResultXmlNodeIsInconclusive()
        {
            XmlNode testNode = testResult.ToXml(true);

            Assert.AreEqual("Inconclusive", testNode.Attributes["result"].Value);
        }

        [Test]
        public void SuiteResultXmlNodeIsInconclusive()
        {
            XmlNode suiteNode = suiteResult.ToXml(true);

            Assert.AreEqual("Inconclusive", suiteNode.Attributes["result"].Value);
            Assert.AreEqual("0", suiteNode.Attributes["passed"].Value);
            Assert.AreEqual("0", suiteNode.Attributes["failed"].Value);
            Assert.AreEqual("0", suiteNode.Attributes["skipped"].Value);
            Assert.AreEqual("1", suiteNode.Attributes["inconclusive"].Value);
            Assert.AreEqual("0", suiteNode.Attributes["asserts"].Value);
        }

        [Test]
        public void SuiteResultXmlNodeHasOneChildTest()
        {
            XmlNode suiteNode = suiteResult.ToXml(true);

            Assert.AreEqual(1, suiteNode.SelectNodes("test-case").Count);
        }
    }

    public class SuccessResultTests : TestResultTests
    {
        protected override void SimulateTestRun()
        {
            testResult.SetResult(ResultState.Success);
            testResult.Time = 0.125;
            suiteResult.Time = 0.125;
            testResult.AssertCount = 2;
            suiteResult.AddResult(testResult);
        }

        [Test]
        public void TestResultIsSuccess()
        {
            Assert.True(testResult.ResultState == ResultState.Success);
            Assert.AreEqual(TestStatus.Passed, testResult.ResultState.Status);
            Assert.That(testResult.ResultState.Label, Is.Empty);
            Assert.AreEqual(0.125, testResult.Time);
        }

        [Test]
        public void SuiteResultIsSuccess()
        {
            Assert.True(suiteResult.ResultState == ResultState.Success);
            Assert.AreEqual(TestStatus.Passed, suiteResult.ResultState.Status);
            Assert.That(suiteResult.ResultState.Label, Is.Empty);

            Assert.AreEqual(1, suiteResult.PassCount);
            Assert.AreEqual(0, suiteResult.FailCount);
            Assert.AreEqual(0, suiteResult.SkipCount);
            Assert.AreEqual(0, suiteResult.InconclusiveCount);
            Assert.AreEqual(2, suiteResult.AssertCount);
        }

        [Test]
        public void TestResultXmlNodeIsSuccess()
        {
            XmlNode testNode = testResult.ToXml(true);

            Assert.AreEqual("Passed", testNode.Attributes["result"].Value);
            Assert.AreEqual("0.125", testNode.Attributes["time"].Value);
            Assert.AreEqual("2", testNode.Attributes["asserts"].Value);
        }

        [Test]
        public void SuiteResultXmlNodeIsSuccess()
        {
            XmlNode suiteNode = suiteResult.ToXml(true);

            Assert.AreEqual("Passed", suiteNode.Attributes["result"].Value);
            Assert.AreEqual("0.125", suiteNode.Attributes["time"].Value);
            Assert.AreEqual("1", suiteNode.Attributes["passed"].Value);
            Assert.AreEqual("0", suiteNode.Attributes["failed"].Value);
            Assert.AreEqual("0", suiteNode.Attributes["skipped"].Value);
            Assert.AreEqual("0", suiteNode.Attributes["inconclusive"].Value);
            Assert.AreEqual("2", suiteNode.Attributes["asserts"].Value);
        }

        [Test]
        public void SuiteResultXmlNodeHasOneChildTest()
        {
            XmlNode suiteNode = suiteResult.ToXml(true);

            Assert.AreEqual(1, suiteNode.SelectNodes("test-case").Count);
        }
    }

    public class IgnoredResultTests : TestResultTests
    {
        protected override void SimulateTestRun()
        {
            testResult.SetResult(ResultState.Ignored, "because");
            suiteResult.AddResult(testResult);
        }

        [Test]
		public void TestResultIsIgnored()
		{
            Assert.AreEqual(ResultState.Ignored, testResult.ResultState);
            Assert.AreEqual(TestStatus.Skipped, testResult.ResultState.Status);
            Assert.AreEqual("Ignored", testResult.ResultState.Label);
            Assert.AreEqual("because", testResult.Message);
        }

        [Test]
        public void SuiteResultIsIgnored()
        {
            Assert.AreEqual(ResultState.Ignored, suiteResult.ResultState);
            Assert.AreEqual(TestStatus.Skipped, suiteResult.ResultState.Status);
            Assert.AreEqual(ignoredChildMessage, suiteResult.Message);

            Assert.AreEqual(0, suiteResult.PassCount);
            Assert.AreEqual(0, suiteResult.FailCount);
            Assert.AreEqual(1, suiteResult.SkipCount);
            Assert.AreEqual(0, suiteResult.InconclusiveCount);
            Assert.AreEqual(0, suiteResult.AssertCount);
        }

        [Test]
        public void TestResultXmlNodeIsIgnored()
        {
            XmlNode testNode = testResult.ToXml(true);

            Assert.AreEqual("Skipped", testNode.Attributes["result"].Value);
            Assert.AreEqual("Ignored", testNode.Attributes["label"].Value);
            XmlNode reason = testNode.SelectSingleNode("reason");
            Assert.NotNull(reason);
            Assert.NotNull(reason.SelectSingleNode("message"));
            Assert.AreEqual("because", reason.SelectSingleNode("message").InnerText);
            Assert.Null(reason.SelectSingleNode("stack-trace"));
        }

        [Test]
        public void SuiteResultXmlNodeIsIgnored()
        {
            XmlNode suiteNode = suiteResult.ToXml(true);

            Assert.AreEqual("Skipped", suiteNode.Attributes["result"].Value);
            Assert.AreEqual("Ignored", suiteNode.Attributes["label"].Value);
            Assert.AreEqual("0", suiteNode.Attributes["passed"].Value);
            Assert.AreEqual("0", suiteNode.Attributes["failed"].Value);
            Assert.AreEqual("1", suiteNode.Attributes["skipped"].Value);
            Assert.AreEqual("0", suiteNode.Attributes["inconclusive"].Value);
            Assert.AreEqual("0", suiteNode.Attributes["asserts"].Value);
        }

        [Test]
        public void SuiteResultXmlNodeHasOneChildTest()
        {
            XmlNode suiteNode = suiteResult.ToXml(true);

            Assert.AreEqual(1, suiteNode.SelectNodes("test-case").Count);
        }
    }

    public class FailedResultTests : TestResultTests
    {
        protected override void SimulateTestRun()
        {
            testResult.SetResult(ResultState.Failure, "message", "stack trace");
            testResult.Time = 0.125;
            suiteResult.Time = 0.125;
            testResult.AssertCount = 3;
            suiteResult.AddResult(testResult);
        }

        [Test]
        public void TestResultIsFailure()
        {
            Assert.AreEqual(ResultState.Failure, testResult.ResultState);
            Assert.AreEqual(TestStatus.Failed, testResult.ResultState.Status);
            Assert.AreEqual("message", testResult.Message);
            Assert.AreEqual("stack trace", testResult.StackTrace);
            Assert.AreEqual(0.125, testResult.Time);
        }

        [Test]
        public void SuiteResultIsFailure()
        {
            Assert.AreEqual(ResultState.Failure, suiteResult.ResultState);
            Assert.AreEqual(TestStatus.Failed, suiteResult.ResultState.Status);
            Assert.AreEqual(failingChildMessage, suiteResult.Message);
            Assert.Null(suiteResult.StackTrace);

            Assert.AreEqual(0, suiteResult.PassCount);
            Assert.AreEqual(1, suiteResult.FailCount);
            Assert.AreEqual(0, suiteResult.SkipCount);
            Assert.AreEqual(0, suiteResult.InconclusiveCount);
            Assert.AreEqual(3, suiteResult.AssertCount);
        }

        [Test]
        public void TestResultXmlNodeIsFailure()
        {
            XmlNode testNode = testResult.ToXml(true);

            Assert.AreEqual("Failed", testNode.Attributes["result"].Value);
            Assert.AreEqual("0.125", testNode.Attributes["time"].Value);

            XmlNode failureNode = testNode.SelectSingleNode("failure");
            Assert.NotNull(failureNode, "No <failure> element found");

            XmlNode messageNode = failureNode.SelectSingleNode("message");
            Assert.NotNull(messageNode, "No <message> element found");
            Assert.AreEqual("message", messageNode.InnerText);

            XmlNode stacktraceNode = failureNode.SelectSingleNode("stack-trace");
            Assert.NotNull(stacktraceNode, "No <stack-trace> element found");
            Assert.AreEqual("stack trace", stacktraceNode.InnerText);
        }

        [Test]
        public void SuiteResultXmlNodeIsFailure()
        {
            XmlNode suiteNode = suiteResult.ToXml(true);

            Assert.AreEqual("Failed", suiteNode.Attributes["result"].Value);
            Assert.AreEqual("0.125", suiteNode.Attributes["time"].Value);

            XmlNode failureNode = suiteNode.SelectSingleNode("failure");
            Assert.NotNull(failureNode, "No <failure> element found");

            XmlNode messageNode = failureNode.SelectSingleNode("message");
            Assert.NotNull(messageNode, "No <message> element found");
            Assert.AreEqual(failingChildMessage, messageNode.InnerText);

            XmlNode stacktraceNode = failureNode.SelectSingleNode("stacktrace");
            Assert.Null(stacktraceNode, "Unexpected <stack-trace> element found");

            Assert.AreEqual("0", suiteNode.Attributes["passed"].Value);
            Assert.AreEqual("1", suiteNode.Attributes["failed"].Value);
            Assert.AreEqual("0", suiteNode.Attributes["skipped"].Value);
            Assert.AreEqual("0", suiteNode.Attributes["inconclusive"].Value);
            Assert.AreEqual("3", suiteNode.Attributes["asserts"].Value);
        }

        [Test]
        public void SuiteResultXmlNodeHasOneChildTest()
        {
            XmlNode suiteNode = suiteResult.ToXml(true);

            Assert.AreEqual(1, suiteNode.SelectNodes("test-case").Count);
        }
    }

    public class MixedResultTests : TestResultTests
    {
        protected override void SimulateTestRun()
        {
            testResult.SetResult(ResultState.Success);
            testResult.AssertCount = 2;
            suiteResult.AddResult(testResult);

            testResult.SetResult(ResultState.Failure, "message", "stack trace");
            testResult.AssertCount = 1;
            suiteResult.AddResult(testResult);

            testResult.SetResult(ResultState.Success);
            testResult.AssertCount = 3;
            suiteResult.AddResult(testResult);

            testResult.SetResult(ResultState.Inconclusive, "inconclusive reason", "stacktrace");
            testResult.AssertCount = 0;
            suiteResult.AddResult(testResult);
        }

        [Test]
        public void SuiteResultIsFailure()
        {
            Assert.AreEqual(ResultState.Failure, suiteResult.ResultState);
            Assert.AreEqual(TestStatus.Failed, suiteResult.ResultState.Status);
            Assert.AreEqual(failingChildMessage, suiteResult.Message);
            Assert.Null(suiteResult.StackTrace, "There should be no stacktrace");

            Assert.AreEqual(2, suiteResult.PassCount);
            Assert.AreEqual(1, suiteResult.FailCount);
            Assert.AreEqual(0, suiteResult.SkipCount);
            Assert.AreEqual(1, suiteResult.InconclusiveCount);
            Assert.AreEqual(6, suiteResult.AssertCount);
        }

        [Test]
        public void SuiteResultXmlNodeIsFailure()
        {
            XmlNode suiteNode = suiteResult.ToXml(true);

            Assert.AreEqual("Failed", suiteNode.Attributes["result"].Value);
            XmlNode failureNode = suiteNode.SelectSingleNode("failure");
            Assert.NotNull(failureNode, "No failure element found");

            XmlNode messageNode = failureNode.SelectSingleNode("message");
            Assert.NotNull(messageNode, "No message element found");
            Assert.AreEqual(failingChildMessage, messageNode.InnerText);

            XmlNode stacktraceNode = failureNode.SelectSingleNode("stacktrace");
            Assert.Null(stacktraceNode, "There should be no stacktrace");

            Assert.AreEqual("2", suiteNode.Attributes["passed"].Value);
            Assert.AreEqual("1", suiteNode.Attributes["failed"].Value);
            Assert.AreEqual("0", suiteNode.Attributes["skipped"].Value);
            Assert.AreEqual("1", suiteNode.Attributes["inconclusive"].Value);
            Assert.AreEqual("6", suiteNode.Attributes["asserts"].Value);
        }

        [Test]
        public void SuiteResultXmlNodeHasFourChildTests()
        {
            XmlNode suiteNode = suiteResult.ToXml(true);

            Assert.AreEqual(4, suiteNode.SelectNodes("test-case").Count);
        }
    }
}
