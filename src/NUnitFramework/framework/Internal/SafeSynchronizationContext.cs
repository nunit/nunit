// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// This context is used to protect test execution from any exceptions
    /// </summary>
    /// <remarks>
    /// Exceptions thrown in 'async void' methods are posted on the current SynchronizationContext
    /// </remarks>
    internal class SafeSynchronizationContext : SynchronizationContext
    {
        private static readonly Logger Log = InternalTrace.GetLogger(nameof(SafeSynchronizationContext));

        public SafeSynchronizationContext(TestExecutionContext executionContext)
        {
            ExecutionContext = executionContext;
        }

        public TestExecutionContext ExecutionContext { get; }

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
                TestExecutionContext context = ExecutionContext;

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
    internal sealed class SafeIndirectSynchronizationContext : SafeSynchronizationContext
    {
        public SafeIndirectSynchronizationContext(SynchronizationContext synchronizationContext, TestExecutionContext executionContext)
            : base(executionContext)
        {
            ActualSynchronizationContext = synchronizationContext;
        }

        public SynchronizationContext ActualSynchronizationContext { get; }

        public override void Send(SendOrPostCallback d, object? state) => ActualSynchronizationContext.Send(s => ExecuteSafely(d, s), state);

        public override void Post(SendOrPostCallback d, object? state) => ActualSynchronizationContext.Post(s => ExecuteSafely(d, s), state);
    }
}
