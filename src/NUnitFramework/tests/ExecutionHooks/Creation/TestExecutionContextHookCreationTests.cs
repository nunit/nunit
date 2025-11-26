// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.ExecutionHooks;

namespace NUnit.Framework.Tests.ExecutionHooks.Creation
{
    internal class TestExecutionContextHookCreationTests
    {
        [Test]
        public void WhenNoHooksAreProvidedNoInstanceOfHooksAreCreated()
        {
            var test = TestBuilder.MakeTestFromMethod(typeof(TestHooksCreationNoHooksFixture), nameof(TestHooksCreationNoHooksFixture.EmptyTest));
            var work = TestBuilder.CreateWorkItem(test);
            work.Execute();

            Assert.That(work.Context.ExecutionHooksEnabled, Is.False);
        }

        [Test]
        public void WhenHooksAreProvidedInstanceOfHooksAreCreated()
        {
            var test = TestBuilder.MakeTestFromMethod(typeof(TestHooksCreationWithHooksFixture), nameof(TestHooksCreationWithHooksFixture.EmptyTest));
            var work = TestBuilder.CreateWorkItem(test);
            work.Execute();

            Assert.That(work.Context.ExecutionHooksEnabled, Is.True);
        }
    }
}
