// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.ExecutionHooks;

namespace NUnit.Framework.Tests.ExecutionHooks.ExceptionHandling
{
    internal class AfterTestHookThrowsExceptionExecutionProceedsTests
    {
        [Test]
        public void AfterTestHookThrowsException_ExecutionProceeds()
        {
            var workItem = TestBuilder.CreateWorkItem(typeof(AfterTestHookThrowsExceptionExecutionProceedsFixture));
            workItem.Execute();

            var currentTestLogs = TestLog.Logs(workItem.Test);

            Assert.That(currentTestLogs, Is.Not.Empty);
            Assert.That(currentTestLogs, Is.EqualTo([
                nameof(AfterTestHookThrowsExceptionExecutionProceedsFixture.OneTimeSetUp),
                nameof(AfterTestHookThrowsExceptionExecutionProceedsFixture.SetUp),
                nameof(ActivateBeforeTestHookAtMethodLevelAttribute),
                nameof(AfterTestHookThrowsExceptionExecutionProceedsFixture.EmptyTest),
                nameof(ActivateAfterTestHookThrowingExceptionAttribute),
                nameof(AfterTestHookThrowsExceptionExecutionProceedsFixture.TearDown),
                nameof(AfterTestHookThrowsExceptionExecutionProceedsFixture.OneTimeTearDown)
            ]));
        }
    }
}
