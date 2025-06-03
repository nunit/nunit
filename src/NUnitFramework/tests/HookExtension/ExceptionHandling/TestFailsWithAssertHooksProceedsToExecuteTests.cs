// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.HookExtensionTests;

namespace NUnit.Framework.Tests.HookExtension.ExceptionHandling
{
    [TestFixture]
    internal class TestFailsWithAssertHooksProceedsToExecuteTests
    {
        [Test]
        public void TestFailsWithAssert_HooksProceedsToExecute()
        {
            TestLog.Clear();

            var workItem = TestBuilder.CreateWorkItem(typeof(EmptyTestFor_TestFailsWithAssert_HooksProceedsToExecute));
            workItem.Execute();

            Assert.That(TestLog.Logs, Is.EqualTo([
                nameof(EmptyTestFor_TestFailsWithAssert_HooksProceedsToExecute.OneTimeSetUp),
                nameof(EmptyTestFor_TestFailsWithAssert_HooksProceedsToExecute.SetUp),
                nameof(ActivateBeforeTestHookAttribute),
                nameof(EmptyTestFor_TestFailsWithAssert_HooksProceedsToExecute.EmptyTest),
                nameof(ActivateAfterTestHookAttribute),
                nameof(EmptyTestFor_TestFailsWithAssert_HooksProceedsToExecute.TearDown),
                nameof(EmptyTestFor_TestFailsWithAssert_HooksProceedsToExecute.OneTimeTearDown)
            ]));
        }
    }
}
