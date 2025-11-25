// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework;
using NUnit.Framework.Internal.ExecutionHooks;

namespace NUnit.TestData.ExecutionHooks
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ActivateBeforeTestHooksContextAttribute : ExecutionHookAttribute
    {
        public override void BeforeTestHook(HookData context)
        {
        }
    }

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
        [ActivateBeforeTestHooksContext]
        public void EmptyTest()
        {
        }
    }
}
