// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Attributes;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.Creation
{
    [TestFixture]
    internal class TestHooksCreationAtClassLevelTests
    {
        internal sealed class ActivateBeforeTestHooksAttribute : ExecutionHookAttribute, IApplyToContext
        {
            public void ApplyToContext(TestExecutionContext context)
            {
                context.ExecutionHooks.AddBeforeTestHandler((sender, eventArgs) => { });
            }
        }

        internal sealed class ActivateAfterTestHooksAttribute : ExecutionHookAttribute, IApplyToContext
        {
            public void ApplyToContext(TestExecutionContext context)
            {
                context.ExecutionHooks.AddAfterTestHandler((sender, eventArgs) => { });
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

            Assert.That(work.Context.ExecutionHooks.BeforeTest, Has.Count.EqualTo(1));
            Assert.That(work.Context.ExecutionHooks.AfterTest, Has.Count.EqualTo(1));
        }
    }
}
