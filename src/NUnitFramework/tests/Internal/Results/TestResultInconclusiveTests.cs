// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;
using System;

namespace NUnit.Framework.Internal.Results
{
    public class TestResultInconclusiveWithReasonGivenTests : TestResultInconclusiveTests
    {
        public TestResultInconclusiveWithReasonGivenTests() : base(NonWhitespaceIgnoreReason, node => ReasonNodeExpectedValidation(node, NonWhitespaceIgnoreReason))
        {
        }
    }

    public class TestResultInconclusiveWithNullReasonGivenTests : TestResultInconclusiveTests
    {
        public TestResultInconclusiveWithNullReasonGivenTests() : base(null, NoReasonNodeExpectedValidation)
        {
        }
    }

    public class TestResultInconclusiveWithEmptyReasonGivenTests : TestResultInconclusiveTests
    {
        public TestResultInconclusiveWithEmptyReasonGivenTests() : base(string.Empty, NoReasonNodeExpectedValidation)
        {
        }
    }

    public class TestResultInconclusiveWithWhitespaceReasonGivenTests : TestResultInconclusiveTests
    {
        public TestResultInconclusiveWithWhitespaceReasonGivenTests() : base(" ", NoReasonNodeExpectedValidation)
        {
        }
    }

    public abstract class TestResultInconclusiveTests : TestResultTests
    {
        protected string _inconclusiveReason;
        private readonly Action<TNode> _xmlReasonNodeValidation;

        protected TestResultInconclusiveTests(string ignoreReason, Action<TNode> xmlReasonNodeValidation)
        {
            _inconclusiveReason = ignoreReason;
            _xmlReasonNodeValidation = xmlReasonNodeValidation;
        }

        [SetUp]
        public void SimulateTestRun()
        {
            _testResult.SetResult(ResultState.Inconclusive, _inconclusiveReason);
            _suiteResult.AddResult(_testResult);
        }

        [Test]
        public void TestResultIsInconclusive()
        {
            Assert.AreEqual(ResultState.Inconclusive, _testResult.ResultState);
            Assert.AreEqual(_inconclusiveReason, _testResult.Message);
        }

        [Test]
        public void SuiteResultIsInconclusive()
        {
            Assert.AreEqual(ResultState.Inconclusive, _suiteResult.ResultState);
            Assert.Null(_suiteResult.Message);
            Assert.AreEqual(1, _suiteResult.TotalCount);
            Assert.AreEqual(0, _suiteResult.PassCount);
            Assert.AreEqual(0, _suiteResult.FailCount);
            Assert.AreEqual(0, _suiteResult.WarningCount);
            Assert.AreEqual(0, _suiteResult.SkipCount);
            Assert.AreEqual(1, _suiteResult.InconclusiveCount);
            Assert.AreEqual(0, _suiteResult.AssertCount);
        }

        [Test]
        public void TestResultXmlNodeIsInconclusive()
        {
            TNode testNode = _testResult.ToXml(true);

            Assert.AreEqual("Inconclusive", testNode.Attributes["result"]);
            Assert.IsNull(testNode.Attributes["label"]);
            Assert.IsNull(testNode.Attributes["site"]);

            _xmlReasonNodeValidation(testNode);
        }

        [Test]
        public void SuiteResultXmlNodeIsInconclusive()
        {
            TNode suiteNode = _suiteResult.ToXml(true);

            Assert.AreEqual("Inconclusive", suiteNode.Attributes["result"]);
            Assert.IsNull(suiteNode.Attributes["label"]);
            Assert.AreEqual("0", suiteNode.Attributes["passed"]);
            Assert.AreEqual("0", suiteNode.Attributes["failed"]);
            Assert.AreEqual("0", suiteNode.Attributes["warnings"]);
            Assert.AreEqual("0", suiteNode.Attributes["skipped"]);
            Assert.AreEqual("1", suiteNode.Attributes["inconclusive"]);
            Assert.AreEqual("0", suiteNode.Attributes["asserts"]);
        }
    }
}
