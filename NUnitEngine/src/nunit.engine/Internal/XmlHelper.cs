// ***********************************************************************
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

#if CLR_2_0 || CLR_4_0
using System.Collections.Generic;
#else
using System.Collections;
#endif
using System.Xml;

namespace NUnit.Engine.Internal
{
    /// <summary>
    /// XmlHelper provides static methods for basic XML operations
    /// </summary>
    public class XmlHelper
    {
        /// <summary>
        /// Creates a new top level element node.
        /// </summary>
        /// <param name="name">The element name.</param>
        /// <returns></returns>
        public static XmlNode CreateTopLevelElement(string name)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml( "<" + name + "/>" );
            return doc.FirstChild;
        }

        /// <summary>
        /// Adds an attribute with a specified name and value to an existing XmlNode.
        /// </summary>
        /// <param name="node">The node to which the attribute should be added.</param>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        public static void AddAttribute(XmlNode node, string name, string value)
        {
            XmlAttribute attr = node.OwnerDocument.CreateAttribute(name);
            attr.Value = value;
            node.Attributes.Append(attr);
        }

        /// <summary>
        /// Adds a new element as a child of an existing XmlNode and returns it.
        /// </summary>
        /// <param name="node">The node to which the element should be added.</param>
        /// <param name="name">The element name.</param>
        /// <returns>The newly created child element</returns>
        public static XmlNode AddElement(XmlNode node, string name)
        {
            XmlNode childNode = node.OwnerDocument.CreateElement(name);
            node.AppendChild(childNode);
            return childNode;
        }

        /// <summary>
        /// Adds the a new element as a child of an existing node and returns it.
        /// A CDataSection is added to the new element using the data provided.
        /// </summary>
        /// <param name="node">The node to which the element should be added.</param>
        /// <param name="name">The element name.</param>
        /// <param name="data">The data for the CDataSection.</param>
        /// <returns></returns>
        public static XmlNode AddElementWithCDataSection(XmlNode node, string name, string data)
        {
            XmlNode childNode = AddElement(node, name);
            childNode.AppendChild(node.OwnerDocument.CreateCDataSection(data));
            return childNode;
        }

        #region Safe Attribute Access

        public static string GetAttribute(XmlNode result, string name)
        {
            XmlAttribute attr = result.Attributes[name];

            return attr == null ? null : attr.Value;
        }

        public static int GetAttribute(XmlNode result, string name, int defaultValue)
        {
            XmlAttribute attr = result.Attributes[name];

            return attr == null
                ? defaultValue
                : int.Parse(attr.Value, System.Globalization.CultureInfo.InvariantCulture);
        }

        public static double GetAttribute(XmlNode result, string name, double defaultValue)
        {
            XmlAttribute attr = result.Attributes[name];

            return attr == null
                ? defaultValue
                : double.Parse(attr.Value, System.Globalization.CultureInfo.InvariantCulture);
        }

        #endregion

#if CLR_2_0 || CLR_4_0
        public static TestResult CombineResults(IList<TestResult> results)
        {
            List<XmlNode> nodes = new List<XmlNode>();
#else
        public static TestResult CombineResults(IList results)
        {
            ArrayList nodes = new ArrayList();
#endif
            foreach (TestResult result in results)
                nodes.Add(result.GetXml());

            return new TestResult(CombineResultNodes(nodes));
        }

#if CLR_2_0 || CLR_4_0
        public static XmlNode CombineResultNodes(IList<XmlNode> results)
#else
        public static XmlNode CombineResultNodes(IList results)
#endif
        {
            XmlNode combined = CreateTopLevelElement("test-suite");

            string status = "Inconclusive";
            double time = 0.0;
            int total = 0;
            int passed = 0;
            int failed = 0;
            int inconclusive = 0;
            int skipped = 0;
            int asserts = 0;

            foreach (XmlNode result in results)
            {
                switch (GetAttribute(result, "result"))
                {
                    case "Skipped":
                        if (status == "Inconclusive")
                            status = "Skipped";
                        break;
                    case "Passed":
                        if (status != "Failed")
                            status = "Passed";
                        break;
                    case "Failed":
                        status = "Failed";
                        break;
                }

                total += GetAttribute(result, "total", 0);
                time += GetAttribute(result, "time", 0.0);
                passed += GetAttribute(result, "passed", 0);
                failed += GetAttribute(result, "failed", 0);
                inconclusive += GetAttribute(result, "inconclusive", 0);
                skipped += GetAttribute(result, "skipped", 0);
                asserts += GetAttribute(result, "asserts", 0);

                XmlNode import = combined.OwnerDocument.ImportNode(result, true);
                combined.AppendChild(import);
            }

            AddAttribute(combined, "type", "test-package");
            AddAttribute(combined, "id", "1000");
            //AddAttribute(combined, "name", package.Name);
            //AddAttribute(combined, "fullName", package.FullName);
            AddAttribute(combined, "result", status);
            AddAttribute(combined, "time", time.ToString());
            AddAttribute(combined, "total", total.ToString());
            AddAttribute(combined, "passed", passed.ToString());
            AddAttribute(combined, "failed", failed.ToString());
            AddAttribute(combined, "inconclusive", inconclusive.ToString());
            AddAttribute(combined, "skipped", skipped.ToString());
            AddAttribute(combined, "asserts", asserts.ToString());


            return combined;
        }
    }
}
