// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.HookExtensionTests;

namespace NUnit.Framework.Tests.HookExtension.ExceptionHandling
{
    [TestFixture]
    internal class TestThrowsExceptionHooksProceedsToExecuteTests
    {
        [Test]
        public void TestThrowsException_HooksProceedsToExecute()
        {
            TestLog.Clear();

            var workItem = TestBuilder.CreateWorkItem(typeof(EmptyTestFor_TestThrowsException_HooksProceedsToExecute));
            workItem.Execute();

            Assert.That(TestLog.Logs, Is.EqualTo([
                nameof(EmptyTestFor_TestThrowsException_HooksProceedsToExecute.OneTimeSetUp),
                nameof(EmptyTestFor_TestThrowsException_HooksProceedsToExecute.SetUp),
                nameof(ActivateBeforeTestHook),
                nameof(EmptyTestFor_TestThrowsException_HooksProceedsToExecute.EmptyTest),
                nameof(ActivateAfterTestHook),
                nameof(EmptyTestFor_TestThrowsException_HooksProceedsToExecute.TearDown),
                nameof(EmptyTestFor_TestThrowsException_HooksProceedsToExecute.OneTimeTearDown)
            ]));
        }
    }
}
