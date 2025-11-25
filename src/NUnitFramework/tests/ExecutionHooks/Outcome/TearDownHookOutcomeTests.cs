// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.TestData.ExecutionHooks;

namespace NUnit.Framework.Tests.ExecutionHooks.Outcome;

[TestFixture(typeof(TearDownHookOutcomeFixture))]
public class TearDownHookOutcomeTests(Type testClassToCheckHookOutcome)
    : HookOutcomeWithTestClassToCheckTestBase(testClassToCheckHookOutcome)
{
    [Test]
    public void TearDownHook_CheckHookOutcomes()
    {
        CheckHookOutcomes();
    }
}
