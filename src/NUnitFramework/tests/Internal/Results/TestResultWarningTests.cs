// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.Internal.Results
{
    public class TestResultWarningTests : TestResultTests
    {
        [SetUp]
        public void SimulateTestRun()
        {
            TestResult.SetResult(ResultState.Warning, "Warning message");
            SuiteResult.AddResult(TestResult);
        }

        [Test]
        public void TestResultIsWarning()
        {
            Assert.Multiple(() =>
            {
                Assert.That(TestResult.ResultState, Is.EqualTo(ResultState.Warning));
                Assert.That(TestResult.Message, Is.EqualTo("Warning message"));
            });
        }

        [Test]
        public void SuiteResultIsWarning()
        {
            Assert.Multiple(() =>
            {
                Assert.That(SuiteResult.ResultState, Is.EqualTo(ResultState.ChildWarning));
                Assert.That(SuiteResult.Message, Is.EqualTo(TestResult.CHILD_WARNINGS_MESSAGE));
                Assert.That(SuiteResult.TotalCount, Is.EqualTo(1));
                Assert.That(SuiteResult.InitiatedCount, Is.EqualTo(1));
                Assert.That(SuiteResult.PassCount, Is.EqualTo(0));
                Assert.That(SuiteResult.FailCount, Is.EqualTo(0));
                Assert.That(SuiteResult.WarningCount, Is.EqualTo(1));
                Assert.That(SuiteResult.CompletedCount, Is.EqualTo(1));
                Assert.That(SuiteResult.SkipCount, Is.EqualTo(0));
                Assert.That(SuiteResult.InconclusiveCount, Is.EqualTo(0));
                Assert.That(SuiteResult.AssertCount, Is.EqualTo(0));
            });
        }

        [Test]
        public void TestResultXmlNodeIsWarning()
        {
            TNode testNode = TestResult.ToXml(true);

            Assert.Multiple(() =>
            {
                Assert.That(testNode.Attributes["result"], Is.EqualTo("Warning"));
                Assert.That(testNode.Attributes["label"], Is.EqualTo(null));
                Assert.That(testNode.Attributes["site"], Is.EqualTo(null));
            });

            TNode? reason = testNode.SelectSingleNode("reason");
            Assert.That(reason, Is.Not.Null);
            Assert.That(reason.SelectSingleNode("message"), Is.Not.Null);
            Assert.That(reason.SelectSingleNode("message").Value, Is.EqualTo("Warning message"));
            Assert.That(reason.SelectSingleNode("stack-trace"), Is.Null);
        }

        [Test]
        public void SuiteResultXmlNodeIsWarning()
        {
            TNode suiteNode = SuiteResult.ToXml(true);

            Assert.Multiple(() =>
            {
                Assert.That(suiteNode.Attributes["result"], Is.EqualTo("Warning"));
                Assert.That(suiteNode.Attributes["label"], Is.EqualTo(null));
                Assert.That(suiteNode.Attributes["site"], Is.EqualTo("Child"));
                Assert.That(suiteNode.Attributes["total"], Is.EqualTo("1"));
                Assert.That(suiteNode.Attributes["initiated"], Is.EqualTo("1"));
                Assert.That(suiteNode.Attributes["passed"], Is.EqualTo("0"));
                Assert.That(suiteNode.Attributes["failed"], Is.EqualTo("0"));
                Assert.That(suiteNode.Attributes["warnings"], Is.EqualTo("1"));
                Assert.That(suiteNode.Attributes["completed"], Is.EqualTo("1"));
                Assert.That(suiteNode.Attributes["skipped"], Is.EqualTo("0"));
                Assert.That(suiteNode.Attributes["inconclusive"], Is.EqualTo("0"));
                Assert.That(suiteNode.Attributes["asserts"], Is.EqualTo("0"));
            });
        }
    }
}
