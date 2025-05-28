// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;
using NUnit.Framework.Internal.Execution;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.HookExtension.Creation
{
    [TestFixture]
    internal class TestHooksCreationAtAssemblyLevelTests
    {
        [AttributeUsage(AttributeTargets.Assembly)]
        internal class ActivateBeforeTestHooks : NUnitAttribute, IApplyToContext
        {
            public virtual void ApplyToContext(TestExecutionContext context)
            {
                context?.HookExtension?.BeforeTestHook.AddHandler((sender, eventArgs) => { });
            }
        }

        [AttributeUsage(AttributeTargets.Assembly)]
        internal class ActivateAfterTestHooks : NUnitAttribute, IApplyToContext
        {
            public virtual void ApplyToContext(TestExecutionContext context)
            {
                context?.HookExtension?.AfterTestHook.AddHandler((sender, eventArgs) => { });
            }
        }

        [Explicit]
        [TestFixture]
        private class SomeEmptyTest
        {
            [Test]
            public void EmptyTest()
            {
            }
        }

        [Test]
        public void NewHooksAppliedToContext()
        {
            var test = TestBuilder.MakeTestFromMethod(typeof(SomeEmptyTest), nameof(SomeEmptyTest.EmptyTest));
            var context = new TestExecutionContext();

            // Simulate "assembly-level"
            var hookAttribute = new ActivateBeforeTestHooks();
            hookAttribute.ApplyToContext(context);

            var work = TestBuilder.CreateWorkItem(test, context) as SimpleWorkItem;
            Assert.That(work, Is.Not.Null);

            var command = work.MakeTestCommand();
            Assert.That(command, Is.TypeOf(typeof(HookDelegatingTestCommand)));
        }

        [Test]
        public void BeforeTestHookAdded()
        {
            var test = TestBuilder.MakeTestFromMethod(typeof(SomeEmptyTest), nameof(SomeEmptyTest.EmptyTest));
            var context = new TestExecutionContext();

            // Simulate "assembly-level"
            var hookAttribute = new ActivateBeforeTestHooks();
            hookAttribute.ApplyToContext(context);

            var work = TestBuilder.CreateWorkItem(test, context) as SimpleWorkItem;
            Assert.That(work, Is.Not.Null);
            Assert.That(work.Context.HookExtension, Is.Not.Null);
            Assert.That(work.Context.HookExtension.BeforeTestHook, Is.Not.Null);
        }

        [Test]
        public void AfterTestHookAdded()
        {
            var test = TestBuilder.MakeTestFromMethod(typeof(SomeEmptyTest), nameof(SomeEmptyTest.EmptyTest));
            var context = new TestExecutionContext();

            // Simulate "assembly-level"
            var hookAttribute = new ActivateBeforeTestHooks();
            hookAttribute.ApplyToContext(context);

            var work = TestBuilder.CreateWorkItem(test, context) as SimpleWorkItem;
            Assert.That(work, Is.Not.Null);
            Assert.That(work.Context.HookExtension, Is.Not.Null);
            Assert.That(work.Context.HookExtension.AfterTestHook, Is.Not.Null);
        }
    }
}
