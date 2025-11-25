// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;

namespace NUnit.TestData.ExecutionHooks
{
    public class AfterTestHookTestFixture
    {
        [OneTimeSetUp]
        public void OneTimeSetUp() => TestLog.LogCurrentMethod();

        [SetUp]
        public void SetUp() => TestLog.LogCurrentMethod();

        [Test]
        [ActivateAfterTestHook]
        public void EmptyTest() => TestLog.LogCurrentMethod();

        [TearDown]
        public void TearDown() => TestLog.LogCurrentMethod();

        [OneTimeTearDown]
        public void OneTimeTearDown() => TestLog.LogCurrentMethod();
    }

    public class AfterTestHookThrowsExceptionExecutionProceedsFixture
    {
        [OneTimeSetUp]
        public void OneTimeSetUp() => TestLog.LogCurrentMethod();

        [SetUp]
        public void SetUp() => TestLog.LogCurrentMethod();

        [Test]
        [ActivateBeforeTestHook]
        [ActivateAfterTestHookThrowingException]
        public void EmptyTest() => TestLog.LogCurrentMethod();

        [TearDown]
        public void TearDown() => TestLog.LogCurrentMethod();

        [OneTimeTearDown]
        public void OneTimeTearDown() => TestLog.LogCurrentMethod();
    }

    public class ExecutionProceedsOnlyAfterAllAfterTestHooksExecuteFixture
    {
        [Test]
        [ActivateAfterTestHook]
        [ActivateAfterTestHook]
        [ActivateAfterTestHook]
        [ActivateAfterTestHookThrowingException]
        public void TestPasses() => TestLog.LogCurrentMethod();

        [TearDown]
        public void TearDown() => TestLog.LogCurrentMethod();

        [OneTimeTearDown]
        public void OneTimeTearDown() => TestLog.LogCurrentMethod();
    }
}
