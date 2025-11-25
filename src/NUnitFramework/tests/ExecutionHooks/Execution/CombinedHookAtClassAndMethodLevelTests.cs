// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.Execution
{
    internal class CombinedHookAtClassAndMethodLevelTests
    {
        [Test]
        public void ExecutionProceedsAfterBothTestHookCompletes()
        {
            var workItem =
                TestBuilder.CreateWorkItem(typeof(TestData.ExecutionHooks.CombinedHookAtClassAndMethodLevelTestsFixture),
                    TestFilter.Explicit);
            workItem.Execute();
            var currentTestLogs = TestData.ExecutionHooks.TestLog.Logs(workItem.Test);

            Assert.That(currentTestLogs, Is.Not.Empty);
            Assert.That(currentTestLogs, Is.EqualTo([
                nameof(TestData.ExecutionHooks.CombinedHookAtClassAndMethodLevelTestsFixture.OneTimeSetUp),

                // Test with hooks starts
                nameof(TestData.ExecutionHooks.CombinedHookAtClassAndMethodLevelTestsFixture.SetUp),
                nameof(TestData.ExecutionHooks.ActivateClassLevelBeforeTestHooksAttribute),
                nameof(TestData.ExecutionHooks.ActivateMethodLevelBeforeTestHooksAttribute),

                nameof(TestData.ExecutionHooks.CombinedHookAtClassAndMethodLevelTestsFixture.EmptyTestWithHooks),

                nameof(TestData.ExecutionHooks.ActivateMethodLevelAfterTestHooksAttribute),
                nameof(TestData.ExecutionHooks.ActivateClassLevelAfterTestHooksAttribute),
                nameof(TestData.ExecutionHooks.CombinedHookAtClassAndMethodLevelTestsFixture.TearDown),
                // Test with hooks ends

                // Test without hooks starts
                nameof(TestData.ExecutionHooks.CombinedHookAtClassAndMethodLevelTestsFixture.SetUp),
                nameof(TestData.ExecutionHooks.ActivateClassLevelBeforeTestHooksAttribute),

                nameof(TestData.ExecutionHooks.CombinedHookAtClassAndMethodLevelTestsFixture.EmptyTestWithoutHooks),

                nameof(TestData.ExecutionHooks.ActivateClassLevelAfterTestHooksAttribute),
                nameof(TestData.ExecutionHooks.CombinedHookAtClassAndMethodLevelTestsFixture.TearDown),
                // Test without hooks ends

                nameof(TestData.ExecutionHooks.CombinedHookAtClassAndMethodLevelTestsFixture.OneTimeTearDown)
            ]));
        }
    }
}
