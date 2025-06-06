// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Linq;

namespace NUnit.Framework.Internal.ExecutionHooks
{
    /// <summary>
    /// Provides hooks for executing custom code before and after test methods.
    /// </summary>
    public sealed class ExecutionHooks
    {
        /// <summary>
        /// Default ctor of <see cref="ExecutionHooks"/> class.
        /// </summary>
        public ExecutionHooks()
        {
            BeforeTest = new TestHook();
            AfterTest = new TestHook();
        }

        /// <summary>
        /// Gets or sets the hook event that is triggered before a test method is executed.
        /// </summary>
        public TestHook BeforeTest { get; }

        /// <summary>
        /// Gets or sets the hook event that is triggered after a test method is executed.
        /// </summary>
        public TestHook AfterTest { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionHooks"/> class by copying hooks from another instance.
        /// </summary>
        /// <param name="other">The instance of <see cref="ExecutionHooks"/> to copy hooks from.</param>
        public ExecutionHooks(ExecutionHooks other) : this()
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
