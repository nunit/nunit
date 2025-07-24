// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.ExecutionHooks.Common;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.ExecutionSequence
{
    internal class ExecutionSequenceWithAllPossibleHooks
    {
        private abstract class TestUnderTestBase
        {
            [OneTimeSetUp]
            public void OneTimeSetUpBase()
            {
                TestLog.LogCurrentMethod();
            }

            [SetUp]
            public void SetupBase()
            {
                TestLog.LogCurrentMethod();
            }

            [TearDown]
            public void TearDownBase()
            {
                TestLog.LogCurrentMethod();
            }

            [OneTimeTearDown]
            public void OneTimeTearDownBase()
            {
                TestLog.LogCurrentMethod();
            }
        }

        [TestFixture]
        [Explicit($"This test should only be run as part of the {nameof(TestProceedsAfterAllAfterTestHooksExecute)} test")]
        private sealed class TestUnderTest : TestUnderTestBase
        {
            [OneTimeSetUp]
            public void OneTimeSetUp()
            {
                TestLog.LogCurrentMethod();
            }

            [SetUp]
            public void Setup()
            {
                TestLog.LogCurrentMethod();
            }

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
            public void TearDown()
            {
                TestLog.LogCurrentMethod();
            }

            [OneTimeTearDown]
            public void OneTimeTearDown()
            {
                TestLog.LogCurrentMethod();
            }
        }

        [Test]
        public void TestProceedsAfterAllAfterTestHooksExecute()
        {
            TestLog.Clear();

            var workItem = TestBuilder.CreateWorkItem(typeof(TestUnderTest), TestFilter.Explicit);
            workItem.Execute();

            Assert.That(TestLog.Logs, Is.EqualTo([
                nameof(TestUnderTestBase.OneTimeSetUpBase),
                nameof(TestUnderTest.OneTimeSetUp),

                HookIdentifiers.BeforeEverySetUpHook,
                nameof(TestUnderTestBase.SetupBase),
                HookIdentifiers.AfterEverySetUpHook,

                HookIdentifiers.BeforeEverySetUpHook,
                nameof(TestUnderTest.Setup),
                HookIdentifiers.AfterEverySetUpHook,

                HookIdentifiers.BeforeTestHook,
                nameof(TestUnderTest.TestPasses),
                HookIdentifiers.AfterTestHook,

                HookIdentifiers.BeforeEveryTearDownHook,
                nameof(TestUnderTest.TearDown),
                HookIdentifiers.AfterEveryTearDownHook,

                HookIdentifiers.BeforeEveryTearDownHook,
                nameof(TestUnderTestBase.TearDownBase),
                HookIdentifiers.AfterEveryTearDownHook,

                HookIdentifiers.BeforeEverySetUpHook,
                nameof(TestUnderTestBase.SetupBase),
                HookIdentifiers.AfterEverySetUpHook,

                HookIdentifiers.BeforeEverySetUpHook,
                nameof(TestUnderTest.Setup),
                HookIdentifiers.AfterEverySetUpHook,

                HookIdentifiers.BeforeTestHook,
                nameof(TestUnderTest.TestFails),
                HookIdentifiers.AfterTestHook,

                HookIdentifiers.BeforeEveryTearDownHook,
                nameof(TestUnderTest.TearDown),
                HookIdentifiers.AfterEveryTearDownHook,

                HookIdentifiers.BeforeEveryTearDownHook,
                nameof(TestUnderTestBase.TearDownBase),
                HookIdentifiers.AfterEveryTearDownHook,

                nameof(TestUnderTest.OneTimeTearDown),
                nameof(TestUnderTestBase.OneTimeTearDownBase),
            ]));
        }
    }
}
