// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;

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
        internal void AddBeforeEverySetUpHandler(Action<HookData> hookHandler)
        {
            BeforeEverySetUp.AddHandler(hookHandler);
        }

        /// <summary>
        /// Adds a hook action to be invoked after every setup method is executed.
        /// </summary>
        /// <param name="hookHandler">The hook action to attach to the after-every-setup hook.</param>
        internal void AddAfterEverySetUpHandler(Action<HookData> hookHandler)
        {
            AfterEverySetUp.AddHandler(hookHandler);
        }

        /// <summary>
        /// Adds a hook action to be invoked before the test method is executed.
        /// </summary>
        /// <param name="hookHandler">The hook action to attach to the before-test hook.</param>
        internal void AddBeforeTestHandler(Action<HookData> hookHandler)
        {
            BeforeTest.AddHandler(hookHandler);
        }

        /// <summary>
        /// Adds a hook action to be invoked after the test method is executed.
        /// </summary>
        /// <param name="hookHandler">The hook action to attach to the after-test hook.</param>
        internal void AddAfterTestHandler(Action<HookData> hookHandler)
        {
            AfterTest.AddHandler(hookHandler);
        }

        /// <summary>
        /// Adds a hook action to be invoked before every teardown method is executed.
        /// </summary>
        /// <param name="hookHandler">The hook action to attach to the before-every-teardown hook.</param>
        internal void AddBeforeEveryTearDownHandler(Action<HookData> hookHandler)
        {
            BeforeEveryTearDown.AddHandler(hookHandler);
        }

        /// <summary>
        /// Adds a hook action to be invoked after every teardown method is executed.
        /// </summary>
        /// <param name="hookHandler">The hook action to attach to the after-every-teardown hook.</param>
        internal void AddAfterEveryTearDownHandler(Action<HookData> hookHandler)
        {
            AfterEveryTearDown.AddHandler(hookHandler);
        }

        /// <summary>
        /// Adds a hook handler to be invoked before the BeforeTest(ITest test) method of an <see cref="ITestAction"></see> is executed.
        /// </summary>
        /// <param name="hookHandler">The hook action to attach to the before-test hook.</param>
        internal void AddBeforeTestActionBeforeTestHandler(Action<HookData> hookHandler)
        {
            BeforeTestActionBeforeTest.AddHandler(hookHandler);
        }

        /// <summary>
        /// Adds a hook handler to be invoked after the BeforeTest(ITest test) method of an <see cref="ITestAction"></see> is executed.
        /// </summary>
        /// <param name="hookHandler">The hook action to attach to the after-test hook.</param>
        internal void AddAfterTestActionBeforeTestHandler(Action<HookData> hookHandler)
        {
            AfterTestActionBeforeTest.AddHandler(hookHandler);
        }

        /// <summary>
        /// Adds a hook handler to be invoked before the AfterTest(ITest test) method of an <see cref="ITestAction"></see> is executed.
        /// </summary>
        /// <param name="hookHandler">The hook action to attach to the before-test hook.</param>
        internal void AddBeforeTestActionAfterTestHandler(Action<HookData> hookHandler)
        {
            BeforeTestActionAfterTest.AddHandler(hookHandler);
        }

        /// <summary>
        /// Adds a hook handler to be invoked after the AfterTest(ITest test) method of an <see cref="ITestAction"></see> is executed.
        /// </summary>
        /// <param name="hookHandler">The hook action to attach to the after-test hook.</param>
        internal void AddAfterTestActionAfterTestHandler(Action<HookData> hookHandler)
        {
            AfterTestActionAfterTest.AddHandler(hookHandler);
        }

        internal void OnBeforeEverySetUp(TestExecutionContext context, IMethodInfo hookedMethod)
        {
            BeforeEverySetUp.InvokeHandlers(new HookData(context, hookedMethod));
        }

        internal void OnAfterEverySetUp(TestExecutionContext context, IMethodInfo hookedMethod, Exception? exceptionContext = null)
        {
            AfterEverySetUp.InvokeHandlers(new HookData(context, hookedMethod, exceptionContext));
        }

        internal void OnBeforeTest(TestExecutionContext context, IMethodInfo hookedMethod)
        {
            BeforeTest.InvokeHandlers(new HookData(context, hookedMethod));
        }

        internal void OnAfterTest(TestExecutionContext context, IMethodInfo hookedMethod, Exception? exceptionContext = null)
        {
            AfterTest.InvokeHandlers(new HookData(context, hookedMethod, exceptionContext));
        }

        internal void OnBeforeEveryTearDown(TestExecutionContext context, IMethodInfo hookedMethod)
        {
            BeforeEveryTearDown.InvokeHandlers(new HookData(context, hookedMethod));
        }

        internal void OnAfterEveryTearDown(TestExecutionContext context, IMethodInfo hookedMethod, Exception? exceptionContext = null)
        {
            AfterEveryTearDown.InvokeHandlers(new HookData(context, hookedMethod, exceptionContext));
        }

        internal void OnBeforeTestActionBeforeTest(TestExecutionContext context, IMethodInfo hookedMethod)
        {
            BeforeTestActionBeforeTest.InvokeHandlers(new HookData(context, hookedMethod));
        }

        internal void OnAfterTestActionBeforeTest(TestExecutionContext context, IMethodInfo hookedMethod, Exception? exceptionContext = null)
        {
            AfterTestActionBeforeTest.InvokeHandlers(new HookData(context, hookedMethod, exceptionContext));
        }

        internal void OnBeforeTestActionAfterTest(TestExecutionContext context, IMethodInfo hookedMethod)
        {
            BeforeTestActionAfterTest.InvokeHandlers(new HookData(context, hookedMethod));
        }

        internal void OnAfterTestActionAfterTest(TestExecutionContext context, IMethodInfo hookedMethod, Exception? exceptionContext = null)
        {
            AfterTestActionAfterTest.InvokeHandlers(new HookData(context, hookedMethod, exceptionContext));
        }
    }
}
