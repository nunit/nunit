// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Runtime.CompilerServices;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;

namespace NUnit.Framework
{
    /// <summary>
    /// Abstract base attribute class for defining execution hooks methods.
    /// </summary>
    public abstract class ExecutionHookMethodsAttribute : NUnitAttribute
    {
        private static void ThrowNeedsOverride([CallerMemberName] string methodName = "")
        {
            throw new NUnitException($"{methodName} must be overridden in a derived class to provide custom logic.");
        }

        /// <summary>
        /// Method that is called <b>immediately before</b> every [SetUp] or [OneTimeSetUp] method is executed.
        /// Override this to implement custom logic to run before the test.
        /// </summary>
        /// <param name="context">The current <see cref="TestExecutionContext"/> for the test.</param>
        public virtual void BeforeEverySetUpHook(TestExecutionContext context)
        {
            // Just to verify our logic for detecting overridden methods works correctly.
            // This method should never be called.
            ThrowNeedsOverride();
        }

        /// <summary>
        /// Method that is called <b>immediately after</b> every [SetUp] or [OneTimeSetUp] method is executed.
        /// Override this to implement custom logic to run after the setup.
        /// </summary>
        /// <param name="context">The current <see cref="TestExecutionContext"/> for the test.</param>
        public virtual void AfterEverySetUpHook(TestExecutionContext context)
        {
            // Just to verify our logic for detecing overridden methods works correctly.
            // This method should never be called.
            ThrowNeedsOverride();
        }

        /// <summary>
        /// Method that is called <b>immediately before</b> the test is executed.
        /// Override this to implement custom logic to run before the test.
        /// </summary>
        /// <param name="context">The current <see cref="TestExecutionContext"/> for the test.</param>
        public virtual void BeforeTestHook(TestExecutionContext context)
        {
            // Just to verify our logic for detecing overridden methods works correctly.
            // This method should never be called.
            ThrowNeedsOverride();
        }

        /// <summary>
        /// Method that is called <b>immediately after</b> the test is executed.
        /// Override this to implement custom logic to run after the test.
        /// </summary>
        /// <param name="context">The current <see cref="TestExecutionContext"/> for the test.</param>
        public virtual void AfterTestHook(TestExecutionContext context)
        {
            // Just to verify our logic for detecing overridden methods works correctly.
            // This method should never be called.
            ThrowNeedsOverride();
        }

        /// <summary>
        /// Method that is called <b>immediately before</b> every [TearDown] or [OneTimeTearDown] method is executed.
        /// Override this to implement custom logic to run before the teardown.
        /// </summary>
        /// <param name="context">The current <see cref="TestExecutionContext"/> for the test.</param>
        public virtual void BeforeEveryTearDownHook(TestExecutionContext context)
        {
            // Just to verify our logic for detecing overridden methods works correctly.
            // This method should never be called.
            ThrowNeedsOverride();
        }

        /// <summary>
        /// Method that is called <b>immediately after</b> every [TearDown] or [OneTimeTearDown] method is executed.
        /// Override this to implement custom logic to run after the teardown.
        /// </summary>
        /// <param name="context">The current <see cref="TestExecutionContext"/> for the test.</param>
        public virtual void AfterEveryTearDownHook(TestExecutionContext context)
        {
            // Just to verify our logic for detecing overridden methods works correctly.
            // This method should never be called.
            ThrowNeedsOverride();
        }
    }

    /// <summary>
    /// Abstract base attribute class for activating execution hooks.
    /// </summary>
    public abstract class ExecutionHookAttribute : ExecutionHookMethodsAttribute, IWrapTestMethod, IApplyToContext
    {
        /// <inheritdoc />
        public TestCommand Wrap(TestCommand command)
        {
            return command is HookDelegatingTestCommand ? command : new HookDelegatingTestCommand(command);
        }

        /// <inheritdoc />
        public void ApplyToContext(TestExecutionContext context)
        {
            if (BeforeEverySetUpHook != base.BeforeEverySetUpHook)
            {
                // Only add the BeforeEverySetUpHook if it has been overridden
                context.ExecutionHooks.AddBeforeEverySetUpHandler(BeforeEverySetUpHook);
            }

            if (AfterEverySetUpHook != base.AfterEverySetUpHook)
            {
                // Only add the AfterEverySetUpHook if it has been overridden
                context.ExecutionHooks.AddAfterEverySetUpHandler(AfterEverySetUpHook);
            }

            if (BeforeTestHook != base.BeforeTestHook)
            {
                // Only add the BeforeTestHook if it has been overridden
                context.ExecutionHooks.AddBeforeTestHandler(BeforeTestHook);
            }

            if (AfterTestHook != base.AfterTestHook)
            {
                // Only add the AfterTestHook if it has been overridden
                context.ExecutionHooks.AddAfterTestHandler(AfterTestHook);
            }

            if (BeforeEveryTearDownHook != base.BeforeEveryTearDownHook)
            {
                // Only add the BeforeEveryTearDownHook if it has been overridden
                context.ExecutionHooks.AddBeforeEveryTearDownHandler(BeforeEveryTearDownHook);
            }

            if (AfterEveryTearDownHook != base.AfterEveryTearDownHook)
            {
                // Only add the AfterEveryTearDownHook if it has been overridden
                context.ExecutionHooks.AddAfterEveryTearDownHandler(AfterEveryTearDownHook);
            }
        }
    }
}
