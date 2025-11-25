// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework;
using NUnit.Framework.Internal.ExecutionHooks;

namespace NUnit.TestData.ExecutionHooks
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ActivateBeforeTestHooksMethodAttribute : ExecutionHookAttribute
    {
        public override void BeforeTestHook(HookData hookData)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ActivateAfterTestHooksMethodAttribute : ExecutionHookAttribute
    {
        public override void AfterTestHook(HookData hookData)
        {
        }
    }

    public class TestHooksCreationAtMethodLevelFixture
    {
        [Test]
        [ActivateBeforeTestHooksMethod]
        [ActivateAfterTestHooksMethod]
        public void EmptyTest()
        {
        }
    }
}
