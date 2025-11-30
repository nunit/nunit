// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading;
using NUnit.Framework.Interfaces;

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
                // Set up a SynchronizationContext to catch posted exceptions
                SynchronizationContext.SetSynchronizationContext(
                    SafeSynchronizationContext.Create(SynchronizationContext.Current, TestExecutionContext.CurrentContext));

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

        /// <summary>
        /// This context is used to protect test execution from any exceptions
        /// </summary>
        /// <remarks>
        /// Exceptions thrown in 'async void' methods are posted on the current SynchronizationContext
        /// </remarks>
        private class SafeSynchronizationContext : SynchronizationContext
        {
            private static readonly Logger Log = InternalTrace.GetLogger(nameof(SafeSynchronizationContext));

            private readonly TestExecutionContext _executionContext;

            public SafeSynchronizationContext(TestExecutionContext executionContext)
            {
                _executionContext = executionContext;
            }

            public override void Send(SendOrPostCallback d, object? state) => ExecuteSafely(d, state);

            public override void Post(SendOrPostCallback d, object? state) => ExecuteSafely(d, state);

            protected void ExecuteSafely(SendOrPostCallback d, object? state)
            {
                try
                {
                    d(state);
                }
                catch (Exception e)
                {
                    TestExecutionContext context = _executionContext;

                    Log.Error($"Unexpected exception from SynchronizationContext in test {context.CurrentTest.FullName}: {e.Message}");
                    lock (context)
                    {
                        context.CurrentResult.RecordException(e, FailureSite.Test);
                        context.CurrentResult.RecordTestCompletion();
                    }
                }
            }

            public static SafeSynchronizationContext Create(
                SynchronizationContext? synchronizationContext,
                TestExecutionContext executionContext)
            {
                return synchronizationContext is null || synchronizationContext is SafeSynchronizationContext
                    ? new SafeSynchronizationContext(executionContext)
                    : new SafeIndirectSynchronizationContext(synchronizationContext, executionContext);
            }
        }

        /// <summary>
        /// Indirect version of <see cref="SafeSynchronizationContext"/>
        /// </summary>
        /// <remarks>
        /// Passes calls through to an underlying SynchronizationContext in case that is a SingleThreadedSynchronizationContext
        /// </remarks>
        private sealed class SafeIndirectSynchronizationContext : SafeSynchronizationContext
        {
            private readonly SynchronizationContext _synchronizationContext;

            public SafeIndirectSynchronizationContext(SynchronizationContext synchronizationContext, TestExecutionContext executionContext)
                : base(executionContext)
            {
                _synchronizationContext = synchronizationContext;
            }

            public override void Send(SendOrPostCallback d, object? state) => _synchronizationContext.Send(s => ExecuteSafely(d, s), state);

            public override void Post(SendOrPostCallback d, object? state) => _synchronizationContext.Post(s => ExecuteSafely(d, s), state);
        }
    }
}
