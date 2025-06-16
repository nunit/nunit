// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Attributes;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.Creation
{
    [TestFixture]
    internal class TestHooksCreationAtAssemblyLevelTests
    {
        [AttributeUsage(AttributeTargets.Assembly)]
        internal sealed class ActivateBeforeTestHooksAttribute : ExecutionHookAttribute, IApplyToContext
        {
            public void ApplyToContext(TestExecutionContext context)
            {
                context.ExecutionHooks.AddBeforeTestHandler((sender, eventArgs) => { });
            }
        }

        [AttributeUsage(AttributeTargets.Assembly)]
        internal sealed class ActivateAfterTestHooksAttribute : ExecutionHookAttribute, IApplyToContext
        {
            public void ApplyToContext(TestExecutionContext context)
            {
                context.ExecutionHooks.AddAfterTestHandler((sender, eventArgs) => { });
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

            var work = TestBuilder.CreateWorkItem(test, context);
            Assert.That(work.Context.ExecutionHooks.BeforeTest, Has.Count.EqualTo(1));
        }

        [Test]
        public void AfterTestHookAdded()
        {
            var test = TestBuilder.MakeTestFromMethod(typeof(SomeEmptyTest), nameof(SomeEmptyTest.EmptyTest));
            var context = new TestExecutionContext();

            // Simulate "assembly-level"
            var hookAttribute = new ActivateAfterTestHooksAttribute();
            hookAttribute.ApplyToContext(context);

            var work = TestBuilder.CreateWorkItem(test, context);
            Assert.That(work.Context.ExecutionHooks.AfterTest, Has.Count.EqualTo(1));
        }
    }
}
