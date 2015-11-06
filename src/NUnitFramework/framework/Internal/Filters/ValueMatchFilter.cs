// ***********************************************************************
// Copyright (c) 2013 Charlie Poole
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
using System.Text.RegularExpressions;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Filters
{
    /// <summary>
    /// ValueMatchFilter selects tests based on some value, which
    /// is expected to be contained in the test.
    /// </summary>
    [Serializable]
    public abstract class ValueMatchFilter : TestFilter
    {
        /// <summary>
        /// Returns the value matched by the filter - used for testing
        /// </summary>
        public string ExpectedValue { get; private set; }

        /// <summary>
        /// Indicates whether the value is a regular expression
        /// </summary>
        public bool IsRegex { get; set; }

        /// <summary>
        /// Construct a ValueMatchFilter for a single value.
        /// </summary>
        /// <param name="expectedValue">The value to be included.</param>
        public ValueMatchFilter(string expectedValue)
        {
            ExpectedValue = expectedValue;
        }

        /// <summary>
        /// Match the input provided by the derived class
        /// </summary>
        /// <param name="input">The value to be matchedT</param>
        /// <returns>True for a match, false otherwise.</returns>
        protected bool Match(string input)
        {
            if (IsRegex)
                return input != null && new Regex(ExpectedValue).IsMatch(input);
            else
                return ExpectedValue == input;
        }

        /// <summary>
        /// Adds an XML node
        /// </summary>
        /// <param name="parentNode">Parent node</param>
        /// <param name="recursive">True if recursive</param>
        /// <returns>The added XML node</returns>
        public override TNode AddToXml(TNode parentNode, bool recursive)
        {
            TNode result = parentNode.AddElement(ElementName, ExpectedValue);
            if (IsRegex)
                result.AddAttribute("re", "1");
            return result;
        }

        /// <summary>
        /// Gets the element name
        /// </summary>
        /// <value>Element name</value>
        protected abstract string ElementName { get; }
    }
}
