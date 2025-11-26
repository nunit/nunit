// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Tests.ExecutionHooks.Outcome;

[TestFixture(typeof(AfterTestActionWithFailures))]
public class AfterTestActionHooksOutcomeTests(Type testClassToCheckHookOutcome)
    : HookOutcomeWithTestClassToCheckTestBase(testClassToCheckHookOutcome)
{
    [Test]
    public void AfterTestActionHook_CheckHookOutcomes()
    {
        CheckHookOutcomes();
    }
}
