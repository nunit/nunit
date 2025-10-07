// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal.Commands
{
    internal sealed class HookDelegatingTestCommand : DelegatingTestCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HookDelegatingTestCommand"/> class.
        /// </summary>
        /// <param name="innerCommand">The inner test command to delegate to.</param>
        public HookDelegatingTestCommand(TestCommand innerCommand) : base(innerCommand)
        {
        }

        /// <summary>
        /// Executes the test command within the provided context
        /// </summary>
        /// <param name="context">The test execution context.</param>
        /// <returns>The result of the test execution.</returns>
        public override TestResult Execute(TestExecutionContext context)
        {
            Assume.That(context.ExecutionHooksEnabled, Is.True, "This command should only have been created if Execution hooks were enabled");

            try
            {
                context.ExecutionHooks.OnBeforeTest(context);
                innerCommand.Execute(context);
            }
            finally
            {
                context.ExecutionHooks.OnAfterTest(context);
            }

            return context.CurrentResult;
        }
    }
}
