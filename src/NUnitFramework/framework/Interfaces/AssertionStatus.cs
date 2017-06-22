// ***********************************************************************
// Copyright (c) 2016 Charlie Poole, Rob Prouse
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

namespace NUnit.Framework.Interfaces
{
    /// <summary>
    /// AssertionStatus enumeration represents the possible outcomes of an assertion.
    /// The order of definition is significant, higher level values override lower
    /// ones in determining the overall result of a test.
    /// </summary>
    public enum AssertionStatus
    {
        /// <summary>
        /// An assumption failed
        /// </summary>
        Inconclusive,

        /// <summary>
        /// The assertion succeeded
        /// </summary>
        Passed,

        /// <summary>
        /// A warning message was issued
        /// </summary>
        Warning,

        /// <summary>
        /// The assertion failed
        /// </summary>
        Failed,

        /// <summary>
        /// An unexpected exception was thrown
        /// </summary>
        Error
    }
}
