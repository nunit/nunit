// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.ExecutionHookTests;

namespace NUnit.Framework.Tests.ExecutionHooks.Execution
{
    internal class AfterTestHookTests
    {
        [Explicit($"This test should only be run as part of the {nameof(ExecutionProceedsAfterTheAfterTestHookCompletes)} test")]
        public class TestWithAfterTestHookOnMethod
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
            [ActivateAfterTestHook]
            public void EmptyTest()
            {
                TestLog.LogCurrentMethod();
            }
        }

        [Test]
        public void ExecutionProceedsAfterTheAfterTestHookCompletes()
        {
            TestLog.Clear();

            var workItem = TestBuilder.CreateWorkItem(typeof(TestWithAfterTestHookOnMethod), TestFilter.Explicit);
            workItem.Execute();

            Assert.That(TestLog.Logs, Is.EqualTo([
                nameof(TestWithAfterTestHookOnMethod.OneTimeSetUp),
                nameof(TestWithAfterTestHookOnMethod.SetUp),
                nameof(TestWithAfterTestHookOnMethod.EmptyTest),
                nameof(ActivateAfterTestHookAttribute),
                nameof(TestWithAfterTestHookOnMethod.TearDown),
                nameof(TestWithAfterTestHookOnMethod.OneTimeTearDown)
            ]));
        }
    }
}
