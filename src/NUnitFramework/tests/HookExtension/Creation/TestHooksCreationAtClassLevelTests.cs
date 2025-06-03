// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.HookExtension.Creation
{
    [TestFixture]
    internal class TestHooksCreationAtClassLevelTests
    {
        internal class ActivateBeforeTestHooks : NUnitAttribute, IApplyToContext
        {
            public virtual void ApplyToContext(TestExecutionContext context)
            {
                context.HookExtension.BeforeTestHook.AddHandler((sender, eventArgs) => { });
            }
        }

        internal class ActivateAfterTestHooks : NUnitAttribute, IApplyToContext
        {
            public virtual void ApplyToContext(TestExecutionContext context)
            {
                context.HookExtension.AfterTestHook.AddHandler((sender, eventArgs) => { });
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

            Assert.That(work.Context.HookExtension.BeforeTestHook.GetHandlers(), Has.Count.EqualTo(1));
            Assert.That(work.Context.HookExtension.AfterTestHook.GetHandlers(), Has.Count.EqualTo(1));
        }
    }
}
