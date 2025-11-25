// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;
using NUnit.Framework.Internal.ExecutionHooks;

namespace NUnit.TestData.ExecutionHooks
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public sealed class TestHookOutcomeLoggerHookAttribute : OutcomeLoggerBaseAttribute
    {
        public override void BeforeTestHook(HookData hookData) => BeforeHook(hookData);

        public override void AfterTestHook(HookData hookData) => AfterHook(hookData);
    }

    [TestHookOutcomeLoggerHook]
    [TestFixtureSource(typeof(FailingReasonExecutor), nameof(FailingReasonExecutor.GetReasonsToFail))]
    public class TestHookOutcomeFixture(FailingReason failingReason)
    {
        [Test]
        public void SomeTest() => FailingReasonExecutor.ExecuteFailingReason(failingReason);
    }
}
