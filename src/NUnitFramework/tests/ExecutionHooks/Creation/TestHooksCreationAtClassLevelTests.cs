// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.Creation
{
    [TestFixture]
    internal class TestHooksCreationAtClassLevelTests
    {
        internal class ActivateBeforeTestHooksAttribute : ExecutionHookAttribute, IApplyToContext
        {
            public virtual void ApplyToContext(TestExecutionContext context)
            {
                context.ExecutionHooks.BeforeTest.AddHandler((sender, eventArgs) => { });
            }
        }

        internal class ActivateAfterTestHooksAttribute : ExecutionHookAttribute, IApplyToContext
        {
            public virtual void ApplyToContext(TestExecutionContext context)
            {
                context.ExecutionHooks.AfterTest.AddHandler((sender, eventArgs) => { });
            }
        }

        [TestFixture]
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

            Assert.That(work.Context.ExecutionHooks.BeforeTest.GetHandlers(), Has.Count.EqualTo(1));
            Assert.That(work.Context.ExecutionHooks.AfterTest.GetHandlers(), Has.Count.EqualTo(1));
        }
    }
}
