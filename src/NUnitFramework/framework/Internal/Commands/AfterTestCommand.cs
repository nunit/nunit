// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// AfterCommand is a DelegatingTestCommand that performs some
    /// specific action after the inner command is run.
    /// </summary>
    public abstract class AfterTestCommand : DelegatingTestCommand
    {
        /// <summary>
        /// Construct an AfterCommand
        /// </summary>
        public AfterTestCommand(TestCommand innerCommand) : base(innerCommand)
        {
        }

        /// <summary>
        /// Execute the command
        /// </summary>
        public override TestResult Execute(TestExecutionContext context)
        {
            Guard.OperationValid(AfterTest is not null, "AfterTest was not set by the derived class constructor");

            RunTestMethodInThreadAbortSafeZone(context, () =>
            {
                context.CurrentResult = innerCommand.Execute(context);
            });

            AfterTest(context);

            return context.CurrentResult;
        }

        /// <summary>
        /// Set this to perform action after the inner command.
        /// </summary>
        protected Action<TestExecutionContext>? AfterTest;
    }
}
