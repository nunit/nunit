// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Interfaces
{
    /// <summary>
    /// Defines a scope for grouping multiple assertions, allowing for collective evaluation and reporting of assertion
    /// results.
    /// </summary>
    /// <remarks>An assertion scope enables aggregating assertion failures within a defined context, which can
    /// be useful for reporting multiple failures together rather than stopping at the first failure. Implementations
    /// may provide additional functionality such as customizing failure messages or controlling assertion behavior
    /// within the scope.</remarks>
    public interface IAssertionScope : IDisposable
    {
        /// <summary>
        /// Gets a count of pending failures (from Multiple Assert)
        /// </summary>
        public bool HasFailuresInScope { get; }
    }
}
