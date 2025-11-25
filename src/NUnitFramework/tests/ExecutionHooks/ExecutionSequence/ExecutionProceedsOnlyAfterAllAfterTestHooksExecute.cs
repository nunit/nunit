// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.ExecutionSequence
{
    internal class ExecutionProceedsOnlyAfterAllAfterTestHooksExecute
    {
        [Test]
        public void TestProceedsAfterAllAfterTestHooksExecute()
        {
            var workItem = TestBuilder.CreateWorkItem(typeof(TestData.ExecutionHooks.ExecutionProceedsOnlyAfterAllAfterTestHooksExecuteFixture), TestFilter.Explicit);
            workItem.Execute();
            var currentTestLogs = TestData.ExecutionHooks.TestLog.Logs(workItem.Test);

            Assert.That(currentTestLogs, Is.EqualTo([
                nameof(TestData.ExecutionHooks.ExecutionProceedsOnlyAfterAllAfterTestHooksExecuteFixture.TestPasses),

                nameof(TestData.ExecutionHooks.ActivateAfterTestHookAttribute),
                nameof(TestData.ExecutionHooks.ActivateAfterTestHookAttribute),
                nameof(TestData.ExecutionHooks.ActivateAfterTestHookAttribute),
                nameof(TestData.ExecutionHooks.ActivateAfterTestHookThrowingExceptionAttribute),

                nameof(TestData.ExecutionHooks.ExecutionProceedsOnlyAfterAllAfterTestHooksExecuteFixture.TearDown),
                nameof(TestData.ExecutionHooks.ExecutionProceedsOnlyAfterAllAfterTestHooksExecuteFixture.OneTimeTearDown)
            ]));
        }
    }
}
