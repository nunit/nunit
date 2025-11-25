// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.ExecutionHooks;

namespace NUnit.Framework.Tests.ExecutionHooks.Execution
{
    internal class BeforeTestHookTests
    {
        [Test]
        public void ExecutionProceedsAfterBeforeTestHookCompletes()
        {
            var workItem = TestBuilder.CreateWorkItem(typeof(BeforeTestHookTestsFixture), TestFilter.Explicit);
            workItem.Execute();
            var currentTestLogs = TestLog.Logs(workItem.Test);

            Assert.That(currentTestLogs, Is.Not.Empty);
            Assert.That(currentTestLogs, Is.EqualTo([
                nameof(BeforeTestHookTestsFixture.OneTimeSetUp),
                nameof(BeforeTestHookTestsFixture.SetUp),
                nameof(ActivateBeforeTestHookAttribute),
                nameof(BeforeTestHookTestsFixture.EmptyTest),
                nameof(BeforeTestHookTestsFixture.TearDown),
                nameof(BeforeTestHookTestsFixture.OneTimeTearDown)
            ]));
        }
    }
}
