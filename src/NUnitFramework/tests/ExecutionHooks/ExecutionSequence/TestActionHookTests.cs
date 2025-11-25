// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.ExecutionSequence
{
    public class TestActionHooksTests
    {
        [Test]
        public void TestActionHooksCalledBeforeAndAfterTestAction()
        {
            var workItem = TestBuilder.CreateWorkItem(typeof(TestData.ExecutionHooks.TestActionHooksFixture), TestFilter.Explicit);
            workItem.Execute();
            var currentTestLogs = TestData.ExecutionHooks.TestLog.Logs(workItem.Test);

            Assert.That(currentTestLogs, Is.Not.Empty);
            Assert.That(currentTestLogs, Is.EqualTo([
                "BeforeTestActionBeforeTestHook(Suite)",
                $"{nameof(TestData.ExecutionHooks.LogTestActionAttribute.BeforeTest)}(Suite)",
                "AfterTestActionBeforeTestHook(Suite)",

                "BeforeTestActionBeforeTestHook(Test)",
                $"{nameof(TestData.ExecutionHooks.LogTestActionAttribute.BeforeTest)}(Test)",
                "AfterTestActionBeforeTestHook(Test)",

                nameof(TestData.ExecutionHooks.TestActionHooksFixture.TestUnderTest),

                "BeforeTestActionAfterTestHook(Test)",
                $"{nameof(TestData.ExecutionHooks.LogTestActionAttribute.AfterTest)}(Test)",
                "AfterTestActionAfterTestHook(Test)",

                "BeforeTestActionAfterTestHook(Suite)",
                $"{nameof(TestData.ExecutionHooks.LogTestActionAttribute.AfterTest)}(Suite)",
                "AfterTestActionAfterTestHook(Suite)"
            ]));
        }
    }
}
