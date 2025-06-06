// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Execution;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.Creation
{
    [TestFixture]
    internal class TestHooksCreationAtAssemblyLevelTests
    {
        [AttributeUsage(AttributeTargets.Assembly)]
        internal class ActivateBeforeTestHooksAttribute : NUnitAttribute, IApplyToContext
        {
            public virtual void ApplyToContext(TestExecutionContext context)
            {
                context.ExecutionHooks.BeforeTest.AddHandler((sender, eventArgs) => { });
            }
        }

        [AttributeUsage(AttributeTargets.Assembly)]
        internal class ActivateAfterTestHooksAttribute : NUnitAttribute, IApplyToContext
        {
            public virtual void ApplyToContext(TestExecutionContext context)
            {
                context.ExecutionHooks.AfterTest.AddHandler((sender, eventArgs) => { });
            }
        }

        [TestFixture]
        private class SomeEmptyTest
        {
            [Test]
            public void EmptyTest()
            {
            }
        }

        [Test]
        public void BeforeTestHookAdded()
        {
            var test = TestBuilder.MakeTestFromMethod(typeof(SomeEmptyTest), nameof(SomeEmptyTest.EmptyTest));
            var context = new TestExecutionContext();

            // Simulate "assembly-level"
            var hookAttribute = new ActivateBeforeTestHooksAttribute();
            hookAttribute.ApplyToContext(context);

            var work = TestBuilder.CreateWorkItem(test, context) as SimpleWorkItem;
            Assert.That(work, Is.Not.Null);
            Assert.That(work!.Context.ExecutionHooks.BeforeTest.GetHandlers(), Has.Count.EqualTo(1));
        }

        [Test]
        public void AfterTestHookAdded()
        {
            var test = TestBuilder.MakeTestFromMethod(typeof(SomeEmptyTest), nameof(SomeEmptyTest.EmptyTest));
            var context = new TestExecutionContext();

            // Simulate "assembly-level"
            var hookAttribute = new ActivateAfterTestHooksAttribute();
            hookAttribute.ApplyToContext(context);

            var work = TestBuilder.CreateWorkItem(test, context) as SimpleWorkItem;
            Assert.That(work, Is.Not.Null);
            Assert.That(work!.Context.ExecutionHooks.AfterTest.GetHandlers(), Has.Count.EqualTo(1));
        }
    }
}
