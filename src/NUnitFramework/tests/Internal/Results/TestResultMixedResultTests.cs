// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.Internal.Results
{
    public class TestResultMixedResultTests : TestResultTests
    {
        [SetUp]
        public void SimulateTestRun()
        {
            TestResult.SetResult(ResultState.Success);
            TestResult.AssertCount = 2;
            SuiteResult.AddResult(TestResult);

            TestResult.SetResult(ResultState.Failure, "message", "stack trace");
            TestResult.AssertCount = 1;
            SuiteResult.AddResult(TestResult);

            TestResult.SetResult(ResultState.Success);
            TestResult.AssertCount = 3;
            SuiteResult.AddResult(TestResult);

            TestResult.SetResult(ResultState.Inconclusive, "inconclusive reason", "stacktrace");
            TestResult.AssertCount = 0;
            SuiteResult.AddResult(TestResult);

            TestResult.SetResult(ResultState.Warning, "message", "warning");
            TestResult.AssertCount = 0;
            SuiteResult.AddResult(TestResult);
        }

        [Test]
        public void SuiteResultIsFailure()
        {
            Assert.Multiple(() =>
            {
                Assert.That(SuiteResult.ResultState, Is.EqualTo(ResultState.ChildFailure));
                Assert.That(SuiteResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(SuiteResult.Message, Is.EqualTo(TestResult.CHILD_ERRORS_MESSAGE));
                Assert.That(SuiteResult.ResultState.Site, Is.EqualTo(FailureSite.Child));
                Assert.That(SuiteResult.StackTrace, Is.Null, "There should be no stacktrace");
                Assert.That(SuiteResult.TotalCount, Is.EqualTo(5));
                Assert.That(SuiteResult.InitiatedCount, Is.EqualTo(5));
                Assert.That(SuiteResult.PassCount, Is.EqualTo(2));
                Assert.That(SuiteResult.FailCount, Is.EqualTo(1));
                Assert.That(SuiteResult.WarningCount, Is.EqualTo(1));
                Assert.That(SuiteResult.CompletedCount, Is.EqualTo(4));
                Assert.That(SuiteResult.SkipCount, Is.EqualTo(0));
                Assert.That(SuiteResult.InconclusiveCount, Is.EqualTo(1));
                Assert.That(SuiteResult.AssertCount, Is.EqualTo(6));
            });
        }

        [Test]
        public void SuiteResultXmlNodeIsFailure()
        {
            TNode suiteNode = SuiteResult.ToXml(true);

            Assert.That(suiteNode.Attributes["result"], Is.EqualTo("Failed"));
            TNode? failureNode = suiteNode.SelectSingleNode("failure");
            Assert.That(failureNode, Is.Not.Null, "No failure element found");

            TNode? messageNode = failureNode.SelectSingleNode("message");
            Assert.That(messageNode, Is.Not.Null, "No message element found");
            Assert.That(messageNode.Value, Is.EqualTo(TestResult.CHILD_ERRORS_MESSAGE));

            TNode? stacktraceNode = failureNode.SelectSingleNode("stacktrace");
            Assert.That(stacktraceNode, Is.Null, "There should be no stacktrace");

            Assert.Multiple(() =>
            {
                Assert.That(suiteNode.Attributes["total"], Is.EqualTo("5"));
                Assert.That(suiteNode.Attributes["initiated"], Is.EqualTo("5"));
                Assert.That(suiteNode.Attributes["passed"], Is.EqualTo("2"));
                Assert.That(suiteNode.Attributes["failed"], Is.EqualTo("1"));
                Assert.That(suiteNode.Attributes["warnings"], Is.EqualTo("1"));
                Assert.That(suiteNode.Attributes["completed"], Is.EqualTo("4"));
                Assert.That(suiteNode.Attributes["skipped"], Is.EqualTo("0"));
                Assert.That(suiteNode.Attributes["inconclusive"], Is.EqualTo("1"));
                Assert.That(suiteNode.Attributes["asserts"], Is.EqualTo("6"));
            });
        }
    }
}
