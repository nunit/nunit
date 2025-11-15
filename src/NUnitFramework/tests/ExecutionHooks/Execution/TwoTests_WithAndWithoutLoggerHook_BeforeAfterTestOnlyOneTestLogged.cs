// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.ExecutionHooks.TestAttributes;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.Execution
{
    public class OneTestWithLoggingHooksAndOneWithout
    {
        [Explicit($"This test should only be run as part of the {nameof(CheckLoggingTest)} test")]
        public class TestUnderTest
        {
            [Test, ActivateTestHook, Order(1)]
            public void TestWithHookLogging() => TestLog.LogCurrentMethod();

            [Test, Order(2)]
            public void TestWithoutHookLogging() => TestLog.LogCurrentMethod();
        }

        [Test]
        public void CheckLoggingTest()
        {
            var workItem = TestBuilder.CreateWorkItem(typeof(TestUnderTest), TestFilter.Explicit);
            workItem.Execute();
            var currentTestLogs = TestLog.Logs(workItem.Test);

            Assert.That(currentTestLogs, Is.Not.Empty);
            Assert.That(currentTestLogs, Is.EqualTo([
                $"{HookIdentifiers.BeforeTestHook}({nameof(TestUnderTest.TestWithHookLogging)})",
                nameof(TestUnderTest.TestWithHookLogging),
                $"{HookIdentifiers.AfterTestHook}({nameof(TestUnderTest.TestWithHookLogging)})",
                nameof(TestUnderTest.TestWithoutHookLogging)
            ]));
        }
    }
}
