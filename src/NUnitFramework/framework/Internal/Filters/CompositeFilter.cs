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

using System;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Filters
{
    /// <summary>
    /// A base class for multi-part filters
    /// </summary>
    public abstract class CompositeFilter : TestFilter
    {
        /// <summary>
        /// Constructs an empty CompositeFilter
        /// </summary>
        public CompositeFilter()
        {
            Filters = new List<ITestFilter>();
        }

        /// <summary>
        /// Constructs a CompositeFilter from an array of filters
        /// </summary>
        /// <param name="filters"></param>
        public CompositeFilter( params ITestFilter[] filters )
        {
            Filters = new List<ITestFilter>(filters);
        }

        /// <summary>
        /// Adds a filter to the list of filters
        /// </summary>
        /// <param name="filter">The filter to be added</param>
        public void Add(ITestFilter filter)
        {
            Filters.Add(filter);
        }

        /// <summary>
        /// Return a list of the composing filters.
        /// </summary>
        public IList<ITestFilter> Filters { get; private set; }

        /// <summary>
        /// Checks whether the CompositeFilter is matched by a test.
        /// </summary>
        /// <param name="test">The test to be matched</param>
        public abstract override bool Pass(ITest test);

        /// <summary>
        /// Checks whether the CompositeFilter is matched by a test.
        /// </summary>
        /// <param name="test">The test to be matched</param>
        public abstract override bool Match(ITest test);

        /// <summary>
        /// Checks whether the CompositeFilter is explicit matched by a test.
        /// </summary>
        /// <param name="test">The test to be matched</param>
        public abstract override bool IsExplicitMatch(ITest test);

        /// <summary>
        /// Adds an XML node
        /// </summary>
        /// <param name="parentNode">Parent node</param>
        /// <param name="recursive">True if recursive</param>
        /// <returns>The added XML node</returns>
        public override TNode AddToXml(TNode parentNode, bool recursive)
        {
            TNode result = parentNode.AddElement(ElementName);

            if (recursive)
                foreach (ITestFilter filter in Filters)
                    filter.AddToXml(result, true);

            return result;
        }

        /// <summary>
        /// Gets the element name
        /// </summary>
        /// <value>Element name</value>
        protected abstract string ElementName { get; }
    }
}
