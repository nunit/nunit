// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.ExecutionHooks.TestAttributes
{
    [AttributeUsage(AttributeTargets.Method)]
    internal sealed class ActivateTestHookAttribute : ExecutionHookAttribute
    {
        public override void BeforeTestHook(TestExecutionContext context)
        {
            TestLog.LogMessage(HookIdentifiers.BeforeTestHook + $"({context.CurrentTest.MethodName})");
        }

        public override void AfterTestHook(TestExecutionContext context)
        {
            TestLog.LogMessage(HookIdentifiers.AfterTestHook + $"({context.CurrentTest.MethodName})");
        }
    }
}
