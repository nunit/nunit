// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.ExecutionHooks;

namespace NUnit.Framework.Tests.ExecutionHooks.ExecutionSequence;

public class ExecutionProceedsOnlyAfterAllBeforeTestHooksExecute
{
    [Test]
    public void CheckThatLongRunningBeforeTestHooksCompleteBeforeTest()
    {
        var workItem = TestBuilder.CreateWorkItem(typeof(ExecutionProceedsOnlyAfterAllBeforeTestHooksExecuteFixture), TestFilter.Explicit);
        workItem.Execute();
        var currentTestLogs = TestLog.Logs(workItem.Test);

        Assert.That(currentTestLogs, Is.Not.Empty);
        Assert.That(currentTestLogs, Is.EqualTo([
            nameof(ActivateLongRunningBeforeTestHookAttribute),
            nameof(ActivateBeforeTestHookAttribute),
            nameof(ActivateBeforeTestHookAttribute),
            nameof(ActivateBeforeTestHookAttribute),
            nameof(ExecutionProceedsOnlyAfterAllBeforeTestHooksExecuteFixture.SomeTest)
        ]));
    }
}
