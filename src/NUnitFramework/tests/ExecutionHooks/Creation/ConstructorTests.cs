// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.ExecutionHooks.Creation
{
    internal class HookExtensionConstructorTests
    {
        [Test]
        public void CopyCtor_CreateNewHookExtension_InvocationListShouldBeEmpty()
        {
            var exHooks = new NUnit.Framework.Internal.ExecutionHooks.ExecutionHooks();

            Assert.Multiple(() =>
            {
                Assert.That(exHooks.BeforeAnySetUps.GetHandlers(), Has.Count.Zero);
                Assert.That(exHooks.AfterAnySetUps.GetHandlers(), Has.Count.Zero);
                Assert.That(exHooks.BeforeTest.GetHandlers(), Has.Count.Zero);
                Assert.That(exHooks.AfterTest.GetHandlers(), Has.Count.Zero);
                Assert.That(exHooks.BeforeAnyTearDowns.GetHandlers(), Has.Count.Zero);
                Assert.That(exHooks.AfterAnyTearDowns.GetHandlers(), Has.Count.Zero);

                Assert.That(exHooks.BeforeTestActionBeforeTest.GetHandlers(), Has.Count.Zero);
                Assert.That(exHooks.AfterTestActionBeforeTest.GetHandlers(), Has.Count.Zero);
                Assert.That(exHooks.BeforeTestActionAfterTest.GetHandlers(), Has.Count.Zero);
                Assert.That(exHooks.AfterTestActionAfterTest.GetHandlers(), Has.Count.Zero);
            });
        }

        [Test]
        public void CopyCtor_CallMultipleTimes_ShallNotIncreaseInvocationList()
        {
            var exHooks = new NUnit.Framework.Internal.ExecutionHooks.ExecutionHooks();
            exHooks.AfterTest.AddHandler((context) => { });

            exHooks = new NUnit.Framework.Internal.ExecutionHooks.ExecutionHooks(exHooks);
            exHooks = new NUnit.Framework.Internal.ExecutionHooks.ExecutionHooks(exHooks);
            exHooks = new NUnit.Framework.Internal.ExecutionHooks.ExecutionHooks(exHooks);

            // handlers shall stay empty
            Assert.Multiple(() =>
            {
                Assert.That(exHooks.BeforeAnySetUps.GetHandlers(), Has.Count.Zero);
                Assert.That(exHooks.AfterAnySetUps.GetHandlers(), Has.Count.Zero);
                Assert.That(exHooks.BeforeTest.GetHandlers(), Has.Count.Zero);

                // initially assigned handlers shall be copied
                Assert.That(exHooks.AfterTest.GetHandlers(), Has.Count.EqualTo(1));

                Assert.That(exHooks.BeforeAnyTearDowns.GetHandlers(), Has.Count.Zero);
                Assert.That(exHooks.AfterAnyTearDowns.GetHandlers(), Has.Count.Zero);
                Assert.That(exHooks.BeforeTestActionBeforeTest.GetHandlers(), Has.Count.Zero);
                Assert.That(exHooks.AfterTestActionBeforeTest.GetHandlers(), Has.Count.Zero);
                Assert.That(exHooks.BeforeTestActionAfterTest.GetHandlers(), Has.Count.Zero);
                Assert.That(exHooks.AfterTestActionAfterTest.GetHandlers(), Has.Count.Zero);
            });
        }
    }
}
