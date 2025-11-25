// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.Execution
{
    public class OneTestWithLoggingHooksAndOneWithout
    {
        [Test]
        public void CheckLoggingTest()
        {
            var workItem = TestBuilder.CreateWorkItem(typeof(TestData.ExecutionHooks.OneTestWithLoggingHooksAndOneWithoutFixture), TestFilter.Explicit);
            workItem.Execute();
            var currentTestLogs = TestData.ExecutionHooks.TestLog.Logs(workItem.Test);

            Assert.That(currentTestLogs, Is.Not.Empty);
            Assert.That(currentTestLogs, Is.EqualTo([
                $"{TestData.ExecutionHooks.HookIdentifiers.BeforeTestHook}({nameof(TestData.ExecutionHooks.OneTestWithLoggingHooksAndOneWithoutFixture.TestWithHookLogging)})",
                nameof(TestData.ExecutionHooks.OneTestWithLoggingHooksAndOneWithoutFixture.TestWithHookLogging),
                $"{TestData.ExecutionHooks.HookIdentifiers.AfterTestHook}({nameof(TestData.ExecutionHooks.OneTestWithLoggingHooksAndOneWithoutFixture.TestWithHookLogging)})",
                nameof(TestData.ExecutionHooks.OneTestWithLoggingHooksAndOneWithoutFixture.TestWithoutHookLogging)
            ]));
        }
    }
}
