// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.ExceptionHandling
{
    internal class TestThrowsExceptionHooksProceedsToExecuteTests
    {
        [Test]
        public void TestThrowsException_HooksProceedsToExecute()
        {
            var workItem = TestBuilder.CreateWorkItem(typeof(TestData.ExecutionHooks.TestThrowsExceptionHooksProceedsToExecuteFixture), TestFilter.Explicit);
            workItem.Execute();
            var currentTestLogs = TestData.ExecutionHooks.TestLog.Logs(workItem.Test);

            Assert.That(currentTestLogs, Is.Not.Empty);
            Assert.That(currentTestLogs, Is.EqualTo([
                nameof(TestData.ExecutionHooks.TestThrowsExceptionHooksProceedsToExecuteFixture.OneTimeSetUp),
                nameof(TestData.ExecutionHooks.TestThrowsExceptionHooksProceedsToExecuteFixture.SetUp),
                nameof(TestData.ExecutionHooks.ActivateBeforeTestHookAttribute),
                nameof(TestData.ExecutionHooks.TestThrowsExceptionHooksProceedsToExecuteFixture.EmptyTest),
                nameof(TestData.ExecutionHooks.ActivateAfterTestHookAttribute),
                nameof(TestData.ExecutionHooks.TestThrowsExceptionHooksProceedsToExecuteFixture.TearDown),
                nameof(TestData.ExecutionHooks.TestThrowsExceptionHooksProceedsToExecuteFixture.OneTimeTearDown)
            ]));
        }
    }
}
