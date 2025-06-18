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
            // using a null suppression operator as we know the method exist
            return GetType().GetMethod(methodName)!.DeclaringType != typeof(ExecutionHookAttribute);
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
