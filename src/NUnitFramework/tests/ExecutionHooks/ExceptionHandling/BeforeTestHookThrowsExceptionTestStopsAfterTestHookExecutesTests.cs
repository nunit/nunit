// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.ExecutionHooks.TestAttributes;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.ExceptionHandling
{
    internal class BeforeTestHookThrowsExceptionTestStopsAfterTestHookExecutesTests
    {
        [Explicit($"This test should only be run as part of the {nameof(BeforeTestHookThrowsException_TestStops_AfterTestHookExecutes)} test")]
        public class TestWithTestHooksOnMethodAndErrorOnBeforeTestHook
        {
            [OneTimeSetUp]
            public void OneTimeSetUp()
            {
                TestLog.LogCurrentMethod();
            }

            [OneTimeTearDown]
            public void OneTimeTearDown()
            {
                TestLog.LogCurrentMethod();
            }

            [SetUp]
            public void SetUp()
            {
                TestLog.LogCurrentMethod();
            }

            [TearDown]
            public void TearDown()
            {
                TestLog.LogCurrentMethod();
            }

            [Test]
            [ActivateBeforeTestHookThrowingException]
            [ActivateAfterTestHook]
            public void EmptyTest()
            {
                TestLog.LogCurrentMethod();
            }
        }

        [Test]
        public void BeforeTestHookThrowsException_TestStops_AfterTestHookExecutes()
        {
            TestLog.Clear();

            var workItem = TestBuilder.CreateWorkItem(typeof(TestWithTestHooksOnMethodAndErrorOnBeforeTestHook), TestFilter.Explicit);
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
