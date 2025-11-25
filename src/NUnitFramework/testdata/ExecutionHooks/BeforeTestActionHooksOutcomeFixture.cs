// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;
using NUnit.Framework.Internal.ExecutionHooks;

namespace NUnit.TestData.ExecutionHooks
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public sealed class BeforeTestActionHooksOutcomeLoggerHookAttribute : OutcomeLoggerBaseAttribute
    {
        public override void BeforeTestActionBeforeTestHook(HookData hookData) => BeforeHook(hookData);

        public override void AfterTestActionBeforeTestHook(HookData hookData) => AfterHook(hookData);
    }

    [BeforeTestActionHooksOutcomeLoggerHook]
    [LogTestAction]
    [TestFixtureSource(typeof(FailingReasonExecutor), nameof(FailingReasonExecutor.GetReasonsToFail))]
    public class BeforeTestActionHooksOutcomeFixture(FailingReason failingReason)
    {
        [Test]
        public void SomeTest() => FailingReasonExecutor.ExecuteFailingReason(failingReason);
    }
}
