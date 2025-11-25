// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.ExceptionHandling
{
    internal class AfterTestHookThrowsExceptionExecutionProceedsTests
    {
        [Test]
        public void AfterTestHookThrowsException_ExecutionProceeds()
        {
            var workItem = TestBuilder.CreateWorkItem(typeof(TestData.ExecutionHooks.AfterTestHookThrowsExceptionExecutionProceedsFixture),
                TestFilter.Explicit);
            workItem.Execute();

            var currentTestLogs = TestData.ExecutionHooks.TestLog.Logs(workItem.Test);

            Assert.That(currentTestLogs, Is.Not.Empty);
            Assert.That(currentTestLogs, Is.EqualTo([
                nameof(TestData.ExecutionHooks.AfterTestHookThrowsExceptionExecutionProceedsFixture.OneTimeSetUp),
                nameof(TestData.ExecutionHooks.AfterTestHookThrowsExceptionExecutionProceedsFixture.SetUp),
                nameof(TestData.ExecutionHooks.ActivateBeforeTestHookAttribute),
                nameof(TestData.ExecutionHooks.AfterTestHookThrowsExceptionExecutionProceedsFixture.EmptyTest),
                nameof(TestData.ExecutionHooks.ActivateAfterTestHookThrowingExceptionAttribute),
                nameof(TestData.ExecutionHooks.AfterTestHookThrowsExceptionExecutionProceedsFixture.TearDown),
                nameof(TestData.ExecutionHooks.AfterTestHookThrowsExceptionExecutionProceedsFixture.OneTimeTearDown)
            ]));
        }
    }
}
