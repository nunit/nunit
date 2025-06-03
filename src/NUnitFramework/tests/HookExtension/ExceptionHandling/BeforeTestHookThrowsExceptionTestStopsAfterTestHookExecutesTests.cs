// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnitLite;

namespace NUnit.Framework.Tests.HookExtension.ExceptionHandling
{
    [TestFixture]
    internal class BeforeTestHookThrowsExceptionTestStopsAfterTestHookExecutesTests
    {
        internal class ActivateBeforeTestHookThrowingException : NUnitAttribute, IApplyToContext
        {
            public virtual void ApplyToContext(TestExecutionContext context)
            {
                context.HookExtension.BeforeTestHook.AddHandler((sender, eventArgs) =>
                {
                    TestLog.LogCurrentMethod();
                    throw new Exception("Before test hook crashed!!");
                });
            }
        }

        internal class ActivateAfterTestHook : NUnitAttribute, IApplyToContext
        {
            public virtual void ApplyToContext(TestExecutionContext context)
            {
                context.HookExtension.AfterTestHook.AddHandler((sender, eventArgs) =>
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
            [ActivateBeforeTestHookThrowingException]
            [ActivateAfterTestHook]
            public void EmptyTest()
            {
                TestLog.LogCurrentMethod();
            }
        }

        [Test]
        public void BeforeTestHookThrowsException_TestStops_AfterTestHookExecutes()
        {
            TestLog.Logs.Clear();

            new AutoRun(typeof(SomeEmptyTest).Assembly).Execute([
                "--where",
                $"class == {typeof(SomeEmptyTest).FullName}"
            ]);

            // no test is executed
            Assert.That(TestLog.Logs, Is.EqualTo([
                nameof(SomeEmptyTest.OneTimeSetUp),
                nameof(SomeEmptyTest.SetUp),
                nameof(ActivateBeforeTestHookThrowingException.ApplyToContext),
                nameof(ActivateAfterTestHook.ApplyToContext),
                nameof(SomeEmptyTest.TearDown),
                nameof(SomeEmptyTest.OneTimeTearDown)
            ]));
        }
    }
}
