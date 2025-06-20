// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.ExecutionHookTests;

namespace NUnit.Framework.Tests.ExecutionHooks.Execution
{
    internal class TestActionAndHookTests
    {
        [Test]
        public void ExecutionProceedsAfterTheAfterTestHookCompletes2()
        {
            TestLog.Clear();

            var workItem = TestBuilder.CreateWorkItem(typeof(TestWithTestHooksAndClassTestActionAttribute));
            workItem.Execute();

            Assert.That(TestLog.Logs, Is.EqualTo([
                nameof(TestWithTestHooksAndClassTestActionAttribute.OneTimeSetUp),
                nameof(SomeTestActionAttribute.BeforeTest),
                nameof(TestWithTestHooksAndClassTestActionAttribute.SetUp),
                nameof(ActivateBeforeTestHookAttribute),
                nameof(TestWithTestHooksAndClassTestActionAttribute.EmptyTest),
                nameof(ActivateAfterTestHookAttribute),
                nameof(TestWithTestHooksAndClassTestActionAttribute.TearDown),
                nameof(SomeTestActionAttribute.AfterTest),
                nameof(TestWithTestHooksAndClassTestActionAttribute.OneTimeTearDown)
            ]));
        }
    }
}
