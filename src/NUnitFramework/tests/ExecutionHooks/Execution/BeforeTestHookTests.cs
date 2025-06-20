// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.ExecutionHookTests;

namespace NUnit.Framework.Tests.ExecutionHooks.Execution
{
    internal class BeforeTestHookTests
    {
        [Test]
        public void ExecutionProceedsAfterBeforeTestHookCompletes()
        {
            TestLog.Clear();

            var workItem = TestBuilder.CreateWorkItem(typeof(TestWithBeforeTestHookOnMethod));
            workItem.Execute();

            Assert.That(TestLog.Logs, Is.EqualTo([
                nameof(TestWithBeforeTestHookOnMethod.OneTimeSetUp),
                nameof(TestWithBeforeTestHookOnMethod.SetUp),
                nameof(ActivateBeforeTestHookAttribute),
                nameof(TestWithBeforeTestHookOnMethod.EmptyTest),
                nameof(TestWithBeforeTestHookOnMethod.TearDown),
                nameof(TestWithBeforeTestHookOnMethod.OneTimeTearDown)
            ]));
        }
    }
}
