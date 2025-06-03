// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.HookExtensionTests;

namespace NUnit.Framework.Tests.HookExtension.ExceptionHandling
{
    [TestFixture]
    internal class BeforeTestHookThrowsExceptionTestStopsAfterTestHookExecutesTests
    {
        [Test]
        public void BeforeTestHookThrowsException_TestStops_AfterTestHookExecutes()
        {
            TestLog.Logs.Clear();

            var workItem = TestBuilder.CreateWorkItem(typeof(EmptyTestFor_BeforeTestHookThrowsException_TestStops_AfterTestHookExecutes));
            workItem.Execute();

            // no test is executed
            Assert.That(TestLog.Logs, Is.EqualTo([
                nameof(EmptyTestFor_BeforeTestHookThrowsException_TestStops_AfterTestHookExecutes.OneTimeSetUp),
                nameof(EmptyTestFor_BeforeTestHookThrowsException_TestStops_AfterTestHookExecutes.SetUp),
                nameof(ActivateBeforeTestHookThrowingException.ApplyToContext),
                nameof(ActivateAfterTestHook.ApplyToContext),
                nameof(EmptyTestFor_BeforeTestHookThrowsException_TestStops_AfterTestHookExecutes.TearDown),
                nameof(EmptyTestFor_BeforeTestHookThrowsException_TestStops_AfterTestHookExecutes.OneTimeTearDown)
            ]));
        }
    }
}
