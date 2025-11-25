// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.Creation
{
    internal class TestHooksCreationAtMethodLevelTests
    {
        [Test]
        public void TestHooksAdded()
        {
            var test = TestBuilder.MakeTestFromMethod(typeof(TestData.ExecutionHooks.TestHooksCreationAtMethodLevelFixture), nameof(TestData.ExecutionHooks.TestHooksCreationAtMethodLevelFixture.EmptyTest));
            var work = TestBuilder.CreateWorkItem(test, TestFilter.Explicit);
            work.Execute();

            Assert.That(work.Context.ExecutionHooks.BeforeTest, Has.Count.EqualTo(1));
            Assert.That(work.Context.ExecutionHooks.AfterTest, Has.Count.EqualTo(1));
        }
    }
}
