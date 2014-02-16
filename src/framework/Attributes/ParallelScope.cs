// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
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

namespace NUnit.Framework
{
    /// <summary>
    /// The ParallelScope enumeration permits specifying the degree to
    /// which a test and its descendants may be run in parallel.
    /// </summary>
    [Flags]
    public enum ParallelScope
    {
        /// <summary>
        /// No Parallelism is permitted
        /// </summary>
        None = 0,
        /// <summary>
        /// The test itself may be run in parallel with others at the same level
        /// </summary>
        Self = 1,
        /// <summary>
        /// Descendants of the test may be run in parallel with one another
        /// </summary>
        Children = 2,
        /// <summary>
        /// Descendants of the test down to the level of TestFixtures may be run in parallel
        /// </summary>
        Fixtures = 4
    }
}
