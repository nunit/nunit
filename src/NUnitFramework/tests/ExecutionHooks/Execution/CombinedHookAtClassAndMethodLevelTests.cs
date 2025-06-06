// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.ExecutionHookTests;

namespace NUnit.Framework.Tests.ExecutionHooks.Execution
{
    internal class CombinedHookAtClassAndMethodLevelTests
    {
        [Test]
        public void ExecutionProceedsAfterBothTestHookCompletes()
        {
            TestLog.Clear();

            const string oneTimeSetUp = nameof(EmptyTestFor_ExecutionProceedsAfterBothTestHooksCompleteAtClassAndMethodLevels.OneTimeSetUp);
            const string setUp = nameof(EmptyTestFor_ExecutionProceedsAfterBothTestHooksCompleteAtClassAndMethodLevels.SetUp);
            const string testWithHooks = nameof(EmptyTestFor_ExecutionProceedsAfterBothTestHooksCompleteAtClassAndMethodLevels.EmptyTestWithHooks);
            const string testWithoutHooks = nameof(EmptyTestFor_ExecutionProceedsAfterBothTestHooksCompleteAtClassAndMethodLevels.EmptyTestWithoutHooks1);
            const string tearDown = nameof(EmptyTestFor_ExecutionProceedsAfterBothTestHooksCompleteAtClassAndMethodLevels.TearDown);
            const string oneTimeTearDown = nameof(EmptyTestFor_ExecutionProceedsAfterBothTestHooksCompleteAtClassAndMethodLevels.OneTimeTearDown);
            var workItem = TestBuilder.CreateWorkItem(typeof(EmptyTestFor_ExecutionProceedsAfterBothTestHooksCompleteAtClassAndMethodLevels));
            workItem.Execute();

            Assert.That(TestLog.Logs, Is.EqualTo([
                oneTimeSetUp,
                // Test with hooks starts
                setUp,
                "ActivateBeforeClassHook",
                "ActivateBeforeTestMethodHook",
                testWithHooks,
                "ActivateAfterClassHook",
                "ActivateAfterTestMethodHook",
                tearDown,
                // Test with hooks ends
                // Test without hooks starts
                setUp,
                "ActivateBeforeClassHook",
                testWithoutHooks,
                "ActivateAfterClassHook",
                tearDown,
                // Test without hooks ends
                oneTimeTearDown
            ]));
        }
    }
}
