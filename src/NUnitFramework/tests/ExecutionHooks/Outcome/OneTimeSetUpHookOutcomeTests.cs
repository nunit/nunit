// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal.ExecutionHooks;

namespace NUnit.Framework.Tests.ExecutionHooks.Outcome;

[TestFixture(typeof(TestWithFailuresInOneTimeSetUp))]
public class OneTimeSetUpHookOutcomeTests(Type testClassToCheckHookOutcome) : HookOutcomeTestsBase(testClassToCheckHookOutcome)
{
    private class OneTimeSetUpOutcomeLoggerHook : OutcomeLoggerBaseAttribute
    {
        public override void BeforeEverySetUpHook(HookData hookData) => BeforeHook(hookData);

        public override void AfterEverySetUpHook(HookData hookData) => AfterHook(hookData);
    }

    [OneTimeSetUpOutcomeLoggerHook]
    [TestFixtureSource(typeof(HookOutcomeTestsBase), nameof(GetReasonsToFail))]
    [Explicit($"This test should only be run as part of the {nameof(CheckHookOutcomes)} test")]
    public class TestWithFailuresInOneTimeSetUp(FailingReason failingReason)
    {
        [OneTimeSetUp]
        public void OneTimeSetUpWithFailure() => ExecuteFailingReason(failingReason);

        [Test]
        public void SomeTest()
        {
        }
    }
}
