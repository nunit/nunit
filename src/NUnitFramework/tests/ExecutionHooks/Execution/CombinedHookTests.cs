// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.ExecutionHookTests;

namespace NUnit.Framework.Tests.ExecutionHooks.Execution
{
    internal class CombinedHookTests
    {
        [Test]
        public void ExecutionProceedsAfterBothTestHookCompletes()
        {
            TestLog.Clear();

            var workItem = TestBuilder.CreateWorkItem(typeof(TestWithNormalAndLongRunningTestHooks));
            workItem.Execute();

            Assert.That(TestLog.Logs, Is.EqualTo([
                nameof(TestWithNormalAndLongRunningTestHooks.OneTimeSetUp),
                nameof(TestWithNormalAndLongRunningTestHooks.SetUp),
                nameof(ActivateLongRunningBeforeTestHookAttribute),
                nameof(ActivateBeforeTestHookAttribute),
                nameof(TestWithNormalAndLongRunningTestHooks.EmptyTest),
                nameof(ActivateLongRunningAfterTestHookAttribute),
                nameof(ActivateAfterTestHookAttribute),
                nameof(TestWithNormalAndLongRunningTestHooks.TearDown),
                nameof(TestWithNormalAndLongRunningTestHooks.OneTimeTearDown)
            ]));
        }
    }
}
