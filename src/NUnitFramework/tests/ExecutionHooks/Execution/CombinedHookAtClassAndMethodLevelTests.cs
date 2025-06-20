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

            var workItem = TestBuilder.CreateWorkItem(typeof(TestClassWithTestHooksOneTestWithoutAndOneWithMethodTestHooks));
            workItem.Execute();

            Assert.That(TestLog.Logs, Is.EqualTo([
                nameof(TestClassWithTestHooksOneTestWithoutAndOneWithMethodTestHooks.OneTimeSetUp),

                // Test with hooks starts
                nameof(TestClassWithTestHooksOneTestWithoutAndOneWithMethodTestHooks.SetUp),
                nameof(ActivateClassLevelBeforeTestHooksAttribute),
                nameof(ActivateMethodLevelBeforeTestHooksAttribute),

                nameof(TestClassWithTestHooksOneTestWithoutAndOneWithMethodTestHooks.EmptyTestWithHooks),

                nameof(ActivateClassLevelAfterTestHooksAttribute),
                nameof(ActivateMethodLevelAfterTestHooksAttribute),
                nameof(TestClassWithTestHooksOneTestWithoutAndOneWithMethodTestHooks.TearDown),
                // Test with hooks ends

                // Test without hooks starts
                nameof(TestClassWithTestHooksOneTestWithoutAndOneWithMethodTestHooks.SetUp),
                nameof(ActivateClassLevelBeforeTestHooksAttribute),

                nameof(TestClassWithTestHooksOneTestWithoutAndOneWithMethodTestHooks.EmptyTestWithoutHooks),

                nameof(ActivateClassLevelAfterTestHooksAttribute),
                nameof(TestClassWithTestHooksOneTestWithoutAndOneWithMethodTestHooks.TearDown),
                // Test without hooks ends

                nameof(TestClassWithTestHooksOneTestWithoutAndOneWithMethodTestHooks.OneTimeTearDown)
            ]));
        }
    }
}
