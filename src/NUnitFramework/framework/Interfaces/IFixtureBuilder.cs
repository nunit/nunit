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

using System;
using System.Collections.Generic;

namespace NUnit.Framework.Interfaces
{
    using Internal;

    // TODO: These methods should really return IEnumerable<TestFixture>, 
    // but that requires changes to the Test hierarchy.

    /// <summary>
    /// The IFixtureBuilder interface is exposed by a class that knows how to
    /// build test fixtures from a specified type. In general, it is exposed
    /// by an attribute, but it may be implemented in a helper class used by the
    /// attribute in some cases.
    /// </summary>
    public interface IFixtureBuilder
    {
        /// <summary>
        /// Builds any number of test fixtures from the specified type.
        /// </summary>
        /// <param name="type">The type to be used as a fixture.</param>
        IEnumerable<TestSuite> BuildFrom(Type type);
    }

    /// <summary>
    /// The IFixtureBuilder2 interface extends IFixtureBuilder by allowing
    /// use of a PreFilter, which is used to select methods as test cases.
    /// </summary>
    public interface IFixtureBuilder2 : IFixtureBuilder
    {
        /// <summary>
        /// Builds any number of test fixtures from the specified type.
        /// </summary>
        /// <param name="type">The type to be used as a fixture.</param>
        /// <param name="filter">PreFilter to be used to select methods.</param>
        IEnumerable<TestSuite> BuildFrom(Type type, PreFilter filter);
    }
}
