// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Internal.ExecutionHooks
{
    /// <summary>
    /// Provides hooks for executing custom code before and after test methods.
    /// </summary>
    public sealed class ExecutionHooks
    {
        internal ExecutionHooks()
        {
        }

        internal TestHook BeforeTest { get; } = new();
        internal TestHook AfterTest { get; } = new();

        /// <summary>
        /// Adds a hook handler to be invoked before the test method is executed.
        /// </summary>
        /// <param name="hookHandler">The event handler to attach to the before-test hook.</param>
        public void AddBeforeTestHandler(EventHandler hookHandler)
        {
            BeforeTest.AddHandler(hookHandler);
        }

        /// <summary>
        /// Adds a hook handler to be invoked after the test method is executed.
        /// </summary>
        /// <param name="hookHandler">The event handler to attach to the after-test hook.</param>
        public void AddAfterTestHandler(EventHandler hookHandler)
        {
            AfterTest.AddHandler(hookHandler);
        }

        internal ExecutionHooks(ExecutionHooks other)
        {
            BeforeTest = new TestHook(other.BeforeTest);
            AfterTest = new TestHook(other.AfterTest);
        }

        internal void OnBeforeTest(TestExecutionContext context)
        {
            BeforeTest.InvokeHandlers(this, new MethodHookEventArgs(context));
        }

        internal void OnAfterTest(TestExecutionContext context)
        {
            AfterTest.InvokeHandlers(this, new MethodHookEventArgs(context));
        }
    }
}
