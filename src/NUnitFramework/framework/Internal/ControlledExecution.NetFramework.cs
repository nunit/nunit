// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt
//
// Adapted from original:
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NETFRAMEWORK

using System.Threading;
using NUnit.Framework.Internal;

namespace System.Runtime
{
    /// <summary>
    /// Allows to run code and abort it asynchronously.
    /// </summary>
    internal static class ControlledExecution
    {
        [ThreadStatic]
        private static bool _executingOnCurrentThread;

        /// <summary>
        /// Runs code that may be aborted asynchronously.
        /// </summary>
        /// <param name="action">The delegate that represents the code to execute.</param>
        /// <param name="cancellationToken">The cancellation token that may be used to abort execution.</param>
        /// <exception cref="PlatformNotSupportedException">The method is not supported on this platform.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> argument is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// The current thread is already running the <see cref="Run"/> method.
        /// </exception>
        /// <exception cref="OperationCanceledException">The execution was aborted.</exception>
        /// <remarks>
        /// <para>This method enables aborting arbitrary managed code in a non-cooperative manner by throwing an exception
        /// in the thread executing that code.  While the exception may be caught by the code, it is re-thrown at the end
        /// of `catch` blocks until the execution flow returns to the `ControlledExecution.Run` method.</para>
        /// <para>Execution of the code is not guaranteed to abort immediately, or at all.  This situation can occur, for
        /// example, if a thread is stuck executing unmanaged code or the `catch` and `finally` blocks that are called as
        /// part of the abort procedure, thereby indefinitely delaying the abort.  Furthermore, execution may not be
        /// aborted immediately if the thread is currently executing a `catch` or `finally` block.</para>
        /// <para>Aborting code at an unexpected location may corrupt the state of data structures in the process and lead
        /// to unpredictable results.  For that reason, this method should not be used in production code and calling it
        /// produces a compile-time warning.</para>
        /// </remarks>
        public static void Run(Action action, CancellationToken cancellationToken)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            // ControlledExecution.Run does not support nested invocations.  If there's one already in flight
            // on this thread, fail.
            if (_executingOnCurrentThread)
            {
                throw new InvalidOperationException("Nested ControlledExecution.Run not allowed");
            }

            // Store the current thread so that it may be referenced by the Canceler.Cancel callback if one occurs.
            Canceler canceler = new(Thread.CurrentThread, ThreadUtility.GetCurrentThreadNativeId());

            try
            {
                // Mark this thread as now running a ControlledExecution.Run to prevent recursive usage.
                _executingOnCurrentThread = true;

                // Register for aborting.  From this moment until ctr.Unregister is called, this thread is subject to being
                // interrupted at any moment.  This could happen during the call to UnsafeRegister if cancellation has
                // already been requested at the time of the registration.
                CancellationTokenRegistration ctr = cancellationToken.Register(e => ((Canceler)e!).Cancel(), canceler, useSynchronizationContext: false);
                try
                {
                    // Invoke the caller's code.
                    action();
                }
                finally
                {
                    // This finally block may be cloned by JIT for the non-exceptional code flow.  In that case the code
                    // below is not guarded against aborting.  That is OK as the outer try block will catch the
                    // ThreadAbortException and call ResetAbortThread.

                    ctr.Dispose();
                }
            }
            catch (ThreadAbortException tae)
            {
                // We don't want to leak ThreadAbortExceptions to user code.  Instead, translate the exception into
                // an OperationCanceledException, preserving stack trace details from the ThreadAbortException in
                // order to aid in diagnostics and debugging.
                OperationCanceledException e = cancellationToken.IsCancellationRequested ? new(cancellationToken) : new();
                if (tae.StackTrace is string stackTrace)
                {
                    e.SetRemoteStackTrace(stackTrace);
                }
                throw e;
            }
            finally
            {
                // Unmark this thread for recursion detection.
                _executingOnCurrentThread = false;

                if (cancellationToken.IsCancellationRequested &&
                    Thread.CurrentThread.ThreadState == ThreadState.AbortRequested)
                {
                    // Reset an abort request that may still be pending on this thread.
                    Thread.ResetAbort();
                }
            }
        }

        private sealed class Canceler
        {
            private readonly Thread _thread;
            private readonly int _nativeId;

            public Canceler(Thread thread, int nativeId)
            {
                _thread = thread;
                _nativeId = nativeId;
            }

            public void Cancel()
            {
                // Abort the thread executing the action (which may be the current thread).
                ThreadUtility.Abort(_thread, _nativeId);
            }
        }
    }
}

#endif
