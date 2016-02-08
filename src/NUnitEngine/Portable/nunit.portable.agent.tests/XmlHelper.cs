// ***********************************************************************
// Copyright (c) 2010-2014 Charlie Poole
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
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace NUnit.Engine.Tests
{
    /// <summary>
    /// XmlHelper provides static methods for basic XML operations.
    /// </summary>
    public static class XmlHelper
    {
        /// <summary>
        /// Loads the given XML string into an XDocument and returns the top level element
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static XElement CreateXElement(string xml)
        {
            XDocument doc = XDocument.Parse(xml);
            return doc.Elements().FirstOrDefault();
        }

        #region Safe Attribute Access

        /// <summary>
        /// Gets the value of the given attribute.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static string GetAttribute(this XNode result, string name)
        {
            XAttribute attr = null;

            var element = result as XElement;
            if (element != null)
            {
                attr = element.Attribute(name);
            }

            return attr == null ? null : attr.Value;
        }

        #endregion
    }
}
