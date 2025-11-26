// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.ExecutionHooks.TestAttributes;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.ExecutionHooks;

namespace NUnit.Framework.Tests.ExecutionHooks.Creation
{
    internal class TestHooksCreationAtAssemblyLevelTests
    {
        [Test]
        public void BeforeTestHookAdded()
        {
            var test = TestBuilder.MakeTestFromMethod(typeof(TestHooksCreationAtAssemblyLevelFixture), nameof(TestHooksCreationAtAssemblyLevelFixture.EmptyTest));
            var context = new TestExecutionContext();

            // Simulate "assembly-level"
            var hookAttribute = new ActivateBeforeTestHookAttribute();
            hookAttribute.ApplyToContext(context);

            var work = TestBuilder.CreateWorkItem(test, context);
            Assert.That(work.Context.ExecutionHooks.BeforeTest, Has.Count.EqualTo(1));
        }

        [Test]
        public void AfterTestHookAdded()
        {
            var test = TestBuilder.MakeTestFromMethod(typeof(TestHooksCreationAtAssemblyLevelFixture), nameof(TestHooksCreationAtAssemblyLevelFixture.EmptyTest));
            var context = new TestExecutionContext();

            // Simulate "assembly-level"
            var hookAttribute = new ActivateAfterTestHookAttribute();
            hookAttribute.ApplyToContext(context);

            var work = TestBuilder.CreateWorkItem(test, context);
            Assert.That(work.Context.ExecutionHooks.AfterTest, Has.Count.EqualTo(1));
        }
    }
}
