// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.ExecutionHookTests;

namespace NUnit.Framework.Tests.ExecutionHooks.Execution
{
    internal class AfterTestHookTests
    {
        [Test]
        public void ExecutionProceedsAfterTheAfterTestHookCompletes()
        {
            TestLog.Clear();

            var workItem = TestBuilder.CreateWorkItem(typeof(TestWithAfterTestHookOnMethod));
            workItem.Execute();

            Assert.That(TestLog.Logs, Is.EqualTo([
                nameof(TestWithAfterTestHookOnMethod.OneTimeSetUp),
                nameof(TestWithAfterTestHookOnMethod.SetUp),
                nameof(TestWithAfterTestHookOnMethod.EmptyTest),
                nameof(ActivateAfterTestHookAttribute),
                nameof(TestWithAfterTestHookOnMethod.TearDown),
                nameof(TestWithAfterTestHookOnMethod.OneTimeTearDown)
            ]));
        }
    }
}
