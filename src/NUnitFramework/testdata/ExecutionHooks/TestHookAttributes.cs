// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.ExecutionHooks;

namespace NUnit.TestData.ExecutionHooks
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class ActivateBeforeTestHookAtClassLevelAttribute : ExecutionHookAttribute
    {
        public override void BeforeTestHook(HookData hookData)
        {
            TestLog.LogMessage(nameof(ActivateBeforeTestHookAtClassLevelAttribute));
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class ActivateBeforeTestHookAtMethodLevelAttribute : ExecutionHookAttribute
    {
        public override void BeforeTestHook(HookData hookData)
        {
            TestLog.LogMessage(nameof(ActivateBeforeTestHookAtMethodLevelAttribute));
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class ActivateAfterTestHookAtClassLevelAttribute : ExecutionHookAttribute
    {
        public override void AfterTestHook(HookData hookData)
        {
            TestLog.LogMessage(nameof(ActivateAfterTestHookAtClassLevelAttribute));
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class ActivateAfterTestHookAtMethodLevelAttribute : ExecutionHookAttribute
    {
        public override void AfterTestHook(HookData hookData)
        {
            TestLog.LogMessage(nameof(ActivateAfterTestHookAtMethodLevelAttribute));
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class ActivateBeforeTestHookThrowingExceptionAttribute : ExecutionHookAttribute
    {
        public override void BeforeTestHook(HookData hookData)
        {
            TestLog.LogMessage(nameof(ActivateBeforeTestHookThrowingExceptionAttribute));
            throw new InvalidOperationException("Exception from BeforeTestHook");
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class ActivateAfterTestHookThrowingExceptionAttribute : ExecutionHookAttribute
    {
        public override void AfterTestHook(HookData hookData)
        {
            TestLog.LogMessage(nameof(ActivateAfterTestHookThrowingExceptionAttribute));
            throw new InvalidOperationException("Exception from AfterTestHook");
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ActivateLongRunningBeforeTestHookAttribute : ExecutionHookAttribute
    {
        public override void BeforeTestHook(HookData hookData)
        {
            Thread.Sleep(500);
            TestLog.LogMessage(nameof(ActivateLongRunningBeforeTestHookAttribute));
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ActivateLongRunningAfterTestHookAttribute : ExecutionHookAttribute
    {
        public override void AfterTestHook(HookData hookData)
        {
            Thread.Sleep(500);
            TestLog.LogMessage(nameof(ActivateLongRunningAfterTestHookAttribute));
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ActivateAllSynchronousTestHooksAttribute : ExecutionHookAttribute
    {
        public override void BeforeEverySetUpHook(HookData hookData)
        {
            TestLog.LogMessage(HookIdentifiers.BeforeEverySetUpHook);
        }

        public override void AfterEverySetUpHook(HookData hookData)
        {
            TestLog.LogMessage(HookIdentifiers.AfterEverySetUpHook);
        }

        public override void BeforeTestHook(HookData hookData)
        {
            TestLog.LogMessage(HookIdentifiers.BeforeTestHook);
        }

        public override void AfterTestHook(HookData hookData)
        {
            TestLog.LogMessage(HookIdentifiers.AfterTestHook);
        }

        public override void BeforeEveryTearDownHook(HookData hookData)
        {
            TestLog.LogMessage(HookIdentifiers.BeforeEveryTearDownHook);
        }

        public override void AfterEveryTearDownHook(HookData hookData)
        {
            TestLog.LogMessage(HookIdentifiers.AfterEveryTearDownHook);
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ActivateTestHookAttribute : ExecutionHookAttribute
    {
        public override void BeforeTestHook(HookData hookData)
        {
            TestLog.LogMessage(HookIdentifiers.BeforeTestHook + $"({hookData.Context.Test.MethodName})");
        }

        public override void AfterTestHook(HookData hookData)
        {
            TestLog.LogMessage(HookIdentifiers.AfterTestHook + $"({hookData.Context.Test.MethodName})");
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ActivateSynchronousTestHookAttribute : ExecutionHookAttribute
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

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class LogTestActionAttribute : Attribute, ITestAction
    {
        public void BeforeTest(ITest test)
        {
            TestLog.LogCurrentMethodWithContextInfo(test.IsSuite ? "Suite" : "Test");
        }

        public void AfterTest(ITest test)
        {
            TestLog.LogCurrentMethodWithContextInfo(test.IsSuite ? "Suite" : "Test");
        }

        public ActionTargets Targets => ActionTargets.Test | ActionTargets.Suite;
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class TestActionLoggingExecutionHooksAttribute : ExecutionHookAttribute
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

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class SimpleTestActionAttribute : Attribute, ITestAction
    {
        public static readonly string LogStringForBeforeTest = $"{nameof(SimpleTestActionAttribute)}.{nameof(BeforeTest)}";
        public static readonly string LogStringForAfterTest = $"{nameof(SimpleTestActionAttribute)}.{nameof(AfterTest)}";

        public void BeforeTest(ITest test)
        {
            TestLog.LogMessage(LogStringForBeforeTest);
        }

        public void AfterTest(ITest test)
        {
            TestLog.LogMessage(LogStringForAfterTest);
        }

        public ActionTargets Targets => ActionTargets.Suite;
    }
}
