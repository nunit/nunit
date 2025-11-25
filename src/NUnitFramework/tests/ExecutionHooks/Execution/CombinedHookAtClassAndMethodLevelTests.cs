// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.ExecutionHooks;

namespace NUnit.Framework.Tests.ExecutionHooks.Execution
{
    internal class CombinedHookAtClassAndMethodLevelTests
    {
        [Test]
        public void ExecutionProceedsAfterBothTestHookCompletes()
        {
            var workItem =
                TestBuilder.CreateWorkItem(typeof(CombinedHookAtClassAndMethodLevelTestsFixture),
                    TestFilter.Explicit);
            workItem.Execute();
            var currentTestLogs = TestLog.Logs(workItem.Test);

            Assert.That(currentTestLogs, Is.Not.Empty);
            Assert.That(currentTestLogs, Is.EqualTo([
                nameof(CombinedHookAtClassAndMethodLevelTestsFixture.OneTimeSetUp),

                // Test with hooks starts
                nameof(CombinedHookAtClassAndMethodLevelTestsFixture.SetUp),

                // Class-level before hook
                nameof(ActivateBeforeTestHookAttribute),
                // Method-level before hook
                nameof(ActivateBeforeTestHookAttribute),

                nameof(CombinedHookAtClassAndMethodLevelTestsFixture.EmptyTestWithHooks),

                // Method-level after hook
                nameof(ActivateAfterTestHookAttribute),
                // Class-level after hook
                nameof(ActivateAfterTestHookAttribute),
                nameof(CombinedHookAtClassAndMethodLevelTestsFixture.TearDown),
                // Test with hooks ends

                // Test without hooks starts
                nameof(CombinedHookAtClassAndMethodLevelTestsFixture.SetUp),
                // Class-level before hook
                nameof(ActivateBeforeTestHookAttribute),

                nameof(CombinedHookAtClassAndMethodLevelTestsFixture.EmptyTestWithoutHooks),

                // Class-level after
                nameof(ActivateAfterTestHookAttribute),
                nameof(CombinedHookAtClassAndMethodLevelTestsFixture.TearDown),
                // Test without hooks ends

                nameof(CombinedHookAtClassAndMethodLevelTestsFixture.OneTimeTearDown)
            ]));
        }
    }
}
