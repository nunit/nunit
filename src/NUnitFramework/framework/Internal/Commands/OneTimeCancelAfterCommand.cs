// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Abstractions;

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// <see cref="OneTimeCancelAfterCommand"/> runs the one-time setup or teardown of a fixture
    /// in a <see cref="CancelAfterCommand"/> if a <see cref="CancelAfterAttribute"/> applies to it.
    /// </summary>
    /// <remarks>
    /// The timeout is taken from the <see cref="TestExecutionContext"/> when the command executes,
    /// after the attributes of the fixture have been applied to it. This uses the same value as the
    /// tests of the fixture and as the message reported when the timeout is exceeded.
    /// </remarks>
    internal sealed class OneTimeCancelAfterCommand : DelegatingTestCommand
    {
        private readonly IDebugger _debugger;
        private readonly FailureSite _failureSite;

        /// <summary>
        /// Initializes a new instance of the <see cref="OneTimeCancelAfterCommand"/> class.
        /// </summary>
        /// <param name="innerCommand">The inner command</param>
        /// <param name="debugger">An <see cref="IDebugger" /> instance</param>
        /// <param name="failureSite">
        /// <see cref="FailureSite.SetUp"/> when wrapping the one-time setup,
        /// <see cref="FailureSite.TearDown"/> when wrapping the one-time teardown.
        /// </param>
        public OneTimeCancelAfterCommand(TestCommand innerCommand, IDebugger debugger, FailureSite failureSite)
            : base(innerCommand)
        {
            Guard.ArgumentValid(innerCommand.Test is TestSuite, "OneTimeCancelAfterCommand may only apply to a TestSuite", nameof(innerCommand));
            ArgumentNullException.ThrowIfNull(debugger);

            _debugger = debugger;
            _failureSite = failureSite;
        }

        /// <summary>
        /// Runs the inner command, cancelling it after the timeout in the context if one applies.
        /// </summary>
        /// <param name="context">The context in which the command should run.</param>
        /// <returns>A TestResult</returns>
        public override TestResult Execute(TestExecutionContext context)
        {
            if (context.UseCancellation && context.TestCaseTimeout > 0)
                return new CancelAfterCommand(innerCommand, context.TestCaseTimeout, _debugger, _failureSite).Execute(context);

            return innerCommand.Execute(context);
        }
    }
}
