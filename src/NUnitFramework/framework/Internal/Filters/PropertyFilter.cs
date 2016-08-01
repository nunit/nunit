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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Filters
{
    /// <summary>
    /// PropertyFilter is able to select or exclude tests
    /// based on their properties.
    /// </summary>
    /// 
    [Serializable]
    public class PropertyFilter : ValueMatchFilter
    {
        private string _propertyName;

        /// <summary>
        /// Construct a PropertyFilter using a property name and expected value
        /// </summary>
        /// <param name="propertyName">A property name</param>
        /// <param name="expectedValue">The expected value of the property</param>
        public PropertyFilter(string propertyName, string expectedValue) : base(expectedValue) 
        {
            _propertyName = propertyName;
        }

        /// <summary>
        /// Check whether the filter matches a test
        /// </summary>
        /// <param name="test">The test to be matched</param>
        /// <returns></returns>
        public override bool Match(ITest test)
        {
            IList values = test.Properties[_propertyName];

            if (values != null)
                foreach (string val in values)
                    if (Match(val))
                        return true;

            return false;
        }

        /// <summary>
        /// Adds an XML node
        /// </summary>
        /// <param name="parentNode">Parent node</param>
        /// <param name="recursive">True if recursive</param>
        /// <returns>The added XML node</returns>
        public override TNode AddToXml(TNode parentNode, bool recursive)
        {
            TNode result = base.AddToXml(parentNode, recursive);
            result.AddAttribute("name", _propertyName);
            return result;
        }

        /// <summary>
        /// Gets the element name
        /// </summary>
        /// <value>Element name</value>
        protected override string ElementName
        {
            get { return "prop"; }
        }
    }
}
