// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading;

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// DelegatingTestCommand wraps an inner TestCommand.
    /// Derived classes may do what they like before or
    /// after running the inner command.
    /// </summary>
    public abstract class DelegatingTestCommand : TestCommand
    {
        /// <summary>TODO: Documentation needed for field</summary>
#pragma warning disable IDE1006
        // ReSharper disable once InconsistentNaming
        // Disregarding naming convention for back-compat
        protected TestCommand innerCommand;
#pragma warning restore IDE1006

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegatingTestCommand"/> class.
        /// </summary>
        /// <param name="innerCommand">The inner command.</param>
        protected DelegatingTestCommand(TestCommand innerCommand)
            : base(innerCommand.Test)
        {
            this.innerCommand = innerCommand;
        }

        /// <summary>
        /// Runs the test with exception handling.
        /// </summary>
        protected static void RunTestMethodInThreadAbortSafeZone(TestExecutionContext context, Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
#if THREAD_ABORT
                if (ex is ThreadAbortException)
                    Thread.ResetAbort();
#endif
                context.CurrentResult.RecordException(ex);
            }
        }
    }
}
