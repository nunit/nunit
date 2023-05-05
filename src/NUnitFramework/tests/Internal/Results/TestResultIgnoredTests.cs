// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;
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
        private readonly string? _ignoreReason;
        private readonly Action<TNode> _xmlReasonNodeValidation;

        protected TestResultIgnoredTests(string? ignoreReason, Action<TNode> xmlReasonNodeValidation)
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
            Assert.Multiple(() =>
            {
                Assert.That(_testResult.ResultState, Is.EqualTo(ResultState.Ignored));
                Assert.That(_testResult.Message, Is.EqualTo(_ignoreReason));
            });
        }

        [Test]
        public void SuiteResultIsIgnored()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_suiteResult.ResultState, Is.EqualTo(ResultState.ChildIgnored));
                Assert.That(_suiteResult.Message, Is.EqualTo(TestResult.CHILD_IGNORE_MESSAGE));
                Assert.That(_suiteResult.TotalCount, Is.EqualTo(1));
                Assert.That(_suiteResult.PassCount, Is.EqualTo(0));
                Assert.That(_suiteResult.FailCount, Is.EqualTo(0));
                Assert.That(_suiteResult.WarningCount, Is.EqualTo(0));
                Assert.That(_suiteResult.SkipCount, Is.EqualTo(1));
                Assert.That(_suiteResult.InconclusiveCount, Is.EqualTo(0));
                Assert.That(_suiteResult.AssertCount, Is.EqualTo(0));
            });
        }

        [Test]
        public void TestResultXmlNodeIsIgnored()
        {
            TNode testNode = _testResult.ToXml(true);

            Assert.Multiple(() =>
            {
                Assert.That(testNode.Attributes["result"], Is.EqualTo("Skipped"));
                Assert.That(testNode.Attributes["label"], Is.EqualTo("Ignored"));
                Assert.That(testNode.Attributes["site"], Is.EqualTo(null));
            });
            _xmlReasonNodeValidation(testNode);
        }

        [Test]
        public void SuiteResultXmlNodeIsIgnored()
        {
            TNode suiteNode = _suiteResult.ToXml(true);

            Assert.Multiple(() =>
            {
                Assert.That(suiteNode.Attributes["result"], Is.EqualTo("Skipped"));
                Assert.That(suiteNode.Attributes["label"], Is.EqualTo("Ignored"));
                Assert.That(suiteNode.Attributes["site"], Is.EqualTo("Child"));
                Assert.That(suiteNode.Attributes["passed"], Is.EqualTo("0"));
                Assert.That(suiteNode.Attributes["failed"], Is.EqualTo("0"));
                Assert.That(suiteNode.Attributes["warnings"], Is.EqualTo("0"));
                Assert.That(suiteNode.Attributes["skipped"], Is.EqualTo("1"));
                Assert.That(suiteNode.Attributes["inconclusive"], Is.EqualTo("0"));
                Assert.That(suiteNode.Attributes["asserts"], Is.EqualTo("0"));
            });
        }
    }
}
