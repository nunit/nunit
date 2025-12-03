// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading;

namespace NUnit.Framework.Internal
{
    internal static class ContextUtils
    {
        public static void DoIsolated(Action action)
        {
            DoIsolated(_ => action.Invoke(), state: null);
        }

        public static T DoIsolated<T>(Func<T> func)
        {
            T? returnValue = default(T);
            DoIsolated(_ => returnValue = func.Invoke(), state: null);
            return returnValue!;
        }

        public static void DoIsolated(ContextCallback callback, object? state)
        {
            var previousState = SandboxedThreadState.Capture();
            try
            {
                var synchronizationContext = SynchronizationContext.Current;

                var executionContext = ExecutionContext.Capture()
                    ?? throw new InvalidOperationException("Execution context flow must not be suppressed.");

                using (executionContext)
                {
                    ExecutionContext.Run(executionContext, s =>
                    {
                        // For NET Framework, we need to set the current context inside the execution context
                        // For NET Core we can set it outside the execution context

                        // Set up a SynchronizationContext to catch posted exceptions
                        SynchronizationContext.SetSynchronizationContext(
                            SafeSynchronizationContext.Create(synchronizationContext, TestExecutionContext.CurrentContext));

                        callback(s);
                    }, state);
                }
            }
            finally
            {
                previousState.Restore();
            }
        }
    }
}
