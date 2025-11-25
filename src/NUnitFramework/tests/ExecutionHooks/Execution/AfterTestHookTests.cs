// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.Execution
{
    internal class AfterTestHookTests
    {
        [Test]
        public void ExecutionProceedsAfterTheAfterTestHookCompletes()
        {
            var workItem = TestBuilder.CreateWorkItem(typeof(TestData.ExecutionHooks.AfterTestHookTestsFixture), TestFilter.Explicit);
            workItem.Execute();
            var currentTestLogs = TestData.ExecutionHooks.TestLog.Logs(workItem.Test);

            Assert.That(currentTestLogs, Is.Not.Empty);
            Assert.That(currentTestLogs, Is.EqualTo([
                nameof(TestData.ExecutionHooks.AfterTestHookTestsFixture.OneTimeSetUp),
                nameof(TestData.ExecutionHooks.AfterTestHookTestsFixture.SetUp),
                nameof(TestData.ExecutionHooks.AfterTestHookTestsFixture.EmptyTest),
                nameof(TestData.ExecutionHooks.ActivateAfterTestHookAttribute),
                nameof(TestData.ExecutionHooks.AfterTestHookTestsFixture.TearDown),
                nameof(TestData.ExecutionHooks.AfterTestHookTestsFixture.OneTimeTearDown)
            ]));
        }
    }
}
