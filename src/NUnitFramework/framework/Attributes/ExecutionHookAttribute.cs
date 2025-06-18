// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;

namespace NUnit.Framework
{
    /// <summary>
    /// Abstract base attribute class for activating execution hooks.
    /// </summary>
    public abstract class ExecutionHookAttribute : NUnitAttribute, IWrapTestMethod, IApplyToContext
    {
        /// <inheritdoc />
        public TestCommand Wrap(TestCommand command)
        {
            return command is HookDelegatingTestCommand ? command : new HookDelegatingTestCommand(command);
        }

        /// <inheritdoc />
        public void ApplyToContext(TestExecutionContext context)
        {
            if (IsHookImplemented(nameof(BeforeTestHook)))
            {
                context.ExecutionHooks.AddBeforeTestHandler((sender, eventArgs) =>
                {
                    BeforeTestHook(context);
                });
            }

            if (IsHookImplemented(nameof(AfterTestHook)))
            {
                context.ExecutionHooks.AddAfterTestHandler((sender, eventArgs) =>
                {
                    AfterTestHook(context);
                });
            }
        }

        private bool IsHookImplemented(string methodName)
        {
            var thisMethod = GetType().GetMethod(methodName);
            var baseMethod = typeof(ExecutionHookAttribute).GetMethod(methodName);

            // If either method is null, this method was not called for a hook method.
            if (thisMethod is null || baseMethod is null)
            {
                return false;
            }

            return thisMethod.DeclaringType != baseMethod.DeclaringType;
        }

        /// <summary>
        /// Method that is called <b>immediately before</b> the test is executed.
        /// Override this to implement custom logic to run before the test.
        /// </summary>
        /// <param name="context">The current <see cref="TestExecutionContext"/> for the test.</param>
        public virtual void BeforeTestHook(TestExecutionContext context)
        {
        }

        /// <summary>
        /// Method that is called <b>immediately after</b> the test is executed.
        /// Override this to implement custom logic to run after the test.
        /// </summary>
        /// <param name="context">The current <see cref="TestExecutionContext"/> for the test.</param>
        public virtual void AfterTestHook(TestExecutionContext context)
        {
        }
    }
}
