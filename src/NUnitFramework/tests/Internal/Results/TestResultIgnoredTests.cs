// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Internal.Results
{
    public class TestResultIgnoredWithReasonGivenTests : TestResultIgnoredTests
    {
        public TestResultIgnoredWithReasonGivenTests() : base(NonWhitespaceIgnoreReason, tnode => ReasonNodeExpectedValidation(tnode, NonWhitespaceIgnoreReason))
        {
        }
    }

    public class TestResultIgnoredWithNullReasonGivenTests : TestResultIgnoredTests
    {
        public TestResultIgnoredWithNullReasonGivenTests() : base(null, NoReasonNodeExpectedValidation)
        {
        }
    }

    public class TestResultIgnoredWithEmptyReasonGivenTests : TestResultIgnoredTests
    {
        public TestResultIgnoredWithEmptyReasonGivenTests() : base(string.Empty, NoReasonNodeExpectedValidation)
        {
        }
    }

    public class TestResultIgnoredWithWhitespaceReasonGivenTests : TestResultIgnoredTests
    {
        public TestResultIgnoredWithWhitespaceReasonGivenTests() : base(" ", NoReasonNodeExpectedValidation)
        {
        }
    }

    public abstract class TestResultIgnoredTests : TestResultTests
    {
        protected string _ignoreReason;
        private readonly Action<TNode> _xmlReasonNodeValidation;

        protected TestResultIgnoredTests(string ignoreReason, Action<TNode> xmlReasonNodeValidation)
        {
            _ignoreReason = ignoreReason;
            _xmlReasonNodeValidation = xmlReasonNodeValidation;
        }

        [SetUp]
        public void SimulateTestRun()
        {
            _testResult.SetResult(ResultState.Ignored, _ignoreReason);
            _suiteResult.AddResult(_testResult);
        }

        [Test]
        public void TestResultIsIgnored()
        {
            Assert.AreEqual(ResultState.Ignored, _testResult.ResultState);
            Assert.AreEqual(_ignoreReason, _testResult.Message);
        }

        [Test]
        public void SuiteResultIsIgnored()
        {
            Assert.AreEqual(ResultState.ChildIgnored, _suiteResult.ResultState);
            Assert.AreEqual(TestResult.CHILD_IGNORE_MESSAGE, _suiteResult.Message);
            Assert.AreEqual(1, _suiteResult.TotalCount);
            Assert.AreEqual(0, _suiteResult.PassCount);
            Assert.AreEqual(0, _suiteResult.FailCount);
            Assert.AreEqual(0, _suiteResult.WarningCount);
            Assert.AreEqual(1, _suiteResult.SkipCount);
            Assert.AreEqual(0, _suiteResult.InconclusiveCount);
            Assert.AreEqual(0, _suiteResult.AssertCount);
        }

        [Test]
        public void TestResultXmlNodeIsIgnored()
        {
            TNode testNode = _testResult.ToXml(true);

            Assert.AreEqual("Skipped", testNode.Attributes["result"]);
            Assert.AreEqual("Ignored", testNode.Attributes["label"]);
            Assert.AreEqual(null, testNode.Attributes["site"]);

            _xmlReasonNodeValidation(testNode);
        }

        [Test]
        public void SuiteResultXmlNodeIsIgnored()
        {
            TNode suiteNode = _suiteResult.ToXml(true);

            Assert.AreEqual("Skipped", suiteNode.Attributes["result"]);
            Assert.AreEqual("Ignored", suiteNode.Attributes["label"]);
            Assert.AreEqual("Child", suiteNode.Attributes["site"]);
            Assert.AreEqual("0", suiteNode.Attributes["passed"]);
            Assert.AreEqual("0", suiteNode.Attributes["failed"]);
            Assert.AreEqual("0", suiteNode.Attributes["warnings"]);
            Assert.AreEqual("1", suiteNode.Attributes["skipped"]);
            Assert.AreEqual("0", suiteNode.Attributes["inconclusive"]);
            Assert.AreEqual("0", suiteNode.Attributes["asserts"]);
        }
    }
}
