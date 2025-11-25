// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;

namespace NUnit.TestData.ExecutionHooks
{
    public class TestExecutionContextHookCreationNoHooksFixture
    {
        [Test]
        public void EmptyTest()
        {
        }
    }

    public class TestExecutionContextHookCreationWithHooksFixture
    {
        [Test]
        [ActivateBeforeTestHook]
        public void EmptyTest()
        {
        }
    }
}
