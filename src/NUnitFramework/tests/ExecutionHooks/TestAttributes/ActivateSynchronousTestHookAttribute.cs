// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.ExecutionHooks;

namespace NUnit.Framework.Tests.ExecutionHooks.TestAttributes
{
    [AttributeUsage(AttributeTargets.Method)]
    internal sealed class ActivateSynchronousTestHookAttribute : ExecutionHookAttribute
    {
        public override void BeforeTestHook(HookData hookData)
        {
            TestLog.LogMessage(HookIdentifiers.BeforeTestHook);

            TestExecutionContext.CurrentContext
                .CurrentTest.Properties
                .Add("BeforeTestHook_ThreadId", Environment.CurrentManagedThreadId);
        }

        public override void AfterTestHook(HookData hookData)
        {
            TestLog.LogMessage(HookIdentifiers.AfterTestHook);

            TestExecutionContext.CurrentContext
                .CurrentTest.Properties
                .Add("AfterTestHook_ThreadId", Environment.CurrentManagedThreadId);
        }
    }
}
