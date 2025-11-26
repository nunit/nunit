// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.ExecutionHooks;

namespace NUnit.Framework.Tests.ExecutionHooks.Execution
{
    public class OneTestWithLoggingHooksAndOneWithout
    {
        [Test]
        public void CheckLoggingTest()
        {
            var workItem = TestBuilder.CreateWorkItem(typeof(OneTestWithLoggingHooksAndOneWithoutFixture));
            workItem.Execute();
            var currentTestLogs = TestLog.Logs(workItem.Test);

            Assert.That(currentTestLogs, Is.Not.Empty);
            Assert.That(currentTestLogs, Is.EqualTo([
                $"{HookIdentifiers.BeforeTestHook}({nameof(OneTestWithLoggingHooksAndOneWithoutFixture.TestWithHookLogging)})",
                nameof(OneTestWithLoggingHooksAndOneWithoutFixture.TestWithHookLogging),
                $"{HookIdentifiers.AfterTestHook}({nameof(OneTestWithLoggingHooksAndOneWithoutFixture.TestWithHookLogging)})",
                nameof(OneTestWithLoggingHooksAndOneWithoutFixture.TestWithoutHookLogging)
            ]));
        }
    }
}
