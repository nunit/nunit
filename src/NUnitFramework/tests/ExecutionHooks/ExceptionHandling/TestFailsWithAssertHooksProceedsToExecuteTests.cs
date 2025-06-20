// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.ExecutionHookTests;

namespace NUnit.Framework.Tests.ExecutionHooks.ExceptionHandling
{
    internal class TestFailsWithAssertHooksProceedsToExecuteTests
    {
        [Test]
        public void TestFailsWithAssert_HooksProceedsToExecute()
        {
            TestLog.Clear();

            var workItem = TestBuilder.CreateWorkItem(typeof(FailingTestWithTestHookOnMethod));
            workItem.Execute();

            Assert.That(TestLog.Logs, Is.EqualTo([
                nameof(FailingTestWithTestHookOnMethod.OneTimeSetUp),
                nameof(FailingTestWithTestHookOnMethod.SetUp),
                nameof(ActivateBeforeTestHookAttribute),
                nameof(FailingTestWithTestHookOnMethod.EmptyTest),
                nameof(ActivateAfterTestHookAttribute),
                nameof(FailingTestWithTestHookOnMethod.TearDown),
                nameof(FailingTestWithTestHookOnMethod.OneTimeTearDown)
            ]));
        }
    }
}
