// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.ExecutionHooks.TestAttributes;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.Execution
{
    internal class CombinedHookTests
    {
        [Explicit($"This test should only be run as part of the {nameof(ExecutionProceedsAfterBothTestHookCompletes)} test")]
        public class TestWithNormalAndLongRunningTestHooks
        {
            [OneTimeSetUp]
            public void OneTimeSetUp() => TestLog.LogCurrentMethod();

            [SetUp]
            public void SetUp() => TestLog.LogCurrentMethod();

            [Test]
            [ActivateBeforeTestHook]
            [ActivateLongRunningBeforeTestHook]
            [ActivateAfterTestHook]
            [ActivateLongRunningAfterTestHook]
            public void EmptyTest() => TestLog.LogCurrentMethod();

            [TearDown]
            public void TearDown() => TestLog.LogCurrentMethod();

            [OneTimeTearDown]
            public void OneTimeTearDown() => TestLog.LogCurrentMethod();
        }

        [Test]
        public void ExecutionProceedsAfterBothTestHookCompletes()
        {
            var workItem =
                TestBuilder.CreateWorkItem(typeof(TestWithNormalAndLongRunningTestHooks), TestFilter.Explicit);
            workItem.Execute();
            var currentTestLogs = TestLog.Logs(workItem.Test);

            Assert.That(currentTestLogs, Is.Not.Empty);
            Assert.That(currentTestLogs, Is.EqualTo([
                nameof(TestWithNormalAndLongRunningTestHooks.OneTimeSetUp),
                nameof(TestWithNormalAndLongRunningTestHooks.SetUp),

                nameof(ActivateLongRunningBeforeTestHookAttribute),
                nameof(ActivateBeforeTestHookAttribute),

                nameof(TestWithNormalAndLongRunningTestHooks.EmptyTest),

                nameof(ActivateAfterTestHookAttribute),
                nameof(ActivateLongRunningAfterTestHookAttribute),

                nameof(TestWithNormalAndLongRunningTestHooks.TearDown),
                nameof(TestWithNormalAndLongRunningTestHooks.OneTimeTearDown)
            ]));
        }
    }
}
