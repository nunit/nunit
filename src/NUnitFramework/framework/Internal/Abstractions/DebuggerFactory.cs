// ***********************************************************************
// Copyright (c) 2019 Charlie Poole, Rob Prouse
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.Diagnostics;

namespace NUnit.Framework.Internal.Abstractions
{
    /// <summary>
    /// Used to create an <see cref="IDebugger"/> implementation instance based on configuration.
    /// </summary>
    public partial class DebuggerFactory
    {
        /// <summary>
        /// A customizable delegate for creating <see cref="IDebugger"/>.
        /// Creates an instance that delegates directly to <see cref="Debugger"/> when <see langword="null"/> (default).
        /// </summary>
        /// <remarks>
        /// This property should only be accessed in tests to emulate debugger attached to the process.
        /// </remarks>
        public static Func<IDebugger> ImplementationFactory { get; set; }

        /// <summary>
        /// Creates an <see cref="IDebugger"/> implementation instance.
        /// </summary>
        /// <returns>An <see cref="IDebugger"/> implementation instace.</returns>
        public IDebugger Create()
        {
            return ImplementationFactory?.Invoke()
                ?? new DebuggerProxy();
        }
    }
}
