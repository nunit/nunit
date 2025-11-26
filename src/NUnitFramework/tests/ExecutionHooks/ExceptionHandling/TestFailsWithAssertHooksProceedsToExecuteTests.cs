// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.ExecutionHooks;

namespace NUnit.Framework.Tests.ExecutionHooks.ExceptionHandling
{
    internal class TestFailsWithAssertHooksProceedsToExecuteTests
    {
        [Test]
        public void TestFailsWithAssert_HooksProceedsToExecute()
        {
            var workItem = TestBuilder.CreateWorkItem(typeof(TestFailsWithAssertHooksProceedsToExecuteFixture));
            workItem.Execute();
            var currentTestLogs = TestLog.Logs(workItem.Test);

            Assert.That(currentTestLogs, Is.Not.Empty);
            Assert.That(currentTestLogs, Is.EqualTo([
                nameof(TestFailsWithAssertHooksProceedsToExecuteFixture.OneTimeSetUp),
                nameof(TestFailsWithAssertHooksProceedsToExecuteFixture.SetUp),
                nameof(ActivateBeforeTestHookAtMethodLevelAttribute),
                nameof(TestFailsWithAssertHooksProceedsToExecuteFixture.EmptyTest),
                nameof(ActivateAfterTestHookAtMethodLevelAttribute),
                nameof(TestFailsWithAssertHooksProceedsToExecuteFixture.TearDown),
                nameof(TestFailsWithAssertHooksProceedsToExecuteFixture.OneTimeTearDown)
            ]));
        }
    }
}
