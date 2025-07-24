// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.ExecutionHooks;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.Creation
{
    internal class TestHooksCreationAtAssemblyLevelTests
    {
        [AttributeUsage(AttributeTargets.Assembly)]
        private sealed class ActivateBeforeTestHooksAttribute : ExecutionHookAttribute
        {
            public override void BeforeTestHook(HookData context)
            {
            }
        }

        [AttributeUsage(AttributeTargets.Assembly)]
        private sealed class ActivateAfterTestHooksAttribute : ExecutionHookAttribute
        {
            public override void AfterTestHook(HookData context)
            {
            }
        }

        [TestFixture]
        [Explicit($"This test should only be run as part of the {nameof(BeforeTestHookAdded)} and {nameof(AfterTestHookAdded)} tests")]
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
