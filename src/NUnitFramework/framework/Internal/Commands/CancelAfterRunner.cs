// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Abstractions;

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// Runs code with a cancellation token that is cancelled when a <see cref="CancelAfterAttribute"/> timeout expires.
    /// Used for tests, the one-time setup and teardown of fixtures, and setup and teardown methods.
    /// </summary>
    internal static class CancelAfterRunner
    {
        /// <summary>
        /// Runs <paramref name="action"/> with a cancellation token that is cancelled when <paramref name="timeout"/> expires,
        /// or when the current token of <paramref name="context"/> is cancelled.
        /// </summary>
        /// <param name="context">The context whose <see cref="TestExecutionContext.CancellationToken"/> is replaced while running.</param>
        /// <param name="timeout">The timeout in milliseconds.</param>
        /// <param name="debugger">
        /// Because a debugger can be attached after the code started running, the token is only cancelled
        /// if no debugger is attached when the timer expires.
        /// </param>
        /// <param name="failureSite">The <see cref="FailureSite"/> used to record a cancellation.</param>
        /// <param name="action">The code to run.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="action"/> completed,
        /// <see langword="false"/> if it was cancelled and the cancellation was recorded.
        /// Other exceptions are not caught.
        /// </returns>
        public static bool Run(TestExecutionContext context, int timeout, IDebugger debugger, FailureSite failureSite, Action action)
        {
            CancellationToken enclosingToken = context.CancellationToken;
            string enclosingPhase = context.CancelAfterPhase;
            int enclosingTimeout = context.CancelAfterTimeout;

            using var timedOutTokenSource = new CancellationTokenSource();
            using var timerTokenSource = new CancellationTokenSource(timeout);

            // The registration is disposed before both token sources. Disposing it waits for a
            // callback that is already running, so the callback can't use a disposed source.
            // If the enclosing token was cancelled first, the code may still be running when this timer expires.
            // Not cancelling then ensures the timeout that expired first is reported.
            using var registration = timerTokenSource.Token.Register(() =>
            {
                if (!debugger.IsAttached && !enclosingToken.IsCancellationRequested)
                    timedOutTokenSource.Cancel();
            });

            using var combinedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(enclosingToken, timedOutTokenSource.Token);

            context.CancellationToken = combinedTokenSource.Token;
            context.CancelAfterPhase = GetPhase(failureSite, context.CurrentTest.IsSuite);
            context.CancelAfterTimeout = timeout;

            try
            {
                action();
                return true;
            }
            catch (Exception ex) when (ex.Unwrap() is OperationCanceledException)
            {
                // If an enclosing timeout cancelled the code, report that one instead of this one.
                if (!timedOutTokenSource.IsCancellationRequested)
                {
                    context.CancelAfterPhase = enclosingPhase;
                    context.CancelAfterTimeout = enclosingTimeout;
                }

                // Record it before the token is reset, so it is reported as exceeding CancelAfter.
                // Exceptions from reflection invoked code, like a fixture constructor, are still wrapped.
                context.CurrentResult.RecordException(ex, failureSite);
                return false;
            }
            finally
            {
                context.CancellationToken = enclosingToken;
                context.CancelAfterPhase = enclosingPhase;
                context.CancelAfterTimeout = enclosingTimeout;
            }
        }

        private static string GetPhase(FailureSite failureSite, bool isSuite) => failureSite switch
        {
            FailureSite.SetUp => isSuite ? "OneTimeSetUp" : "SetUp",
            FailureSite.TearDown => isSuite ? "OneTimeTearDown" : "TearDown",
            _ => "Test",
        };
    }
}
