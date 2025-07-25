using System;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.ExecutionHooks.Common
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ActivateMethodLevelAfterTestHooksAttribute : ExecutionHookAttribute
    {
        public override void AfterTestHook(TestExecutionContext context)
        {
            TestLog.LogMessage(nameof(ActivateMethodLevelAfterTestHooksAttribute));
        }
    }
}
