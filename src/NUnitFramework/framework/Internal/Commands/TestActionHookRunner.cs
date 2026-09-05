// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// Runs a single ITestAction.BeforeTest/AfterTest invocation, wrapping it with the
    /// matching pair of ExecutionHooks callbacks when hooks are enabled for the context.
    /// </summary>
    internal static class TestActionHookRunner
    {
        public static void Run(
            TestExecutionContext context,
            Type actionType,
            string methodName,
            Action<TestExecutionContext, IMethodInfo> onBeforeHook,
            Action<TestExecutionContext, IMethodInfo, Exception?> onAfterHook,
            Action run)
        {
            if (!context.ExecutionHooksEnabled)
            {
                run();
                return;
            }

            var hookedMethodInfo = new MethodWrapper(actionType, methodName);
            try
            {
                onBeforeHook(context, hookedMethodInfo);
                run();
            }
            catch (Exception ex)
            {
                onAfterHook(context, hookedMethodInfo, ex);
                throw;
            }

            onAfterHook(context, hookedMethodInfo, null);
        }
    }
}
