// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Internal.HookExtensions
{
    internal class MethodHookEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MethodHookEventArgs"/> class.
        /// </summary>
        /// <param name="context">The test execution context.</param>
        public MethodHookEventArgs(TestExecutionContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Gets the test execution context.
        /// </summary>
        public TestExecutionContext Context { get; }
    }
}
