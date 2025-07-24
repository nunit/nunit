// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.ExecutionHooks.TestAttributes;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.ExecutionSequence
{
    public class TestActionHooksTests
    {
        [Explicit($"This test should only be run as part of the {nameof(TestActionHooksCalledBeforeAndAfterTestAction)} test")]
        [LogTestAction]
        [TestActionLoggingExecutionHooks]
        public class TestClassWithTestAction
        {
            [Test]
            public void TestUnderTest() => TestLog.LogCurrentMethod();
        }

        [Test]
        public void TestActionHooksCalledBeforeAndAfterTestAction()
        {
            var workItem = TestBuilder.CreateWorkItem(typeof(TestClassWithTestAction), TestFilter.Explicit);
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
