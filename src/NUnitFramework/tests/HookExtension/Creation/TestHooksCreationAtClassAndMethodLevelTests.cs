// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.HookExtensionTests;

namespace NUnit.Framework.Tests.HookExtension.Creation
{
    [TestFixture]
    internal class TestHooksCreationAtClassAndMethodLevelTests
    {
        [Test]
        public void TestHooksAdded()
        {
            var work = TestBuilder.CreateWorkItem(typeof(EmptyTestFor_ExecutionProceedsAfterBothTestHooksCompleteAtClassAndMethodLevels));
            work.Execute();

            Assert.That(work.Context.HookExtension.BeforeTestHook.GetHandlers(), Has.Count.EqualTo(2));
            Assert.That(work.Context.HookExtension.AfterTestHook.GetHandlers(), Has.Count.EqualTo(2));
        }
    }
}
