// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.Creation
{
    internal class TestExecutionContextHookCreationTests
    {
        [Test]
        public void WhenNoHooksAreProvidedNoInstanceOfHooksAreCreated()
        {
            var test = TestBuilder.MakeTestFromMethod(typeof(TestData.ExecutionHooks.TestExecutionContextHookCreationNoHooksFixture), nameof(TestData.ExecutionHooks.TestExecutionContextHookCreationNoHooksFixture.EmptyTest));
            var work = TestBuilder.CreateWorkItem(test, TestFilter.Explicit);
            work.Execute();

            Assert.That(work.Context.ExecutionHooksEnabled, Is.False);
        }

        [Test]
        public void WhenHooksAreProvidedInstanceOfHooksAreCreated()
        {
            var test = TestBuilder.MakeTestFromMethod(typeof(TestData.ExecutionHooks.TestExecutionContextHookCreationWithHooksFixture), nameof(TestData.ExecutionHooks.TestExecutionContextHookCreationWithHooksFixture.EmptyTest));
            var work = TestBuilder.CreateWorkItem(test);
            work.Execute();

            Assert.That(work.Context.ExecutionHooksEnabled, Is.True);
        }
    }
}
