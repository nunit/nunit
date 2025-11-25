// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.ExecutionHooks;

namespace NUnit.Framework.Tests.ExecutionHooks.ExceptionHandling
{
    internal class TestFailsWithAssertHooksProceedsToExecuteTests
    {
        [Test]
        public void TestFailsWithAssert_HooksProceedsToExecute()
        {
            var workItem = TestBuilder.CreateWorkItem(typeof(TestFailsWithAssertHooksProceedsToExecuteFixture), TestFilter.Explicit);
            workItem.Execute();
            var currentTestLogs = TestLog.Logs(workItem.Test);

            Assert.That(currentTestLogs, Is.Not.Empty);
            Assert.That(currentTestLogs, Is.EqualTo([
                nameof(TestFailsWithAssertHooksProceedsToExecuteFixture.OneTimeSetUp),
                nameof(TestFailsWithAssertHooksProceedsToExecuteFixture.SetUp),
                nameof(ActivateBeforeTestHookAttribute),
                nameof(TestFailsWithAssertHooksProceedsToExecuteFixture.EmptyTest),
                nameof(ActivateAfterTestHookAttribute),
                nameof(TestFailsWithAssertHooksProceedsToExecuteFixture.TearDown),
                nameof(TestFailsWithAssertHooksProceedsToExecuteFixture.OneTimeTearDown)
            ]));
        }
    }
}
