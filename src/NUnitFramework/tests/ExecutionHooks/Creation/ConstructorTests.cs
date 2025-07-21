// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.ExecutionHooks.Creation
{
    internal class ExecutionHooksConstructorTests
    {
        [Test]
        public void CopyCtor_CreateNewExecutionHook_InvocationListShouldBeEmpty()
        {
            var executionHooks = new NUnit.Framework.Internal.ExecutionHooks.ExecutionHooks();

            Assert.Multiple(() =>
            {
                Assert.That(executionHooks.BeforeEverySetUp.GetHandlers(), Has.Count.Zero);
                Assert.That(executionHooks.AfterEverySetUp.GetHandlers(), Has.Count.Zero);
                Assert.That(executionHooks.BeforeTest.GetHandlers(), Has.Count.Zero);
                Assert.That(executionHooks.AfterTest.GetHandlers(), Has.Count.Zero);
                Assert.That(executionHooks.BeforeEveryTearDown.GetHandlers(), Has.Count.Zero);
                Assert.That(executionHooks.AfterEveryTearDown.GetHandlers(), Has.Count.Zero);

                Assert.That(executionHooks.BeforeTestActionBeforeTest.GetHandlers(), Has.Count.Zero);
                Assert.That(executionHooks.AfterTestActionBeforeTest.GetHandlers(), Has.Count.Zero);
                Assert.That(executionHooks.BeforeTestActionAfterTest.GetHandlers(), Has.Count.Zero);
                Assert.That(executionHooks.AfterTestActionAfterTest.GetHandlers(), Has.Count.Zero);
            });
        }

        [Test]
        public void CopyCtor_CallMultipleTimes_ShallNotIncreaseInvocationList()
        {
            var executionHooks = new NUnit.Framework.Internal.ExecutionHooks.ExecutionHooks();
            executionHooks.AfterTest.AddHandler((context) => { });

            executionHooks = new NUnit.Framework.Internal.ExecutionHooks.ExecutionHooks(executionHooks);
            executionHooks = new NUnit.Framework.Internal.ExecutionHooks.ExecutionHooks(executionHooks);
            executionHooks = new NUnit.Framework.Internal.ExecutionHooks.ExecutionHooks(executionHooks);

            // handlers shall stay empty
            Assert.Multiple(() =>
            {
                Assert.That(executionHooks.BeforeEverySetUp.GetHandlers(), Has.Count.Zero);
                Assert.That(executionHooks.AfterEverySetUp.GetHandlers(), Has.Count.Zero);
                Assert.That(executionHooks.BeforeTest.GetHandlers(), Has.Count.Zero);

                // initially assigned handlers shall be copied
                Assert.That(executionHooks.AfterTest.GetHandlers(), Has.Count.EqualTo(1));

                Assert.That(executionHooks.BeforeEveryTearDown.GetHandlers(), Has.Count.Zero);
                Assert.That(executionHooks.AfterEveryTearDown.GetHandlers(), Has.Count.Zero);
                Assert.That(executionHooks.BeforeTestActionBeforeTest.GetHandlers(), Has.Count.Zero);
                Assert.That(executionHooks.AfterTestActionBeforeTest.GetHandlers(), Has.Count.Zero);
                Assert.That(executionHooks.BeforeTestActionAfterTest.GetHandlers(), Has.Count.Zero);
                Assert.That(executionHooks.AfterTestActionAfterTest.GetHandlers(), Has.Count.Zero);
            });
        }
    }
}
