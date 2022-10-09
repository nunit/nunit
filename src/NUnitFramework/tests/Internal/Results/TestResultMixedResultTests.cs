// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
            Assert.AreEqual(ResultState.ChildFailure, _suiteResult.ResultState);
            Assert.AreEqual(TestStatus.Failed, _suiteResult.ResultState.Status);
            Assert.AreEqual(TestResult.CHILD_ERRORS_MESSAGE, _suiteResult.Message);
            Assert.That(_suiteResult.ResultState.Site, Is.EqualTo(FailureSite.Child));
            Assert.Null(_suiteResult.StackTrace, "There should be no stacktrace");
            Assert.AreEqual(5, _suiteResult.TotalCount);
            Assert.AreEqual(2, _suiteResult.PassCount);
            Assert.AreEqual(1, _suiteResult.FailCount);
            Assert.AreEqual(1, _suiteResult.WarningCount);
            Assert.AreEqual(0, _suiteResult.SkipCount);
            Assert.AreEqual(1, _suiteResult.InconclusiveCount);
            Assert.AreEqual(6, _suiteResult.AssertCount);
        }

        [Test]
        public void SuiteResultXmlNodeIsFailure()
        {
            TNode suiteNode = _suiteResult.ToXml(true);

            Assert.AreEqual("Failed", suiteNode.Attributes["result"]);
            TNode failureNode = suiteNode.SelectSingleNode("failure");
            Assert.NotNull(failureNode, "No failure element found");

            TNode messageNode = failureNode.SelectSingleNode("message");
            Assert.NotNull(messageNode, "No message element found");
            Assert.AreEqual(TestResult.CHILD_ERRORS_MESSAGE, messageNode.Value);

            TNode stacktraceNode = failureNode.SelectSingleNode("stacktrace");
            Assert.Null(stacktraceNode, "There should be no stacktrace");
            Assert.AreEqual("5", suiteNode.Attributes["total"]);
            Assert.AreEqual("2", suiteNode.Attributes["passed"]);
            Assert.AreEqual("1", suiteNode.Attributes["failed"]);
            Assert.AreEqual("1", suiteNode.Attributes["warnings"]);
            Assert.AreEqual("0", suiteNode.Attributes["skipped"]);
            Assert.AreEqual("1", suiteNode.Attributes["inconclusive"]);
            Assert.AreEqual("6", suiteNode.Attributes["asserts"]);
        }
    }
}
