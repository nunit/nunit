// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.ExecutionHookTests;

namespace NUnit.Framework.Tests.ExecutionHooks.ExecutionSequence
{
    internal class ExecutionProceedsOnlyAfterAllAfterTestHooksExecute
    {
        [Test]
        public void TestProceedsAfterAllAfterTestHooksExecute()
        {
            TestLog.Clear();

            var workItem = TestBuilder.CreateWorkItem(typeof(ExecutionProceedsOnlyAfterAllAfterTestHooksExecute_TestUnderTest));
            workItem.Execute();

            Assert.That(TestLog.Logs, Is.EqualTo([
                nameof(ExecutionProceedsOnlyAfterAllAfterTestHooksExecute_TestUnderTest.TestPasses),

                nameof(ActivateAfterTestHookAttribute),
                nameof(ActivateAfterTestHookAttribute),
                nameof(ActivateAfterTestHookAttribute),
                nameof(ActivateAfterTestHookThrowingExceptionAttribute),

                nameof(ExecutionProceedsOnlyAfterAllAfterTestHooksExecute_TestUnderTest.TearDown),
                nameof(ExecutionProceedsOnlyAfterAllAfterTestHooksExecute_TestUnderTest.OneTimeTearDown)
            ]));

            TestLog.Logs.Clear();
        }
    }
}
