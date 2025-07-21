// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.ExecutionHookTests;

namespace NUnit.Framework.Tests.ExecutionHooks.ExecutionSequence
{
    internal class ExecutionSequenceWithAllPossibleHooks
    {
        [TestFixture]
        public class TestUnderTestBase
        {
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
        }

        [TestFixture]
        public class TestUnderTest : TestUnderTestBase
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

            var workItem = TestBuilder.CreateWorkItem(typeof(TestUnderTest));
            workItem.Execute();

            Assert.That(TestLog.Logs, Is.EqualTo([
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

                nameof(TestUnderTest.OneTimeTearDown)
            ]));
        }
    }
}
