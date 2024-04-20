// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt
//
// Adapted from original:
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NET6_0

using System;
using System.Threading;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Allows to run code and abort it asynchronously.
    /// </summary>
    internal static class NUnitControlledExecution
    {
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

            throw new PlatformNotSupportedException(
                ".NET 6.0 has no Thread.Abort functionality, use cooperative cancellation using the CancelAfterAttribute");
        }
    }
}

#endif
