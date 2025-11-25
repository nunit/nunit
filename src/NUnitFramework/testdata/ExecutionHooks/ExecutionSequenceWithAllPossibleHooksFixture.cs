// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;

namespace NUnit.TestData.ExecutionHooks
{
    public abstract class ExecutionSequenceWithAllPossibleHooksFixtureBase
    {
        [OneTimeSetUp]
        public void OneTimeSetUpBase() => TestLog.LogCurrentMethod();

        [SetUp]
        public void SetupBase() => TestLog.LogCurrentMethod();

        [TearDown]
        public void TearDownBase() => TestLog.LogCurrentMethod();

        [OneTimeTearDown]
        public void OneTimeTearDownBase() => TestLog.LogCurrentMethod();
    }

    public class ExecutionSequenceWithAllPossibleHooksFixture : ExecutionSequenceWithAllPossibleHooksFixtureBase
    {
        [OneTimeSetUp]
        public void OneTimeSetUp() => TestLog.LogCurrentMethod();

        [SetUp]
        public void Setup() => TestLog.LogCurrentMethod();

        [Test]
        [ActivateAllSynchronousTestHooks]
        public void TestPasses()
        {
            TestLog.LogCurrentMethod();
            Assert.Pass("AfterHooks execute.");
        }

        [Test]
        [ActivateAllSynchronousTestHooks]
        public void TestFails()
        {
            TestLog.LogCurrentMethod();
            Assert.Fail("AfterHooks should still execute even if the test fails.");
        }

        [TearDown]
        public void TearDown() => TestLog.LogCurrentMethod();

        [OneTimeTearDown]
        public void OneTimeTearDown() => TestLog.LogCurrentMethod();
    }
}
