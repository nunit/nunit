// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.ExecutionHooks;

namespace NUnit.Framework.Tests.ExecutionHooks.Outcome;

[TestFixture(typeof(BeforeTestActionWithFailures))]
public class BeforeTestActionHooksOutcomeTests(Type testClassToCheckHookOutcome) : HookOutcomeTestsBase(testClassToCheckHookOutcome)
{
    private static readonly string FailingReasonPropertyKey = "FailingReason";

    private class TestActionWithFailureOnBeforeTest(ActionTargets actionTarget) : TestActionAttribute
    {
        public override void BeforeTest(ITest test)
        {
            ExecuteFailingReason((HookOutcomeTestsBase.FailingReason)TestContext.CurrentContext.Test.Parent!.Properties.Get(FailingReasonPropertyKey)!);
        }

        public override ActionTargets Targets => actionTarget;
    }

    private class TestActionBeforeTestOutcomeLoggerHook : OutcomeLoggerBaseAttribute
    {
        public override void BeforeTestActionBeforeTestHook(HookData hookData) => BeforeHook(hookData);

        public override void AfterTestActionBeforeTestHook(HookData hookData) => AfterHook(hookData);
    }

    [TestActionBeforeTestOutcomeLoggerHook]
    [TestFixtureSource(typeof(HookOutcomeTestsBase), nameof(GetReasonsToFail))]
    [Explicit($"This test should only be run as part of the {nameof(CheckHookOutcomes)} test")]
    public class BeforeTestActionWithFailures(FailingReason failingReason)
    {
        [OneTimeSetUp]
        public void StoreFailingReasonInPropertyBag()
        {
            TestExecutionContext.CurrentContext.CurrentTest.Properties.Set(FailingReasonPropertyKey, failingReason);
        }

        [Test]
        [TestActionWithFailureOnBeforeTest(ActionTargets.Suite)]
        public void SomeTestWithTestActionOnSuiteLevel()
        {
        }

        [Test]
        [TestActionWithFailureOnBeforeTest(ActionTargets.Test)]
        public void SomeTestWithActionOnTestLevel()
        {
        }
    }
}
