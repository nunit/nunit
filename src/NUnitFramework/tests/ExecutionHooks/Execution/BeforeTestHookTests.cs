// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.Execution
{
    internal class BeforeTestHookTests
    {
        [Test]
        public void ExecutionProceedsAfterBeforeTestHookCompletes()
        {
            var workItem = TestBuilder.CreateWorkItem(typeof(TestData.ExecutionHooks.BeforeTestHookTestsFixture), TestFilter.Explicit);
            workItem.Execute();
            var currentTestLogs = TestData.ExecutionHooks.TestLog.Logs(workItem.Test);

            Assert.That(currentTestLogs, Is.Not.Empty);
            Assert.That(currentTestLogs, Is.EqualTo([
                nameof(TestData.ExecutionHooks.BeforeTestHookTestsFixture.OneTimeSetUp),
                nameof(TestData.ExecutionHooks.BeforeTestHookTestsFixture.SetUp),
                nameof(TestData.ExecutionHooks.ActivateBeforeTestHookAttribute),
                nameof(TestData.ExecutionHooks.BeforeTestHookTestsFixture.EmptyTest),
                nameof(TestData.ExecutionHooks.BeforeTestHookTestsFixture.TearDown),
                nameof(TestData.ExecutionHooks.BeforeTestHookTestsFixture.OneTimeTearDown)
            ]));
        }
    }
}
