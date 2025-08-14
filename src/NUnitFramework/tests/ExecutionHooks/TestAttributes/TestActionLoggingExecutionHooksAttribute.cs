// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal.ExecutionHooks;

namespace NUnit.Framework.Tests.ExecutionHooks.TestAttributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    internal sealed class TestActionLoggingExecutionHooksAttribute : ExecutionHookAttribute
    {
        public override void BeforeTestActionBeforeTestHook(HookData hookData)
        {
            TestLog.LogCurrentMethod(
                $"{nameof(BeforeTestActionBeforeTestHook)}({(hookData.Context.Test.IsSuite ? "Suite" : "Test")})");
        }

        public override void BeforeTestActionAfterTestHook(HookData hookData)
        {
            TestLog.LogCurrentMethod(
                $"{nameof(BeforeTestActionAfterTestHook)}({(hookData.Context.Test.IsSuite ? "Suite" : "Test")})");
        }

        public override void AfterTestActionBeforeTestHook(HookData hookData)
        {
            TestLog.LogCurrentMethod(
                $"{nameof(AfterTestActionBeforeTestHook)}({(hookData.Context.Test.IsSuite ? "Suite" : "Test")})");
        }

        public override void AfterTestActionAfterTestHook(HookData hookData)
        {
            TestLog.LogCurrentMethod(
                $"{nameof(AfterTestActionAfterTestHook)}({(hookData.Context.Test.IsSuite ? "Suite" : "Test")})");
        }
    }
}
