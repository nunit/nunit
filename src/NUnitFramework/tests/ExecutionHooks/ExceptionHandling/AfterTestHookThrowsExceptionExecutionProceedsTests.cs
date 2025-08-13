// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.ExecutionHooks.TestAttributes;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.ExceptionHandling
{
    internal class AfterTestHookThrowsExceptionExecutionProceedsTests
    {
        [Explicit($"This test should only be run as part of the {nameof(AfterTestHookThrowsException_ExecutionProceeds)} test")]
        public class TestWithTestHooksOnMethodAndErrorOnAfterTestHook
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
            [ActivateBeforeTestHook]
            [ActivateAfterTestHookThrowingException]
            public void EmptyTest()
            {
                TestLog.LogCurrentMethod();
            }
        }

        [Test]
        public void AfterTestHookThrowsException_ExecutionProceeds()
        {
            TestLog.Clear();

            var workItem = TestBuilder.CreateWorkItem(typeof(TestWithTestHooksOnMethodAndErrorOnAfterTestHook), TestFilter.Explicit);
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
