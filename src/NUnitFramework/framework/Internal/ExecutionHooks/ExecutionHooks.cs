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

        internal BeforeHooks BeforeEverySetUp { get; } = new();
        internal AfterHooks AfterEverySetUp { get; } = new();
        internal BeforeHooks BeforeTest { get; } = new();
        internal AfterHooks AfterTest { get; } = new();
        internal BeforeHooks BeforeEveryTearDown { get; } = new();
        internal AfterHooks AfterEveryTearDown { get; } = new();
        internal BeforeHooks BeforeTestActionBeforeTest { get; } = new();
        internal AfterHooks AfterTestActionBeforeTest { get; } = new();
        internal BeforeHooks BeforeTestActionAfterTest { get; } = new();
        internal AfterHooks AfterTestActionAfterTest { get; } = new();

        /// <summary>
        /// Adds a hook action to be invoked before every setup method is executed.
        /// </summary>
        /// <param name="hookHandler">The hook action to attach to the before-every-setup hook.</param>
        public void AddBeforeEverySetUpHandler(Action<TestExecutionContext> hookHandler)
        {
            BeforeEverySetUp.AddHandler(hookHandler);
        }

        /// <summary>
        /// Adds a hook action to be invoked after every setup method is executed.
        /// </summary>
        /// <param name="hookHandler">The hook action to attach to the after-every-setup hook.</param>
        public void AddAfterEverySetUpHandler(Action<TestExecutionContext> hookHandler)
        {
            AfterEverySetUp.AddHandler(hookHandler);
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
        /// Adds a hook action to be invoked before every teardown method is executed.
        /// </summary>
        /// <param name="hookHandler">The hook action to attach to the before-every-teardown hook.</param>
        public void AddBeforeEveryTearDownHandler(Action<TestExecutionContext> hookHandler)
        {
            BeforeEveryTearDown.AddHandler(hookHandler);
        }

        /// <summary>
        /// Adds a hook action to be invoked after every teardown method is executed.
        /// </summary>
        /// <param name="hookHandler">The hook action to attach to the after-every-teardown hook.</param>
        public void AddAfterEveryTearDownHandler(Action<TestExecutionContext> hookHandler)
        {
            AfterEveryTearDown.AddHandler(hookHandler);
        }

        internal ExecutionHooks(ExecutionHooks other)
        {
            BeforeEverySetUp = new BeforeHooks(other.BeforeEverySetUp);
            AfterEverySetUp = new AfterHooks(other.AfterEverySetUp);
            BeforeTest = new BeforeHooks(other.BeforeTest);
            AfterTest = new AfterHooks(other.AfterTest);
            BeforeEveryTearDown = new BeforeHooks(other.BeforeEveryTearDown);
            AfterEveryTearDown = new AfterHooks(other.AfterEveryTearDown);

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

        internal void OnBeforeEverySetUp(TestExecutionContext context)
        {
            BeforeEverySetUp.InvokeHandlers(context);
        }

        internal void OnAfterEverySetUp(TestExecutionContext context)
        {
            AfterEverySetUp.InvokeHandlers(context);
        }

        internal void OnBeforeTest(TestExecutionContext context)
        {
            BeforeTest.InvokeHandlers(context);
        }

        internal void OnAfterTest(TestExecutionContext context)
        {
            AfterTest.InvokeHandlers(context);
        }

        internal void OnBeforeEveryTearDown(TestExecutionContext context)
        {
            BeforeEveryTearDown.InvokeHandlers(context);
        }

        internal void OnAfterEveryTearDown(TestExecutionContext context)
        {
            AfterEveryTearDown.InvokeHandlers(context);
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
