// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.ExecutionHookTests;

namespace NUnit.Framework.Tests.ExecutionHooks.ExecutionSequence
{
    public class TestActionHooksTests
    {
        [Test]
        public void TestActionHooksCalledBeforeAndAfterTestAction()
        {
            TestLog.Clear();

            var workItem = TestBuilder.CreateWorkItem(typeof(TestClassWithTestAction));
            workItem.Execute();

            Assert.That(TestLog.Logs, Is.EqualTo([
                "BeforeTestActionBeforeTestHook(Suite)",
                $"{nameof(LogTestActionAttribute.BeforeTest)}(Suite)",
                "AfterTestActionBeforeTestHook(Suite)",

                "BeforeTestActionBeforeTestHook(Test)",
                $"{nameof(LogTestActionAttribute.BeforeTest)}(Test)",
                "AfterTestActionBeforeTestHook(Test)",

                nameof(TestClassWithTestAction.TestUnderTest),

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
