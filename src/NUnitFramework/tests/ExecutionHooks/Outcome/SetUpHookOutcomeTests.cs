// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal.ExecutionHooks;

namespace NUnit.Framework.Tests.ExecutionHooks.Outcome;

[TestFixture(typeof(TestWithFailuresInSetUp))]
public class SetUpHookOutcomeTests(Type testClassToCheckHookOutcome) : HookOutcomeTestsBase(testClassToCheckHookOutcome)
{
    private class SetUpOutcomeLoggerHook : OutcomeLoggerBaseAttribute
    {
        public override void BeforeEverySetUpHook(HookData hookData) => BeforeHook(hookData);

        public override void AfterEverySetUpHook(HookData hookData) => AfterHook(hookData);
    }

    [SetUpOutcomeLoggerHook]
    [TestFixtureSource(typeof(HookOutcomeTestsBase), nameof(GetReasonsToFail))]
    [Explicit($"This test should only be run as part of the {nameof(CheckHookOutcomes)} test")]
    public class TestWithFailuresInSetUp(FailingReason failingReason)
    {
        [SetUp]
        public void SetUpWithFailure() => ExecuteFailingReason(failingReason);

        [Test]
        public void SomeTest()
        {
        }
    }
}
