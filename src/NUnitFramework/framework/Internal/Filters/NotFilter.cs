// ***********************************************************************
// Copyright (c) 2007-2015 Charlie Poole
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
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Filters
{
    /// <summary>
    /// NotFilter negates the operation of another filter
    /// </summary>
    [Serializable]
    public class NotFilter : TestFilter
    {
        /// <summary>
        /// Construct a not filter on another filter
        /// </summary>
        /// <param name="baseFilter">The filter to be negated</param>
        public NotFilter( ITestFilter baseFilter)
        {
            BaseFilter = baseFilter;
        }

        /// <summary>
        /// Gets the base filter
        /// </summary>
        public ITestFilter BaseFilter { get; private set; }

        /// <summary>
        /// Check whether the filter matches a test
        /// </summary>
        /// <param name="test">The test to be matched</param>
        /// <returns>True if it matches, otherwise false</returns>
        public override bool Match( ITest test )
        {
            if (TopLevel && test.RunState == RunState.Explicit)
                return false;

            return !BaseFilter.Pass( test );
        }

        /// <summary>
        /// Determine whether any descendant of the test matches the filter criteria.
        /// </summary>
        /// <param name="test">The test to be matched</param>
        /// <returns>True if at least one descendant matches the filter criteria</returns>
        protected override bool MatchDescendant(ITest test)
        {
            if (!test.HasChildren || test.Tests == null || TopLevel && test.RunState == RunState.Explicit)
                return false;

            foreach (ITest child in test.Tests)
            {
                if (Match(child) || MatchDescendant(child))
                    return true;
            }

            return false;
        }	
    }
}
