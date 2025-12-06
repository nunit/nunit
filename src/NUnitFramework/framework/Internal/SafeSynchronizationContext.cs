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
    public class SafeSynchronizationContext : SynchronizationContext
    {
        private static readonly Logger Log = InternalTrace.GetLogger(nameof(SafeSynchronizationContext));

        /// <summary>
        /// Initializes a new instance of the SafeSynchronizationContext class.
        /// </summary>
        /// <param name="executionContext">The test execution context to associate with this synchronization context. Cannot be null.</param>
        internal SafeSynchronizationContext(TestExecutionContext executionContext)
        {
            ExecutionContext = executionContext;
        }

        internal TestExecutionContext ExecutionContext { get; }

        /// <inheritdoc/>
        public override void Send(SendOrPostCallback d, object? state) => ExecuteSafely(d, state);

        /// <inheritdoc/>
        public override void Post(SendOrPostCallback d, object? state) => ExecuteSafely(d, state);

        /// <summary>
        /// Executes the delegate safely, catching any exceptions and recording them in the test result.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="state"></param>
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

        /// <summary>
        /// Creates either a <see cref="SafeSynchronizationContext"/> or a <see cref="SafeIndirectSynchronizationContext "/>
        /// </summary>
        /// <remarks>If the supplied synchronization context is null or already a
        /// SafeSynchronizationContext, the method creates a direct SafeSynchronizationContext. Otherwise, it creates a
        /// SafeIndirectSynchronizationContext to wrap the original context. This ensures consistent behavior for test
        /// execution scenarios that require synchronization context safety.</remarks>
        /// <param name="synchronizationContext">The synchronization context to wrap.</param>
        /// <param name="executionContext">The test execution context to associate with the new synchronization context.</param>
        /// <returns>A SafeSynchronizationContext instance that encapsulates the specified synchronization context and execution
        /// context.</returns>
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
    public sealed class SafeIndirectSynchronizationContext : SafeSynchronizationContext
    {
        internal SafeIndirectSynchronizationContext(SynchronizationContext synchronizationContext, TestExecutionContext executionContext)
            : base(executionContext)
        {
            ActualSynchronizationContext = synchronizationContext;
        }

        /// <summary>
        /// Gets the underlying <see cref="SynchronizationContext"/> associated with the current context.
        /// </summary>
        /// <remarks>Use this property to access the original synchronization context for read-only purposes.
        /// It should not be used to Post or Send, bypassing the safety wrapper.</remarks>
        public SynchronizationContext ActualSynchronizationContext { get; }

        /// <inheritdoc/>
        public override void Send(SendOrPostCallback d, object? state) => ActualSynchronizationContext.Send(s => ExecuteSafely(d, s), state);

        /// <inheritdoc/>
        public override void Post(SendOrPostCallback d, object? state) => ActualSynchronizationContext.Post(s => ExecuteSafely(d, s), state);
    }
}
