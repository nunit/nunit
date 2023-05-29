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
            TestResult.SetResult(ResultState.Inconclusive, _inconclusiveReason);
            SuiteResult.AddResult(TestResult);
        }

        [Test]
        public void TestResultIsInconclusive()
        {
            Assert.Multiple(() =>
            {
                Assert.That(TestResult.ResultState, Is.EqualTo(ResultState.Inconclusive));
                Assert.That(TestResult.Message, Is.EqualTo(_inconclusiveReason));
            });
        }

        [Test]
        public void SuiteResultIsInconclusive()
        {
            Assert.Multiple(() =>
            {
                Assert.That(SuiteResult.ResultState, Is.EqualTo(ResultState.Inconclusive));
                Assert.That(SuiteResult.Message, Is.Null);
                Assert.That(SuiteResult.TotalCount, Is.EqualTo(1));
                Assert.That(SuiteResult.PassCount, Is.EqualTo(0));
                Assert.That(SuiteResult.FailCount, Is.EqualTo(0));
                Assert.That(SuiteResult.WarningCount, Is.EqualTo(0));
                Assert.That(SuiteResult.SkipCount, Is.EqualTo(0));
                Assert.That(SuiteResult.InconclusiveCount, Is.EqualTo(1));
                Assert.That(SuiteResult.AssertCount, Is.EqualTo(0));
            });
        }

        [Test]
        public void TestResultXmlNodeIsInconclusive()
        {
            TNode testNode = TestResult.ToXml(true);

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
            TNode suiteNode = SuiteResult.ToXml(true);

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
