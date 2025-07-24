// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.TestData.ExecutionHookTests
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class SomeTestActionAttribute : Attribute, ITestAction
    {
        public static readonly string LogStringForBeforeTest = $"{nameof(SomeTestActionAttribute)}.{nameof(BeforeTest)}";
        public static readonly string LogStringForAfterTest = $"{nameof(SomeTestActionAttribute)}.{nameof(AfterTest)}";

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

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ActivateClassLevelBeforeTestHooksAttribute : ExecutionHookAttribute
    {
        public override void BeforeTestHook(TestExecutionContext context)
        {
            TestLog.LogMessage(nameof(ActivateClassLevelBeforeTestHooksAttribute));
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ActivateClassLevelAfterTestHooksAttribute : ExecutionHookAttribute
    {
        public override void AfterTestHook(TestExecutionContext context)
        {
            TestLog.LogMessage(nameof(ActivateClassLevelAfterTestHooksAttribute));
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ActivateMethodLevelBeforeTestHooksAttribute : ExecutionHookAttribute
    {
        public override void BeforeTestHook(TestExecutionContext context)
        {
            TestLog.LogMessage(nameof(ActivateMethodLevelBeforeTestHooksAttribute));
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ActivateMethodLevelAfterTestHooksAttribute : ExecutionHookAttribute
    {
        public override void AfterTestHook(TestExecutionContext context)
        {
            TestLog.LogMessage(nameof(ActivateMethodLevelAfterTestHooksAttribute));
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ActivateBeforeTestHookAttribute : ExecutionHookAttribute
    {
        public override void BeforeTestHook(TestExecutionContext context)
        {
            TestLog.LogMessage(nameof(ActivateBeforeTestHookAttribute));
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ActivateBeforeTestHookThrowingExceptionAttribute : ExecutionHookAttribute
    {
        public override void BeforeTestHook(TestExecutionContext context)
        {
            TestLog.LogMessage(nameof(ActivateBeforeTestHookThrowingExceptionAttribute));
            throw new Exception("Before test hook crashed!!");
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ActivateLongRunningBeforeTestHookAttribute : ExecutionHookAttribute
    {
        public override void BeforeTestHook(TestExecutionContext context)
        {
            Thread.Sleep(500);
            TestLog.LogMessage(nameof(ActivateLongRunningBeforeTestHookAttribute));
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class ActivateBeforeTestHooksAttribute : ExecutionHookAttribute
    {
        public override void BeforeTestHook(TestExecutionContext context)
        {
            TestLog.LogMessage(HookIdentifiers.BeforeTestHook);
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ActivateLongRunningBeforeTestHooksAttribute : ExecutionHookAttribute
    {
        public override void BeforeTestHook(TestExecutionContext context)
        {
            // Delay to ensure that handlers run longer than the test case
            Thread.Sleep(1000);
            TestLog.LogMessage(HookIdentifiers.BeforeTestHook);
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class ActivateAfterTestHookAttribute : ExecutionHookAttribute
    {
        public override void AfterTestHook(TestExecutionContext context)
        {
            TestLog.LogMessage(nameof(ActivateAfterTestHookAttribute));
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ActivateAfterTestHookThrowingExceptionAttribute : ExecutionHookAttribute
    {
        public override void AfterTestHook(TestExecutionContext context)
        {
            TestLog.LogMessage(nameof(ActivateAfterTestHookThrowingExceptionAttribute));
            throw new Exception("After test hook crashed!!");
        }
    }

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

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ActivateTestHookAttribute : ExecutionHookAttribute
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

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class LogTestActionAttribute : Attribute, ITestAction
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
    public class TestActionLoggingExecutionHooksAttribute : NUnitAttribute, IApplyToContext
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

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ActivateAllSynchronousTestHooksAttribute : ExecutionHookAttribute
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

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ActivateSynchronousTestHookAttribute : ExecutionHookAttribute
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

    public static class HookIdentifiers
    {
        public static readonly string Hook = "_Hook";

        public static readonly string AfterTestHook = $"AfterTestHook{Hook}";
        public static readonly string BeforeEverySetUpHook = $"BeforeEverySetUpHook{Hook}";
        public static readonly string AfterEverySetUpHook = $"AfterEverySetUpHook{Hook}";
        public static readonly string BeforeTestHook = $"BeforeTestHook{Hook}";
        public static readonly string BeforeEveryTearDownHook = $"BeforeEveryTearDownHook{Hook}";
        public static readonly string AfterEveryTearDownHook = $"AfterEveryTearDownHook{Hook}";
    }
}
