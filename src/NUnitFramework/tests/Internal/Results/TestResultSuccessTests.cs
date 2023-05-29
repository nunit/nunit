// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;
using System;

namespace NUnit.Framework.Internal.Results
{
    public class TestResultSuccessWithReasonGivenTests : TestResultSuccessTests
    {
        public TestResultSuccessWithReasonGivenTests() : base(TestPassedReason, node => ReasonNodeExpectedValidation(node, TestPassedReason))
        {
        }
    }

    public class TestResultSuccessWithNullReasonGivenTests : TestResultSuccessTests
    {
        public TestResultSuccessWithNullReasonGivenTests() : base(null, NoReasonNodeExpectedValidation)
        {
        }
    }

    public class TestResultSuccessWithEmptyReasonGivenTests : TestResultSuccessTests
    {
        public TestResultSuccessWithEmptyReasonGivenTests() : base(string.Empty, NoReasonNodeExpectedValidation)
        {
        }
    }

    public class TestSuccessInconclusiveWithWhitespaceReasonGivenTests : TestResultSuccessTests
    {
        public TestSuccessInconclusiveWithWhitespaceReasonGivenTests() : base(" ", NoReasonNodeExpectedValidation)
        {
        }
    }

    public abstract class TestResultSuccessTests : TestResultTests
    {
        private readonly string? _successMessage;
        private readonly Action<TNode> _xmlReasonNodeValidation;

        public const string TestPassedReason = "Test passed!";

        protected TestResultSuccessTests(string? ignoreReason, Action<TNode> xmlReasonNodeValidation)
        {
            _successMessage = ignoreReason;
            _xmlReasonNodeValidation = xmlReasonNodeValidation;
        }

        [SetUp]
        public void SimulateTestRun()
        {
            TestResult.SetResult(ResultState.Success, _successMessage);
            TestResult.AssertCount = 2;

            SuiteResult.AddResult(TestResult);
        }

        [Test]
        public void TestResultIsSuccess()
        {
            Assert.Multiple(() =>
            {
                Assert.That(TestResult.ResultState, Is.EqualTo(ResultState.Success));
                Assert.That(TestResult.Message, Is.EqualTo(_successMessage));
            });
        }

        [Test]
        public void SuiteResultIsSuccess()
        {
            Assert.Multiple(() =>
            {
                Assert.That(SuiteResult.ResultState, Is.EqualTo(ResultState.Success));
                Assert.That(SuiteResult.TotalCount, Is.EqualTo(1));
                Assert.That(SuiteResult.PassCount, Is.EqualTo(1));
                Assert.That(SuiteResult.FailCount, Is.EqualTo(0));
                Assert.That(SuiteResult.WarningCount, Is.EqualTo(0));
                Assert.That(SuiteResult.SkipCount, Is.EqualTo(0));
                Assert.That(SuiteResult.InconclusiveCount, Is.EqualTo(0));
                Assert.That(SuiteResult.AssertCount, Is.EqualTo(2));
            });
        }

        [Test]
        public void TestResultXmlNodeIsSuccess()
        {
            TNode testNode = TestResult.ToXml(true);

            Assert.Multiple(() =>
            {
                Assert.That(testNode.Attributes["result"], Is.EqualTo("Passed"));
                Assert.That(testNode.Attributes["label"], Is.EqualTo(null));
                Assert.That(testNode.Attributes["site"], Is.EqualTo(null));
                Assert.That(testNode.Attributes["asserts"], Is.EqualTo("2"));
            });

            _xmlReasonNodeValidation(testNode);
        }

        [Test]
        public void SuiteResultXmlNodeIsSuccess()
        {
            TNode suiteNode = SuiteResult.ToXml(true);

            Assert.Multiple(() =>
            {
                Assert.That(suiteNode.Attributes["result"], Is.EqualTo("Passed"));
                Assert.That(suiteNode.Attributes["label"], Is.EqualTo(null));
                Assert.That(suiteNode.Attributes["site"], Is.EqualTo(null));
                Assert.That(suiteNode.Attributes["total"], Is.EqualTo("1"));
                Assert.That(suiteNode.Attributes["passed"], Is.EqualTo("1"));
                Assert.That(suiteNode.Attributes["failed"], Is.EqualTo("0"));
                Assert.That(suiteNode.Attributes["warnings"], Is.EqualTo("0"));
                Assert.That(suiteNode.Attributes["skipped"], Is.EqualTo("0"));
                Assert.That(suiteNode.Attributes["inconclusive"], Is.EqualTo("0"));
                Assert.That(suiteNode.Attributes["asserts"], Is.EqualTo("2"));
            });
        }
    }
}
