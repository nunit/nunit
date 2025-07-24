// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;
using TestResult = NUnit.Framework.Internal.TestResult;

namespace NUnit.Framework.Tests.ExecutionHooks.Outcome;

public class AfterTearDownHooksEvaluateTestOutcomeTests
{
    public class AfterTearDownOutcomeLogger : NUnitAttribute, IApplyToContext
    {
        internal static readonly string OutcomeMatched = "Outcome Matched";
        internal static readonly string OutcomeMismatch = "Outcome Mismatch!!!";

        public void ApplyToContext(TestExecutionContext context)
        {
            TestResult? beforeHookTestResult = null;
            context.ExecutionHooks.BeforeEveryTearDown.AddHandler((hookData) =>
            {
                beforeHookTestResult = hookData.Context.CurrentResult.Clone();
            });

            context.ExecutionHooks.AfterEveryTearDown.AddHandler((hookData) =>
            {
                Assert.That(beforeHookTestResult, Is.Not.Null, "BeforeEveryTearDown was not called before AfterEveryTearDown.");

                TestResult tearDownTestResult
                    = hookData.Context.CurrentResult.CalculateDeltaWithPrevious(beforeHookTestResult, hookData.ExceptionContext);

                string outcomeMatchStatement = tearDownTestResult.ResultState switch
                {
                    { Status: TestStatus.Failed } when hookData.Context.CurrentTest.FullName.Contains("4Failed") => OutcomeMatched,
                    { Status: TestStatus.Passed } when hookData.Context.CurrentTest.FullName.Contains("4Passed") => OutcomeMatched,
                    { Status: TestStatus.Skipped } when hookData.Context.CurrentTest.FullName.Contains("4Ignored") => OutcomeMatched,
                    { Status: TestStatus.Inconclusive } when hookData.Context.CurrentTest.FullName.Contains("4Inconclusive") => OutcomeMatched,
                    { Status: TestStatus.Warning } when hookData.Context.CurrentTest.FullName.Contains("4Warning") => OutcomeMatched,
                    _ => OutcomeMismatch
                };

                TestLog.LogMessage($"{outcomeMatchStatement}: {hookData.Context.CurrentTest.FullName} -> {hookData.Context.CurrentResult.ResultState}");
            });
        }
    }

    public enum FailingReason
    {
        Assertion4Failed,
        MultiAssertion4Failed,
        Exception4Failed,
        IgnoreAssertion4Ignored,
        IgnoreException4Ignored,
        Inconclusive4Inconclusive,
        Warning4Warning, // Warn counts on OneTimeTearDown level as passed and on TearDown level as warning!
        None4Passed
    }

    [Explicit($"This test should only be run as part of the {nameof(CheckTearDownOutcomes)} test")]
    [AfterTearDownOutcomeLogger]
    [TestFixtureSource(nameof(GetReasonsToFail))]
    public class TestsUnderTestsWithDifferentTearDownOutcome(FailingReason failingReason)
    {
        private static IEnumerable<TestFixtureData> GetReasonsToFail()
        {
            return Enum.GetValues(typeof(FailingReason)).Cast<FailingReason>().Select(failingReason => new TestFixtureData(failingReason));
        }

        [TearDown]
        public void TearDown() => ExecuteFailingReason();

        private void ExecuteFailingReason()
        {
            switch (failingReason)
            {
                case FailingReason.Assertion4Failed:
                    Assert.Fail("TearDown fails by Assertion_Failed.");
                    break;
                case FailingReason.MultiAssertion4Failed:
                    Assert.Multiple(() =>
                    {
                        Assert.Fail("1st failure");
                        Assert.Fail("2nd failure");
                    });
                    break;
                case FailingReason.Exception4Failed:
                    throw new Exception("TearDown throwing an exception.");
                case FailingReason.None4Passed:
                    break;
                case FailingReason.IgnoreAssertion4Ignored:
                    Assert.Ignore("TearDown ignored by Assert.Ignore.");
                    break;
                case FailingReason.IgnoreException4Ignored:
                    throw new IgnoreException("TearDown ignored by IgnoreException.");
                case FailingReason.Inconclusive4Inconclusive:
                    Assert.Inconclusive("TearDown ignored by Assert.Inconclusive.");
                    break;
                case FailingReason.Warning4Warning:
                    Assert.Warn("TearDown with warning.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [Test]
        public void SomeTest()
        {
        }
    }

    [Test]
    public void CheckTearDownOutcomes()
    {
        var workItem = TestBuilder.CreateWorkItem(typeof(TestsUnderTestsWithDifferentTearDownOutcome), TestFilter.Explicit);
        workItem.Execute();
        var currentTestLogs = TestLog.Logs(workItem.Test);

        Assert.That(currentTestLogs, Is.Not.Empty);
        Assert.Multiple(() =>
        {
            foreach (var log in currentTestLogs)
            {
                Assert.That(log, Does.Not.Contain(AfterTearDownOutcomeLogger.OutcomeMismatch));
            }

            var failingReasons = Enum.GetValues(typeof(AfterOneTimeSetUpHooksEvaluateTestOutcomeTests.FailingReason)).Cast<AfterOneTimeSetUpHooksEvaluateTestOutcomeTests.FailingReason>().ToList();
            Assert.That(workItem.Result.PassCount, Is.EqualTo(failingReasons.Count(reason => reason.ToString().EndsWith("4Passed"))));
            Assert.That(workItem.Result.FailCount, Is.EqualTo(failingReasons.Count(reason => reason.ToString().EndsWith("4Failed"))));
            Assert.That(workItem.Result.SkipCount, Is.EqualTo(failingReasons.Count(reason => reason.ToString().EndsWith("4Ignored"))));
            Assert.That(workItem.Result.InconclusiveCount, Is.EqualTo(failingReasons.Count(reason => reason.ToString().EndsWith("4Inconclusive"))));
            Assert.That(workItem.Result.TotalCount, Is.EqualTo(failingReasons.Count));
        });
    }
}
