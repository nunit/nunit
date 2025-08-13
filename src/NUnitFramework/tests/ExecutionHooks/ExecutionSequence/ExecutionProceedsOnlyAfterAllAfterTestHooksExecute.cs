// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.ExecutionHooks.TestAttributes;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.ExecutionSequence
{
    internal class ExecutionProceedsOnlyAfterAllAfterTestHooksExecute
    {
        [Explicit($"This test should only be run as part of the {nameof(TestProceedsAfterAllAfterTestHooksExecute)} test")]
        public class ExecutionProceedsOnlyAfterAllAfterTestHooksExecuteTestUnderTest
        {
            [Test]
            [ActivateAfterTestHook]
            [ActivateAfterTestHook]
            [ActivateAfterTestHook]
            [ActivateAfterTestHookThrowingException]
            public void TestPasses() => TestLog.LogCurrentMethod();

            [TearDown]
            public void TearDown() => TestLog.LogCurrentMethod();

            [OneTimeTearDown]
            public void OneTimeTearDown() => TestLog.LogCurrentMethod();
        }

        [Test]
        public void TestProceedsAfterAllAfterTestHooksExecute()
        {
            TestLog.Clear();

            var workItem = TestBuilder.CreateWorkItem(typeof(ExecutionProceedsOnlyAfterAllAfterTestHooksExecuteTestUnderTest), TestFilter.Explicit);
            workItem.Execute();

            Assert.That(TestLog.Logs, Is.EqualTo([
                nameof(ExecutionProceedsOnlyAfterAllAfterTestHooksExecuteTestUnderTest.TestPasses),

                nameof(ActivateAfterTestHookAttribute),
                nameof(ActivateAfterTestHookAttribute),
                nameof(ActivateAfterTestHookAttribute),
                nameof(ActivateAfterTestHookThrowingExceptionAttribute),

                nameof(ExecutionProceedsOnlyAfterAllAfterTestHooksExecuteTestUnderTest.TearDown),
                nameof(ExecutionProceedsOnlyAfterAllAfterTestHooksExecuteTestUnderTest.OneTimeTearDown)
            ]));

            TestLog.Logs.Clear();
        }
    }
}
