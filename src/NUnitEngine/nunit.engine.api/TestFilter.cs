﻿// ***********************************************************************
// Copyright (c) 2011 Charlie Poole
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
using System.Xml;

namespace NUnit.Engine
{
    /// <summary>
    /// Abstract base for all test filters. A filter is represented
    /// by an XmlNode with &lt;filter&gt; as it's topmost element.
    /// In the console runner, filters serve only to carry this
    /// XML representation, as all filtering is done by the engine.
    /// </summary>
#if !PORTABLE
    [Serializable]
#endif
    public class TestFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestFilter"/> class.
        /// </summary>
        /// <param name="xmlText">The XML text that specifies the filter.</param>
        public TestFilter(string xmlText)
        {
            Text = xmlText;
        }

        /// <summary>
        /// The empty filter - one that always passes.
        /// </summary>
        public static readonly TestFilter Empty = new TestFilter("<filter/>");

        /// <summary>
        /// Gets the XML representation of this filter as a string.
        /// </summary>
        public string Text { get; private set; }
    }
}
