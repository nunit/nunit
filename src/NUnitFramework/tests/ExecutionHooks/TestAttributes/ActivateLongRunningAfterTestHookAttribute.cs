using System;
using System.Threading;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.ExecutionHooks.Common
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ActivateLongRunningAfterTestHookAttribute : ExecutionHookAttribute
    {
        public override void AfterTestHook(TestExecutionContext context)
        {
            // Simulate a long-running after test hook
            Thread.Sleep(500);
            TestLog.LogMessage(nameof(ActivateLongRunningAfterTestHookAttribute));
        }
    }
}
