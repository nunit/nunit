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
using NUnit.Framework.Interfaces;
using NUnit.TestUtilities;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Summary description for TestResultTests.
    /// </summary>
    [TestFixture]
    public abstract class TestResultTests
    {
        protected TestResult testResult;
        protected TestResult suiteResult;
        protected TestMethod test;

        protected string ignoredChildMessage = "One or more child tests were ignored";
        protected string failingChildMessage = "One or more child tests had errors";

        protected TimeSpan expectedDuration;
        protected DateTime expectedStart;
        protected DateTime expectedEnd;

        [SetUp]
        public void SetUp()
        {
            expectedDuration = TimeSpan.FromMilliseconds(125);
            expectedStart = new DateTime(1968, 4, 8, 15, 05, 30, 250, DateTimeKind.Utc);
            expectedEnd = expectedStart + expectedDuration;

            test = new TestMethod(typeof(DummySuite).GetMethod("DummyMethod"));
            test.Properties.Set(PropertyNames.Description, "Test description");
            test.Properties.Add(PropertyNames.Category, "Dubious");
            test.Properties.Set("Priority", "low");
            testResult = test.MakeTestResult();

            TestSuite suite = new TestSuite(typeof(DummySuite));
            suite.Properties.Set(PropertyNames.Description, "Suite description");
            suite.Properties.Add(PropertyNames.Category, "Fast");
            suite.Properties.Add("Value", 3);
            suiteResult = suite.MakeTestResult();

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

            Assert.NotNull(testNode.Attributes["id"]);
            Assert.AreEqual("test-case", testNode.Name);
            Assert.AreEqual("DummyMethod", testNode.Attributes["name"]);
            Assert.AreEqual("NUnit.Framework.Internal.TestResultTests+DummySuite.DummyMethod", testNode.Attributes["fullname"]);

            Assert.AreEqual("Test description", testNode.FindDescendant("properties/property[@name='Description']").Attributes["value"]);
            Assert.AreEqual("Dubious", testNode.FindDescendant("properties/property[@name='Category']").Attributes["value"]);
            Assert.AreEqual("low", testNode.FindDescendant("properties/property[@name='Priority']").Attributes["value"]);

            Assert.AreEqual(0, testNode.FindDescendants("test-case").Count);
        }

        [Test]
        public void SuiteResultXmlNodeBasicInfo()
        {
            XmlNode suiteNode = suiteResult.ToXml(true);

            Assert.NotNull(suiteNode.Attributes["id"]);
            Assert.AreEqual("test-suite", suiteNode.Name);
            Assert.AreEqual("TestResultTests+DummySuite", suiteNode.Attributes["name"]);
            Assert.AreEqual("NUnit.Framework.Internal.TestResultTests+DummySuite", suiteNode.Attributes["fullname"]);

            Assert.AreEqual("Suite description", suiteNode.FindDescendant("properties/property[@name='Description']").Attributes["value"]);
            Assert.AreEqual("Fast", suiteNode.FindDescendant("properties/property[@name='Category']").Attributes["value"]);
            Assert.AreEqual("3", suiteNode.FindDescendant("properties/property[@name='Value']").Attributes["value"]);
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
            Assert.AreEqual(TimeSpan.Zero, testResult.Duration);
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

            Assert.AreEqual("Inconclusive", testNode.Attributes["result"]);
        }

        [Test]
        public void SuiteResultXmlNodeIsInconclusive()
        {
            XmlNode suiteNode = suiteResult.ToXml(true);

            Assert.AreEqual("Inconclusive", suiteNode.Attributes["result"]);
            Assert.AreEqual("0", suiteNode.Attributes["passed"]);
            Assert.AreEqual("0", suiteNode.Attributes["failed"]);
            Assert.AreEqual("0", suiteNode.Attributes["skipped"]);
            Assert.AreEqual("1", suiteNode.Attributes["inconclusive"]);
            Assert.AreEqual("0", suiteNode.Attributes["asserts"]);
        }

        [Test]
        public void SuiteResultXmlNodeHasOneChildTest()
        {
            XmlNode suiteNode = suiteResult.ToXml(true);

            Assert.AreEqual(1, suiteNode.FindDescendants("test-case").Count);
        }
    }

    public class SuccessResultTests : TestResultTests
    {
        protected override void SimulateTestRun()
        {
            testResult.SetResult(ResultState.Success, "Test passed!");
            testResult.StartTime = expectedStart;
            testResult.EndTime = expectedEnd;
            testResult.Duration = expectedDuration;
            suiteResult.StartTime = expectedStart;
            suiteResult.EndTime = expectedEnd;
            suiteResult.Duration = expectedDuration;
            testResult.AssertCount = 2;
            suiteResult.AddResult(testResult);
        }

        [Test]
        public void TestResultIsSuccess()
        {
            Assert.True(testResult.ResultState == ResultState.Success);
            Assert.AreEqual(TestStatus.Passed, testResult.ResultState.Status);
            Assert.That(testResult.ResultState.Label, Is.Empty);
            Assert.AreEqual("Test passed!", testResult.Message);
            Assert.AreEqual(expectedStart, testResult.StartTime);
            Assert.AreEqual(expectedEnd, testResult.EndTime);
            Assert.AreEqual(expectedDuration, testResult.Duration);
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

            Assert.AreEqual("Passed", testNode.Attributes["result"]);
            Assert.AreEqual("1968-04-08 15:05:30Z", testNode.Attributes["start-time"]);
            Assert.AreEqual("1968-04-08 15:05:30Z", testNode.Attributes["end-time"]);
            Assert.AreEqual("0.125000", testNode.Attributes["duration"]);
            Assert.AreEqual("2", testNode.Attributes["asserts"]);

            XmlNode reason = testNode.FindDescendant("reason");
            Assert.NotNull(reason);
            Assert.NotNull(reason.FindDescendant("message"));
            Assert.AreEqual("Test passed!", reason.FindDescendant("message").TextContent);
            Assert.Null(reason.FindDescendant("stack-trace"));
        }

        [Test]
        public void SuiteResultXmlNodeIsSuccess()
        {
            XmlNode suiteNode = suiteResult.ToXml(true);

            Assert.AreEqual("Passed", suiteNode.Attributes["result"]);
            Assert.AreEqual("1968-04-08 15:05:30Z", suiteNode.Attributes["start-time"]);
            Assert.AreEqual("1968-04-08 15:05:30Z", suiteNode.Attributes["end-time"]);
            Assert.AreEqual("0.125000", suiteNode.Attributes["duration"]);
            Assert.AreEqual("1", suiteNode.Attributes["passed"]);
            Assert.AreEqual("0", suiteNode.Attributes["failed"]);
            Assert.AreEqual("0", suiteNode.Attributes["skipped"]);
            Assert.AreEqual("0", suiteNode.Attributes["inconclusive"]);
            Assert.AreEqual("2", suiteNode.Attributes["asserts"]);
        }

        [Test]
        public void SuiteResultXmlNodeHasOneChildTest()
        {
            XmlNode suiteNode = suiteResult.ToXml(true);

            Assert.AreEqual(1, suiteNode.FindDescendants("test-case").Count);
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

            Assert.AreEqual("Skipped", testNode.Attributes["result"]);
            Assert.AreEqual("Ignored", testNode.Attributes["label"]);
            XmlNode reason = testNode.FindDescendant("reason");
            Assert.NotNull(reason);
            Assert.NotNull(reason.FindDescendant("message"));
            Assert.AreEqual("because", reason.FindDescendant("message").TextContent);
            Assert.Null(reason.FindDescendant("stack-trace"));
        }

        [Test]
        public void SuiteResultXmlNodeIsIgnored()
        {
            XmlNode suiteNode = suiteResult.ToXml(true);

            Assert.AreEqual("Skipped", suiteNode.Attributes["result"]);
            Assert.AreEqual("Ignored", suiteNode.Attributes["label"]);
            Assert.AreEqual("0", suiteNode.Attributes["passed"]);
            Assert.AreEqual("0", suiteNode.Attributes["failed"]);
            Assert.AreEqual("1", suiteNode.Attributes["skipped"]);
            Assert.AreEqual("0", suiteNode.Attributes["inconclusive"]);
            Assert.AreEqual("0", suiteNode.Attributes["asserts"]);
        }

        [Test]
        public void SuiteResultXmlNodeHasOneChildTest()
        {
            XmlNode suiteNode = suiteResult.ToXml(true);

            Assert.AreEqual(1, suiteNode.FindDescendants("test-case").Count);
        }
    }

    public class NotRunnableResultTests : TestResultTests
    {
        protected override void SimulateTestRun()
        {
            testResult.SetResult(ResultState.NotRunnable, "bad test");
            suiteResult.AddResult(testResult);
        }

        [Test]
        public void TestResultIsNotRunnable()
        {
            Assert.AreEqual(ResultState.NotRunnable, testResult.ResultState);
            Assert.AreEqual(TestStatus.Skipped, testResult.ResultState.Status);
            Assert.AreEqual("Invalid", testResult.ResultState.Label);
            Assert.AreEqual("bad test", testResult.Message);
        }

        [Test]
        public void SuiteResultIsFailure()
        {
            Assert.AreEqual(ResultState.Failure, suiteResult.ResultState);
            Assert.AreEqual(TestStatus.Failed, suiteResult.ResultState.Status);
            Assert.AreEqual(failingChildMessage, suiteResult.Message);

            Assert.AreEqual(0, suiteResult.PassCount);
            Assert.AreEqual(0, suiteResult.FailCount);
            Assert.AreEqual(1, suiteResult.SkipCount);
            Assert.AreEqual(0, suiteResult.InconclusiveCount);
            Assert.AreEqual(0, suiteResult.AssertCount);
        }

        [Test]
        public void TestResultXmlNodeIsNotRunnable()
        {
            XmlNode testNode = testResult.ToXml(true);

            Assert.AreEqual("Skipped", testNode.Attributes["result"]);
            Assert.AreEqual("Invalid", testNode.Attributes["label"]);
            XmlNode reason = testNode.FindDescendant("reason");
            Assert.NotNull(reason);
            Assert.NotNull(reason.FindDescendant("message"));
            Assert.AreEqual("bad test", reason.FindDescendant("message").TextContent);
            Assert.Null(reason.FindDescendant("stack-trace"));
        }

        [Test]
        public void SuiteResultXmlNodeIsFailure()
        {
            XmlNode suiteNode = suiteResult.ToXml(true);

            Assert.AreEqual("Failed", suiteNode.Attributes["result"]);
            Assert.Null(suiteNode.Attributes["label"]);
            Assert.AreEqual("0", suiteNode.Attributes["passed"]);
            Assert.AreEqual("0", suiteNode.Attributes["failed"]);
            Assert.AreEqual("1", suiteNode.Attributes["skipped"]);
            Assert.AreEqual("0", suiteNode.Attributes["inconclusive"]);
            Assert.AreEqual("0", suiteNode.Attributes["asserts"]);
        }

        [Test]
        public void SuiteResultXmlNodeHasOneChildTest()
        {
            XmlNode suiteNode = suiteResult.ToXml(true);

            Assert.AreEqual(1, suiteNode.FindDescendants("test-case").Count);
        }
    }

    public class FailedResultTests : TestResultTests
    {
        protected override void SimulateTestRun()
        {
            testResult.SetResult(ResultState.Failure, "message", "stack trace");
            testResult.StartTime = expectedStart;
            testResult.EndTime = expectedEnd;
            testResult.Duration = expectedDuration;
            suiteResult.StartTime = expectedStart;
            suiteResult.EndTime = expectedEnd;
            suiteResult.Duration = expectedDuration;
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
            Assert.AreEqual(expectedStart, testResult.StartTime);
            Assert.AreEqual(expectedEnd, testResult.EndTime);
            Assert.AreEqual(expectedDuration, testResult.Duration);
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

            Assert.AreEqual("Failed", testNode.Attributes["result"]);
            Assert.AreEqual("1968-04-08 15:05:30Z", testNode.Attributes["start-time"]);
            Assert.AreEqual("1968-04-08 15:05:30Z", testNode.Attributes["end-time"]);
            Assert.AreEqual("0.125000", testNode.Attributes["duration"]);

            XmlNode failureNode = testNode.FindDescendant("failure");
            Assert.NotNull(failureNode, "No <failure> element found");

            XmlNode messageNode = failureNode.FindDescendant("message");
            Assert.NotNull(messageNode, "No <message> element found");
            Assert.AreEqual("message", messageNode.TextContent);

            XmlNode stacktraceNode = failureNode.FindDescendant("stack-trace");
            Assert.NotNull(stacktraceNode, "No <stack-trace> element found");
            Assert.AreEqual("stack trace", stacktraceNode.TextContent);
        }

        [Test]
        public void SuiteResultXmlNodeIsFailure()
        {
            XmlNode suiteNode = suiteResult.ToXml(true);

            Assert.AreEqual("Failed", suiteNode.Attributes["result"]);
            Assert.AreEqual("1968-04-08 15:05:30Z", suiteNode.Attributes["start-time"]);
            Assert.AreEqual("1968-04-08 15:05:30Z", suiteNode.Attributes["end-time"]);
            Assert.AreEqual("0.125000", suiteNode.Attributes["duration"]);

            XmlNode failureNode = suiteNode.FindDescendant("failure");
            Assert.NotNull(failureNode, "No <failure> element found");

            XmlNode messageNode = failureNode.FindDescendant("message");
            Assert.NotNull(messageNode, "No <message> element found");
            Assert.AreEqual(failingChildMessage, messageNode.TextContent);

            XmlNode stacktraceNode = failureNode.FindDescendant("stacktrace");
            Assert.Null(stacktraceNode, "Unexpected <stack-trace> element found");

            Assert.AreEqual("0", suiteNode.Attributes["passed"]);
            Assert.AreEqual("1", suiteNode.Attributes["failed"]);
           Assert.AreEqual("0", suiteNode.Attributes["skipped"]);
            Assert.AreEqual("0", suiteNode.Attributes["inconclusive"]);
            Assert.AreEqual("3", suiteNode.Attributes["asserts"]);
        }

        [Test]
        public void SuiteResultXmlNodeHasOneChildTest()
        {
            XmlNode suiteNode = suiteResult.ToXml(true);

            Assert.AreEqual(1, suiteNode.FindDescendants("test-case").Count);
        }
    }

    public class InconclusiveResultTests : TestResultTests
    {
        protected override void SimulateTestRun()
        {
            testResult.SetResult(ResultState.Inconclusive, "because");
            suiteResult.AddResult(testResult);
        }

        [Test]
        public void TestResultIsInconclusive()
        {
            Assert.AreEqual(ResultState.Inconclusive, testResult.ResultState);
            Assert.AreEqual(TestStatus.Inconclusive, testResult.ResultState.Status);
            Assert.That(testResult.ResultState.Label, Is.Empty);
            Assert.AreEqual("because", testResult.Message);
        }

        [Test]
        public void SuiteResultIsInconclusive()
        {
            Assert.AreEqual(ResultState.Inconclusive, suiteResult.ResultState);
            Assert.AreEqual(TestStatus.Inconclusive, suiteResult.ResultState.Status);
            Assert.Null(suiteResult.Message);

            Assert.AreEqual(0, suiteResult.PassCount);
            Assert.AreEqual(0, suiteResult.FailCount);
            Assert.AreEqual(0, suiteResult.SkipCount);
            Assert.AreEqual(1, suiteResult.InconclusiveCount);
            Assert.AreEqual(0, suiteResult.AssertCount);
        }

        [Test]
        public void TestResultXmlNodeIsInconclusive()
        {
            XmlNode testNode = testResult.ToXml(true);

            Assert.AreEqual("Inconclusive", testNode.Attributes["result"]);
            Assert.IsNull(testNode.Attributes["label"]);
            XmlNode reason = testNode.FindDescendant("reason");
            Assert.NotNull(reason);
            Assert.NotNull(reason.FindDescendant("message"));
            Assert.AreEqual("because", reason.FindDescendant("message").TextContent);
            Assert.Null(reason.FindDescendant("stack-trace"));
        }

        [Test]
        public void SuiteResultXmlNodeIsInconclusive()
        {
            XmlNode suiteNode = suiteResult.ToXml(true);

            Assert.AreEqual("Inconclusive", suiteNode.Attributes["result"]);
            Assert.IsNull(suiteNode.Attributes["label"]);
            Assert.AreEqual("0", suiteNode.Attributes["passed"]);
            Assert.AreEqual("0", suiteNode.Attributes["failed"]);
            Assert.AreEqual("0", suiteNode.Attributes["skipped"]);
            Assert.AreEqual("1", suiteNode.Attributes["inconclusive"]);
            Assert.AreEqual("0", suiteNode.Attributes["asserts"]);
        }

        [Test]
        public void SuiteResultXmlNodeHasOneChildTest()
        {
            XmlNode suiteNode = suiteResult.ToXml(true);

            Assert.AreEqual(1, suiteNode.FindDescendants("test-case").Count);
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

            Assert.AreEqual("Failed", suiteNode.Attributes["result"]);
            XmlNode failureNode = suiteNode.FindDescendant("failure");
            Assert.NotNull(failureNode, "No failure element found");

            XmlNode messageNode = failureNode.FindDescendant("message");
            Assert.NotNull(messageNode, "No message element found");
            Assert.AreEqual(failingChildMessage, messageNode.TextContent);

            XmlNode stacktraceNode = failureNode.FindDescendant("stacktrace");
            Assert.Null(stacktraceNode, "There should be no stacktrace");

            Assert.AreEqual("2", suiteNode.Attributes["passed"]);
            Assert.AreEqual("1", suiteNode.Attributes["failed"]);
            Assert.AreEqual("0", suiteNode.Attributes["skipped"]);
            Assert.AreEqual("1", suiteNode.Attributes["inconclusive"]);
            Assert.AreEqual("6", suiteNode.Attributes["asserts"]);
        }

        [Test]
        public void SuiteResultXmlNodeHasFourChildTests()
        {
            XmlNode suiteNode = suiteResult.ToXml(true);

            Assert.AreEqual(4, suiteNode.FindDescendants("test-case").Count);
        }
    }
}
