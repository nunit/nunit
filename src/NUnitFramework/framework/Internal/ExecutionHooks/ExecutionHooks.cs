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

        internal BeforeHooks BeforeTest { get; } = new();
        internal AfterHooks AfterTest { get; } = new();

        /// <summary>
        /// Adds a hook action to be invoked before the test method is executed.
        /// </summary>
        /// <param name="hookHandler">The hook action to attach to the before-test hook.</param>
        public void AddBeforeTestHandler(Action<TestExecutionContext> hookHandler)
        {
            BeforeTest.AddHandler(hookHandler);
        }

        /// <summary>
        /// Adds a hook action to be invoked after the test method is executed.
        /// </summary>
        /// <param name="hookHandler">The hook action to attach to the after-test hook.</param>
        public void AddAfterTestHandler(Action<TestExecutionContext> hookHandler)
        {
            AfterTest.AddHandler(hookHandler);
        }

        internal ExecutionHooks(ExecutionHooks other)
        {
            BeforeTest = new BeforeHooks(other.BeforeTest);
            AfterTest = new AfterHooks(other.AfterTest);
        }

        internal void OnBeforeTest(TestExecutionContext context)
        {
            BeforeTest.InvokeHandlers(context);
        }

        internal void OnAfterTest(TestExecutionContext context)
        {
            AfterTest.InvokeHandlers(context);
        }
    }
}
