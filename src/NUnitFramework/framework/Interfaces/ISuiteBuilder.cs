// ***********************************************************************
// Copyright (c) 2007–2018 Charlie Poole, Rob Prouse
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
using NUnit.Framework.Internal;

namespace NUnit.Framework.Interfaces
{
    /// <summary>
    /// The ISuiteBuilder interface is exposed by a class that knows how to
    /// build a single test suite from a specified type.
    /// </summary>
    public interface ISuiteBuilder
    {
        /// <summary>
        /// Examine the type and determine if it is suitable for
        /// this builder to use in building a TestSuite.
        ///
        /// Note that returning false will cause the type to be ignored
        /// in loading the tests. If it is desired to load the suite
        /// but label it as non-runnable, ignored, etc., then this
        /// method must return true.
        /// </summary>
        /// <param name="type">The type to be used as a suite.</param>
        bool CanBuildFrom(Type type);

        /// <summary>
        /// Builds a single test suite from the specified type.
        /// </summary>
        /// <param name="type">The type to be used as a suite.</param>
        TestSuite BuildFrom(Type type);

        /// <summary>
        /// Builds a single test suite from the specified type, subject
        /// to a filter that decides which methods are included.
        /// </summary>
        /// <param name="type">The Type to be used as a suite.</param>
        /// <param name="filter">A PreFilter for selecting methods.</param>
        /// <returns></returns>
        TestSuite BuildFrom(Type type, IPreFilter filter);
    }
}
