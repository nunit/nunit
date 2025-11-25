// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.ExecutionSequence;

public class ExecutionProceedsOnlyAfterAllBeforeTestHooksExecute
{
    [Test]
    public void CheckThatLongRunningBeforeTestHooksCompleteBeforeTest()
    {
        var workItem = TestBuilder.CreateWorkItem(typeof(TestData.ExecutionHooks.ExecutionProceedsOnlyAfterAllBeforeTestHooksExecuteFixture), TestFilter.Explicit);
        workItem.Execute();
        var currentTestLogs = TestData.ExecutionHooks.TestLog.Logs(workItem.Test);

        Assert.That(currentTestLogs, Is.Not.Empty);
        Assert.That(currentTestLogs, Is.EqualTo([
            nameof(TestData.ExecutionHooks.ActivateLongRunningBeforeTestHookAttribute),
            nameof(TestData.ExecutionHooks.ActivateBeforeTestHookAttribute),
            nameof(TestData.ExecutionHooks.ActivateBeforeTestHookAttribute),
            nameof(TestData.ExecutionHooks.ActivateBeforeTestHookAttribute),
            nameof(TestData.ExecutionHooks.ExecutionProceedsOnlyAfterAllBeforeTestHooksExecuteFixture.SomeTest)
        ]));
    }
}
