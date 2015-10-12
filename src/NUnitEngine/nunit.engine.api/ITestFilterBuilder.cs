// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
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

namespace NUnit.Engine
{
    /// <summary>
    /// Interface to a TestFilterBuilder, which is used to create TestFilters
    /// </summary>
    public interface ITestFilterBuilder
    {
        /// <summary>
        /// Add a test to be selected
        /// </summary>
        /// <param name="fullName">The full name of the test, as created by NUnit</param>
        void AddTest(string fullName);

        /// <summary>
        /// Specify what is to be included by the filter using a where clause.
        /// </summary>
        /// <param name="whereClause">A where clause that will be parsed by NUnit to create the filter.</param>
        void SelectWhere(string whereClause);

        /// <summary>
        /// Get a TestFilter constructed according to the criteria specified by the other calls.
        /// </summary>
        /// <returns>A TestFilter.</returns>
        TestFilter GetFilter();
    }
}
