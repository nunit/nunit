// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.ExecutionHooks;

namespace NUnit.Framework.Tests.ExecutionHooks.ExceptionHandling
{
    internal class TestThrowsExceptionHooksProceedsToExecuteTests
    {
        [Test]
        public void TestThrowsException_HooksProceedsToExecute()
        {
            var workItem = TestBuilder.CreateWorkItem(typeof(TestThrowsExceptionHooksProceedsToExecuteFixture));
            workItem.Execute();
            var currentTestLogs = TestLog.Logs(workItem.Test);

            Assert.That(currentTestLogs, Is.Not.Empty);
            Assert.That(currentTestLogs, Is.EqualTo([
                nameof(TestThrowsExceptionHooksProceedsToExecuteFixture.OneTimeSetUp),
                nameof(TestThrowsExceptionHooksProceedsToExecuteFixture.SetUp),
                nameof(ActivateBeforeTestHookAttribute),
                nameof(TestThrowsExceptionHooksProceedsToExecuteFixture.EmptyTest),
                nameof(ActivateAfterTestHookAttribute),
                nameof(TestThrowsExceptionHooksProceedsToExecuteFixture.TearDown),
                nameof(TestThrowsExceptionHooksProceedsToExecuteFixture.OneTimeTearDown)
            ]));
        }
    }
}
