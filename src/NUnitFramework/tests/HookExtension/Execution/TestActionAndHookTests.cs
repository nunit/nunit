// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.HookExtension.Execution
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class SomeTestActionAttribute : Attribute, ITestAction
    {
        public void BeforeTest(ITest test)
        {
            TestLog.LogCurrentMethod("BeforeTest_Action");
        }

        public void AfterTest(ITest test)
        {
            TestLog.LogCurrentMethod("AfterTest_Action");
        }

        public ActionTargets Targets { get; }
    }

    internal class ActivateAfterTestHooks : NUnitAttribute, IApplyToContext
    {
        public virtual void ApplyToContext(TestExecutionContext context)
        {
            context.HookExtension.AfterTestHook.AddHandler((sender, eventArgs) => { TestLog.LogCurrentMethod(); });
        }
    }

    internal class ActivateBeforeTestHooks : NUnitAttribute, IApplyToContext
    {
        public virtual void ApplyToContext(TestExecutionContext context)
        {
            context.HookExtension.BeforeTestHook.AddHandler((sender, eventArgs) => { TestLog.LogCurrentMethod(); });
        }
    }

    internal class TestActionAndHookTests
    {
        [TestFixture]
        [SomeTestAction]
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
            [ActivateAfterTestHooks]
            public void EmptyTest()
            {
                TestLog.LogCurrentMethod();
            }
        }

        [Test]
        public void ExecutionProceedsAfterTheAfterTestHookCompletes()
        {
            TestLog.Logs.Clear();

            var workItem = TestBuilder.CreateWorkItem(typeof(SomeEmptyTest));
            workItem.Execute();

            Assert.That(TestLog.Logs, Is.EqualTo([
                nameof(SomeEmptyTest.OneTimeSetUp),
                "BeforeTest_Action",
                nameof(SomeEmptyTest.SetUp),
                nameof(ActivateBeforeTestHooks.ApplyToContext),
                nameof(SomeEmptyTest.EmptyTest),
                nameof(ActivateAfterTestHooks.ApplyToContext),
                nameof(SomeEmptyTest.TearDown),
                "AfterTest_Action",
                nameof(SomeEmptyTest.OneTimeTearDown)
            ]));
        }
    }
}
