// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnitLite;

namespace NUnit.Framework.Tests.HookExtension.Execution
{
    [TestFixture]
    internal class BeforeTestHookTests
    {
        internal class ActivateBeforeTestHooks : NUnitAttribute, IApplyToContext
        {
            public virtual void ApplyToContext(TestExecutionContext context)
            {
                context.HookExtension.BeforeTestHook.AddHandler((sender, eventArgs) =>
                {
                    TestLog.LogCurrentMethod();
                });
            }
        }

        [Explicit]
        [TestFixture]
        private class SomeEmptyTest
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
            [ActivateBeforeTestHooks]
            public void EmptyTest()
            {
                TestLog.LogCurrentMethod();
            }
        }

        [Test]
        public void ExecutionProceedsAfterBeforeTestHookCompletes()
        {
            TestLog.Logs.Clear();

            new AutoRun(typeof(SomeEmptyTest).Assembly).Execute([
                "--where",
                $"class == {typeof(SomeEmptyTest).FullName}"
            ]);
            Assert.That(TestLog.Logs, Is.EqualTo([
                nameof(SomeEmptyTest.OneTimeSetUp),
                nameof(SomeEmptyTest.SetUp),
                nameof(ActivateBeforeTestHooks.ApplyToContext),
                nameof(SomeEmptyTest.EmptyTest),
                nameof(SomeEmptyTest.TearDown),
                nameof(SomeEmptyTest.OneTimeTearDown)
            ]));
        }
    }
}
