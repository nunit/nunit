// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Tests.ExecutionHooks.Outcome;

[TestFixture(typeof(BeforeTestActionWithFailures))]
public class BeforeTestActionHooksOutcomeTests(Type testClassToCheckHookOutcome)
    : HookOutcomeWithTestClassToCheckTestBase(testClassToCheckHookOutcome)
{
    [Test]
    public void BeforeTestActionHook_CheckHookOutcomes()
    {
        CheckHookOutcomes();
    }
}
