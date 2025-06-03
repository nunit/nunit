// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.HookExtensionTests;

namespace NUnit.Framework.Tests.HookExtension.Execution
{
    internal class AfterTestHookTests
    {
        [Test]
        public void ExecutionProceedsAfterTheAfterTestHookCompletes()
        {
            TestLog.Logs.Clear();

            var workItem = TestBuilder.CreateWorkItem(typeof(EmptyTestFor_ExecutionProceedsAfterTheAfterTestHookCompletes));
            workItem.Execute();

            Assert.That(TestLog.Logs, Is.EqualTo([
                nameof(EmptyTestFor_ExecutionProceedsAfterTheAfterTestHookCompletes.OneTimeSetUp),
                nameof(EmptyTestFor_ExecutionProceedsAfterTheAfterTestHookCompletes.SetUp),
                nameof(EmptyTestFor_ExecutionProceedsAfterTheAfterTestHookCompletes.EmptyTest),
                nameof(ActivateAfterTestHook.ApplyToContext),
                nameof(EmptyTestFor_ExecutionProceedsAfterTheAfterTestHookCompletes.TearDown),
                nameof(EmptyTestFor_ExecutionProceedsAfterTheAfterTestHookCompletes.OneTimeTearDown)
            ]));
        }
    }
}
