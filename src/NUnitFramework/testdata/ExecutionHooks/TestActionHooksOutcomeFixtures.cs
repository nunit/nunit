// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.ExecutionHooks;

namespace NUnit.TestData.ExecutionHooks
{
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class OutcomeLoggerBaseAttribute : ExecutionHookAttribute
    {
        private TestContext.ResultAdapter? _beforeHookTestResult;
        private static readonly string OutcomeMatched = "Outcome Matched";
        private static readonly string OutcomeMismatch = "Outcome Mismatch!!!";
        private static readonly string OutcomePropertyKey = "ExpectedOutcome";

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

    [System.AttributeUsage(System.AttributeTargets.Class)]
    internal sealed class AfterTestActionHooksOutcomeLoggerHookAttribute : OutcomeLoggerBaseAttribute
    {
        public override void BeforeTestActionAfterTestHook(HookData hookData) => BeforeHook(hookData);

        public override void AfterTestActionAfterTestHook(HookData hookData) => AfterHook(hookData);
    }

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public sealed class BeforeTestActionHooksOutcomeLoggerHookAttribute : OutcomeLoggerBaseAttribute
    {
        public override void BeforeTestActionBeforeTestHook(HookData hookData) => BeforeHook(hookData);

        public override void AfterTestActionBeforeTestHook(HookData hookData) => AfterHook(hookData);
    }

    [AfterTestActionHooksOutcomeLoggerHook]
    [LogTestAction]
    [TestFixtureSource(typeof(FailingReasonExecutor), nameof(FailingReasonExecutor.GetReasonsToFail))]
    public class AfterTestActionHooksOutcomeFixture(FailingReason failingReason)
    {
        [Test]
        public void SomeTest() => FailingReasonExecutor.ExecuteFailingReason(failingReason);
    }

    [BeforeTestActionHooksOutcomeLoggerHook]
    [LogTestAction]
    [TestFixtureSource(typeof(FailingReasonExecutor), nameof(FailingReasonExecutor.GetReasonsToFail))]
    public class BeforeTestActionHooksOutcomeFixture(FailingReason failingReason)
    {
        [Test]
        public void SomeTest() => FailingReasonExecutor.ExecuteFailingReason(failingReason);
    }

    [LogTestAction]
    [TestActionLoggingExecutionHooks]
    public class TestActionHooksFixture
    {
        [Test]
        public void TestUnderTest() => TestLog.LogCurrentMethod();
    }
}
