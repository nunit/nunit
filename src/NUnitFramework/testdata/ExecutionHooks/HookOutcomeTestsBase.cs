// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.ExecutionHooks;

namespace NUnit.TestData.ExecutionHooks;

public abstract class HookOutcomeTestsBase()
{
    private static readonly string OutcomeMatched = "Outcome Matched";
    private static readonly string OutcomeMismatch = "Outcome Mismatch!!!";
    private static readonly string OutcomePropertyKey = "ExpectedOutcome";
    private static readonly string FailingReasonPropertyKey = "FailingReason";

    public enum FailingReason
    {
        Assertion,
        MultiAssertion,
        Exception,
        IgnoreAssertion,
        IgnoreException,
        Inconclusive,
        Warning,
        NoFailing
    }

    protected static IEnumerable<TestFixtureData> GetReasonsToFail()
    {
        return Enum.GetValues(typeof(FailingReason)).Cast<FailingReason>().Select(failingReason => new TestFixtureData(failingReason));
    }

    protected static void ExecuteFailingReason(FailingReason failingReason)
    {
        switch (failingReason)
        {
            case FailingReason.Assertion:
                TestExecutionContext.CurrentContext.CurrentTest.Properties.Set(OutcomePropertyKey, ResultState.Failure);
                Assert.Fail("Hooked method: Assertion.Fail");
                break;
            case FailingReason.MultiAssertion:
                TestExecutionContext.CurrentContext.CurrentTest.Properties.Set(OutcomePropertyKey, ResultState.Failure);
                Assert.Multiple(() =>
                {
                    Assert.Fail("Hooked method: 1st Assert.Fail of Assertion.Multiple");
                    Assert.Fail("Hooked method: 2nd Assert.Fail of Assertion.Multiple");
                });
                break;
            case FailingReason.Exception:
                TestExecutionContext.CurrentContext.CurrentTest.Properties.Set(OutcomePropertyKey, ResultState.Error);
                throw new Exception("Hooked method: throws exception");
            case FailingReason.NoFailing:
                TestExecutionContext.CurrentContext.CurrentTest.Properties.Set(OutcomePropertyKey, ResultState.Success);
                break;
            case FailingReason.IgnoreAssertion:
                TestExecutionContext.CurrentContext.CurrentTest.Properties.Set(OutcomePropertyKey, ResultState.Ignored);
                Assert.Ignore("Hooked method: Assert.Ignore.");
                break;
            case FailingReason.IgnoreException:
                TestExecutionContext.CurrentContext.CurrentTest.Properties.Set(OutcomePropertyKey, ResultState.Ignored);
                throw new IgnoreException("Hooked method: throws IgnoreException");
            case FailingReason.Inconclusive:
                TestExecutionContext.CurrentContext.CurrentTest.Properties.Set(OutcomePropertyKey, ResultState.Inconclusive);
                Assert.Inconclusive("Hooked method: Assert.Inconclusive.");
                break;
            case FailingReason.Warning:
                TestExecutionContext.CurrentContext.CurrentTest.Properties.Set(OutcomePropertyKey, ResultState.Warning);
                Assert.Warn("Hooked method: Assert.Warn");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    protected abstract class OutcomeLoggerBaseAttribute : ExecutionHookAttribute
    {
        private TestContext.ResultAdapter? _beforeHookTestResult;

        protected void BeforeHook(HookData hookData)
        {
            _beforeHookTestResult = hookData.Context.Result.Clone();
        }

        protected void AfterHook(HookData hookData)
        {
            if (_beforeHookTestResult is null)
            {
                return;
            }

            string outcomeMatchStatement;
            var hookedMethodTestResult =
                hookData.Context.Result.CalculateDeltaWithPrevious(_beforeHookTestResult, hookData.Exception);
            var expectedOutcome = hookData.Context.Test.Properties.Get(OutcomePropertyKey);
            if (expectedOutcome is not null && (ResultState)expectedOutcome == hookedMethodTestResult.Outcome)
            {
                outcomeMatchStatement = OutcomeMatched;
            }
            else
            {
                outcomeMatchStatement = OutcomeMismatch;
            }

            TestLog.LogMessage(
                $"{outcomeMatchStatement}: {hookData.Context.Test.FullName} -> {hookData.Context.Result.Outcome}, but expected was {expectedOutcome}");
        }
    }

    private class TestActionWithFailureOnAfterTest(ActionTargets actionTarget) : TestActionAttribute
    {
        public override void AfterTest(ITest test)
        {
            ExecuteFailingReason((HookOutcomeTestsBase.FailingReason)TestContext.CurrentContext.Test.Parent!.Properties.Get(FailingReasonPropertyKey)!);
        }

        public override ActionTargets Targets => actionTarget;
    }

    private class TestActionWithFailureOnBeforeTest(ActionTargets actionTarget) : TestActionAttribute
    {
        public override void BeforeTest(ITest test)
        {
            ExecuteFailingReason((HookOutcomeTestsBase.FailingReason)TestContext.CurrentContext.Test.Parent!.Properties.Get(FailingReasonPropertyKey)!);
        }

        public override ActionTargets Targets => actionTarget;
    }

    [AfterTestActionHooksOutcomeLoggerHook]
    [TestFixtureSource(typeof(HookOutcomeTestsBase), nameof(GetReasonsToFail))]
    public class AfterTestActionWithFailures(FailingReason failingReason)
    {
        [OneTimeSetUp]
        public void StoreFailingReasonInPropertyBag()
        {
            TestExecutionContext.CurrentContext.CurrentTest.Properties.Set(FailingReasonPropertyKey, failingReason);
        }

        [Test]
        [TestActionWithFailureOnAfterTest(ActionTargets.Suite)]
        public void SomeTestWithTestActionOnSuiteLevel()
        {
        }

        [Test]
        [TestActionWithFailureOnAfterTest(ActionTargets.Test)]
        public void SomeTestWithActionOnTestLevel()
        {
        }
    }

    [BeforeTestActionHooksOutcomeLoggerHook]
    [TestFixtureSource(typeof(HookOutcomeTestsBase), nameof(GetReasonsToFail))]
    public class BeforeTestActionWithFailures(FailingReason failingReason)
    {
        [OneTimeSetUp]
        public void StoreFailingReasonInPropertyBag()
        {
            TestExecutionContext.CurrentContext.CurrentTest.Properties.Set(FailingReasonPropertyKey, failingReason);
        }

        [Test]
        [TestActionWithFailureOnBeforeTest(ActionTargets.Suite)]
        public void SomeTestWithTestActionOnSuiteLevel()
        {
        }

        [Test]
        [TestActionWithFailureOnBeforeTest(ActionTargets.Test)]
        public void SomeTestWithActionOnTestLevel()
        {
        }
    }
}
