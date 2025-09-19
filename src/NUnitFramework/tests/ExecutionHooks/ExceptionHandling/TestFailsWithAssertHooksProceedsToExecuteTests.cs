// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.ExecutionHooks.TestAttributes;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.ExceptionHandling
{
    internal class TestFailsWithAssertHooksProceedsToExecuteTests
    {
        [Explicit($"This test should only be run as part of the {nameof(TestFailsWithAssert_HooksProceedsToExecute)} test")]
        public class FailingTestWithTestHookOnMethod
        {
            [OneTimeSetUp]
            public void OneTimeSetUp() => TestLog.LogCurrentMethod();

            [SetUp]
            public void SetUp() => TestLog.LogCurrentMethod();

            [Test]
            [ActivateBeforeTestHook]
            [ActivateAfterTestHook]
            public void EmptyTest()
            {
                TestLog.LogCurrentMethod();
                Assert.Fail("Some failure in test");
            }

            [TearDown]
            public void TearDown() => TestLog.LogCurrentMethod();

            [OneTimeTearDown]
            public void OneTimeTearDown() => TestLog.LogCurrentMethod();
        }

        [Test]
        public void TestFailsWithAssert_HooksProceedsToExecute()
        {
            TestLog.Clear();

            var workItem = TestBuilder.CreateWorkItem(typeof(FailingTestWithTestHookOnMethod), TestFilter.Explicit);
            workItem.Execute();

            Assert.That(TestLog.Logs, Is.EqualTo([
                nameof(FailingTestWithTestHookOnMethod.OneTimeSetUp),
                nameof(FailingTestWithTestHookOnMethod.SetUp),
                nameof(ActivateBeforeTestHookAttribute),
                nameof(FailingTestWithTestHookOnMethod.EmptyTest),
                nameof(ActivateAfterTestHookAttribute),
                nameof(FailingTestWithTestHookOnMethod.TearDown),
                nameof(FailingTestWithTestHookOnMethod.OneTimeTearDown)
            ]));
        }
    }
}
