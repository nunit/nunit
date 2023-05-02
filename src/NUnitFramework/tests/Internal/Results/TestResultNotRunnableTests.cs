// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Results
{
    public class TestResultNotRunnableTests : TestResultTests
    {
        [SetUp]
        public void SimulateTestRun()
        {
            _testResult.SetResult(ResultState.NotRunnable, "bad test");
            _suiteResult.AddResult(_testResult);
        }

        [Test]
        public void TestResultIsNotRunnable()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_testResult.ResultState, Is.EqualTo(ResultState.NotRunnable));
                Assert.That(_testResult.Message, Is.EqualTo("bad test"));
            });
        }

        [Test]
        public void SuiteResultIsFailure()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_suiteResult.ResultState, Is.EqualTo(ResultState.ChildFailure));
                Assert.That(_suiteResult.Message, Is.EqualTo(TestResult.CHILD_ERRORS_MESSAGE));
                Assert.That(_suiteResult.ResultState.Site, Is.EqualTo(FailureSite.Child));
                Assert.That(_suiteResult.TotalCount, Is.EqualTo(1));
                Assert.That(_suiteResult.PassCount, Is.EqualTo(0));
                Assert.That(_suiteResult.FailCount, Is.EqualTo(1));
                Assert.That(_suiteResult.WarningCount, Is.EqualTo(0));
                Assert.That(_suiteResult.SkipCount, Is.EqualTo(0));
                Assert.That(_suiteResult.InconclusiveCount, Is.EqualTo(0));
                Assert.That(_suiteResult.AssertCount, Is.EqualTo(0));
            });
        }

        [Test]
        public void TestResultXmlNodeIsNotRunnable()
        {
            TNode testNode = _testResult.ToXml(true);

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
            TNode suiteNode = _suiteResult.ToXml(true);

            Assert.Multiple(() =>
            {
                Assert.That(suiteNode.Attributes["result"], Is.EqualTo("Failed"));
                Assert.That(suiteNode.Attributes["label"], Is.EqualTo(null));
                Assert.That(suiteNode.Attributes["site"], Is.EqualTo("Child"));
                Assert.That(suiteNode.Attributes["passed"], Is.EqualTo("0"));
                Assert.That(suiteNode.Attributes["failed"], Is.EqualTo("1"));
                Assert.That(suiteNode.Attributes["warnings"], Is.EqualTo("0"));
                Assert.That(suiteNode.Attributes["skipped"], Is.EqualTo("0"));
                Assert.That(suiteNode.Attributes["inconclusive"], Is.EqualTo("0"));
                Assert.That(suiteNode.Attributes["asserts"], Is.EqualTo("0"));
            });
        }
    }
}
