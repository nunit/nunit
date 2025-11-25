// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework;
using NUnit.Framework.Internal.ExecutionHooks;

namespace NUnit.TestData.ExecutionHooks
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ActivateBeforeTestHooksAttribute : ExecutionHookAttribute
    {
        public override void BeforeTestHook(HookData context)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ActivateAfterTestHooksAttribute : ExecutionHookAttribute
    {
        public override void AfterTestHook(HookData context)
        {
        }
    }

    [ActivateBeforeTestHooks]
    [ActivateAfterTestHooks]
    public class TestHooksCreationAtClassLevelFixture
    {
        [Test]
        public void EmptyTest()
        {
        }
    }
}
