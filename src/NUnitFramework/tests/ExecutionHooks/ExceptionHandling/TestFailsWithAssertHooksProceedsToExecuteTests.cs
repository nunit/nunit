// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.ExceptionHandling
{
    internal class TestFailsWithAssertHooksProceedsToExecuteTests
    {
        [Test]
        public void TestFailsWithAssert_HooksProceedsToExecute()
        {
            var workItem = TestBuilder.CreateWorkItem(typeof(TestData.ExecutionHooks.TestFailsWithAssertHooksProceedsToExecuteFixture), TestFilter.Explicit);
            workItem.Execute();
            var currentTestLogs = TestData.ExecutionHooks.TestLog.Logs(workItem.Test);

            Assert.That(currentTestLogs, Is.Not.Empty);
            Assert.That(currentTestLogs, Is.EqualTo([
                nameof(TestData.ExecutionHooks.TestFailsWithAssertHooksProceedsToExecuteFixture.OneTimeSetUp),
                nameof(TestData.ExecutionHooks.TestFailsWithAssertHooksProceedsToExecuteFixture.SetUp),
                nameof(TestData.ExecutionHooks.ActivateBeforeTestHookAttribute),
                nameof(TestData.ExecutionHooks.TestFailsWithAssertHooksProceedsToExecuteFixture.EmptyTest),
                nameof(TestData.ExecutionHooks.ActivateAfterTestHookAttribute),
                nameof(TestData.ExecutionHooks.TestFailsWithAssertHooksProceedsToExecuteFixture.TearDown),
                nameof(TestData.ExecutionHooks.TestFailsWithAssertHooksProceedsToExecuteFixture.OneTimeTearDown)
            ]));
        }
    }
}
