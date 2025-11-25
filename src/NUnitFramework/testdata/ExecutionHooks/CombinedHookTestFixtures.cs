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
        [ActivateBeforeTestHook]
        [ActivateLongRunningBeforeTestHook]
        [ActivateAfterTestHook]
        [ActivateLongRunningAfterTestHook]
        public void EmptyTest() => TestLog.LogCurrentMethod();

        [TearDown]
        public void TearDown() => TestLog.LogCurrentMethod();

        [OneTimeTearDown]
        public void OneTimeTearDown() => TestLog.LogCurrentMethod();
    }

    [ActivateBeforeTestHook]
    [ActivateAfterTestHook]
    public class CombinedHookAtClassAndMethodLevelTestsFixture
    {
        [OneTimeSetUp]
        public void OneTimeSetUp() => TestLog.LogCurrentMethod();

        [SetUp]
        public void SetUp() => TestLog.LogCurrentMethod();

        [Test]
        [ActivateBeforeTestHook]
        [ActivateAfterTestHook]
        public void EmptyTestWithHooks() => TestLog.LogCurrentMethod();

        [Test]
        public void EmptyTestWithoutHooks() => TestLog.LogCurrentMethod();

        [TearDown]
        public void TearDown() => TestLog.LogCurrentMethod();

        [OneTimeTearDown]
        public void OneTimeTearDown() => TestLog.LogCurrentMethod();
    }
}
