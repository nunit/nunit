// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal.ExecutionHooks;

namespace NUnit.Framework.Tests.ExecutionHooks.Outcome;

[TestFixture(typeof(TestWithFailuresInOneTimeTearDown))]
public class OneTimeTearDownHookOutcomeTests(Type testClassToCheckHookOutcome) : HookOutcomeTestsBase(testClassToCheckHookOutcome)
{
    private class OneTimeTearDownOutcomeLoggerHook : OutcomeLoggerBaseAttribute
    {
        public override void BeforeEveryTearDownHook(HookData hookData) => BeforeHook(hookData);

        public override void AfterEveryTearDownHook(HookData hookData) => AfterHook(hookData);
    }

    [OneTimeTearDownOutcomeLoggerHook]
    [TestFixtureSource(typeof(HookOutcomeTestsBase), nameof(GetReasonsToFail))]
    [Explicit($"This test should only be run as part of the {nameof(CheckHookOutcomes)} test")]
    public class TestWithFailuresInOneTimeTearDown(FailingReason failingReason)
    {
        [Test]
        public void SomeTest()
        {
        }

        [OneTimeTearDown]
        public void OneTimeTearDownWithFailure() => ExecuteFailingReason(failingReason);
    }
}
