using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.ExecutionHooks.Common
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    internal sealed class TestActionLoggingExecutionHooksAttribute : NUnitAttribute, IApplyToContext
    {
        public void ApplyToContext(TestExecutionContext context)
        {
            context.ExecutionHooks.AddBeforeTestActionBeforeTestHandler((context) =>
            {
                TestLog.LogCurrentMethod($"BeforeTestActionBeforeTestHook({(context.CurrentTest.IsSuite ? "Suite" : "Test")})");
            });
            context.ExecutionHooks.AddAfterTestActionBeforeTestHandler((context) =>
            {
                TestLog.LogCurrentMethod($"AfterTestActionBeforeTestHook({(context.CurrentTest.IsSuite ? "Suite" : "Test")})");
            });
            context.ExecutionHooks.AddBeforeTestActionAfterTestHandler((context) =>
            {
                TestLog.LogCurrentMethod($"BeforeTestActionAfterTestHook({(context.CurrentTest.IsSuite ? "Suite" : "Test")})");
            });
            context.ExecutionHooks.AddAfterTestActionAfterTestHandler((context) =>
            {
                TestLog.LogCurrentMethod($"AfterTestActionAfterTestHook({(context.CurrentTest.IsSuite ? "Suite" : "Test")})");
            });
        }
    }
}
