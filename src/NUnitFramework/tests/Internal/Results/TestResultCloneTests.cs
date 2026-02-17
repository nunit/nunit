// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.Internal.Results
{
    internal class TestResultCloneTests : TestResultTests
    {
        [Test]
        public void TestCaseResult_Clone_CreatesExactCopy()
        {
            var clone = TestResult.Clone();

            AssertCommonProperties(clone, TestResult);
            Assert.That(clone.HasChildren, Is.False);
        }

        [Test]
        public void TestSuiteResult_Clone_CreatesExactCopy()
        {
            // Add a child result to the suite to test cloning with children
            SuiteResult.AddResult(TestResult);

            var clone = SuiteResult.Clone();

            AssertCommonProperties(clone, SuiteResult);
            Assert.That(clone.HasChildren, Is.True);
            Assert.That(clone.Children, Is.Not.SameAs(SuiteResult.Children));
            Assert.That(clone.Children, Is.EqualTo(SuiteResult.Children));
        }

        private void AssertCommonProperties(TestResult clone, TestResult original)
        {
            Assert.Multiple(() =>
            {
                Assert.That(clone, Is.Not.SameAs(original));
                Assert.That(clone.Test, Is.SameAs(original.Test));
                Assert.That(clone.Name, Is.EqualTo(original.Name));
                Assert.That(clone.FullName, Is.EqualTo(original.FullName));
                Assert.That(clone.Duration, Is.EqualTo(original.Duration));
                Assert.That(clone.StartTime, Is.EqualTo(original.StartTime));
                Assert.That(clone.EndTime, Is.EqualTo(original.EndTime));
                Assert.That(clone.ResultState, Is.EqualTo(original.ResultState));
                Assert.That(clone.Message, Is.EqualTo(original.Message));
                Assert.That(clone.StackTrace, Is.EqualTo(original.StackTrace));

                Assert.That(clone.TotalCount, Is.EqualTo(original.TotalCount));
                Assert.That(clone.InitiatedCount, Is.EqualTo(original.InitiatedCount));
                Assert.That(clone.PassCount, Is.EqualTo(original.PassCount));
                Assert.That(clone.FailCount, Is.EqualTo(original.FailCount));
                Assert.That(clone.WarningCount, Is.EqualTo(original.WarningCount));
                Assert.That(clone.CompletedCount, Is.EqualTo(original.CompletedCount));
                Assert.That(clone.SkipCount, Is.EqualTo(original.SkipCount));
                Assert.That(clone.InconclusiveCount, Is.EqualTo(original.InconclusiveCount));
                Assert.That(clone.AssertCount, Is.EqualTo(original.AssertCount));

                Assert.That(clone.AssertionResults, Is.Not.Null);
                Assert.That(clone.AssertionResults, Is.Not.SameAs(original.AssertionResults));
                Assert.That(clone.AssertionResults, Is.EqualTo(original.AssertionResults));

                Assert.That(clone.TestAttachments, Is.Not.Null);
                Assert.That(clone.TestAttachments, Is.Not.SameAs(original.TestAttachments));
                Assert.That(clone.TestAttachments, Is.EqualTo(original.TestAttachments));
            });
        }
    }
}
