// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal.HookExtensions
{
    /// <summary>
    /// Hook Extension interface to run custom code synchronously before or after any test activity.
    /// </summary>
    public class HookExtension
    {
        /// <summary>
        /// Default ctor of <see cref="HookExtension"/> class.
        /// </summary>
        public HookExtension()
        {
            BeforeTestHook = new TestHook();
            AfterTestHook = new TestHook();
        }

        /// <summary>
        /// Gets or sets the hook event that is triggered before a test method is executed.
        /// </summary>
        public TestHook BeforeTestHook { get; set; }

        /// <summary>
        /// Gets or sets the hook event that is triggered after a test method is executed.
        /// </summary>
        public TestHook AfterTestHook { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HookExtension"/> class by copying hooks from another instance.
        /// </summary>
        /// <param name="other">The instance of <see cref="HookExtension"/> to copy hooks from.</param>
        public HookExtension(HookExtension other) : this()
        {
        }

        internal void OnBeforeTest(TestExecutionContext context)
        {
            BeforeTestHook.InvokeHandlers(this, new MethodHookEventArgs(context));
        }

        internal void OnAfterTest(TestExecutionContext context)
        {
            AfterTestHook.InvokeHandlers(this, new MethodHookEventArgs(context));
        }
    }
}
