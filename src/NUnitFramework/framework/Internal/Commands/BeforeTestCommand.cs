// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading.Tasks;

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// BeforeTestCommand is a DelegatingTestCommand that performs some
    /// specific action before the inner command is run.
    /// </summary>
    public abstract class BeforeTestCommand : DelegatingTestCommand
    {
        /// <summary>
        /// Construct a BeforeCommand
        /// </summary>
        public BeforeTestCommand(TestCommand innerCommand) : base(innerCommand) { }

        /// <summary>
        /// Execute the command
        /// </summary>
        public override async Task<TestResult> Execute(TestExecutionContext context)
        {
            Guard.OperationValid(BeforeTest != null, "BeforeTest was not set by the derived class constructor");

            BeforeTest(context);

            return context.CurrentResult = await innerCommand.Execute(context);
        }

        /// <summary>
        /// Action to perform before the inner command.
        /// </summary>
        protected Action<TestExecutionContext> BeforeTest;
    }
}
