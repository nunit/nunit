// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;
using TestResult = NUnit.Framework.Internal.TestResult;

namespace NUnit.Framework.Tests.ExecutionHooks.Outcome;

public class AfterOneTimeOneTimeTearDownHooksEvaluateTestOutcomeTests
{
    public class AfterOneTimeTearDownOutcomeLogger : NUnitAttribute, IApplyToContext
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
                TestResult oneTimeTearDownTestResult
                    = hookData.Context.CurrentResult.CalculateDeltaWithPrevious(beforeHookTestResult, hookData.Exception);

                string outcomeMatchStatement = oneTimeTearDownTestResult.ResultState switch
                {
                    { Status: TestStatus.Failed } when
                        hookData.Context.CurrentTest.FullName.Contains("4Failed") => OutcomeMatched,
                    { Status: TestStatus.Passed } when
                        hookData.Context.CurrentTest.FullName.Contains("4Passed") => OutcomeMatched,
                    { Status: TestStatus.Skipped } when
                        hookData.Context.CurrentTest.FullName.Contains("4Ignored") => OutcomeMatched,
                    { Status: TestStatus.Inconclusive } when
                       hookData.Context.CurrentTest.FullName.Contains("4Inconclusive") => OutcomeMatched,
                    { Status: TestStatus.Warning } when
                        hookData.Context.CurrentTest.FullName.Contains("4Warning") => OutcomeMatched,
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
        Warning4Warning, // Warn counts on OneTimeOneTimeTearDown level as passed and on OneTimeTearDown level as warning!
        None4Passed
    }

    [Explicit($"This test should only be run as part of the {nameof(CheckOneTimeTearDownOutcomes)} test")]
    [AfterOneTimeTearDownOutcomeLogger]
    [TestFixtureSource(nameof(GetReasonsToFail))]
    public class TestsUnderTestsWithDifferentOneTimeTearDownOutcome(FailingReason failingReason)
    {
        private static IEnumerable<TestFixtureData> GetReasonsToFail()
        {
            return Enum.GetValues(typeof(FailingReason)).Cast<FailingReason>().Select(failingReason => new TestFixtureData(failingReason));
        }

        [OneTimeTearDown]
        public void OneTimeTearDown() => ExecuteFailingReason();

        private void ExecuteFailingReason()
        {
            switch (failingReason)
            {
                case FailingReason.Assertion4Failed:
                    Assert.Fail("OneTimeTearDown fails by Assertion_Failed.");
                    break;
                case FailingReason.MultiAssertion4Failed:
                    Assert.Multiple(() =>
                    {
                        Assert.Fail("1st failure");
                        Assert.Fail("2nd failure");
                    });
                    break;
                case FailingReason.Exception4Failed:
                    throw new Exception("OneTimeTearDown throwing an exception.");
                case FailingReason.None4Passed:
                    break;
                case FailingReason.IgnoreAssertion4Ignored:
                    Assert.Ignore("OneTimeTearDown ignored by Assert.Ignore.");
                    break;
                case FailingReason.IgnoreException4Ignored:
                    throw new IgnoreException("OneTimeTearDown ignored by IgnoreException.");
                case FailingReason.Inconclusive4Inconclusive:
                    Assert.Inconclusive("OneTimeTearDown ignored by Assert.Inconclusive.");
                    break;
                case FailingReason.Warning4Warning:
                    Assert.Warn("OneTimeTearDown with warning.");
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
    public void CheckOneTimeTearDownOutcomes()
    {
        var workItem = TestBuilder.CreateWorkItem(typeof(TestsUnderTestsWithDifferentOneTimeTearDownOutcome), TestFilter.Explicit);
        workItem.Execute();
        var currentTestLogs = TestLog.Logs(workItem.Test);

        Assert.That(currentTestLogs, Is.Not.Empty);
        Assert.Multiple(() =>
        {
            foreach (var log in currentTestLogs)
            {
                Assert.That(log, Does.Not.Contain(AfterOneTimeTearDownOutcomeLogger.OutcomeMismatch));
            }

            var numberOfTests = Enum.GetValues(typeof(AfterOneTimeSetUpHooksEvaluateTestOutcomeTests.FailingReason)).Cast<AfterOneTimeSetUpHooksEvaluateTestOutcomeTests.FailingReason>().Count();
            Assert.That(workItem.Result.PassCount, Is.EqualTo(numberOfTests));
            Assert.That(workItem.Result.TotalCount, Is.EqualTo(numberOfTests));
        });
    }
}
