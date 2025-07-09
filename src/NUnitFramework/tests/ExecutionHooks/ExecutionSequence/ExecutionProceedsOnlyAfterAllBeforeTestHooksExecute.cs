// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.ExecutionHookTests;

namespace NUnit.Framework.Tests.HookExtension.ExecutionSequence;

public class ExecutionProceedsOnlyAfterAllBeforeTestHooksExecute
{
    public class TestUnderTest
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

        var workItem = TestBuilder.CreateWorkItem(typeof(TestUnderTest));
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
