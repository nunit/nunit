// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.ExecutionHookTests;

namespace NUnit.Framework.Tests.ExecutionHooks.ExecutionSequence;

public class ExecutionProceedsOnlyAfterAllBeforeTestHooksExecute
{
    [Explicit($"This test should only be run as part of the {nameof(CheckThatLongRunningBeforeTestHooksCompleteBeforeTest)} test")]
    private sealed class TestUnderTest
    {
        [Test]
        [ActivateBeforeTestHooks]
        [ActivateBeforeTestHooks]
        [ActivateBeforeTestHooks]
        [ActivateLongRunningBeforeTestHooks]
        public void SomeTest()
        {
            TestLog.LogCurrentMethod();
        }
    }

    [Test]
    public void CheckThatLongRunningBeforeTestHooksCompleteBeforeTest()
    {
        TestLog.Clear();

        var workItem = TestBuilder.CreateWorkItem(typeof(TestUnderTest), TestFilter.Explicit);
        workItem.Execute();

        Assert.That(TestLog.Logs, Is.EqualTo([
            HookIdentifiers.BeforeTestHook,
            HookIdentifiers.BeforeTestHook,
            HookIdentifiers.BeforeTestHook,
            HookIdentifiers.BeforeTestHook,
            nameof(TestUnderTest.SomeTest)
        ]));
    }
}
