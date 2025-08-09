// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Commands
{
    internal sealed class HookDelegatingTestMethodCommand : TestCommand
    {
        private readonly TestMethodCommand _innerCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="HookDelegatingTestMethodCommand"/> class.
        /// </summary>
        /// <param name="innerCommand">The inner test command to delegate to.</param>
        public HookDelegatingTestMethodCommand(TestMethodCommand innerCommand) : base(innerCommand.Test)
        {
            _innerCommand = innerCommand;
        }

        /// <summary>
        /// Executes the test command within the provided context and
        /// executes the hooks before and after the test method.
        /// </summary>
        /// <param name="context">The test execution context.</param>
        /// <returns>The result of the test execution.</returns>
        public override TestResult Execute(TestExecutionContext context)
        {
            IMethodInfo hookedMethodInfo = _innerCommand.TestMethod.Method;

            try
            {
                context.ExecutionHooks.OnBeforeTest(context, hookedMethodInfo);
                _innerCommand.Execute(context);
            }
            catch (Exception ex)
            {
                context.ExecutionHooks.OnAfterTest(context, hookedMethodInfo, ex);
                throw;
            }
            context.ExecutionHooks.OnAfterTest(context, hookedMethodInfo);

            return context.CurrentResult;
        }
    }
}
