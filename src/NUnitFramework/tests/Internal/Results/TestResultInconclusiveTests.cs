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
        private readonly string? _inconclusiveReason;
        private readonly Action<TNode> _xmlReasonNodeValidation;

        protected TestResultInconclusiveTests(string? ignoreReason, Action<TNode> xmlReasonNodeValidation)
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
            Assert.Multiple(() =>
            {
                Assert.That(_testResult.ResultState, Is.EqualTo(ResultState.Inconclusive));
                Assert.That(_testResult.Message, Is.EqualTo(_inconclusiveReason));
            });
        }

        [Test]
        public void SuiteResultIsInconclusive()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_suiteResult.ResultState, Is.EqualTo(ResultState.Inconclusive));
                Assert.That(_suiteResult.Message, Is.Null);
                Assert.That(_suiteResult.TotalCount, Is.EqualTo(1));
                Assert.That(_suiteResult.PassCount, Is.EqualTo(0));
                Assert.That(_suiteResult.FailCount, Is.EqualTo(0));
                Assert.That(_suiteResult.WarningCount, Is.EqualTo(0));
                Assert.That(_suiteResult.SkipCount, Is.EqualTo(0));
                Assert.That(_suiteResult.InconclusiveCount, Is.EqualTo(1));
                Assert.That(_suiteResult.AssertCount, Is.EqualTo(0));
            });
        }

        [Test]
        public void TestResultXmlNodeIsInconclusive()
        {
            TNode testNode = _testResult.ToXml(true);

            Assert.Multiple(() =>
            {
                Assert.That(testNode.Attributes["result"], Is.EqualTo("Inconclusive"));
                Assert.That(testNode.Attributes["label"], Is.Null);
                Assert.That(testNode.Attributes["site"], Is.Null);
            });
            _xmlReasonNodeValidation(testNode);
        }

        [Test]
        public void SuiteResultXmlNodeIsInconclusive()
        {
            TNode suiteNode = _suiteResult.ToXml(true);

            Assert.Multiple(() =>
            {
                Assert.That(suiteNode.Attributes["result"], Is.EqualTo("Inconclusive"));
                Assert.That(suiteNode.Attributes["label"], Is.Null);
                Assert.That(suiteNode.Attributes["passed"], Is.EqualTo("0"));
                Assert.That(suiteNode.Attributes["failed"], Is.EqualTo("0"));
                Assert.That(suiteNode.Attributes["warnings"], Is.EqualTo("0"));
                Assert.That(suiteNode.Attributes["skipped"], Is.EqualTo("0"));
                Assert.That(suiteNode.Attributes["inconclusive"], Is.EqualTo("1"));
                Assert.That(suiteNode.Attributes["asserts"], Is.EqualTo("0"));
            });
        }
    }
}
