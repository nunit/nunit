// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.HookExtensionTests;

namespace NUnit.Framework.Tests.HookExtension.Execution
{
    [TestFixture]
    internal class BeforeTestHookTests
    {
        [Test]
        public void ExecutionProceedsAfterBeforeTestHookCompletes()
        {
            TestLog.Clear();

            var workItem = TestBuilder.CreateWorkItem(typeof(EmptyTestFor_ExecutionProceedsAfterBeforeTestHookCompletes));
            workItem.Execute();

            Assert.That(TestLog.Logs, Is.EqualTo([
                nameof(EmptyTestFor_ExecutionProceedsAfterBeforeTestHookCompletes.OneTimeSetUp),
                nameof(EmptyTestFor_ExecutionProceedsAfterBeforeTestHookCompletes.SetUp),
                nameof(ActivateBeforeTestHook.ApplyToContext),
                nameof(EmptyTestFor_ExecutionProceedsAfterBeforeTestHookCompletes.EmptyTest),
                nameof(EmptyTestFor_ExecutionProceedsAfterBeforeTestHookCompletes.TearDown),
                nameof(EmptyTestFor_ExecutionProceedsAfterBeforeTestHookCompletes.OneTimeTearDown)
            ]));
        }
    }
}
