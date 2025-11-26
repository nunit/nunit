// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.ExecutionHooks;

namespace NUnit.Framework.Tests.ExecutionHooks.Execution
{
    internal class CombinedHookTests
    {
        [Test]
        public void ExecutionProceedsAfterBothTestHookCompletes()
        {
            var workItem = TestBuilder.CreateWorkItem(typeof(CombinedHookTestsFixture));
            workItem.Execute();
            var currentTestLogs = TestLog.Logs(workItem.Test);

            Assert.That(currentTestLogs, Is.Not.Empty);
            Assert.That(currentTestLogs, Is.EqualTo([
                nameof(CombinedHookTestsFixture.OneTimeSetUp),
                nameof(CombinedHookTestsFixture.SetUp),

                nameof(ActivateLongRunningBeforeTestHookAttribute),
                nameof(ActivateBeforeTestHookAtMethodLevelAttribute),

                nameof(CombinedHookTestsFixture.EmptyTest),

                nameof(ActivateAfterTestHookAtMethodLevelAttribute),
                nameof(ActivateLongRunningAfterTestHookAttribute),

                nameof(CombinedHookTestsFixture.TearDown),
                nameof(CombinedHookTestsFixture.OneTimeTearDown)
            ]));
        }
    }
}
