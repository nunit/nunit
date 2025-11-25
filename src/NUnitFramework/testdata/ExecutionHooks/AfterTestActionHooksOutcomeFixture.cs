// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;
using NUnit.Framework.Internal.ExecutionHooks;

namespace NUnit.TestData.ExecutionHooks
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public sealed class AfterTestActionHooksOutcomeLoggerHookAttribute : OutcomeLoggerBaseAttribute
    {
        public override void BeforeTestActionAfterTestHook(HookData hookData) => BeforeHook(hookData);

        public override void AfterTestActionAfterTestHook(HookData hookData) => AfterHook(hookData);
    }

    [AfterTestActionHooksOutcomeLoggerHook]
    [LogTestAction]
    [TestFixtureSource(typeof(FailingReasonExecutor), nameof(FailingReasonExecutor.GetReasonsToFail))]
    public class AfterTestActionHooksOutcomeFixture(FailingReason failingReason)
    {
        [Test]
        public void SomeTest() => FailingReasonExecutor.ExecuteFailingReason(failingReason);
    }
}
