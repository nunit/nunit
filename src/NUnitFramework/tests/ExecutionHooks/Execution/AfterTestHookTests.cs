// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.ExecutionHooks;

namespace NUnit.Framework.Tests.ExecutionHooks.Execution
{
    internal class AfterTestHookTests
    {
        [Test]
        public void ExecutionProceedsAfterTheAfterTestHookCompletes()
        {
            var workItem = TestBuilder.CreateWorkItem(typeof(AfterTestHookTestFixture), TestFilter.Explicit);
            workItem.Execute();
            var currentTestLogs = TestLog.Logs(workItem.Test);

            Assert.That(currentTestLogs, Is.Not.Empty);
            Assert.That(currentTestLogs, Is.EqualTo([
                nameof(AfterTestHookTestFixture.OneTimeSetUp),
                nameof(AfterTestHookTestFixture.SetUp),
                nameof(AfterTestHookTestFixture.EmptyTest),
                nameof(ActivateAfterTestHookAttribute),
                nameof(AfterTestHookTestFixture.TearDown),
                nameof(AfterTestHookTestFixture.OneTimeTearDown)
            ]));
        }
    }
}
