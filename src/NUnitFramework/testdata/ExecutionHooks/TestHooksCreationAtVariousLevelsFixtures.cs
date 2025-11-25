// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;

namespace NUnit.TestData.ExecutionHooks
{
    [TestFixture]
    public class TestHooksCreationAtAssemblyLevelFixture
    {
        [Test]
        public void EmptyTest()
        {
        }
    }

    [ActivateBeforeTestHook]
    [ActivateAfterTestHook]
    public class TestHooksCreationAtClassLevelFixture
    {
        [Test]
        public void EmptyTest()
        {
        }
    }

    public class TestHooksCreationAtMethodLevelFixture
    {
        [Test]
        [ActivateBeforeTestHook]
        [ActivateAfterTestHook]
        public void EmptyTest()
        {
        }
    }
}
