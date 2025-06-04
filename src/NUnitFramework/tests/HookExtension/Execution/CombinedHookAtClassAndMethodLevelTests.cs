// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.HookExtensionTests;

namespace NUnit.Framework.Tests.HookExtension.Execution
{
    internal class CombinedHookAtClassAndMethodLevelTests
    {
        [Test]
        public void ExecutionProceedsAfterBothTestHookCompletes()
        {
            TestLog.Clear();

            var workItem = TestBuilder.CreateWorkItem(typeof(EmptyTestFor_ExecutionProceedsAfterBothTestHooksCompleteAtClassAndMethodLevels));
            workItem.Execute();

            Assert.That(TestLog.Logs, Is.EqualTo([
                nameof(ActivateClassLevelAfterTestHooksAttribute),
                nameof(ActivateClassLevelBeforeTestHooksAttribute),
                nameof(EmptyTestFor_ExecutionProceedsAfterBothTestHooksComplete.OneTimeSetUp),
                nameof(ActivateMethodLevelAfterTestHooksAttribute),
                nameof(ActivateMethodLevelBeforeTestHooksAttribute),
                nameof(EmptyTestFor_ExecutionProceedsAfterBothTestHooksComplete.SetUp),
                nameof(EmptyTestFor_ExecutionProceedsAfterBothTestHooksComplete.EmptyTest),
                nameof(EmptyTestFor_ExecutionProceedsAfterBothTestHooksComplete.TearDown),
                nameof(EmptyTestFor_ExecutionProceedsAfterBothTestHooksComplete.OneTimeTearDown)
            ]));
        }
    }
}
