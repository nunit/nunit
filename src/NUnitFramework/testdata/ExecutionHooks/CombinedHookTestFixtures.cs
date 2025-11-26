// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;

namespace NUnit.TestData.ExecutionHooks
{
    public class CombinedHookTestsFixture
    {
        [OneTimeSetUp]
        public void OneTimeSetUp() => TestLog.LogCurrentMethod();

        [SetUp]
        public void SetUp() => TestLog.LogCurrentMethod();

        [Test]
        [ActivateBeforeTestHookAtMethodLevel]
        [ActivateLongRunningBeforeTestHook]
        [ActivateAfterTestHookAtMethodLevel]
        [ActivateLongRunningAfterTestHook]
        public void EmptyTest() => TestLog.LogCurrentMethod();

        [TearDown]
        public void TearDown() => TestLog.LogCurrentMethod();

        [OneTimeTearDown]
        public void OneTimeTearDown() => TestLog.LogCurrentMethod();
    }

    [ActivateBeforeTestHookAtClassLevel]
    [ActivateAfterTestHookAtClassLevel]
    public class CombinedHookAtClassAndMethodLevelTestsFixture
    {
        [OneTimeSetUp]
        public void OneTimeSetUp() => TestLog.LogCurrentMethod();

        [SetUp]
        public void SetUp() => TestLog.LogCurrentMethod();

        [Test]
        [ActivateBeforeTestHookAtMethodLevel]
        [ActivateAfterTestHookAtMethodLevel]
        public void EmptyTestWithHooks() => TestLog.LogCurrentMethod();

        [Test]
        public void EmptyTestWithoutHooks() => TestLog.LogCurrentMethod();

        [TearDown]
        public void TearDown() => TestLog.LogCurrentMethod();

        [OneTimeTearDown]
        public void OneTimeTearDown() => TestLog.LogCurrentMethod();
    }

    public class OneTestWithLoggingHooksAndOneWithoutFixture
    {
        [Test, ActivateTestHook, Order(1)]
        public void TestWithHookLogging() => TestLog.LogCurrentMethod();

        [Test, Order(2)]
        public void TestWithoutHookLogging() => TestLog.LogCurrentMethod();
    }

    public class TestThrowsExceptionHooksProceedsToExecuteFixture
    {
        [OneTimeSetUp]
        public void OneTimeSetUp() => TestLog.LogCurrentMethod();

        [SetUp]
        public void SetUp() => TestLog.LogCurrentMethod();

        [Test]
        [ActivateBeforeTestHookAtMethodLevel]
        [ActivateAfterTestHookAtMethodLevel]
        public void EmptyTest() => TestLog.LogCurrentMethod();

        [TearDown]
        public void TearDown() => TestLog.LogCurrentMethod();

        [OneTimeTearDown]
        public void OneTimeTearDown() => TestLog.LogCurrentMethod();
    }

    public class TestFailsWithAssertHooksProceedsToExecuteFixture
    {
        [OneTimeSetUp]
        public void OneTimeSetUp() => TestLog.LogCurrentMethod();

        [SetUp]
        public void SetUp() => TestLog.LogCurrentMethod();

        [Test]
        [ActivateBeforeTestHookAtMethodLevel]
        [ActivateAfterTestHookAtMethodLevel]
        public void EmptyTest()
        {
            TestLog.LogCurrentMethod();
            Assert.Fail("Some failure in test");
        }

        [TearDown]
        public void TearDown() => TestLog.LogCurrentMethod();

        [OneTimeTearDown]
        public void OneTimeTearDown() => TestLog.LogCurrentMethod();
    }

    [SimpleTestAction]
    public class TestActionAndHookCombinationFixture
    {
        [OneTimeSetUp]
        public void OneTimeSetUp() => TestLog.LogCurrentMethod();

        [SetUp]
        public void SetUp() => TestLog.LogCurrentMethod();

        [Test]
        [ActivateBeforeTestHookAtMethodLevel]
        [ActivateAfterTestHookAtMethodLevel]
        public void EmptyTest() => TestLog.LogCurrentMethod();

        [TearDown]
        public void TearDown() => TestLog.LogCurrentMethod();

        [OneTimeTearDown]
        public void OneTimeTearDown() => TestLog.LogCurrentMethod();
    }
}
