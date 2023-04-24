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
        protected string _successMessage;
        private readonly Action<TNode> _xmlReasonNodeValidation;

        public const string TestPassedReason = "Test passed!";

        protected TestResultSuccessTests(string ignoreReason, Action<TNode> xmlReasonNodeValidation)
        {
            _successMessage = ignoreReason;
            _xmlReasonNodeValidation = xmlReasonNodeValidation;
        }

        [SetUp]
        public void SimulateTestRun()
        {
            _testResult.SetResult(ResultState.Success, _successMessage);
            _testResult.AssertCount = 2;

            _suiteResult.AddResult(_testResult);
        }

        [Test]
        public void TestResultIsSuccess()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_testResult.ResultState, Is.EqualTo(ResultState.Success));
                Assert.That(_testResult.Message, Is.EqualTo(_successMessage));
            });
        }

        [Test]
        public void SuiteResultIsSuccess()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_suiteResult.ResultState, Is.EqualTo(ResultState.Success));
                Assert.That(_suiteResult.TotalCount, Is.EqualTo(1));
                Assert.That(_suiteResult.PassCount, Is.EqualTo(1));
                Assert.That(_suiteResult.FailCount, Is.EqualTo(0));
                Assert.That(_suiteResult.WarningCount, Is.EqualTo(0));
                Assert.That(_suiteResult.SkipCount, Is.EqualTo(0));
                Assert.That(_suiteResult.InconclusiveCount, Is.EqualTo(0));
                Assert.That(_suiteResult.AssertCount, Is.EqualTo(2));
            });
        }

        [Test]
        public void TestResultXmlNodeIsSuccess()
        {
            TNode testNode = _testResult.ToXml(true);

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
            TNode suiteNode = _suiteResult.ToXml(true);

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
