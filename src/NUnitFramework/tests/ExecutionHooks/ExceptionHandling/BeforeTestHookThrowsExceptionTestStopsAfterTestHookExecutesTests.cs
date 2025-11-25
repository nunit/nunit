// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.ExceptionHandling
{
    internal class BeforeTestHookThrowsExceptionTestStopsAfterTestHookExecutesTests
    {
        [Test]
        public void BeforeTestHookThrowsException_TestStops_AfterTestHookExecutes()
        {
            var workItem = TestBuilder.CreateWorkItem(typeof(TestData.ExecutionHooks.BeforeTestHookThrowsExceptionTestStopsAfterTestHookExecutesFixture), TestFilter.Explicit);
            workItem.Execute();
            var currentTestLogs = TestData.ExecutionHooks.TestLog.Logs(workItem.Test);

            Assert.That(currentTestLogs, Is.Not.Empty);
            Assert.That(currentTestLogs, Is.EqualTo([
                nameof(TestData.ExecutionHooks.BeforeTestHookThrowsExceptionTestStopsAfterTestHookExecutesFixture.OneTimeSetUp),
                nameof(TestData.ExecutionHooks.BeforeTestHookThrowsExceptionTestStopsAfterTestHookExecutesFixture.SetUp),
                nameof(TestData.ExecutionHooks.ActivateBeforeTestHookThrowingExceptionAttribute),
                nameof(TestData.ExecutionHooks.ActivateAfterTestHookAttribute),
                nameof(TestData.ExecutionHooks.BeforeTestHookThrowsExceptionTestStopsAfterTestHookExecutesFixture.TearDown),
                nameof(TestData.ExecutionHooks.BeforeTestHookThrowsExceptionTestStopsAfterTestHookExecutesFixture.OneTimeTearDown)
            ]));
        }
    }
}
