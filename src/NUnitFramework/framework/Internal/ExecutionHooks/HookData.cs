// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.ExecutionHooks
{
    /// <summary>
    /// Holds context information for called execution hooks.
    /// </summary>
    public class HookData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HookData"/> class.
        /// </summary>
        /// <param name="context">The test execution context.</param>
        /// <param name="hookedMethod">The hooked method information.</param>
        /// <param name="exception">The exception that was thrown during the hooked method execution, if any.</param>
        public HookData(TestExecutionContext context, IMethodInfo hookedMethod, Exception? exception = null)
        {
            Context = new TestContext(context);
            HookedMethod = hookedMethod;
            Exception = exception;
        }

        /// <summary>
        /// Gets the test execution context.
        /// </summary>
        public TestContext Context { get; }

        /// <summary>
        /// Gets the method information of the hooked method.
        /// </summary>
        public IMethodInfo HookedMethod { get; }

        /// <summary>
        /// Gets the exception that was thrown during the method execution, if any.
        /// </summary>
        public Exception? Exception { get; }
    }
}
