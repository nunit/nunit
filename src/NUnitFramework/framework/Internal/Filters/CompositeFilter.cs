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
    public abstract class CompositeFilter : TestFilter
    {
        /// <summary>
        /// List of component filters
        /// </summary>
        protected List<ITestFilter> _filters = new List<ITestFilter>();

        /// <summary>
        /// Constructs an empty CompoundFilter
        /// </summary>
        public CompositeFilter() { }

        /// <summary>
        /// Constructs a CompoundFilter from an array of filters
        /// </summary>
        /// <param name="filters"></param>
        public CompositeFilter( params ITestFilter[] filters )
        {
            _filters.AddRange( filters );
        }

        /// <summary>
        /// Adds a filter to the list of filters
        /// </summary>
        /// <param name="filter">The filter to be added</param>
        public void Add(ITestFilter filter)
        {
            _filters.Add(filter);
        }

        /// <summary>
        /// Return an array of the composing filters
        /// </summary>
        public ITestFilter[] Filters
        {
            get { return _filters.ToArray(); }
        }
    }
}
