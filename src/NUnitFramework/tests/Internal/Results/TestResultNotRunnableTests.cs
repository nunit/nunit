// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.Internal.Results
{
    public class TestResultNotRunnableTests : TestResultTests
    {
        [SetUp]
        public void SimulateTestRun()
        {
            TestResult.SetResult(ResultState.NotRunnable, "bad test");
            SuiteResult.AddResult(TestResult);
        }

        [Test]
        public void TestResultIsNotRunnable()
        {
            Assert.Multiple(() =>
            {
                Assert.That(TestResult.ResultState, Is.EqualTo(ResultState.NotRunnable));
                Assert.That(TestResult.Message, Is.EqualTo("bad test"));
            });
        }

        [Test]
        public void SuiteResultIsFailure()
        {
            Assert.Multiple(() =>
            {
                Assert.That(SuiteResult.ResultState, Is.EqualTo(ResultState.ChildFailure));
                Assert.That(SuiteResult.Message, Is.EqualTo(TestResult.CHILD_ERRORS_MESSAGE));
                Assert.That(SuiteResult.ResultState.Site, Is.EqualTo(FailureSite.Child));
                Assert.That(SuiteResult.TotalCount, Is.EqualTo(1));
                Assert.That(SuiteResult.InitiatedCount, Is.EqualTo(1));
                Assert.That(SuiteResult.PassCount, Is.EqualTo(0));
                Assert.That(SuiteResult.FailCount, Is.EqualTo(1));
                Assert.That(SuiteResult.WarningCount, Is.EqualTo(0));
                Assert.That(SuiteResult.CompletedCount, Is.EqualTo(1));
                Assert.That(SuiteResult.SkipCount, Is.EqualTo(0));
                Assert.That(SuiteResult.InconclusiveCount, Is.EqualTo(0));
                Assert.That(SuiteResult.AssertCount, Is.EqualTo(0));
            });
        }

        [Test]
        public void TestResultXmlNodeIsNotRunnable()
        {
            TNode testNode = TestResult.ToXml(true);

            Assert.Multiple(() =>
            {
                Assert.That(testNode.Attributes["result"], Is.EqualTo("Failed"));
                Assert.That(testNode.Attributes["label"], Is.EqualTo("Invalid"));
                Assert.That(testNode.Attributes["site"], Is.EqualTo(null));
            });

            TNode? failure = testNode.SelectSingleNode("failure");
            Assert.That(failure, Is.Not.Null);
            Assert.That(failure.SelectSingleNode("message"), Is.Not.Null);
            Assert.That(failure.SelectSingleNode("message").Value, Is.EqualTo("bad test"));
            Assert.That(failure.SelectSingleNode("stack-trace"), Is.Null);
        }

        [Test]
        public void SuiteResultXmlNodeIsFailure()
        {
            TNode suiteNode = SuiteResult.ToXml(true);

            Assert.Multiple(() =>
            {
                Assert.That(suiteNode.Attributes["result"], Is.EqualTo("Failed"));
                Assert.That(suiteNode.Attributes["label"], Is.EqualTo(null));
                Assert.That(suiteNode.Attributes["site"], Is.EqualTo("Child"));
                Assert.That(suiteNode.Attributes["total"], Is.EqualTo("1"));
                Assert.That(suiteNode.Attributes["initiated"], Is.EqualTo("1"));
                Assert.That(suiteNode.Attributes["passed"], Is.EqualTo("0"));
                Assert.That(suiteNode.Attributes["failed"], Is.EqualTo("1"));
                Assert.That(suiteNode.Attributes["warnings"], Is.EqualTo("0"));
                Assert.That(suiteNode.Attributes["completed"], Is.EqualTo("1"));
                Assert.That(suiteNode.Attributes["skipped"], Is.EqualTo("0"));
                Assert.That(suiteNode.Attributes["inconclusive"], Is.EqualTo("0"));
                Assert.That(suiteNode.Attributes["asserts"], Is.EqualTo("0"));
            });
        }
    }
}
