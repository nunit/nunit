// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal.ExecutionHooks;

namespace NUnit.Framework.Tests.ExecutionHooks.Outcome;

[TestFixture(typeof(TestWithFailures))]
public class TestHookOutcomeTests(Type testClassToCheckHookOutcome) : HookOutcomeTestsBase(testClassToCheckHookOutcome)
{
    private class TestHookOutcomeLoggerHook : OutcomeLoggerBaseAttribute
    {
        public override void BeforeTestHook(HookData hookData) => BeforeHook(hookData);

        public override void AfterTestHook(HookData hookData) => AfterHook(hookData);
    }

    [TestHookOutcomeLoggerHook]
    [TestFixtureSource(typeof(HookOutcomeTestsBase), nameof(GetReasonsToFail))]
    [Explicit($"This test should only be run as part of the {nameof(CheckHookOutcomes)} test")]
    public class TestWithFailures(FailingReason failingReason)
    {
        [Test]
        public void SomeTest() => ExecuteFailingReason(failingReason);
    }
}
