// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.ExecutionHooks.TestAttributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    internal sealed class TestActionLoggingExecutionHooksAttribute : ExecutionHookAttribute
    {
        public override void BeforeTestActionBeforeTestHook(TestExecutionContext context)
        {
            TestLog.LogCurrentMethod(
                $"{nameof(BeforeTestActionBeforeTestHook)}({(context.CurrentTest.IsSuite ? "Suite" : "Test")})");
        }

        public override void BeforeTestActionAfterTestHook(TestExecutionContext context)
        {
            TestLog.LogCurrentMethod(
                $"{nameof(BeforeTestActionAfterTestHook)}({(context.CurrentTest.IsSuite ? "Suite" : "Test")})");
        }

        public override void AfterTestActionBeforeTestHook(TestExecutionContext context)
        {
            TestLog.LogCurrentMethod(
                $"{nameof(AfterTestActionBeforeTestHook)}({(context.CurrentTest.IsSuite ? "Suite" : "Test")})");
        }

        public override void AfterTestActionAfterTestHook(TestExecutionContext context)
        {
            TestLog.LogCurrentMethod(
                $"{nameof(AfterTestActionAfterTestHook)}({(context.CurrentTest.IsSuite ? "Suite" : "Test")})");
        }
    }
}
