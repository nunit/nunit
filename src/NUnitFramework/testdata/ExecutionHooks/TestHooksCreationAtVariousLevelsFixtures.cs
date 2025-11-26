// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;

namespace NUnit.TestData.ExecutionHooks
{
    public class TestHooksCreationNoHooksFixture
    {
        [Test]
        public void EmptyTest()
        {
        }
    }

    public class TestHooksCreationWithHooksFixture
    {
        [Test]
        [ActivateBeforeTestHookAtMethodLevel]
        public void EmptyTest()
        {
        }
    }

    [TestFixture]
    public class TestHooksCreationAtAssemblyLevelFixture
    {
        [Test]
        public void EmptyTest()
        {
        }
    }

    [ActivateBeforeTestHookAtClassLevel]
    [ActivateAfterTestHookAtClassLevel]
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
        [ActivateBeforeTestHookAtMethodLevel]
        [ActivateAfterTestHookAtMethodLevel]
        public void EmptyTest()
        {
        }
    }
}
