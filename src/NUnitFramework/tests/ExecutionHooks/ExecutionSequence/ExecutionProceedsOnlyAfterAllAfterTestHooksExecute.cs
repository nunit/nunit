// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.ExecutionHooks;

namespace NUnit.Framework.Tests.ExecutionHooks.ExecutionSequence
{
    internal class ExecutionProceedsOnlyAfterAllAfterTestHooksExecute
    {
        [Test]
        public void TestProceedsAfterAllAfterTestHooksExecute()
        {
            var workItem = TestBuilder.CreateWorkItem(typeof(ExecutionProceedsOnlyAfterAllAfterTestHooksExecuteFixture));
            workItem.Execute();
            var currentTestLogs = TestLog.Logs(workItem.Test);

            Assert.That(currentTestLogs, Is.EqualTo([
                nameof(ExecutionProceedsOnlyAfterAllAfterTestHooksExecuteFixture.TestPasses),

                nameof(ActivateAfterTestHookAttribute),
                nameof(ActivateAfterTestHookAttribute),
                nameof(ActivateAfterTestHookAttribute),
                nameof(ActivateAfterTestHookThrowingExceptionAttribute),

                nameof(ExecutionProceedsOnlyAfterAllAfterTestHooksExecuteFixture.TearDown),
                nameof(ExecutionProceedsOnlyAfterAllAfterTestHooksExecuteFixture.OneTimeTearDown)
            ]));
        }
    }
}
