// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.ExecutionHooks;

namespace NUnit.Framework.Tests.ExecutionHooks.ExceptionHandling
{
    internal class BeforeTestHookThrowsExceptionTestStopsAfterTestHookExecutesTests
    {
        [Test]
        public void BeforeTestHookThrowsException_TestStops_AfterTestHookExecutes()
        {
            var workItem =
                TestBuilder.CreateWorkItem(typeof(BeforeTestHookThrowsExceptionTestStopsAfterTestHookExecutesFixture));
            workItem.Execute();
            var currentTestLogs = TestLog.Logs(workItem.Test);

            Assert.That(currentTestLogs, Is.Not.Empty);
            Assert.That(currentTestLogs, Is.EqualTo([
                nameof(BeforeTestHookThrowsExceptionTestStopsAfterTestHookExecutesFixture.OneTimeSetUp),
                nameof(BeforeTestHookThrowsExceptionTestStopsAfterTestHookExecutesFixture.SetUp),
                nameof(ActivateBeforeTestHookThrowingExceptionAttribute),
                nameof(ActivateAfterTestHookAtMethodLevelAttribute),
                nameof(BeforeTestHookThrowsExceptionTestStopsAfterTestHookExecutesFixture.TearDown),
                nameof(BeforeTestHookThrowsExceptionTestStopsAfterTestHookExecutesFixture.OneTimeTearDown)
            ]));
        }
    }
}
