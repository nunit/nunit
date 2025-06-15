// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.Creation
{
    [TestFixture]
    internal class TestExecutionContextHookCreationTests
    {
        internal sealed class ActivateBeforeTestHooksAttribute : ExecutionHookAttribute, IApplyToContext
        {
            public void ApplyToContext(TestExecutionContext context)
            {
                context.ExecutionHooks.AddBeforeTestHandler((sender, eventArgs) => { });
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
            var work = TestBuilder.CreateWorkItem(test);
            work.Execute();

            Assert.That(work.Context.ExecutionHooksEnabled, Is.False);
        }

        [Test]

        public void WhenHooksAreProvidedInstanceOfHooksAreCreated()
        {
            var test = TestBuilder.MakeTestFromMethod(typeof(SomeEmptyTestWithHooks), nameof(SomeEmptyTestWithHooks.EmptyTest));
            var work = TestBuilder.CreateWorkItem(test);
            work.Execute();

            Assert.That(work.Context.ExecutionHooksEnabled, Is.True);
        }
    }
}
