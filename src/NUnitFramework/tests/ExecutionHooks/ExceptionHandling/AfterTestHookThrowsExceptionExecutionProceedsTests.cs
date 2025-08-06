// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.ExecutionHookTests;

namespace NUnit.Framework.Tests.ExecutionHooks.ExceptionHandling
{
    internal class AfterTestHookThrowsExceptionExecutionProceedsTests
    {
        [Test]
        public void AfterTestHookThrowsException_ExecutionProceeds()
        {
            TestLog.Clear();

            var workItem = TestBuilder.CreateWorkItem(typeof(TestWithTestHooksOnMethodAndErrorOnAfterTestHook));
            workItem.Execute();

            Assert.That(TestLog.Logs, Is.EqualTo([
                nameof(TestWithTestHooksOnMethodAndErrorOnAfterTestHook.OneTimeSetUp),
                nameof(TestWithTestHooksOnMethodAndErrorOnAfterTestHook.SetUp),
                nameof(ActivateBeforeTestHookAttribute),
                nameof(TestWithTestHooksOnMethodAndErrorOnAfterTestHook.EmptyTest),
                nameof(ActivateAfterTestHookThrowingExceptionAttribute),
                nameof(TestWithTestHooksOnMethodAndErrorOnAfterTestHook.TearDown),
                nameof(TestWithTestHooksOnMethodAndErrorOnAfterTestHook.OneTimeTearDown)
            ]));
        }
    }
}
