// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.ExecutionHookTests;

namespace NUnit.Framework.Tests.ExecutionHooks.ExceptionHandling
{
    internal class BeforeTestHookThrowsExceptionTestStopsAfterTestHookExecutesTests
    {
        [Test]
        public void BeforeTestHookThrowsException_TestStops_AfterTestHookExecutes()
        {
            TestLog.Clear();

            var workItem = TestBuilder.CreateWorkItem(typeof(TestWithTestHooksOnMethodAndErrorOnBeforeTestHook));
            workItem.Execute();

            // no test is executed
            Assert.That(TestLog.Logs, Is.EqualTo([
                nameof(TestWithTestHooksOnMethodAndErrorOnBeforeTestHook.OneTimeSetUp),
                nameof(TestWithTestHooksOnMethodAndErrorOnBeforeTestHook.SetUp),
                nameof(ActivateBeforeTestHookThrowingExceptionAttribute),
                nameof(ActivateAfterTestHookAttribute),
                nameof(TestWithTestHooksOnMethodAndErrorOnBeforeTestHook.TearDown),
                nameof(TestWithTestHooksOnMethodAndErrorOnBeforeTestHook.OneTimeTearDown)
            ]));
        }
    }
}
