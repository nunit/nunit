// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;
using NUnit.Framework.Internal.ExecutionHooks;

namespace NUnit.TestData.ExecutionHooks
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public sealed class SetUpHookOutcomeLoggerHookAttribute : OutcomeLoggerBaseAttribute
    {
        public override void BeforeEverySetUpHook(HookData hookData) => BeforeHook(hookData);

        public override void AfterEverySetUpHook(HookData hookData) => AfterHook(hookData);
    }

    [SetUpHookOutcomeLoggerHook]
    [TestFixtureSource(typeof(FailingReasonExecutor), nameof(FailingReasonExecutor.GetReasonsToFail))]
    public class SetUpHookOutcomeFixture(FailingReason failingReason)
    {
        [SetUp]
        public void SetUp() => FailingReasonExecutor.ExecuteFailingReason(failingReason);

        [Test]
        public void SomeTest()
        {
        }
    }
}
