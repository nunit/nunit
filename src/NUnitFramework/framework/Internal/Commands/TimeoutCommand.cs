// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal.Abstractions;

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// Unlike <see cref="CancelAfterCommand"/> this command will try to Abort the current executing Thread after the timeout.
    /// </summary>
    public class TimeoutCommand : CancelAfterCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeoutCommand"/> class.
        /// </summary>
        /// <param name="innerCommand">The inner command</param>
        /// <param name="timeout">Timeout value</param>
        /// <param name="debugger">An <see cref="IDebugger" /> instance</param>
        internal TimeoutCommand(TestCommand innerCommand, int timeout, IDebugger debugger)
            : base(innerCommand, timeout, debugger)
        {
        }

        /// <inheritdoc/>
        protected override void ExecuteInnerCommand(TestExecutionContext context)
        {
            NUnitControlledExecution.Run(() => base.ExecuteInnerCommand(context), context.CancellationToken);
        }
    }
}
