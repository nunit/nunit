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
    [TestFixtureSource(typeof(HookOutcomeTestsBase), nameof(HookOutcomeTestsBase.GetReasonsToFail))]
    public class OneTimeSetUpHookOutcomeFixture(HookOutcomeTestsBase.FailingReason failingReason)
    {
        [OneTimeSetUp]
        public void OneTimeSetUp() => HookOutcomeTestsBase.ExecuteFailingReason(failingReason);

        [Test]
        public void SomeTest()
        {
        }
    }

    [OneTimeTearDownHookOutcomeLoggerHook]
    [TestFixtureSource(typeof(HookOutcomeTestsBase), nameof(HookOutcomeTestsBase.GetReasonsToFail))]
    public class OneTimeTearDownHookOutcomeFixture(HookOutcomeTestsBase.FailingReason failingReason)
    {
        [OneTimeTearDown]
        public void OneTimeTearDown() => HookOutcomeTestsBase.ExecuteFailingReason(failingReason);

        [Test]
        public void SomeTest()
        {
        }
    }
}
