// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.Creation
{
    internal class TestHooksCreationAtMethodLevelTests
    {
        [AttributeUsage(AttributeTargets.Method)]
        private sealed class ActivateBeforeTestHooksAttribute : ExecutionHookAttribute
        {
            public override void BeforeTestHook(TestExecutionContext context)
            {
            }
        }

        [AttributeUsage(AttributeTargets.Method)]
        private sealed class ActivateAfterTestHooksAttribute : ExecutionHookAttribute
        {
            public override void AfterTestHook(TestExecutionContext context)
            {
            }
        }

        [Explicit($"This test should only be run as part of the {nameof(TestHooksAdded)} test")]
        private class SomeEmptyTest
        {
            [Test]
            [ActivateBeforeTestHooks]
            [ActivateAfterTestHooks]
            public void EmptyTest()
            {
            }
        }

        [Test]
        public void TestHooksAdded()
        {
            var test = TestBuilder.MakeTestFromMethod(typeof(SomeEmptyTest), nameof(SomeEmptyTest.EmptyTest));
            var work = TestBuilder.CreateWorkItem(test, TestFilter.Explicit);
            work.Execute();

            Assert.That(work.Context.ExecutionHooks.BeforeTest, Has.Count.EqualTo(1));
            Assert.That(work.Context.ExecutionHooks.AfterTest, Has.Count.EqualTo(1));
        }
    }
}
