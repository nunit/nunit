// ***********************************************************************
// Copyright (c) 2014–2018 Charlie Poole, Rob Prouse
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

using NUnit.Framework.Internal; // TODO: We shouldn't reference this in the interface

namespace NUnit.Framework.Interfaces
{
    /// <summary>
    /// The ISimpleTestBuilder interface is exposed by a class that knows how to
    /// build a single tests from a specified method. In general,
    /// it is exposed by an attribute, but it may be implemented in a helper class
    /// used by the attribute in some cases.
    /// </summary>
    public interface ISimpleTestBuilder
    {
        /// <summary>
        /// Builds a single test from the specified method and context.
        /// </summary>
        /// <param name="method">The method to be used as a test.</param>
        /// <param name="suite">The parent to which the test will be added.</param>
        TestMethod BuildFrom(FixtureMethod method, Test suite);
    }
}
