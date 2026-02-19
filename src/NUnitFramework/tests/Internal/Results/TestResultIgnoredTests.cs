// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.Internal.Results
{
    public class TestResultIgnoredWithReasonGivenTests : TestResultIgnoredTests
    {
        public TestResultIgnoredWithReasonGivenTests() : base(NonWhitespaceIgnoreReason, tnode => ReasonNodeExpectedValidation(tnode, NonWhitespaceIgnoreReason))
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
        private readonly string _ignoreReason;
        private readonly Action<TNode> _xmlReasonNodeValidation;

        protected TestResultIgnoredTests(string ignoreReason, Action<TNode> xmlReasonNodeValidation)
        {
            _ignoreReason = ignoreReason;
            _xmlReasonNodeValidation = xmlReasonNodeValidation;
        }

        [SetUp]
        public void SimulateTestRun()
        {
            TestResult.SetResult(ResultState.Ignored, _ignoreReason);
            SuiteResult.AddResult(TestResult);
        }

        [Test]
        public void TestResultIsIgnored()
        {
            Assert.Multiple(() =>
            {
                Assert.That(TestResult.ResultState, Is.EqualTo(ResultState.Ignored));
                Assert.That(TestResult.Message, Is.EqualTo(_ignoreReason));
            });
        }

        [Test]
        public void SuiteResultIsIgnored()
        {
            Assert.Multiple(() =>
            {
                Assert.That(SuiteResult.ResultState, Is.EqualTo(ResultState.ChildIgnored));
                Assert.That(SuiteResult.Message, Is.EqualTo(TestResult.CHILD_IGNORE_MESSAGE));
                Assert.That(SuiteResult.TotalCount, Is.EqualTo(1));
                Assert.That(SuiteResult.InitiatedCount, Is.EqualTo(0));
                Assert.That(SuiteResult.PassCount, Is.EqualTo(0));
                Assert.That(SuiteResult.FailCount, Is.EqualTo(0));
                Assert.That(SuiteResult.WarningCount, Is.EqualTo(0));
                Assert.That(SuiteResult.CompletedCount, Is.EqualTo(0));
                Assert.That(SuiteResult.SkipCount, Is.EqualTo(1));
                Assert.That(SuiteResult.InconclusiveCount, Is.EqualTo(0));
                Assert.That(SuiteResult.AssertCount, Is.EqualTo(0));
            });
        }

        [Test]
        public void TestResultXmlNodeIsIgnored()
        {
            TNode testNode = TestResult.ToXml(true);

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
            TNode suiteNode = SuiteResult.ToXml(true);

            Assert.Multiple(() =>
            {
                Assert.That(suiteNode.Attributes["result"], Is.EqualTo("Skipped"));
                Assert.That(suiteNode.Attributes["label"], Is.EqualTo("Ignored"));
                Assert.That(suiteNode.Attributes["site"], Is.EqualTo("Child"));
                Assert.That(suiteNode.Attributes["total"], Is.EqualTo("1"));
                Assert.That(suiteNode.Attributes["initiated"], Is.EqualTo("0"));
                Assert.That(suiteNode.Attributes["passed"], Is.EqualTo("0"));
                Assert.That(suiteNode.Attributes["failed"], Is.EqualTo("0"));
                Assert.That(suiteNode.Attributes["warnings"], Is.EqualTo("0"));
                Assert.That(suiteNode.Attributes["completed"], Is.EqualTo("0"));
                Assert.That(suiteNode.Attributes["skipped"], Is.EqualTo("1"));
                Assert.That(suiteNode.Attributes["inconclusive"], Is.EqualTo("0"));
                Assert.That(suiteNode.Attributes["asserts"], Is.EqualTo("0"));
            });
        }
    }
}
