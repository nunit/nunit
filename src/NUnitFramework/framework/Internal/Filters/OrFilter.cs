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
using System.Collections.Generic;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Filters
{
    /// <summary>
    /// Combines multiple filters so that a test must pass one 
    /// of them in order to pass this filter.
    /// </summary>
    [Serializable]
    public class OrFilter : CompositeFilter
    {
        /// <summary>
        /// Constructs an empty OrFilter
        /// </summary>
        public OrFilter() { }

        /// <summary>
        /// Constructs an AndFilter from an array of filters
        /// </summary>
        /// <param name="filters"></param>
        public OrFilter( params ITestFilter[] filters ) : base(filters) { }

        /// <summary>
        /// Checks whether the OrFilter is matched by a test
        /// </summary>
        /// <param name="test">The test to be matched</param>
        /// <returns>True if any of the component filters pass, otherwise false</returns>
        public override bool Pass( ITest test )
        {
            foreach( ITestFilter filter in Filters )
                if ( filter.Pass( test ) )
                    return true;

            return false;
        }

        /// <summary>
        /// Checks whether the OrFilter is matched by a test
        /// </summary>
        /// <param name="test">The test to be matched</param>
        /// <returns>True if any of the component filters match, otherwise false</returns>
        public override bool Match( ITest test )
        {
            foreach( TestFilter filter in Filters )
                if ( filter.Match( test ) )
                    return true;

            return false;
        }

        /// <summary>
        /// Checks whether the OrFilter is explicit matched by a test
        /// </summary>
        /// <param name="test">The test to be matched</param>
        /// <returns>True if any of the component filters explicit match, otherwise false</returns>
        public override bool IsExplicitMatch( ITest test )
        {
            foreach( TestFilter filter in Filters )
                if ( filter.IsExplicitMatch( test ) )
                    return true;

            return false;
        }

        /// <summary>
        /// Gets the element name
        /// </summary>
        /// <value>Element name</value>
        protected override string ElementName
        {
            get { return "or"; }
        }
    }
}
