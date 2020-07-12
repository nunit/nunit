// ***********************************************************************
// Copyright (c) 2020 Charlie Poole
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

namespace NUnit.Framework
{
    /// <summary>
    /// Represents reference to an argument
    /// from the test fixture in form of 0-based
    /// index. Used for forwarding values
    /// to the test case sources.
    ///
    /// If your fixture contains more than 10 arguments
    /// you can always cast any higher index
    /// to <see cref="TestFixtureArgumentRef"/>
    /// </summary>
    public enum TestFixtureArgumentRef
    {
        /// <summary>
        /// Argument at index 0
        /// </summary>
        Arg0 = 0,
        /// <summary>
        /// Argument at index 1
        /// </summary>
        Arg1 = 1,
        /// <summary>
        /// Argument at index 2
        /// </summary>
        Arg2 = 2,
        /// <summary>
        /// Argument at index 3
        /// </summary>
        Arg3 = 3,
        /// <summary>
        /// Argument at index 4
        /// </summary>
        Arg4 = 4,
        /// <summary>
        /// Argument at index 5
        /// </summary>
        Arg5 = 5,
        /// <summary>
        /// Argument at index 6
        /// </summary>
        Arg6 = 6,
        /// <summary>
        /// Argument at index 7
        /// </summary>
        Arg7 = 7,
        /// <summary>
        /// Argument at index 8
        /// </summary>
        Arg8 = 8,
        /// <summary>
        /// Argument at index 9
        /// </summary>
        Arg9 = 9
    }
}
