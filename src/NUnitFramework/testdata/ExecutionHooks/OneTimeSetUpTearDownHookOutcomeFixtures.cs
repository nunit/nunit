// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;
using NUnit.Framework.Internal.ExecutionHooks;

namespace NUnit.TestData.ExecutionHooks
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public sealed class OneTimeSetUpHookOutcomeLoggerHookAttribute : OutcomeLoggerBaseAttribute
    {
        public override void BeforeEverySetUpHook(HookData hookData) => BeforeHook(hookData);

        public override void AfterEverySetUpHook(HookData hookData) => AfterHook(hookData);
    }

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public sealed class OneTimeTearDownHookOutcomeLoggerHookAttribute : OutcomeLoggerBaseAttribute
    {
        public override void BeforeEveryTearDownHook(HookData hookData) => BeforeHook(hookData);

        public override void AfterEveryTearDownHook(HookData hookData) => AfterHook(hookData);
    }

    [OneTimeSetUpHookOutcomeLoggerHook]
    [TestFixtureSource(typeof(FailingReasonExecutor), nameof(FailingReasonExecutor.GetReasonsToFail))]
    public class OneTimeSetUpHookOutcomeFixture(FailingReason failingReason)
    {
        [OneTimeSetUp]
        public void OneTimeSetUp() => FailingReasonExecutor.ExecuteFailingReason(failingReason);

        [Test]
        public void SomeTest()
        {
        }
    }

    [OneTimeTearDownHookOutcomeLoggerHook]
    [TestFixtureSource(typeof(FailingReasonExecutor), nameof(FailingReasonExecutor.GetReasonsToFail))]
    public class OneTimeTearDownHookOutcomeFixture(FailingReason failingReason)
    {
        [OneTimeTearDown]
        public void OneTimeTearDown() => FailingReasonExecutor.ExecuteFailingReason(failingReason);

        [Test]
        public void SomeTest()
        {
        }
    }
}
