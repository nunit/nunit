// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.ExecutionHookTests;

namespace NUnit.Framework.Tests.ExecutionHooks.ExceptionHandling
{
    internal class TestThrowsExceptionHooksProceedsToExecuteTests
    {
        [Test]
        public void TestThrowsException_HooksProceedsToExecute()
        {
            TestLog.Clear();

            var workItem = TestBuilder.CreateWorkItem(typeof(TestWithTestHooksOnMethod));
            workItem.Execute();

            Assert.That(TestLog.Logs, Is.EqualTo([
                nameof(TestWithTestHooksOnMethod.OneTimeSetUp),
                nameof(TestWithTestHooksOnMethod.SetUp),
                nameof(ActivateBeforeTestHookAttribute),
                nameof(TestWithTestHooksOnMethod.EmptyTest),
                nameof(ActivateAfterTestHookAttribute),
                nameof(TestWithTestHooksOnMethod.TearDown),
                nameof(TestWithTestHooksOnMethod.OneTimeTearDown)
            ]));
        }
    }
}
