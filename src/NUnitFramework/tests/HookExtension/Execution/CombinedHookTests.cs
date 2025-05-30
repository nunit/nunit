// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Threading;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnitLite;

namespace NUnit.Framework.Tests.HookExtension.Execution
{
    internal class CombinedHookTests
    {
        internal class ActivateAfterTestHooks : NUnitAttribute, IApplyToContext
        {
            public virtual void ApplyToContext(TestExecutionContext context)
            {
                context?.HookExtension?.AfterTestHook.AddHandler((sender, eventArgs) =>
                {
                    TestLog.LogCurrentMethod();
                });
            }
        }

        internal class ActivateLongRunningAfterTestHooks : NUnitAttribute, IApplyToContext
        {
            public virtual void ApplyToContext(TestExecutionContext context)
            {
                context?.HookExtension?.AfterTestHook.AddHandler((sender, eventArgs) =>
                {
                    Thread.Sleep(500);
                    TestLog.LogCurrentMethod();
                });
            }
        }

        internal class ActivateBeforeTestHooks : NUnitAttribute, IApplyToContext
        {
            public virtual void ApplyToContext(TestExecutionContext context)
            {
                context?.HookExtension?.BeforeTestHook.AddHandler((sender, eventArgs) =>
                {
                    TestLog.LogCurrentMethod();
                });
            }
        }

        internal class ActivateLongRunningBeforeTestHooks : NUnitAttribute, IApplyToContext
        {
            public virtual void ApplyToContext(TestExecutionContext context)
            {
                context?.HookExtension?.BeforeTestHook.AddHandler((sender, eventArgs) =>
                {
                    Thread.Sleep(500);
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
            [ActivateLongRunningBeforeTestHooks]
            [ActivateAfterTestHooks]
            [ActivateLongRunningAfterTestHooks]
            public void EmptyTest()
            {
                TestLog.LogCurrentMethod();
            }
        }

        [Test]
        public void ExecutionProceedsAfterTheAfterTestHookCompletes()
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
                nameof(ActivateLongRunningBeforeTestHooks.ApplyToContext),
                nameof(SomeEmptyTest.EmptyTest),
                nameof(ActivateAfterTestHooks.ApplyToContext),
                nameof(ActivateLongRunningAfterTestHooks.ApplyToContext),
                nameof(SomeEmptyTest.TearDown),
                nameof(SomeEmptyTest.OneTimeTearDown)
            ]));
        }
    }
}
