// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Execution;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.Creation
{
    [TestFixture]
    internal class TestExecutionContextHookCreationTests
    {
        internal class ActivateBeforeTestHooksAttribute : ExecutionHookAttribute, IApplyToContext
        {
            public virtual void ApplyToContext(TestExecutionContext context)
            {
                context.ExecutionHooks.BeforeTest.AddHandler((sender, eventArgs) => { });
            }
        }

        [TestFixture]
        private class SomeEmptyTestWithNoHooks
        {
            [Test]
            public void EmptyTest()
            {
            }
        }

        [TestFixture]
        private class SomeEmptyTestWithHooks
        {
            [Test]
            [ActivateBeforeTestHooks]
            public void EmptyTest()
            {
            }
        }

        [Test]
        public void WhenNoHooksAreProvidedNoInstanceOfHooksAreCreated()
        {
            var test = TestBuilder.MakeTestFromMethod(typeof(SomeEmptyTestWithNoHooks), nameof(SomeEmptyTestWithNoHooks.EmptyTest));
            var work = TestBuilder.CreateWorkItem(test) as SimpleWorkItem;
            work!.Execute();

            Assert.That(TestExecutionContext.CurrentContext.ExecutionHooksInternal, Is.Null);
        }

        [Test]

        public void WhenHooksAreProvidedInstanceOfHooksAreCreated()
        {
            var test = TestBuilder.MakeTestFromMethod(typeof(SomeEmptyTestWithHooks), nameof(SomeEmptyTestWithHooks.EmptyTest));
            var work = TestBuilder.CreateWorkItem(test) as SimpleWorkItem;
            work!.Execute();

            Assert.That(TestExecutionContext.CurrentContext.ExecutionHooksInternal, Is.Not.Null);
        }
    }
}
