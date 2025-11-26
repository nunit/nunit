// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.ExecutionHooks;

namespace NUnit.Framework.Tests.ExecutionHooks.Execution
{
    internal class CombinedHookAtClassAndMethodLevelTests
    {
        [Test]
        public void ExecutionProceedsAfterBothTestHookCompletes()
        {
            var workItem = TestBuilder.CreateWorkItem(typeof(CombinedHookAtClassAndMethodLevelTestsFixture));
            workItem.Execute();
            var currentTestLogs = TestLog.Logs(workItem.Test);

            Assert.That(currentTestLogs, Is.Not.Empty);
            Assert.That(currentTestLogs, Is.EqualTo([
                nameof(CombinedHookAtClassAndMethodLevelTestsFixture.OneTimeSetUp),

                nameof(CombinedHookAtClassAndMethodLevelTestsFixture.SetUp),

                nameof(ActivateBeforeTestHookAtClassLevelAttribute),
                nameof(ActivateBeforeTestHookAtMethodLevelAttribute),

                nameof(CombinedHookAtClassAndMethodLevelTestsFixture.EmptyTestWithHooks),

                nameof(ActivateAfterTestHookAtMethodLevelAttribute),
                nameof(ActivateAfterTestHookAtClassLevelAttribute),
                nameof(CombinedHookAtClassAndMethodLevelTestsFixture.TearDown),

                nameof(CombinedHookAtClassAndMethodLevelTestsFixture.SetUp),
                nameof(ActivateBeforeTestHookAtClassLevelAttribute),
                nameof(CombinedHookAtClassAndMethodLevelTestsFixture.EmptyTestWithoutHooks),

                nameof(ActivateAfterTestHookAtClassLevelAttribute),
                nameof(CombinedHookAtClassAndMethodLevelTestsFixture.TearDown),

                nameof(CombinedHookAtClassAndMethodLevelTestsFixture.OneTimeTearDown)
            ]));
        }
    }
}
