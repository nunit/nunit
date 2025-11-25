// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.Execution
{
    internal class TestActionAndHookTests
    {
        [Test]
        public void ExecutionProceedsAfterTheAfterTestHookCompletes2()
        {
            var workItem = TestBuilder.CreateWorkItem(typeof(TestData.ExecutionHooks.TestActionAndHookTestsFixture),
                TestFilter.Explicit);
            workItem.Execute();
            var currentTestLogs = TestData.ExecutionHooks.TestLog.Logs(workItem.Test);

            Assert.That(currentTestLogs, Is.Not.Empty);
            Assert.That(currentTestLogs, Is.EqualTo([
                nameof(TestData.ExecutionHooks.TestActionAndHookTestsFixture.OneTimeSetUp),
                TestData.ExecutionHooks.SimpleTestActionAttribute.LogStringForBeforeTest,
                nameof(TestData.ExecutionHooks.TestActionAndHookTestsFixture.SetUp),
                nameof(TestData.ExecutionHooks.ActivateBeforeTestHookAttribute),
                nameof(TestData.ExecutionHooks.TestActionAndHookTestsFixture.EmptyTest),
                nameof(TestData.ExecutionHooks.ActivateAfterTestHookAttribute),
                nameof(TestData.ExecutionHooks.TestActionAndHookTestsFixture.TearDown),
                TestData.ExecutionHooks.SimpleTestActionAttribute.LogStringForAfterTest,
                nameof(TestData.ExecutionHooks.TestActionAndHookTestsFixture.OneTimeTearDown)
            ]));
        }
    }
}
