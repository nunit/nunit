// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;

namespace NUnit.TestData.ExecutionHooks
{
    public class ExecutionProceedsOnlyAfterAllBeforeTestHooksExecuteFixture
    {
        [Test]
        [ActivateBeforeTestHook]
        [ActivateBeforeTestHook]
        [ActivateBeforeTestHook]
        [ActivateLongRunningBeforeTestHook]
        public void SomeTest() => TestLog.LogCurrentMethod();
    }
}
