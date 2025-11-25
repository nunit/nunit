// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.ExecutionHooks;

namespace NUnit.Framework.Tests.ExecutionHooks.Execution
{
    internal class TestActionAndHookTests
    {
        [Test]
        public void ExecutionProceedsAfterTheAfterTestHookCompletes2()
        {
            var workItem = TestBuilder.CreateWorkItem(typeof(TestActionAndHookCombinationFixture));
            workItem.Execute();
            var currentTestLogs = TestLog.Logs(workItem.Test);

            Assert.That(currentTestLogs, Is.Not.Empty);
            Assert.That(currentTestLogs, Is.EqualTo([
                nameof(TestActionAndHookCombinationFixture.OneTimeSetUp),
                SimpleTestActionAttribute.LogStringForBeforeTest,
                nameof(TestActionAndHookCombinationFixture.SetUp),
                nameof(ActivateBeforeTestHookAttribute),
                nameof(TestActionAndHookCombinationFixture.EmptyTest),
                nameof(ActivateAfterTestHookAttribute),
                nameof(TestActionAndHookCombinationFixture.TearDown),
                SimpleTestActionAttribute.LogStringForAfterTest,
                nameof(TestActionAndHookCombinationFixture.OneTimeTearDown)
            ]));
        }
    }
}
