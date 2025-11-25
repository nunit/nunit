// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.ExecutionHooks;

namespace NUnit.Framework.Tests.ExecutionHooks.Execution
{
    internal class TestActionAndHookTests
    {
        [Test]
        public void ExecutionProceedsAfterTheAfterTestHookCompletes2()
        {
            var workItem = TestBuilder.CreateWorkItem(typeof(TestActionAndHookTestsFixture),
                TestFilter.Explicit);
            workItem.Execute();
            var currentTestLogs = TestLog.Logs(workItem.Test);

            Assert.That(currentTestLogs, Is.Not.Empty);
            Assert.That(currentTestLogs, Is.EqualTo([
                nameof(TestActionAndHookTestsFixture.OneTimeSetUp),
                SimpleTestActionAttribute.LogStringForBeforeTest,
                nameof(TestActionAndHookTestsFixture.SetUp),
                nameof(ActivateBeforeTestHookAttribute),
                nameof(TestActionAndHookTestsFixture.EmptyTest),
                nameof(ActivateAfterTestHookAttribute),
                nameof(TestActionAndHookTestsFixture.TearDown),
                SimpleTestActionAttribute.LogStringForAfterTest,
                nameof(TestActionAndHookTestsFixture.OneTimeTearDown)
            ]));
        }
    }
}
