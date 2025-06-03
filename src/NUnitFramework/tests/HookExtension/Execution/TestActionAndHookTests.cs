// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.HookExtensionTests;

namespace NUnit.Framework.Tests.HookExtension.Execution
{
    [TestFixture]
    internal class TestActionAndHookTests
    {
        [Test]
        public void ExecutionProceedsAfterTheAfterTestHookCompletes2()
        {
            TestLog.Clear();

            var workItem = TestBuilder.CreateWorkItem(typeof(EmptyTestFor_ExecutionProceedsAfterTheAfterTestHookCompletes2));
            workItem.Execute();

            Assert.That(TestLog.Logs, Is.EqualTo([
                nameof(EmptyTestFor_ExecutionProceedsAfterTheAfterTestHookCompletes2.OneTimeSetUp),
                "BeforeTest_Action",
                nameof(EmptyTestFor_ExecutionProceedsAfterTheAfterTestHookCompletes2.SetUp),
                nameof(ActivateBeforeTestHookAttribute),
                nameof(EmptyTestFor_ExecutionProceedsAfterTheAfterTestHookCompletes2.EmptyTest),
                nameof(ActivateAfterTestHookAttribute),
                nameof(EmptyTestFor_ExecutionProceedsAfterTheAfterTestHookCompletes2.TearDown),
                "AfterTest_Action",
                nameof(EmptyTestFor_ExecutionProceedsAfterTheAfterTestHookCompletes2.OneTimeTearDown)
            ]));
        }
    }
}
