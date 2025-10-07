// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Runtime.CompilerServices;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;
using NUnit.Framework.Internal.ExecutionHooks;

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
        /// <param name="hookData">The current <see cref="HookData"/> for the test.</param>
        public virtual void BeforeEverySetUpHook(HookData hookData)
        {
            // Just to verify our logic for detecting overridden methods works correctly.
            // This method should never be called.
            ThrowNeedsOverride();
        }

        /// <summary>
        /// Method that is called <b>immediately after</b> every [SetUp] or [OneTimeSetUp] method is executed.
        /// Override this to implement custom logic to run after the setup.
        /// </summary>
        /// <param name="hookData">The current <see cref="HookData"/> for the test.</param>
        public virtual void AfterEverySetUpHook(HookData hookData)
        {
            // Just to verify our logic for detecting overridden methods works correctly.
            // This method should never be called.
            ThrowNeedsOverride();
        }

        /// <summary>
        /// Method that is called <b>immediately before</b> the test is executed.
        /// Override this to implement custom logic to run before the test.
        /// </summary>
        /// <param name="hookData">The current <see cref="HookData"/> for the test.</param>
        public virtual void BeforeTestHook(HookData hookData)
        {
            // Just to verify our logic for detecting overridden methods works correctly.
            // This method should never be called.
            ThrowNeedsOverride();
        }

        /// <summary>
        /// Method that is called <b>immediately after</b> the test is executed.
        /// Override this to implement custom logic to run after the test.
        /// </summary>
        /// <param name="hookData">The current <see cref="HookData"/> for the test.</param>
        public virtual void AfterTestHook(HookData hookData)
        {
            // Just to verify our logic for detecting overridden methods works correctly.
            // This method should never be called.
            ThrowNeedsOverride();
        }

        /// <summary>
        /// Method that is called <b>immediately before</b> every [TearDown] or [OneTimeTearDown] method is executed.
        /// Override this to implement custom logic to run before the teardown.
        /// </summary>
        /// <param name="hookData">The current <see cref="HookData"/> for the test.</param>
        public virtual void BeforeEveryTearDownHook(HookData hookData)
        {
            // Just to verify our logic for detecting overridden methods works correctly.
            // This method should never be called.
            ThrowNeedsOverride();
        }

        /// <summary>
        /// Method that is called <b>immediately after</b> every [TearDown] or [OneTimeTearDown] method is executed.
        /// Override this to implement custom logic to run after the teardown.
        /// </summary>
        /// <param name="hookData">The current <see cref="HookData"/> for the test.</param>
        public virtual void AfterEveryTearDownHook(HookData hookData)
        {
            // Just to verify our logic for detecting overridden methods works correctly.
            // This method should never be called.
            ThrowNeedsOverride();
        }

        /// <summary>
        /// Method that is called <b>immediately before</b> the BeforeTest(ITest test) method of a <see cref="ITestAction"></see> is executed
        /// </summary>
        /// <param name="hookData">The current <see cref="HookData"/> for the test.</param>
        public virtual void BeforeTestActionBeforeTestHook(HookData hookData)
        {
            // Just to verify our logic for detecting overridden methods works correctly.
            // This method should never be called.
            ThrowNeedsOverride();
        }

        /// <summary>
        /// Method that is called <b>immediately after</b> the BeforeTest(ITest test) method of a <see cref="ITestAction"></see> is executed
        /// </summary>
        /// <param name="hookData">The current <see cref="HookData"/> for the test.</param>
        public virtual void BeforeTestActionAfterTestHook(HookData hookData)
        {
            // Just to verify our logic for detecting overridden methods works correctly.
            // This method should never be called.
            ThrowNeedsOverride();
        }

        /// <summary>
        /// Method that is called <b>immediately before</b> the AfterTest(ITest test) method of a <see cref="ITestAction"></see> is executed
        /// </summary>
        /// <param name="hookData">The current <see cref="HookData"/> for the test.</param>
        public virtual void AfterTestActionBeforeTestHook(HookData hookData)
        {
            // Just to verify our logic for detecting overridden methods works correctly.
            // This method should never be called.
            ThrowNeedsOverride();
        }

        /// <summary>
        /// Method that is called <b>immediately after</b> the AfterTest(ITest test) method of a <see cref="ITestAction"></see> is executed
        /// </summary>
        /// <param name="hookData">The current <see cref="HookData"/> for the test.</param>
        public virtual void AfterTestActionAfterTestHook(HookData hookData)
        {
            // Just to verify our logic for detecting overridden methods works correctly.
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
            if (command is HookDelegatingTestMethodCommand)
            {
                return command;
            }

            if (command is not TestMethodCommand testMethodCommand)
            {
                throw new NUnitException($"{nameof(ExecutionHookAttribute)} " +
                    $"can only be applied to {nameof(TestMethodCommand)}. " +
                    $"Received: {command.GetType().Name}.");
            }

            return new HookDelegatingTestMethodCommand(testMethodCommand);
        }

        /// <inheritdoc />
        public void ApplyToContext(TestExecutionContext context)
        {
            ExecutionHooks executionHooks = context.GetOrCreateExecutionHooks();

            if (BeforeEverySetUpHook != base.BeforeEverySetUpHook)
            {
                // Only add the BeforeEverySetUpHook if it has been overridden
                executionHooks.AddBeforeEverySetUpHandler(BeforeEverySetUpHook);
            }

            if (AfterEverySetUpHook != base.AfterEverySetUpHook)
            {
                // Only add the AfterEverySetUpHook if it has been overridden
                executionHooks.AddAfterEverySetUpHandler(AfterEverySetUpHook);
            }

            if (BeforeTestHook != base.BeforeTestHook)
            {
                // Only add the BeforeTestHook if it has been overridden
                executionHooks.AddBeforeTestHandler(BeforeTestHook);
            }

            if (AfterTestHook != base.AfterTestHook)
            {
                // Only add the AfterTestHook if it has been overridden
                executionHooks.AddAfterTestHandler(AfterTestHook);
            }

            if (BeforeEveryTearDownHook != base.BeforeEveryTearDownHook)
            {
                // Only add the BeforeEveryTearDownHook if it has been overridden
                executionHooks.AddBeforeEveryTearDownHandler(BeforeEveryTearDownHook);
            }

            if (AfterEveryTearDownHook != base.AfterEveryTearDownHook)
            {
                // Only add the AfterEveryTearDownHook if it has been overridden
                executionHooks.AddAfterEveryTearDownHandler(AfterEveryTearDownHook);
            }

            if (BeforeTestActionBeforeTestHook != base.BeforeTestActionBeforeTestHook)
            {
                // Only add the BeforeTestActionBeforeTestHook if it has been overridden
                executionHooks.AddBeforeTestActionBeforeTestHandler(BeforeTestActionBeforeTestHook);
            }

            if (AfterTestActionBeforeTestHook != base.AfterTestActionBeforeTestHook)
            {
                // Only add the AfterTestActionBeforeTestHook if it has been overridden
                executionHooks.AddAfterTestActionBeforeTestHandler(AfterTestActionBeforeTestHook);
            }

            if (BeforeTestActionAfterTestHook != base.BeforeTestActionAfterTestHook)
            {
                // Only add the BeforeTestActionAfterTestHook if it has been overridden
                context.ExecutionHooks.AddBeforeTestActionAfterTestHandler(BeforeTestActionAfterTestHook);
            }

            if (AfterTestActionAfterTestHook != base.AfterTestActionAfterTestHook)
            {
                // Only add the AfterTestActionAfterTestHook if it has been overridden
                executionHooks.AddAfterTestActionAfterTestHandler(AfterTestActionAfterTestHook);
            }
        }
    }
}
