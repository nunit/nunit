// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
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
        private readonly FailureSite _failureSite;

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelAfterCommand"/> class.
        /// </summary>
        /// <param name="innerCommand">The inner command</param>
        /// <param name="timeout">Timeout value</param>
        /// <param name="debugger">An <see cref="IDebugger" /> instance</param>
        /// <param name="failureSite">
        /// The <see cref="FailureSite"/> used to record a cancellation:
        /// <see cref="FailureSite.Test"/> when wrapping a test, or <see cref="FailureSite.SetUp"/>
        /// or <see cref="FailureSite.TearDown"/> when wrapping the one-time setup or teardown of a fixture.
        /// </param>
        internal CancelAfterCommand(TestCommand innerCommand, int timeout, IDebugger debugger, FailureSite failureSite) : base(innerCommand)
        {
            _timeout = timeout;
            _debugger = debugger;
            _failureSite = failureSite;

            Guard.ArgumentValid(innerCommand.Test is TestMethod or TestSuite, "CancelAfterCommand may only apply to a TestMethod or TestSuite", nameof(innerCommand));
            Guard.ArgumentValid(timeout > 0, "Timeout value must be greater than zero", nameof(timeout));
            ArgumentNullException.ThrowIfNull(debugger);
        }

        /// <summary>
        /// Runs the test, saving a TestResult in the supplied TestExecutionContext.
        /// </summary>
        /// <param name="context">The context in which the test should run.</param>
        /// <returns>A TestResult</returns>
        public override TestResult Execute(TestExecutionContext context)
        {
            CancelAfterRunner.Run(context, _timeout, _debugger, _failureSite, () => innerCommand.Execute(context));
            return context.CurrentResult;
        }
    }
}
