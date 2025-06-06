// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.ExecutionHookTests;

namespace NUnit.Framework.Tests.ExecutionHooks.Execution
{
    internal class CombinedHookTests
    {
        [Test]
        public void ExecutionProceedsAfterBothTestHookCompletes()
        {
            TestLog.Clear();

            var workItem = TestBuilder.CreateWorkItem(typeof(EmptyTestFor_ExecutionProceedsAfterBothTestHooksComplete));
            workItem.Execute();

            Assert.That(TestLog.Logs, Is.EqualTo([
                nameof(EmptyTestFor_ExecutionProceedsAfterBothTestHooksComplete.OneTimeSetUp),
                nameof(EmptyTestFor_ExecutionProceedsAfterBothTestHooksComplete.SetUp),
                nameof(ActivateLongRunningBeforeTestHookAttribute),
                nameof(ActivateBeforeTestHookAttribute),
                nameof(EmptyTestFor_ExecutionProceedsAfterBothTestHooksComplete.EmptyTest),
                nameof(ActivateLongRunningAfterTestHookAttribute),
                nameof(ActivateAfterTestHookAttribute),
                nameof(EmptyTestFor_ExecutionProceedsAfterBothTestHooksComplete.TearDown),
                nameof(EmptyTestFor_ExecutionProceedsAfterBothTestHooksComplete.OneTimeTearDown)
            ]));
        }
    }
}
