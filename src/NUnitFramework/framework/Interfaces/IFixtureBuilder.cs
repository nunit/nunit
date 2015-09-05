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
using System.Collections.Generic;

namespace NUnit.Framework.Interfaces
{
    using Internal;

    /// <summary>
    /// The IFixtureBuilder interface is exposed by a class that knows how to
    /// build a TestFixture from one or more Types. In general, it is exposed
    /// by an attribute, but may be implemented in a helper class used by the
    /// attribute in some cases.
    /// </summary>
    public interface IFixtureBuilder
    {
        /// <summary>
        /// Build one or more TestFixtures from type provided. At least one
        /// non-null TestSuite must always be returned, since the method is 
        /// generally called because the user has marked the target class as 
        /// a fixture. If something prevents the fixture from being used, it
        /// will be returned nonetheless, labelled as non-runnable.
        /// </summary>
        /// <param name="typeInfo">The type info of the fixture to be used.</param>
        /// <returns>A TestSuite object or one derived from TestSuite.</returns>
        // TODO: This should really return a TestFixture, but that requires changes to the Test hierarchy.
        IEnumerable<TestSuite> BuildFrom(ITypeInfo typeInfo);
    }
}
