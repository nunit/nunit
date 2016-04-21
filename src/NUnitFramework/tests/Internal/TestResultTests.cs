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
        protected TestSuiteResult suiteResult;
        protected TestMethod test;

        protected double expectedDuration;
        protected DateTime expectedStart;
        protected DateTime expectedEnd;

        [SetUp]
        public void SetUp()
        {
            expectedDuration = 0.125;
            expectedStart = new DateTime(1968, 4, 8, 15, 05, 30, 250, DateTimeKind.Utc);
            expectedEnd = expectedStart.AddSeconds(expectedDuration);

            test = new TestMethod(new MethodWrapper(typeof(DummySuite), "DummyMethod"));
            test.Properties.Set(PropertyNames.Description, "Test description");
            test.Properties.Add(PropertyNames.Category, "Dubious");
            test.Properties.Set("Priority", "low");
            testResult = test.MakeTestResult();

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
            TNode testNode = testResult.ToXml(true);

            Assert.NotNull(testNode.Attributes["id"]);
            Assert.AreEqual("test-case", testNode.Name);
            Assert.AreEqual("DummyMethod", testNode.Attributes["name"]);
            Assert.AreEqual("NUnit.Framework.Internal.TestResultTests+DummySuite.DummyMethod", testNode.Attributes["fullname"]);

            Assert.AreEqual("Test description", testNode.SelectSingleNode("properties/property[@name='Description']").Attributes["value"]);
            Assert.AreEqual("Dubious", testNode.SelectSingleNode("properties/property[@name='Category']").Attributes["value"]);
            Assert.AreEqual("low", testNode.SelectSingleNode("properties/property[@name='Priority']").Attributes["value"]);

            Assert.AreEqual(0, testNode.SelectNodes("test-case").Count);
        }

        [Test]
        public void SuiteResultXmlNodeBasicInfo()
        {
            TNode suiteNode = suiteResult.ToXml(true);

            Assert.NotNull(suiteNode.Attributes["id"]);
            Assert.AreEqual("test-suite", suiteNode.Name);
            Assert.AreEqual("TestResultTests+DummySuite", suiteNode.Attributes["name"]);
            Assert.AreEqual("NUnit.Framework.Internal.TestResultTests+DummySuite", suiteNode.Attributes["fullname"]);

            Assert.AreEqual("Suite description", suiteNode.SelectSingleNode("properties/property[@name='Description']").Attributes["value"]);
            Assert.AreEqual("Fast", suiteNode.SelectSingleNode("properties/property[@name='Category']").Attributes["value"]);
            Assert.AreEqual("3", suiteNode.SelectSingleNode("properties/property[@name='Value']").Attributes["value"]);
        }

        protected virtual ResultState ResultState
        {
            get { return null; }
        }

        protected virtual string ReasonNodeName
        {
            get { return "reason"; }
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
            Assert.That(testResult.Duration, Is.EqualTo(0d));
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
            TNode testNode = testResult.ToXml(true);

            Assert.AreEqual("Inconclusive", testNode.Attributes["result"]);
        }

        [Test]
        public void SuiteResultXmlNodeIsInconclusive()
        {
            TNode suiteNode = suiteResult.ToXml(true);

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
            TNode suiteNode = suiteResult.ToXml(true);

            Assert.AreEqual(1, suiteNode.SelectNodes("test-case").Count);
        }
    }

    public class SuccessResultTests : TestResultTests
    {
        protected override ResultState ResultState
        {
            get { return ResultState.Success; }
        }

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
            TNode testNode = testResult.ToXml(true);

            Assert.AreEqual("Passed", testNode.Attributes["result"]);
            Assert.AreEqual(null, testNode.Attributes["label"]);
            Assert.AreEqual(null, testNode.Attributes["site"]);
            Assert.AreEqual("1968-04-08 15:05:30Z", testNode.Attributes["start-time"]);
            Assert.AreEqual("1968-04-08 15:05:30Z", testNode.Attributes["end-time"]);
            Assert.AreEqual("0.125000", testNode.Attributes["duration"]);
            Assert.AreEqual("2", testNode.Attributes["asserts"]);

            TNode reason = testNode.SelectSingleNode("reason");
            Assert.NotNull(reason);
            Assert.NotNull(reason.SelectSingleNode("message"));
            Assert.AreEqual("Test passed!", reason.SelectSingleNode("message").Value);
            Assert.Null(reason.SelectSingleNode("stack-trace"));
        }

        [Test]
        public void SuiteResultXmlNodeIsSuccess()
        {
            TNode suiteNode = suiteResult.ToXml(true);

            Assert.AreEqual("Passed", suiteNode.Attributes["result"]);
            Assert.AreEqual(null, suiteNode.Attributes["label"]);
            Assert.AreEqual(null, suiteNode.Attributes["site"]);
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
            TNode suiteNode = suiteResult.ToXml(true);

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
            Assert.AreEqual(TestResult.CHILD_IGNORE_MESSAGE, suiteResult.Message);

            Assert.AreEqual(0, suiteResult.PassCount);
            Assert.AreEqual(0, suiteResult.FailCount);
            Assert.AreEqual(1, suiteResult.SkipCount);
            Assert.AreEqual(0, suiteResult.InconclusiveCount);
            Assert.AreEqual(0, suiteResult.AssertCount);
        }

        [Test]
        public void TestResultXmlNodeIsIgnored()
        {
            TNode testNode = testResult.ToXml(true);

            Assert.AreEqual("Skipped", testNode.Attributes["result"]);
            Assert.AreEqual("Ignored", testNode.Attributes["label"]);
            Assert.AreEqual(null, testNode.Attributes["site"]);
            TNode reason = testNode.SelectSingleNode("reason");
            Assert.NotNull(reason);
            Assert.NotNull(reason.SelectSingleNode("message"));
            Assert.AreEqual("because", reason.SelectSingleNode("message").Value);
            Assert.Null(reason.SelectSingleNode("stack-trace"));
        }

        protected override ResultState ResultState
        {
            get { return ResultState.Ignored; }
        }

        [Test]
        public void SuiteResultXmlNodeIsIgnored()
        {
            TNode suiteNode = suiteResult.ToXml(true);

            Assert.AreEqual("Skipped", suiteNode.Attributes["result"]);
            Assert.AreEqual("Ignored", suiteNode.Attributes["label"]);
            Assert.AreEqual(null, suiteNode.Attributes["site"]);
            Assert.AreEqual("0", suiteNode.Attributes["passed"]);
            Assert.AreEqual("0", suiteNode.Attributes["failed"]);
            Assert.AreEqual("1", suiteNode.Attributes["skipped"]);
            Assert.AreEqual("0", suiteNode.Attributes["inconclusive"]);
            Assert.AreEqual("0", suiteNode.Attributes["asserts"]);
        }

        [Test]
        public void SuiteResultXmlNodeHasOneChildTest()
        {
            TNode suiteNode = suiteResult.ToXml(true);

            Assert.AreEqual(1, suiteNode.SelectNodes("test-case").Count);
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
            Assert.AreEqual(TestStatus.Failed, testResult.ResultState.Status);
            Assert.AreEqual("Invalid", testResult.ResultState.Label);
            Assert.AreEqual("bad test", testResult.Message);
        }

        [Test]
        public void SuiteResultIsFailure()
        {
            Assert.AreEqual(ResultState.ChildFailure, suiteResult.ResultState);
            Assert.AreEqual(TestStatus.Failed, suiteResult.ResultState.Status);
            Assert.AreEqual(TestResult.CHILD_ERRORS_MESSAGE, suiteResult.Message);
            Assert.That(suiteResult.ResultState.Site, Is.EqualTo(FailureSite.Child));

            Assert.AreEqual(0, suiteResult.PassCount);
            Assert.AreEqual(1, suiteResult.FailCount);
            Assert.AreEqual(0, suiteResult.SkipCount);
            Assert.AreEqual(0, suiteResult.InconclusiveCount);
            Assert.AreEqual(0, suiteResult.AssertCount);
        }

        [Test]
        public void TestResultXmlNodeIsNotRunnable()
        {
            TNode testNode = testResult.ToXml(true);

            Assert.AreEqual("Failed", testNode.Attributes["result"]);
            Assert.AreEqual("Invalid", testNode.Attributes["label"]);
            Assert.AreEqual(null, testNode.Attributes["site"]);
            TNode failure = testNode.SelectSingleNode("failure");
            Assert.NotNull(failure);
            Assert.NotNull(failure.SelectSingleNode("message"));
            Assert.AreEqual("bad test", failure.SelectSingleNode("message").Value);
            Assert.Null(failure.SelectSingleNode("stack-trace"));
        }

        [Test]
        public void SuiteResultXmlNodeIsFailure()
        {
            TNode suiteNode = suiteResult.ToXml(true);

            Assert.AreEqual("Failed", suiteNode.Attributes["result"]);
            Assert.AreEqual(null, suiteNode.Attributes["label"]);
            Assert.AreEqual("Child", suiteNode.Attributes["site"]);
            Assert.AreEqual("0", suiteNode.Attributes["passed"]);
            Assert.AreEqual("1", suiteNode.Attributes["failed"]);
            Assert.AreEqual("0", suiteNode.Attributes["skipped"]);
            Assert.AreEqual("0", suiteNode.Attributes["inconclusive"]);
            Assert.AreEqual("0", suiteNode.Attributes["asserts"]);
        }

        [Test]
        public void SuiteResultXmlNodeHasOneChildTest()
        {
            TNode suiteNode = suiteResult.ToXml(true);

            Assert.AreEqual(1, suiteNode.SelectNodes("test-case").Count);
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
            Assert.AreEqual(ResultState.ChildFailure, suiteResult.ResultState);
            Assert.AreEqual(TestStatus.Failed, suiteResult.ResultState.Status);
            Assert.AreEqual(TestResult.CHILD_ERRORS_MESSAGE, suiteResult.Message);
            Assert.That(suiteResult.ResultState.Site, Is.EqualTo(FailureSite.Child));
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
            TNode testNode = testResult.ToXml(true);

            Assert.AreEqual("Failed", testNode.Attributes["result"]);
            Assert.AreEqual(null, testNode.Attributes["label"]);
            Assert.AreEqual(null, testNode.Attributes["site"]);
            Assert.AreEqual("1968-04-08 15:05:30Z", testNode.Attributes["start-time"]);
            Assert.AreEqual("1968-04-08 15:05:30Z", testNode.Attributes["end-time"]);
            Assert.AreEqual("0.125000", testNode.Attributes["duration"]);

            TNode failureNode = testNode.SelectSingleNode("failure");
            Assert.NotNull(failureNode, "No <failure> element found");

            TNode messageNode = failureNode.SelectSingleNode("message");
            Assert.NotNull(messageNode, "No <message> element found");
            Assert.AreEqual("message", messageNode.Value);

            TNode stacktraceNode = failureNode.SelectSingleNode("stack-trace");
            Assert.NotNull(stacktraceNode, "No <stack-trace> element found");
            Assert.AreEqual("stack trace", stacktraceNode.Value);
        }

        [Test]
        public void TestResultXmlNodeEscapesInvalidXmlCharacters()
        {
            if ( ResultState == null )
                Assert.Ignore( "Test ignored because ResultState is not set" );

            testResult.SetResult( ResultState, "Invalid Characters: \u0001\u0008\u000b\u001f\ud800; Valid Characters: \u0009\u000a\u000d\u0020\ufffd\ud800\udc00" );
            TNode testNode = testResult.ToXml( true );
            TNode reasonNode = testNode.SelectSingleNode( ReasonNodeName );

            Assert.That( reasonNode, Is.Not.Null, "No <{0}> element found", ReasonNodeName );

            TNode messageNode = reasonNode.SelectSingleNode( "message" );

            Assert.That( messageNode, Is.Not.Null, "No <message> element found" );
            Assert.That( messageNode.Value, Is.EqualTo( "Invalid Characters: \\u0001\\u0008\\u000b\\u001f\\ud800; Valid Characters: \u0009\u000a\u000d\u0020\ufffd\ud800\udc00" ) );
        }

        protected override ResultState ResultState
        {
            get { return ResultState.Failure; }
        }

        protected override string ReasonNodeName
        {
            get { return "failure"; }
        }

        [Test]
        public void SuiteResultXmlNodeIsFailure()
        {
            TNode suiteNode = suiteResult.ToXml(true);

            Assert.AreEqual("Failed", suiteNode.Attributes["result"]);
            Assert.AreEqual(null, suiteNode.Attributes["label"]);
            Assert.AreEqual("Child", suiteNode.Attributes["site"]);
            Assert.AreEqual("1968-04-08 15:05:30Z", suiteNode.Attributes["start-time"]);
            Assert.AreEqual("1968-04-08 15:05:30Z", suiteNode.Attributes["end-time"]);
            Assert.AreEqual("0.125000", suiteNode.Attributes["duration"]);

            TNode failureNode = suiteNode.SelectSingleNode("failure");
            Assert.NotNull(failureNode, "No <failure> element found");

            TNode messageNode = failureNode.SelectSingleNode("message");
            Assert.NotNull(messageNode, "No <message> element found");
            Assert.AreEqual(TestResult.CHILD_ERRORS_MESSAGE, messageNode.Value);

            TNode stacktraceNode = failureNode.SelectSingleNode("stacktrace");
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
            TNode suiteNode = suiteResult.ToXml(true);

            Assert.AreEqual(1, suiteNode.SelectNodes("test-case").Count);
        }
    }

    public class InconclusiveResultTests : TestResultTests
    {
        protected override ResultState ResultState
        {
            get { return ResultState.Inconclusive; }
        }

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
            TNode testNode = testResult.ToXml(true);

            Assert.AreEqual("Inconclusive", testNode.Attributes["result"]);
            Assert.IsNull(testNode.Attributes["label"]);
            Assert.IsNull(testNode.Attributes["site"]);
            TNode reason = testNode.SelectSingleNode("reason");
            Assert.NotNull(reason);
            Assert.NotNull(reason.SelectSingleNode("message"));
            Assert.AreEqual("because", reason.SelectSingleNode("message").Value);
            Assert.Null(reason.SelectSingleNode("stack-trace"));
        }

        [Test]
        public void SuiteResultXmlNodeIsInconclusive()
        {
            TNode suiteNode = suiteResult.ToXml(true);

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
            TNode suiteNode = suiteResult.ToXml(true);

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
            Assert.AreEqual(ResultState.ChildFailure, suiteResult.ResultState);
            Assert.AreEqual(TestStatus.Failed, suiteResult.ResultState.Status);
            Assert.AreEqual(TestResult.CHILD_ERRORS_MESSAGE, suiteResult.Message);
            Assert.That(suiteResult.ResultState.Site, Is.EqualTo(FailureSite.Child));
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
            TNode suiteNode = suiteResult.ToXml(true);

            Assert.AreEqual("Failed", suiteNode.Attributes["result"]);
            TNode failureNode = suiteNode.SelectSingleNode("failure");
            Assert.NotNull(failureNode, "No failure element found");

            TNode messageNode = failureNode.SelectSingleNode("message");
            Assert.NotNull(messageNode, "No message element found");
            Assert.AreEqual(TestResult.CHILD_ERRORS_MESSAGE, messageNode.Value);

            TNode stacktraceNode = failureNode.SelectSingleNode("stacktrace");
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
            TNode suiteNode = suiteResult.ToXml(true);

            Assert.AreEqual(4, suiteNode.SelectNodes("test-case").Count);
        }
    }

    public class MinimumDurationResultTests : TestResultTests
    {
        protected override ResultState ResultState
        {
            get { return ResultState.Success; }
        }

        protected override void SimulateTestRun()
        {
            // Change the duration from that provided in the base
            expectedDuration = TestResult.MIN_DURATION - 0.0000001d;
            expectedEnd = expectedStart.AddSeconds(expectedDuration);

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
        public void TestResultHasMinimumDuration()
        {
            Assert.That(testResult.Duration, Is.EqualTo(TestResult.MIN_DURATION));
            Assert.That(suiteResult.Duration, Is.EqualTo(TestResult.MIN_DURATION));
        }
    }
 }
