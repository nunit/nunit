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
        public NotFilter( TestFilter baseFilter)
        {
            BaseFilter = baseFilter;
        }

        /// <summary>
        /// Gets the base filter
        /// </summary>
        public TestFilter BaseFilter { get; private set; }

        /// <summary>
        /// Determine if a particular test passes the filter criteria. The default 
        /// implementation checks the test itself, its parents and any descendants.
        /// 
        /// Derived classes may override this method or any of the Match methods
        /// to change the behavior of the filter.
        /// </summary>
        /// <param name="test">The test to which the filter is applied</param>
        /// <returns>True if the test passes the filter, otherwise false</returns>
        public override bool Pass(ITest test)
        {
            return !BaseFilter.Match (test) && !BaseFilter.MatchParent (test);
        }

        /// <summary>
        /// Check whether the filter matches a test
        /// </summary>
        /// <param name="test">The test to be matched</param>
        /// <returns>True if it matches, otherwise false</returns>
        public override bool Match( ITest test )
        {
            return !BaseFilter.Match( test );
        }

        /// <summary>
        /// Determine if a test matches the filter expicitly. That is, it must
        /// be a direct match of the test itself or one of it's children.
        /// </summary>
        /// <param name="test">The test to which the filter is applied</param>
        /// <returns>True if the test matches the filter explicityly, otherwise false</returns>
        public override bool IsExplicitMatch(ITest test)
        {
            return false;
        }

        ///// <summary>
        ///// Determine whether any descendant of the test matches the filter criteria.
        ///// </summary>
        ///// <param name="test">The test to be matched</param>
        ///// <returns>True if at least one descendant matches the filter criteria</returns>
        //protected override bool MatchDescendant(ITest test)
        //{
        //    if (!test.HasChildren || test.Tests == null || TopLevel && test.RunState == RunState.Explicit)
        //        return false;

        //    foreach (ITest child in test.Tests)
        //    {
        //        if (Match(child) || MatchDescendant(child))
        //            return true;
        //    }

        //    return false;
        //}	

        /// <summary>
        /// Adds an XML node
        /// </summary>
        /// <param name="parentNode">Parent node</param>
        /// <param name="recursive">True if recursive</param>
        /// <returns>The added XML node</returns>
        public override TNode AddToXml(TNode parentNode, bool recursive)
        {
            TNode result = parentNode.AddElement("not");
            if (recursive)
                BaseFilter.AddToXml(result, true);
            return result;
        }
    }
}
