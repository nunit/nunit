// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Results
{
    public class TestResultMixedResultTests : TestResultTests
    {
        [SetUp]
        public void SimulateTestRun()
        {
            _testResult.SetResult(ResultState.Success);
            _testResult.AssertCount = 2;
            _suiteResult.AddResult(_testResult);

            _testResult.SetResult(ResultState.Failure, "message", "stack trace");
            _testResult.AssertCount = 1;
            _suiteResult.AddResult(_testResult);

            _testResult.SetResult(ResultState.Success);
            _testResult.AssertCount = 3;
            _suiteResult.AddResult(_testResult);

            _testResult.SetResult(ResultState.Inconclusive, "inconclusive reason", "stacktrace");
            _testResult.AssertCount = 0;
            _suiteResult.AddResult(_testResult);

            _testResult.SetResult(ResultState.Warning, "message", "warning");
            _testResult.AssertCount = 0;
            _suiteResult.AddResult(_testResult);
        }

        [Test]
        public void SuiteResultIsFailure()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_suiteResult.ResultState, Is.EqualTo(ResultState.ChildFailure));
                Assert.That(_suiteResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(_suiteResult.Message, Is.EqualTo(TestResult.CHILD_ERRORS_MESSAGE));
                Assert.That(_suiteResult.ResultState.Site, Is.EqualTo(FailureSite.Child));
                Assert.That(_suiteResult.StackTrace, Is.Null, "There should be no stacktrace");
                Assert.That(_suiteResult.TotalCount, Is.EqualTo(5));
                Assert.That(_suiteResult.PassCount, Is.EqualTo(2));
                Assert.That(_suiteResult.FailCount, Is.EqualTo(1));
                Assert.That(_suiteResult.WarningCount, Is.EqualTo(1));
                Assert.That(_suiteResult.SkipCount, Is.EqualTo(0));
                Assert.That(_suiteResult.InconclusiveCount, Is.EqualTo(1));
                Assert.That(_suiteResult.AssertCount, Is.EqualTo(6));
            });
        }

        [Test]
        public void SuiteResultXmlNodeIsFailure()
        {
            TNode suiteNode = _suiteResult.ToXml(true);

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
                Assert.That(suiteNode.Attributes["passed"], Is.EqualTo("2"));
                Assert.That(suiteNode.Attributes["failed"], Is.EqualTo("1"));
                Assert.That(suiteNode.Attributes["warnings"], Is.EqualTo("1"));
                Assert.That(suiteNode.Attributes["skipped"], Is.EqualTo("0"));
                Assert.That(suiteNode.Attributes["inconclusive"], Is.EqualTo("1"));
                Assert.That(suiteNode.Attributes["asserts"], Is.EqualTo("6"));
            });
        }
    }
}
