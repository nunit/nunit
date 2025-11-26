// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.TestData.ExecutionHooks;

namespace NUnit.Framework.Tests.ExecutionHooks.Outcome;

[TestFixture(typeof(OneTimeTearDownHookOutcomeFixture))]
public class OneTimeTearDownHookOutcomeTests(Type testClassToCheckHookOutcome)
    : HookOutcomeWithTestClassToCheckTestBase(testClassToCheckHookOutcome)
{
    [Test]
    public void OneTimeTearDownHook_CheckHookOutcomes()
    {
        CheckHookOutcomes();
    }
}
