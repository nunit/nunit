// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.ExecutionHookTests;

namespace NUnit.Framework.Tests.ExecutionHooks.Execution
{
    internal class TestActionAndHookTests
    {
        [Explicit($"This test should only be run as part of the {nameof(ExecutionProceedsAfterTheAfterTestHookCompletes2)} test")]
        [SomeTestAction]
        public class TestWithTestHooksAndClassTestActionAttribute
        {
            [OneTimeSetUp]
            public void OneTimeSetUp()
            {
                TestLog.LogCurrentMethod();
            }

            [OneTimeTearDown]
            public void OneTimeTearDown()
            {
                TestLog.LogCurrentMethod();
            }

            [SetUp]
            public void SetUp()
            {
                TestLog.LogCurrentMethod();
            }

            [TearDown]
            public void TearDown()
            {
                TestLog.LogCurrentMethod();
            }

            [Test]
            [ActivateBeforeTestHook]
            [ActivateAfterTestHook]
            public void EmptyTest()
            {
                TestLog.LogCurrentMethod();
            }
        }

        [Test]
        public void ExecutionProceedsAfterTheAfterTestHookCompletes2()
        {
            TestLog.Clear();

            var workItem = TestBuilder.CreateWorkItem(typeof(TestWithTestHooksAndClassTestActionAttribute), TestFilter.Explicit);
            workItem.Execute();

            Assert.That(TestLog.Logs, Is.EqualTo([
                nameof(TestWithTestHooksAndClassTestActionAttribute.OneTimeSetUp),
                SomeTestActionAttribute.LogStringForBeforeTest,
                nameof(TestWithTestHooksAndClassTestActionAttribute.SetUp),
                nameof(ActivateBeforeTestHookAttribute),
                nameof(TestWithTestHooksAndClassTestActionAttribute.EmptyTest),
                nameof(ActivateAfterTestHookAttribute),
                nameof(TestWithTestHooksAndClassTestActionAttribute.TearDown),
                SomeTestActionAttribute.LogStringForAfterTest,
                nameof(TestWithTestHooksAndClassTestActionAttribute.OneTimeTearDown)
            ]));
        }
    }
}
