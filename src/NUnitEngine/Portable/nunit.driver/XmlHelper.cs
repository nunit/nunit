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

namespace NUnit.Engine
{

    /// <summary>
    /// XmlHelper provides static methods for basic XML operations.
    /// </summary>
    public static class XmlHelper
    {
        /// <summary>
        /// Creates a new top level element node.
        /// </summary>
        /// <param name="name">The element name.</param>
        /// <returns>A new XNode</returns>
        public static XNode CreateTopLevelElement(string name)
        {
            XDocument doc = new XDocument(new XElement(name));
            return doc.FirstNode;
        }

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

        /// <summary>
        /// Adds an attribute with a specified name and value to an existing XNode.
        /// </summary>
        /// <param name="node">The node to which the attribute should be added.</param>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        public static void AddAttribute(this XNode node, string name, string value)
        {
            var element = node as XElement;
            if (element != null)
            {
                element.SetAttributeValue(name, value);
            }
        }

        /// <summary>
        /// Adds a new element as a child of an existing XNode and returns it.
        /// </summary>
        /// <param name="node">The node to which the element should be added.</param>
        /// <param name="name">The element name.</param>
        /// <returns>The newly created child element</returns>
        public static XNode AddElement(this XNode node, string name)
        {
            XElement child = null;
            var element = node as XElement;
            if (element != null)
            {
                child = new XElement(name);
                element.Add(child);
            }
            return child;
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

        /// <summary>
        /// Gets the value of the given attribute as an int.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static int GetAttribute(this XNode result, string name, int defaultValue)
        {
            string attr = result.GetAttribute(name);

            return attr == null
                ? defaultValue
                : int.Parse(attr, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets the value of the given attribute as a double.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static double GetAttribute(this XNode result, string name, double defaultValue)
        {
            string attr = result.GetAttribute(name);

            return attr == null
                ? defaultValue
                : double.Parse(attr, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets the value of the given attribute as a DateTime.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static DateTime GetAttribute(this XNode result, string name, DateTime defaultValue)
        {
            string dateStr = GetAttribute(result, name);
            if (dateStr == null)
                return defaultValue;

            DateTime date;
            if (!DateTime.TryParse(dateStr, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AllowWhiteSpaces, out date))
                return defaultValue;

            return date;
        }

        #endregion
    }
}
