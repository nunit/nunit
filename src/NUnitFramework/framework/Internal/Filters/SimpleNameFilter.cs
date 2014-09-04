// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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
    /// SimpleName filter selects tests based on their name
    /// </summary>
    [Serializable]
    public class SimpleNameFilter : ValueMatchFilter<string>
    {
        /// <summary>
        /// Construct an empty SimpleNameFilter
        /// </summary>
        public SimpleNameFilter() { }

        /// <summary>
        /// Construct a SimpleNameFilter for a single name
        /// </summary>
        /// <param name="nameToAdd">The name the filter will recognize.</param>
        public SimpleNameFilter(string nameToAdd) : base (nameToAdd) { }

        /// <summary>
        /// Construct a SimpleNameFilter for an array of ids
        /// </summary>
        /// <param name="namesToAdd">The ids the filter will recognize.</param>
        public SimpleNameFilter(IEnumerable<string> namesToAdd) : base(namesToAdd) { }

        /// <summary>
        /// Match a test against a single value.
        /// </summary>
        protected override bool Match(ITest test, string value)
        {
            return test.FullName == value;
        }
    }
}
