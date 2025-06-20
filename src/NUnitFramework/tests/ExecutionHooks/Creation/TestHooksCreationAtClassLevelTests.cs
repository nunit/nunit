// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.Creation
{
    internal class TestHooksCreationAtClassLevelTests
    {
        private sealed class ActivateBeforeTestHooksAttribute : ExecutionHookAttribute
        {
            public override void BeforeTestHook(TestExecutionContext context)
            {
            }
        }

        private sealed class ActivateAfterTestHooksAttribute : ExecutionHookAttribute
        {
            public override void AfterTestHook(TestExecutionContext context)
            {
            }
        }

        [ActivateBeforeTestHooks]
        [ActivateAfterTestHooks]
        private class SomeEmptyTest
        {
            [Test]
            public void EmptyTest()
            {
            }
        }

        [Test]
        public void TestHooksAdded()
        {
            var work = TestBuilder.CreateWorkItem(typeof(SomeEmptyTest));
            work.Execute();

            Assert.That(work.Context.ExecutionHooks.BeforeTest, Has.Count.EqualTo(1));
            Assert.That(work.Context.ExecutionHooks.AfterTest, Has.Count.EqualTo(1));
        }
    }
}
