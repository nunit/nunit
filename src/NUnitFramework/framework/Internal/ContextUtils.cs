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
                var executionContext = ExecutionContext.Capture()
                    ?? throw new InvalidOperationException("Execution context flow must not be suppressed.");

                using (executionContext as IDisposable)
                {
                    ExecutionContext.Run(executionContext, callback, state);
                }
            }
            finally
            {
                previousState.Restore();
            }
        }
    }
}
