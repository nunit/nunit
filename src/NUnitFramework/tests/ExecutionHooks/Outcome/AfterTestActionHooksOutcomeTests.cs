// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.ExecutionHooks;

namespace NUnit.Framework.Tests.ExecutionHooks.Outcome;

[TestFixture(typeof(AfterTestActionWithFailures))]
public class AfterTestActionHooksOutcomeTests(Type testClassToCheckHookOutcome) : HookOutcomeTestsBase(testClassToCheckHookOutcome)
{
    private static readonly string FailingReasonPropertyKey = "FailingReason";

    private class TestActionWithFailureOnAfterTest(ActionTargets actionTarget) : TestActionAttribute
    {
        public override void AfterTest(ITest test)
        {
            ExecuteFailingReason((HookOutcomeTestsBase.FailingReason)TestContext.CurrentContext.Test.Parent!.Properties.Get(FailingReasonPropertyKey)!);
        }

        public override ActionTargets Targets => actionTarget;
    }

    private class TestActionAfterTestOutcomeLoggerHook : OutcomeLoggerBaseAttribute
    {
        public override void BeforeTestActionAfterTestHook(HookData hookData) => BeforeHook(hookData);

        public override void AfterTestActionAfterTestHook(HookData hookData) => AfterHook(hookData);
    }

    [TestActionAfterTestOutcomeLoggerHook]
    [TestFixtureSource(typeof(HookOutcomeTestsBase), nameof(GetReasonsToFail))]
    [Explicit($"This test should only be run as part of the {nameof(CheckHookOutcomes)} test")]
    public class AfterTestActionWithFailures(FailingReason failingReason)
    {
        [OneTimeSetUp]
        public void StoreFailingReasonInPropertyBag()
        {
            TestExecutionContext.CurrentContext.CurrentTest.Properties.Set(FailingReasonPropertyKey, failingReason);
        }

        [Test]
        [TestActionWithFailureOnAfterTest(ActionTargets.Suite)]
        public void SomeTestWithTestActionOnSuiteLevel()
        {
        }

        [Test]
        [TestActionWithFailureOnAfterTest(ActionTargets.Test)]
        public void SomeTestWithActionOnTestLevel()
        {
        }
    }
}
