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
    [TestFixtureSource(typeof(HookOutcomeTestsBase), nameof(HookOutcomeTestsBase.GetReasonsToFail))]
    public class TestHookOutcomeFixture(HookOutcomeTestsBase.FailingReason failingReason)
    {
        [Test]
        public void SomeTest() => HookOutcomeTestsBase.ExecuteFailingReason(failingReason);
    }
}
