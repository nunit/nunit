// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal.ExecutionHooks;

namespace NUnit.Framework.Tests.ExecutionHooks.Outcome;

[TestFixture(typeof(TestWithFailuresInTearDown))]
public class TearDownHookOutcomeTests(Type testClassToCheckHookOutcome) : HookOutcomeTestsBase(testClassToCheckHookOutcome)
{
    private class TearDownOutcomeLoggerHook : OutcomeLoggerBaseAttribute
    {
        public override void BeforeEveryTearDownHook(HookData hookData) => BeforeHook(hookData);

        public override void AfterEveryTearDownHook(HookData hookData) => AfterHook(hookData);
    }

    [TearDownOutcomeLoggerHook]
    [TestFixtureSource(typeof(HookOutcomeTestsBase), nameof(GetReasonsToFail))]
    [Explicit($"This test should only be run as part of the {nameof(CheckHookOutcomes)} test")]
    public class TestWithFailuresInTearDown(FailingReason failingReason)
    {
        [Test]
        public void SomeTest()
        {
        }

        [TearDown]
        public void TearDownWithFailure() => ExecuteFailingReason(failingReason);
    }
}
