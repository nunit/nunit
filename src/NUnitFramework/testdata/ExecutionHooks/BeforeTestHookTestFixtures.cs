// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;

namespace NUnit.TestData.ExecutionHooks
{
    public class BeforeTestHookTestsFixture
    {
        [OneTimeSetUp]
        public void OneTimeSetUp() => TestLog.LogCurrentMethod();

        [SetUp]
        public void SetUp() => TestLog.LogCurrentMethod();

        [Test]
        [ActivateBeforeTestHook]
        public void EmptyTest() => TestLog.LogCurrentMethod();

        [TearDown]
        public void TearDown() => TestLog.LogCurrentMethod();

        [OneTimeTearDown]
        public void OneTimeTearDown() => TestLog.LogCurrentMethod();
    }

    public class BeforeTestHookThrowsExceptionTestStopsAfterTestHookExecutesFixture
    {
        [OneTimeSetUp]
        public void OneTimeSetUp() => TestLog.LogCurrentMethod();

        [SetUp]
        public void SetUp() => TestLog.LogCurrentMethod();

        [Test]
        [ActivateBeforeTestHookThrowingException]
        [ActivateAfterTestHook]
        public void EmptyTest()
        {
            TestLog.LogCurrentMethod();
        }

        [TearDown]
        public void TearDown() => TestLog.LogCurrentMethod();

        [OneTimeTearDown]
        public void OneTimeTearDown() => TestLog.LogCurrentMethod();
    }

    public class ExecutionProceedsOnlyAfterAllBeforeTestHooksExecuteFixture
    {
        [Test]
        [ActivateBeforeTestHook]
        [ActivateBeforeTestHook]
        [ActivateBeforeTestHook]
        [ActivateLongRunningBeforeTestHook]
        public void SomeTest() => TestLog.LogCurrentMethod();
    }
}
