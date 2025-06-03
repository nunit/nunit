// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnitLite;

namespace NUnit.Framework.Tests.HookExtension.ExceptionHandling
{
    [TestFixture]
    internal class AfterTestHookThrowsExceptionExecutionProceedsTests
    {
        internal class ActivateBeforeTestHook : NUnitAttribute, IApplyToContext
        {
            public virtual void ApplyToContext(TestExecutionContext context)
            {
                context?.HookExtension.BeforeTestHook.AddHandler((sender, eventArgs) =>
                {
                    TestLog.LogCurrentMethod();
                });
            }
        }

        internal class ActivateAfterTestHookThrowingException : NUnitAttribute, IApplyToContext
        {
            public virtual void ApplyToContext(TestExecutionContext context)
            {
                context?.HookExtension.AfterTestHook.AddHandler((sender, eventArgs) =>
                {
                    TestLog.LogCurrentMethod();
                    throw new Exception("After test hook crashed!!");
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
            [ActivateBeforeTestHook]
            [ActivateAfterTestHookThrowingException]
            public void EmptyTest()
            {
                TestLog.LogCurrentMethod();
            }
        }

        [Test]
        public void AfterTestHookThrowsException_ExecutionProceeds()
        {
            TestLog.Logs.Clear();

            new AutoRun(typeof(SomeEmptyTest).Assembly).Execute([
                "--where",
                $"class == {typeof(SomeEmptyTest).FullName}"
            ]);
            Assert.That(TestLog.Logs, Is.EqualTo([
                nameof(SomeEmptyTest.OneTimeSetUp),
                nameof(SomeEmptyTest.SetUp),
                nameof(ActivateBeforeTestHook.ApplyToContext),
                nameof(SomeEmptyTest.EmptyTest),
                nameof(ActivateAfterTestHookThrowingException.ApplyToContext),
                nameof(SomeEmptyTest.TearDown),
                nameof(SomeEmptyTest.OneTimeTearDown)
            ]));
        }
    }
}
