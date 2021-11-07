// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal.Abstractions
{
    /// <summary>
    /// A layer of abstraction around <see cref="System.Diagnostics.Debugger"/> to facilitate testing.
    /// </summary>
    internal interface IDebugger
    {
        /// <summary>
        /// Whether a debugger is currently attached to the process.
        /// </summary>
        bool IsAttached { get; }
    }
}
