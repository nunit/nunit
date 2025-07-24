// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.ExecutionHookTests;

namespace NUnit.Framework.Tests.ExecutionHooks.Execution
{
    internal class CombinedHookAtClassAndMethodLevelTests
    {
        [Explicit($"This test should only be run as part of the {nameof(ExecutionProceedsAfterBothTestHookCompletes)} test")]
        [ActivateClassLevelBeforeTestHooks]
        [ActivateClassLevelAfterTestHooks]
        public class TestClassWithTestHooksOneTestWithoutAndOneWithMethodTestHooks
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
            [ActivateMethodLevelBeforeTestHooks]
            [ActivateMethodLevelAfterTestHooks]
            public void EmptyTestWithHooks()
            {
                TestLog.LogCurrentMethod();
            }

            [Test]
            public void EmptyTestWithoutHooks()
            {
                TestLog.LogCurrentMethod();
            }
        }

        [Test]
        public void ExecutionProceedsAfterBothTestHookCompletes()
        {
            TestLog.Clear();

            var workItem = TestBuilder.CreateWorkItem(typeof(TestClassWithTestHooksOneTestWithoutAndOneWithMethodTestHooks), TestFilter.Explicit);
            workItem.Execute();

            Assert.That(TestLog.Logs, Is.EqualTo([
                nameof(TestClassWithTestHooksOneTestWithoutAndOneWithMethodTestHooks.OneTimeSetUp),

                // Test with hooks starts
                nameof(TestClassWithTestHooksOneTestWithoutAndOneWithMethodTestHooks.SetUp),
                nameof(ActivateClassLevelBeforeTestHooksAttribute),
                nameof(ActivateMethodLevelBeforeTestHooksAttribute),

                nameof(TestClassWithTestHooksOneTestWithoutAndOneWithMethodTestHooks.EmptyTestWithHooks),

                nameof(ActivateMethodLevelAfterTestHooksAttribute),
                nameof(ActivateClassLevelAfterTestHooksAttribute),
                nameof(TestClassWithTestHooksOneTestWithoutAndOneWithMethodTestHooks.TearDown),
                // Test with hooks ends

                // Test without hooks starts
                nameof(TestClassWithTestHooksOneTestWithoutAndOneWithMethodTestHooks.SetUp),
                nameof(ActivateClassLevelBeforeTestHooksAttribute),

                nameof(TestClassWithTestHooksOneTestWithoutAndOneWithMethodTestHooks.EmptyTestWithoutHooks),

                nameof(ActivateClassLevelAfterTestHooksAttribute),
                nameof(TestClassWithTestHooksOneTestWithoutAndOneWithMethodTestHooks.TearDown),
                // Test without hooks ends

                nameof(TestClassWithTestHooksOneTestWithoutAndOneWithMethodTestHooks.OneTimeTearDown)
            ]));
        }
    }
}
