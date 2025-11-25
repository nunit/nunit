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

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public sealed class TearDownHookOutcomeLoggerHookAttribute : OutcomeLoggerBaseAttribute
    {
        public override void BeforeEveryTearDownHook(HookData hookData) => BeforeHook(hookData);

        public override void AfterEveryTearDownHook(HookData hookData) => AfterHook(hookData);
    }

    [SetUpHookOutcomeLoggerHook]
    [TestFixtureSource(typeof(HookOutcomeTestsBase), nameof(HookOutcomeTestsBase.GetReasonsToFail))]
    public class SetUpHookOutcomeFixture(HookOutcomeTestsBase.FailingReason failingReason)
    {
        [SetUp]
        public void SetUp() => HookOutcomeTestsBase.ExecuteFailingReason(failingReason);

        [Test]
        public void SomeTest()
        {
        }
    }

    [TearDownHookOutcomeLoggerHook]
    [TestFixtureSource(typeof(HookOutcomeTestsBase), nameof(HookOutcomeTestsBase.GetReasonsToFail))]
    public class TearDownHookOutcomeFixture(HookOutcomeTestsBase.FailingReason failingReason)
    {
        [TearDown]
        public void TearDown() => HookOutcomeTestsBase.ExecuteFailingReason(failingReason);

        [Test]
        public void SomeTest()
        {
        }
    }
}
