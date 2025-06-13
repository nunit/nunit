// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Linq;

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

        /// <summary>
        /// Gets a value indicating whether any test hooks are registered
        /// in either <see cref="BeforeTest"/> or <see cref="AfterTest"/>.
        /// </summary>
        internal bool TestHooksUsed => BeforeTest.GetHandlers().Count > 0 || AfterTest.GetHandlers().Count > 0;

        internal ExecutionHooks(ExecutionHooks other)
        {
            other.BeforeTest.GetHandlers().ToList().ForEach(d => BeforeTest.AddHandler(d));
            other.AfterTest.GetHandlers().ToList().ForEach(d => AfterTest.AddHandler(d));
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
