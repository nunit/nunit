// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.ExecutionHooks.TestAttributes
{
    [AttributeUsage(AttributeTargets.Method)]
    internal sealed class ActivateAllSynchronousTestHooksAttribute : ExecutionHookAttribute
    {
        public override void BeforeEverySetUpHook(TestExecutionContext context)
        {
            TestLog.LogMessage(HookIdentifiers.BeforeEverySetUpHook);
        }

        public override void AfterEverySetUpHook(TestExecutionContext context)
        {
            TestLog.LogMessage(HookIdentifiers.AfterEverySetUpHook);
        }

        public override void BeforeTestHook(TestExecutionContext context)
        {
            TestLog.LogMessage(HookIdentifiers.BeforeTestHook);
        }

        public override void AfterTestHook(TestExecutionContext context)
        {
            TestLog.LogMessage(HookIdentifiers.AfterTestHook);
        }

        public override void BeforeEveryTearDownHook(TestExecutionContext context)
        {
            TestLog.LogMessage(HookIdentifiers.BeforeEveryTearDownHook);
        }

        public override void AfterEveryTearDownHook(TestExecutionContext context)
        {
            TestLog.LogMessage(HookIdentifiers.AfterEveryTearDownHook);
        }
    }
}
