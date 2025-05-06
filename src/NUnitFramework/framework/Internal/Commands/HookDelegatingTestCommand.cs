// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Internal.Commands
{
    internal class HookDelegatingTestCommand : DelegatingTestCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HookDelegatingTestCommand"/> class.
        /// </summary>
        /// <param name="innerCommand">The inner test command to delegate to.</param>
        public HookDelegatingTestCommand(TestCommand innerCommand) : base(innerCommand) { }

        /// <summary>
        /// Executes the test command within the provided context
        /// </summary>
        /// <param name="context">The test execution context.</param>
        /// <returns>The result of the test execution.</returns>
        public override TestResult Execute(TestExecutionContext context)
        {
            var afterTestExecutedWithExceptionContext = false;

            try
            {
                context.HookExtension?.OnBeforeTest(context);
                innerCommand.Execute(context);
            }
            catch (Exception ex)
            {
                afterTestExecutedWithExceptionContext = true;
                context.HookExtension?.OnAfterTest(context);
                throw;
            }

            // Ensure that after test hooks are not again executed when there are exceptions from inner command
            if (!afterTestExecutedWithExceptionContext)
            {
                context.HookExtension?.OnAfterTest(context);
            }

            return context.CurrentResult;
        }
    }
}
