// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal.Results
{
    public class TestResultWarningTests : TestResultTests
    {
        [SetUp]
        public void SimulateTestRun()
        {
            _testResult.SetResult(ResultState.Warning, "Warning message");
            _suiteResult.AddResult(_testResult);
        }

        [Test]
        public void TestResultIsWarning()
        {
            Assert.AreEqual(ResultState.Warning, _testResult.ResultState);
            Assert.AreEqual("Warning message", _testResult.Message);
        }

        [Test]
        public void SuiteResultIsWarning()
        {
            Assert.AreEqual(ResultState.ChildWarning, _suiteResult.ResultState);
            Assert.AreEqual(TestResult.CHILD_WARNINGS_MESSAGE, _suiteResult.Message);
            Assert.AreEqual(1, _suiteResult.TotalCount);
            Assert.AreEqual(0, _suiteResult.PassCount);
            Assert.AreEqual(0, _suiteResult.FailCount);
            Assert.AreEqual(1, _suiteResult.WarningCount);
            Assert.AreEqual(0, _suiteResult.SkipCount);
            Assert.AreEqual(0, _suiteResult.InconclusiveCount);
            Assert.AreEqual(0, _suiteResult.AssertCount);
        }

        [Test]
        public void TestResultXmlNodeIsWarning()
        {
            TNode testNode = _testResult.ToXml(true);

            Assert.AreEqual("Warning", testNode.Attributes["result"]);
            Assert.AreEqual(null, testNode.Attributes["label"]);
            Assert.AreEqual(null, testNode.Attributes["site"]);

            TNode reason = testNode.SelectSingleNode("reason");
            Assert.NotNull(reason);
            Assert.NotNull(reason.SelectSingleNode("message"));
            Assert.AreEqual("Warning message", reason.SelectSingleNode("message").Value);
            Assert.Null(reason.SelectSingleNode("stack-trace"));
        }

        [Test]
        public void SuiteResultXmlNodeIsWarning()
        {
            TNode suiteNode = _suiteResult.ToXml(true);

            Assert.AreEqual("Warning", suiteNode.Attributes["result"]);
            Assert.AreEqual(null, suiteNode.Attributes["label"]);
            Assert.AreEqual("Child", suiteNode.Attributes["site"]);
            Assert.AreEqual("0", suiteNode.Attributes["passed"]);
            Assert.AreEqual("0", suiteNode.Attributes["failed"]);
            Assert.AreEqual("1", suiteNode.Attributes["warnings"]);
            Assert.AreEqual("0", suiteNode.Attributes["skipped"]);
            Assert.AreEqual("0", suiteNode.Attributes["inconclusive"]);
            Assert.AreEqual("0", suiteNode.Attributes["asserts"]);
        }
    }
}
