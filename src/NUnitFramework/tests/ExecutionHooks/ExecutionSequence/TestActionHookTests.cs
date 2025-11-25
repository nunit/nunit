// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.ExecutionHooks;

namespace NUnit.Framework.Tests.ExecutionHooks.ExecutionSequence
{
    public class TestActionHooksTests
    {
        [Test]
        public void TestActionHooksCalledBeforeAndAfterTestAction()
        {
            var workItem = TestBuilder.CreateWorkItem(typeof(TestActionHooksFixture));
            workItem.Execute();
            var currentTestLogs = TestLog.Logs(workItem.Test);

            Assert.That(currentTestLogs, Is.Not.Empty);
            Assert.That(currentTestLogs, Is.EqualTo([
                "BeforeTestActionBeforeTestHook(Suite)",
                $"{nameof(LogTestActionAttribute.BeforeTest)}(Suite)",
                "AfterTestActionBeforeTestHook(Suite)",

                "BeforeTestActionBeforeTestHook(Test)",
                $"{nameof(LogTestActionAttribute.BeforeTest)}(Test)",
                "AfterTestActionBeforeTestHook(Test)",

                nameof(TestActionHooksFixture.TestUnderTest),

                "BeforeTestActionAfterTestHook(Test)",
                $"{nameof(LogTestActionAttribute.AfterTest)}(Test)",
                "AfterTestActionAfterTestHook(Test)",

                "BeforeTestActionAfterTestHook(Suite)",
                $"{nameof(LogTestActionAttribute.AfterTest)}(Suite)",
                "AfterTestActionAfterTestHook(Suite)"
            ]));
        }
    }
}
