// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Execution;
using NUnit.Framework.Internal.ExecutionHooks;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.Outcome;

public class AfterSetUpHooksEvaluateTestOutcomeTests
{
    public class AfterSetUpOutcomeLogger : ExecutionHookAttribute, IApplyToContext
    {
        internal static readonly string OutcomeMatched = "Outcome Matched";
        internal static readonly string OutcomeMismatch = "Outcome Mismatch!!!";

        private TestContext.ResultAdapter? _beforeHookTestResult;

        public override void BeforeEverySetUpHook(HookData hookData)
        {
             _beforeHookTestResult = hookData.Context.Result.Clone();
        }

        public override void AfterEverySetUpHook(HookData hookData)
        {
            Assert.That(_beforeHookTestResult, Is.Not.Null, "BeforeEverySetUp was not called before AfterEverySetUp.");

            TestContext.ResultAdapter setUpTestResult
                = hookData.Context.Result.CalculateDeltaWithPrevious(_beforeHookTestResult, hookData.Exception);

            string outcomeMatchStatement = setUpTestResult.Outcome switch
            {
                { Status: TestStatus.Failed } when
                    hookData.Context.Test.FullName.Contains("4Failed") => OutcomeMatched,
                { Status: TestStatus.Passed } when
                    hookData.Context.Test.FullName.Contains("4Passed") => OutcomeMatched,
                { Status: TestStatus.Skipped } when
                    hookData.Context.Test.FullName.Contains("4Ignored") => OutcomeMatched,
                { Status: TestStatus.Inconclusive } when
                    hookData.Context.Test.FullName.Contains("4Inconclusive") => OutcomeMatched,
                { Status: TestStatus.Warning } when
                    hookData.Context.Test.FullName.Contains("4Warning") => OutcomeMatched,
                _ => OutcomeMismatch
            };

            TestLog.LogMessage($"{outcomeMatchStatement}: {hookData.Context.Test.FullName} -> {hookData.Context.Result.Outcome}");
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
        Warning4Warning, // Warn counts on OneTimeSetUp level as passed and on SetUp level as warning!
        None4Passed
    }

    [Explicit($"This test should only be run as part of the {nameof(CheckSetUpOutcomes)} test")]
    [AfterSetUpOutcomeLogger]
    [TestFixtureSource(nameof(GetReasonsToFail))]
    public class TestsUnderTestsWithDifferentSetUpOutcome(FailingReason failingReason)
    {
        private static IEnumerable<TestFixtureData> GetReasonsToFail()
        {
            return Enum.GetValues(typeof(FailingReason)).Cast<FailingReason>().Select(failingReason => new TestFixtureData(failingReason));
        }

        [SetUp]
        public void SetUp() => ExecuteFailingReason();

        private void ExecuteFailingReason()
        {
            switch (failingReason)
            {
                case FailingReason.Assertion4Failed:
                    Assert.Fail("SetUp fails by Assertion_Failed.");
                    break;
                case FailingReason.MultiAssertion4Failed:
                    Assert.Multiple(() =>
                    {
                        Assert.Fail("1st failure");
                        Assert.Fail("2nd failure");
                    });
                    break;
                case FailingReason.Exception4Failed:
                    throw new Exception("SetUp throwing an exception.");
                case FailingReason.None4Passed:
                    break;
                case FailingReason.IgnoreAssertion4Ignored:
                    Assert.Ignore("SetUp ignored by Assert.Ignore.");
                    break;
                case FailingReason.IgnoreException4Ignored:
                    throw new IgnoreException("SetUp ignored by IgnoreException.");
                case FailingReason.Inconclusive4Inconclusive:
                    Assert.Inconclusive("SetUp ignored by Assert.Inconclusive.");
                    break;
                case FailingReason.Warning4Warning:
                    Assert.Warn("SetUp with warning.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [Test]
        public void SomeTest()
        {
            Assert.That(TestContext.CurrentContext.Test.Parent, Is.Not.Null);

            var fixtureName = TestContext.CurrentContext.Test.Parent.FullName;
            if (!(fixtureName.Contains("4Passed") || fixtureName.Contains("4Warning")))
            {
                TestLog.LogMessage(AfterSetUpOutcomeLogger.OutcomeMismatch +
                            $" -> Test HookedMethod of '{fixtureName}' executed unexpected!");
            }
        }
    }

    [Test]
    public void CheckSetUpOutcomes()
    {
        var workItem = TestBuilder.CreateWorkItem(typeof(TestsUnderTestsWithDifferentSetUpOutcome), TestFilter.Explicit);
        workItem.Execute();
        var currentTestLogs = TestLog.Logs(workItem.Test);

        Assert.That(currentTestLogs, Is.Not.Empty);
        Assert.Multiple(() =>
        {
            foreach (var log in currentTestLogs)
            {
                Assert.That(log, Does.Not.Contain(AfterSetUpOutcomeLogger.OutcomeMismatch));
            }

            foreach (var testCase in ((CompositeWorkItem)workItem).Children)
            {
                var resultString = testCase.Result.ResultState.Status.ToString();
                Assert.That(testCase.Name,
                    Does.Contain(resultString == "Skipped" ? "Ignored" : resultString));
            }

            var failingReasons = Enum.GetValues(typeof(FailingReason)).Cast<FailingReason>().ToList();
            Assert.That(workItem.Result.PassCount, Is.EqualTo(failingReasons.Count(reason => reason.ToString().EndsWith("4Passed"))));
            Assert.That(workItem.Result.FailCount, Is.EqualTo(failingReasons.Count(reason => reason.ToString().EndsWith("4Failed"))));
            Assert.That(workItem.Result.SkipCount, Is.EqualTo(failingReasons.Count(reason => reason.ToString().EndsWith("4Ignored"))));
            Assert.That(workItem.Result.TotalCount, Is.EqualTo(failingReasons.Count));
        });
    }
}
