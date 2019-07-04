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
using System.Collections;

namespace NUnit.Framework.Internal
{
    public partial class UnexpectedExceptionTests
    {
        /// <summary>
        /// This models a .NET Framework race condition that resulted in stack traces throwing AccessViolationException
        /// which would produce another AccessViolationException while attempting to print its stack trace in turn.
        /// </summary>
        private sealed class RecursivelyThrowingException : Exception
        {
            public override string Message => throw this;

            public override IDictionary Data => throw this;

            public override string StackTrace => throw this;

            public override string HelpLink
            {
                get => throw this;
                set => throw this;
            }

            public override string Source
            {
                get => throw this;
                set => throw this;
            }

            public override bool Equals(object obj) => throw this;

            public override Exception GetBaseException() => throw this;

            public override int GetHashCode() => throw this;

            public override string ToString() => throw this;
        }
    }
}
