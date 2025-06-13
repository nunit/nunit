// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.ExecutionHookTests;

namespace NUnit.Framework.Tests.ExecutionHooks.ExceptionHandling
{
    [TestFixture]
    internal class AfterTestHookThrowsExceptionExecutionProceedsTests
    {
        [Test]
        public void AfterTestHookThrowsException_ExecutionProceeds()
        {
            TestLog.Clear();

            var workItem = TestBuilder.CreateWorkItem(typeof(EmptyTestFor_AfterTestHookThrowsException_ExecutionProceeds));
            workItem.Execute();

            Assert.That(TestLog.Logs, Is.EqualTo([
                nameof(EmptyTestFor_AfterTestHookThrowsException_ExecutionProceeds.OneTimeSetUp),
                nameof(EmptyTestFor_AfterTestHookThrowsException_ExecutionProceeds.SetUp),
                nameof(ActivateBeforeTestHookAttribute),
                nameof(EmptyTestFor_AfterTestHookThrowsException_ExecutionProceeds.EmptyTest),
                nameof(ActivateAfterTestHookThrowingExceptionAttribute),
                nameof(EmptyTestFor_AfterTestHookThrowsException_ExecutionProceeds.TearDown),
                nameof(EmptyTestFor_AfterTestHookThrowsException_ExecutionProceeds.OneTimeTearDown)
            ]));
        }
    }
}
