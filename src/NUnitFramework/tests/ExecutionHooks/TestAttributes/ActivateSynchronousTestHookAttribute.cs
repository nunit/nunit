using System;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.ExecutionHooks.Common
{
    [AttributeUsage(AttributeTargets.Method)]
    internal sealed class ActivateSynchronousTestHookAttribute : ExecutionHookAttribute
    {
        public override void BeforeTestHook(TestExecutionContext context)
        {
            TestLog.LogMessage(HookIdentifiers.BeforeTestHook);

            TestExecutionContext.CurrentContext
                .CurrentTest.Properties
                .Add("BeforeTestHook_ThreadId", Environment.CurrentManagedThreadId);
        }

        public override void AfterTestHook(TestExecutionContext context)
        {
            TestLog.LogMessage(HookIdentifiers.AfterTestHook);

            TestExecutionContext.CurrentContext
                .CurrentTest.Properties
                .Add("AfterTestHook_ThreadId", Environment.CurrentManagedThreadId);
        }
    }
}
