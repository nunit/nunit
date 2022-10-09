// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal.Results
{
    public class TestResultNotRunnableTests : TestResultTests
    {
        [SetUp]
        public void SimulateTestRun()
        {
            _testResult.SetResult(ResultState.NotRunnable, "bad test");
            _suiteResult.AddResult(_testResult);
        }

        [Test]
        public void TestResultIsNotRunnable()
        {
            Assert.AreEqual(ResultState.NotRunnable, _testResult.ResultState);
            Assert.AreEqual("bad test", _testResult.Message);
        }

        [Test]
        public void SuiteResultIsFailure()
        {
            Assert.AreEqual(ResultState.ChildFailure, _suiteResult.ResultState);
            Assert.AreEqual(TestResult.CHILD_ERRORS_MESSAGE, _suiteResult.Message);
            Assert.That(_suiteResult.ResultState.Site, Is.EqualTo(FailureSite.Child));
            Assert.AreEqual(1, _suiteResult.TotalCount);
            Assert.AreEqual(0, _suiteResult.PassCount);
            Assert.AreEqual(1, _suiteResult.FailCount);
            Assert.AreEqual(0, _suiteResult.WarningCount);
            Assert.AreEqual(0, _suiteResult.SkipCount);
            Assert.AreEqual(0, _suiteResult.InconclusiveCount);
            Assert.AreEqual(0, _suiteResult.AssertCount);
        }

        [Test]
        public void TestResultXmlNodeIsNotRunnable()
        {
            TNode testNode = _testResult.ToXml(true);

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
            TNode suiteNode = _suiteResult.ToXml(true);

            Assert.AreEqual("Failed", suiteNode.Attributes["result"]);
            Assert.AreEqual(null, suiteNode.Attributes["label"]);
            Assert.AreEqual("Child", suiteNode.Attributes["site"]);
            Assert.AreEqual("0", suiteNode.Attributes["passed"]);
            Assert.AreEqual("1", suiteNode.Attributes["failed"]);
            Assert.AreEqual("0", suiteNode.Attributes["warnings"]);
            Assert.AreEqual("0", suiteNode.Attributes["skipped"]);
            Assert.AreEqual("0", suiteNode.Attributes["inconclusive"]);
            Assert.AreEqual("0", suiteNode.Attributes["asserts"]);
        }
    }
}
