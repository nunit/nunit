// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.Execution
{
    internal class CombinedHookTests
    {
        [Test]
        public void ExecutionProceedsAfterBothTestHookCompletes()
        {
            var workItem =
                TestBuilder.CreateWorkItem(typeof(TestData.ExecutionHooks.CombinedHookTestsFixture), TestFilter.Explicit);
            workItem.Execute();
            var currentTestLogs = TestData.ExecutionHooks.TestLog.Logs(workItem.Test);

            Assert.That(currentTestLogs, Is.Not.Empty);
            Assert.That(currentTestLogs, Is.EqualTo([
                nameof(TestData.ExecutionHooks.CombinedHookTestsFixture.OneTimeSetUp),
                nameof(TestData.ExecutionHooks.CombinedHookTestsFixture.SetUp),

                nameof(TestData.ExecutionHooks.ActivateLongRunningBeforeTestHookAttribute),
                nameof(TestData.ExecutionHooks.ActivateBeforeTestHookAttribute),

                nameof(TestData.ExecutionHooks.CombinedHookTestsFixture.EmptyTest),

                nameof(TestData.ExecutionHooks.ActivateAfterTestHookAttribute),
                nameof(TestData.ExecutionHooks.ActivateLongRunningAfterTestHookAttribute),

                nameof(TestData.ExecutionHooks.CombinedHookTestsFixture.TearDown),
                nameof(TestData.ExecutionHooks.CombinedHookTestsFixture.OneTimeTearDown)
            ]));
        }
    }
}
