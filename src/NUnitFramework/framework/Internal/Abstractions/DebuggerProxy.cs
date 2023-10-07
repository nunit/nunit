// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Diagnostics;

namespace NUnit.Framework.Internal.Abstractions
{
    /// <summary>
    /// A production <see cref="IDebugger"/> implementation that delegates directly to .NET's <see cref="Debugger"/>.
    /// </summary>
    internal sealed class DebuggerProxy : IDebugger
    {
        /// <summary>
        /// Returns whether a debugger is currently attached to the process
        /// </summary>
        public bool IsAttached => Debugger.IsAttached;
    }
}
