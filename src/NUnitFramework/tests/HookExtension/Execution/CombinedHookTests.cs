// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.HookExtensionTests;

namespace NUnit.Framework.Tests.HookExtension.Execution
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
                nameof(ActivateLongRunningBeforeTestHook),
                nameof(ActivateBeforeTestHook),
                nameof(EmptyTestFor_ExecutionProceedsAfterBothTestHooksComplete.EmptyTest),
                nameof(ActivateLongRunningAfterTestHook),
                nameof(ActivateAfterTestHook),
                nameof(EmptyTestFor_ExecutionProceedsAfterBothTestHooksComplete.TearDown),
                nameof(EmptyTestFor_ExecutionProceedsAfterBothTestHooksComplete.OneTimeTearDown)
            ]));
        }
    }
}
