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

        internal BeforeHooks BeforeAnySetUps { get; } = new();
        internal AfterHooks AfterAnySetUps { get; } = new();
        internal BeforeHooks BeforeTest { get; } = new();
        internal AfterHooks AfterTest { get; } = new();
        internal BeforeHooks BeforeAnyTearDowns { get; } = new();
        internal AfterHooks AfterAnyTearDowns { get; } = new();
        internal BeforeHooks BeforeTestActionBeforeTest { get; set; } = new();
        internal AfterHooks AfterTestActionBeforeTest { get; set; } = new();
        internal BeforeHooks BeforeTestActionAfterTest { get; set; } = new();
        internal AfterHooks AfterTestActionAfterTest { get; set; } = new();

        /// <summary>
        /// Adds a hook action to be invoked before any setup methods are executed.
        /// </summary>
        /// <param name="hookHandler">The hook action to attach to the before-any-setups hook.</param>
        public void AddBeforeAnySetUpsHandler(Action<TestExecutionContext> hookHandler)
        {
            BeforeAnySetUps.AddHandler(hookHandler);
        }

        /// <summary>
        /// Adds a hook action to be invoked after any setup methods are executed.
        /// </summary>
        /// <param name="hookHandler">The hook action to attach to the after-any-setups hook.</param>
        public void AddAfterAnySetUpsHandler(Action<TestExecutionContext> hookHandler)
        {
            AfterAnySetUps.AddHandler(hookHandler);
        }

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

        /// <summary>
        /// Adds a hook action to be invoked before any teardown methods are executed.
        /// </summary>
        /// <param name="hookHandler">The hook action to attach to the before-any-teardowns hook.</param>
        public void AddBeforeAnyTearDownsHandler(Action<TestExecutionContext> hookHandler)
        {
            BeforeAnyTearDowns.AddHandler(hookHandler);
        }

        /// <summary>
        /// Adds a hook action to be invoked after any teardown methods are executed.
        /// </summary>
        /// <param name="hookHandler">The hook action to attach to the after-any-teardowns hook.</param>
        public void AddAfterAnyTearDownsHandler(Action<TestExecutionContext> hookHandler)
        {
            AfterAnyTearDowns.AddHandler(hookHandler);
        }

        internal ExecutionHooks(ExecutionHooks other)
        {
            BeforeAnySetUps = new BeforeHooks(other.BeforeAnySetUps);
            AfterAnySetUps = new AfterHooks(other.AfterAnySetUps);
            BeforeTest = new BeforeHooks(other.BeforeTest);
            AfterTest = new AfterHooks(other.AfterTest);
            BeforeAnyTearDowns = new BeforeHooks(other.BeforeAnyTearDowns);
            AfterAnyTearDowns = new AfterHooks(other.AfterAnyTearDowns);

            BeforeTestActionBeforeTest = new BeforeHooks(other.BeforeTestActionBeforeTest);
            AfterTestActionBeforeTest = new AfterHooks(other.AfterTestActionBeforeTest);
            BeforeTestActionAfterTest = new BeforeHooks(other.BeforeTestActionAfterTest);
            AfterTestActionAfterTest = new AfterHooks(other.AfterTestActionAfterTest);
        }

        /// <summary>
        /// Adds a hook handler to be invoked before the BeforeTest(ITest test) method of an <see cref="ITestAction"></see> is executed.
        /// </summary>
        /// <param name="hookHandler">The hook action to attach to the before-test hook.</param>
        public void AddBeforeTestActionBeforeTestHandler(Action<TestExecutionContext> hookHandler)
        {
            BeforeTestActionBeforeTest.AddHandler(hookHandler);
        }

        /// <summary>
        /// Adds a hook handler to be invoked after the BeforeTest(ITest test) method of an <see cref="ITestAction"></see> is executed.
        /// </summary>
        /// <param name="hookHandler">The hook action to attach to the after-test hook.</param>
        public void AddAfterTestActionBeforeTestHandler(Action<TestExecutionContext> hookHandler)
        {
            AfterTestActionBeforeTest.AddHandler(hookHandler);
        }

        /// <summary>
        /// Adds a hook handler to be invoked before the AfterTest(ITest test) method of an <see cref="ITestAction"></see> is executed.
        /// </summary>
        /// <param name="hookHandler">The hook action to attach to the before-test hook.</param>
        public void AddBeforeTestActionAfterTestHandler(Action<TestExecutionContext> hookHandler)
        {
            BeforeTestActionAfterTest.AddHandler(hookHandler);
        }

        /// <summary>
        /// Adds a hook handler to be invoked after the AfterTest(ITest test) method of an <see cref="ITestAction"></see> is executed.
        /// </summary>
        /// <param name="hookHandler">The hook action to attach to the after-test hook.</param>
        public void AddAfterTestActionAfterTestHandler(Action<TestExecutionContext> hookHandler)
        {
            AfterTestActionAfterTest.AddHandler(hookHandler);
        }

        internal void OnBeforeAnySetUps(TestExecutionContext context)
        {
            BeforeAnySetUps.InvokeHandlers(context);
        }

        internal void OnAfterAnySetUps(TestExecutionContext context)
        {
            AfterAnySetUps.InvokeHandlers(context);
        }

        internal void OnBeforeTest(TestExecutionContext context)
        {
            BeforeTest.InvokeHandlers(context);
        }

        internal void OnAfterTest(TestExecutionContext context)
        {
            AfterTest.InvokeHandlers(context);
        }

        internal void OnBeforeAnyTearDowns(TestExecutionContext context)
        {
            BeforeAnyTearDowns.InvokeHandlers(context);
        }

        internal void OnAfterAnyTearDowns(TestExecutionContext context)
        {
            AfterAnyTearDowns.InvokeHandlers(context);
        }

        internal void OnBeforeTestActionBeforeTest(TestExecutionContext context)
        {
            BeforeTestActionBeforeTest.InvokeHandlers(context);
        }

        internal void OnAfterTestActionBeforeTest(TestExecutionContext context)
        {
            AfterTestActionBeforeTest.InvokeHandlers(context);
        }

        internal void OnBeforeTestActionAfterTest(TestExecutionContext context)
        {
            BeforeTestActionAfterTest.InvokeHandlers(context);
        }

        internal void OnAfterTestActionAfterTest(TestExecutionContext context)
        {
            AfterTestActionAfterTest.InvokeHandlers(context);
        }
    }
}
