// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading;
using NUnit.Framework.Internal.Abstractions;

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// <see cref="CancelAfterCommand"/> creates a timer in order to cancel
    /// a test if it exceeds a specified time and adjusts
    /// the test result if it did time out.
    /// </summary>
    public class CancelAfterCommand : DelegatingTestCommand
    {
        private readonly int _timeout;
        private readonly IDebugger _debugger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeoutCommand"/> class.
        /// </summary>
        /// <param name="innerCommand">The inner command</param>
        /// <param name="timeout">Timeout value</param>
        /// <param name="debugger">An <see cref="IDebugger" /> instance</param>
        internal CancelAfterCommand(TestCommand innerCommand, int timeout, IDebugger debugger) : base(innerCommand)
        {
            _timeout = timeout;
            _debugger = debugger;

            Guard.ArgumentValid(innerCommand.Test is TestMethod, "CancelAfterCommand may only apply to a TestMethod", nameof(innerCommand));
            Guard.ArgumentValid(timeout > 0, "Timeout value must be greater than zero", nameof(timeout));
            Guard.ArgumentNotNull(debugger, nameof(debugger));
        }

        /// <summary>
        /// Runs the test, saving a TestResult in the supplied TestExecutionContext.
        /// </summary>
        /// <param name="context">The context in which the test should run.</param>
        /// <returns>A TestResult</returns>
        public override TestResult Execute(TestExecutionContext context)
        {
            // Because of the debugger possibly attaching after the test method is started
            // we use a 2-prong approach where we only cancel the test's cancellation token
            // if the debugger is not attached when the timer expires.
            using var testCancellationTokenSource = new CancellationTokenSource();
            using var timedCancellationTokenSource = new CancellationTokenSource(_timeout);
            timedCancellationTokenSource.Token.Register(() =>
            {
                if (!_debugger.IsAttached)
                    testCancellationTokenSource.Cancel();
            });

            context.CancellationToken = testCancellationTokenSource.Token;

            try
            {
                innerCommand.Execute(context);
            }
            catch (OperationCanceledException ex)
            {
                context.CurrentResult.RecordException(ex);
            }
            finally
            {
                context.CancellationToken = CancellationToken.None;
            }

            return context.CurrentResult;
        }
    }
}
