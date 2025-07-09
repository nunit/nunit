// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.ExecutionHookTests;

namespace NUnit.Framework.Tests.ExecutionHooks.Execution
{
    public class OneTestWithLoggingHooksAndOneWithout
    {
        public class TestUnderTest
        {
            [Test, ActivateTestHook, Order(1)]
            public void TestWithHookLogging()
            {
                TestLog.LogCurrentMethod();
            }

            [Test, Order(2)]
            public void TestWithoutHookLogging()
            {
                TestLog.LogCurrentMethod();
            }
        }

        [Test]
        public void CheckLoggingTest()
        {
            TestLog.Clear();

            var workItem = TestBuilder.CreateWorkItem(typeof(TestUnderTest));
            workItem.Execute();

            Assert.That(TestLog.Logs, Is.EqualTo([
                $"{HookIdentifiers.BeforeTestHook}({nameof(TestUnderTest.TestWithHookLogging)})",
                nameof(TestUnderTest.TestWithHookLogging),
                $"{HookIdentifiers.AfterTestHook}({nameof(TestUnderTest.TestWithHookLogging)})",
                nameof(TestUnderTest.TestWithoutHookLogging)
            ]));
        }
    }
}
