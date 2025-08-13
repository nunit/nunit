// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.ExecutionHooks.TestAttributes;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.ExecutionSequence;

public class ExecutionProceedsOnlyAfterAllBeforeTestHooksExecute
{
    [Explicit($"This test should only be run as part of the {nameof(CheckThatLongRunningBeforeTestHooksCompleteBeforeTest)} test")]
    private sealed class TestUnderTest
    {
        [Test]
        [ActivateBeforeTestHook]
        [ActivateBeforeTestHook]
        [ActivateBeforeTestHook]
        [ActivateLongRunningBeforeTestHook]
        public void SomeTest() => TestLog.LogCurrentMethod();
    }

    [Test]
    public void CheckThatLongRunningBeforeTestHooksCompleteBeforeTest()
    {
        TestLog.Clear();

        var workItem = TestBuilder.CreateWorkItem(typeof(TestUnderTest), TestFilter.Explicit);
        workItem.Execute();

        Assert.That(TestLog.Logs, Is.EqualTo([
            nameof(ActivateLongRunningBeforeTestHookAttribute),
            nameof(ActivateBeforeTestHookAttribute),
            nameof(ActivateBeforeTestHookAttribute),
            nameof(ActivateBeforeTestHookAttribute),
            nameof(TestUnderTest.SomeTest)
        ]));
    }
}
