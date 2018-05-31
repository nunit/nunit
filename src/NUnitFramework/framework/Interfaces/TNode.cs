// ***********************************************************************
// Copyright (c) 2012-2015 Charlie Poole, Rob Prouse
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
using System.Text.RegularExpressions;
using System.Xml;

#if NETSTANDARD1_6
using System.Xml.Linq;
#endif

namespace NUnit.Framework.Interfaces
{
    /// <summary>
    /// TNode represents a single node in the XML representation
    /// of a Test or TestResult. It replaces System.Xml.XmlNode and
    /// System.Xml.Linq.XElement, providing a minimal set of methods
    /// for operating on the XML in a platform-independent manner.
    /// </summary>
    public class TNode
    {
        #region Constructors

        /// <summary>
        /// Constructs a new instance of TNode
        /// </summary>
        /// <param name="name">The name of the node</param>
        public TNode(string name)
        {
            Name = name;
            Attributes = new AttributeDictionary();
            ChildNodes = new NodeList();
        }

        /// <summary>
        /// Constructs a new instance of TNode with a value
        /// </summary>
        /// <param name="name">The name of the node</param>
        /// <param name="value">The text content of the node</param>
        public TNode(string name, string value) : this(name, value, false) { }

        /// <summary>
        /// Constructs a new instance of TNode with a value
        /// </summary>
        /// <param name="name">The name of the node</param>
        /// <param name="value">The text content of the node</param>
        /// <param name="valueIsCDATA">Flag indicating whether to use CDATA when writing the text</param>
        public TNode(string name, string value, bool valueIsCDATA)
            : this(name)
        {
            Value = value;
            ValueIsCDATA = valueIsCDATA;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the node
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the value of the node
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets a flag indicating whether the value should be output using CDATA.
        /// </summary>
        public bool ValueIsCDATA { get; }

        /// <summary>
        /// Gets the dictionary of attributes
        /// </summary>
        public AttributeDictionary Attributes { get; }

        /// <summary>
        /// Gets a list of child nodes
        /// </summary>
        public NodeList ChildNodes { get; }

        /// <summary>
        /// Gets the first ChildNode
        /// </summary>
        public TNode FirstChild
        {
            get { return ChildNodes.Count == 0 ? null : ChildNodes[0]; }
        }

        /// <summary>
        /// Gets the XML representation of this node.
        /// </summary>
        public string OuterXml
        {
            get
            {
                var stringWriter = new System.IO.StringWriter();
                var settings = new XmlWriterSettings();
                settings.ConformanceLevel = ConformanceLevel.Fragment;

                using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, settings))
                {
                    WriteTo(xmlWriter);
                }

                return stringWriter.ToString();
            }
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Create a TNode from its XML text representation
        /// </summary>
        /// <param name="xmlText">The XML text to be parsed</param>
        /// <returns>A TNode</returns>
        public static TNode FromXml(string xmlText)
        {
#if NETSTANDARD1_6
            return FromXml(XElement.Parse(xmlText));
#else
            var doc = new XmlDocument();
            doc.LoadXml(xmlText);
            return FromXml(doc.FirstChild);
#endif
        }

        #endregion

        #region Instance Methods

        /// <summary>
        /// Adds a new element as a child of the current node and returns it.
        /// </summary>
        /// <param name="name">The element name.</param>
        /// <returns>The newly created child element</returns>
        public TNode AddElement(string name)
        {
            TNode childResult = new TNode(name);
            ChildNodes.Add(childResult);
            return childResult;
        }

        /// <summary>
        /// Adds a new element with a value as a child of the current node and returns it.
        /// </summary>
        /// <param name="name">The element name</param>
        /// <param name="value">The text content of the new element</param>
        /// <returns>The newly created child element</returns>
        public TNode AddElement(string name, string value)
        {
            TNode childResult = new TNode(name, EscapeInvalidXmlCharacters(value));
            ChildNodes.Add(childResult);
            return childResult;
        }

        /// <summary>
        /// Adds a new element with a value as a child of the current node and returns it.
        /// The value will be output using a CDATA section.
        /// </summary>
        /// <param name="name">The element name</param>
        /// <param name="value">The text content of the new element</param>
        /// <returns>The newly created child element</returns>
        public TNode AddElementWithCDATA(string name, string value)
        {
            TNode childResult = new TNode(name, EscapeInvalidXmlCharacters(value), true);
            ChildNodes.Add(childResult);
            return childResult;
        }

        /// <summary>
        /// Adds an attribute with a specified name and value to the XmlNode.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        public void AddAttribute(string name, string value)
        {
            Attributes.Add(name, EscapeInvalidXmlCharacters(value));
        }

        /// <summary>
        /// Finds a single descendant of this node matching an XPath
        /// specification. The format of the specification is
        /// limited to what is needed by NUnit and its tests.
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public TNode SelectSingleNode(string xpath)
        {
            NodeList nodes = SelectNodes(xpath);

            return nodes.Count > 0
                ? nodes[0] as TNode
                : null;
        }

        /// <summary>
        /// Finds all descendants of this node matching an XPath
        /// specification. The format of the specification is
        /// limited to what is needed by NUnit and its tests.
        /// </summary>
        public NodeList SelectNodes(string xpath)
        {
            NodeList nodeList = new NodeList();
            nodeList.Add(this);

            return ApplySelection(nodeList, xpath);
        }

        /// <summary>
        /// Writes the XML representation of the node to an XmlWriter
        /// </summary>
        /// <param name="writer"></param>
        public void WriteTo(XmlWriter writer)
        {
            writer.WriteStartElement(Name);

            foreach (string name in Attributes.Keys)
                writer.WriteAttributeString(name, Attributes[name]);

            if (Value != null)
                if (ValueIsCDATA)
                    WriteCDataTo(writer);
                else
                    writer.WriteString(Value);

            foreach (TNode node in ChildNodes)
                node.WriteTo(writer);

            writer.WriteEndElement();
        }

        #endregion

        #region Helper Methods

#if NETSTANDARD1_6
        private static TNode FromXml(XElement xElement)
        {
            TNode tNode = new TNode(xElement.Name.ToString(), xElement.Value);

            foreach (var attr in xElement.Attributes())
                tNode.AddAttribute(attr.Name.ToString(), attr.Value);

            foreach (var child in xElement.Elements())
                tNode.ChildNodes.Add(FromXml(child));

            return tNode;
        }
#else
        private static TNode FromXml(XmlNode xmlNode)
        {
            TNode tNode = new TNode(xmlNode.Name, xmlNode.InnerText);

            foreach (XmlAttribute attr in xmlNode.Attributes)
                tNode.AddAttribute(attr.Name, attr.Value);

            foreach (XmlNode child in xmlNode.ChildNodes)
                if (child.NodeType == XmlNodeType.Element)
                    tNode.ChildNodes.Add(FromXml(child));

            return tNode;
        }
#endif

        private static NodeList ApplySelection(NodeList nodeList, string xpath)
        {
            Guard.ArgumentNotNullOrEmpty(xpath, nameof(xpath));
            if (xpath[0] == '/')
                throw new ArgumentException("XPath expressions starting with '/' are not supported", nameof(xpath));
            if (xpath.IndexOf("//") >= 0)
                throw new ArgumentException("XPath expressions with '//' are not supported", nameof(xpath));

            string head = xpath;
            string tail = null;

            int slash = xpath.IndexOf('/');
            if (slash >= 0)
            {
                head = xpath.Substring(0, slash);
                tail = xpath.Substring(slash + 1);
            }

            NodeList resultNodes = new NodeList();
            NodeFilter filter = new NodeFilter(head);

            foreach(TNode node in nodeList)
                foreach (TNode childNode in node.ChildNodes)
                    if (filter.Pass(childNode))
                        resultNodes.Add(childNode);

            return tail != null
                ? ApplySelection(resultNodes, tail)
                : resultNodes;
        }

        private static readonly Regex InvalidXmlCharactersRegex = new Regex("[^\u0009\u000a\u000d\u0020-\ufffd]|([\ud800-\udbff](?![\udc00-\udfff]))|((?<![\ud800-\udbff])[\udc00-\udfff])", RegexOptions.Compiled);
        private static string EscapeInvalidXmlCharacters(string str)
        {
            if (str == null) return null;

            // Based on the XML spec http://www.w3.org/TR/xml/#charsets
            // For detailed explanation of the regex see http://mnaoumov.wordpress.com/2014/06/15/escaping-invalid-xml-unicode-characters/

            return InvalidXmlCharactersRegex.Replace(str, match => CharToUnicodeSequence(match.Value[0]));
        }

        private static string CharToUnicodeSequence(char symbol)
        {
            return string.Format("\\u{0}", ((int)symbol).ToString("x4"));
        }

        private void WriteCDataTo(XmlWriter writer)
        {
            int start = 0;
            string text = Value;

            while (true)
            {
                int illegal = text.IndexOf("]]>", start);
                if (illegal < 0)
                    break;
                writer.WriteCData(text.Substring(start, illegal - start + 2));
                start = illegal + 2;
                if (start >= text.Length)
                    return;
            }

            if (start > 0)
                writer.WriteCData(text.Substring(start));
            else
                writer.WriteCData(text);
        }

        #endregion

        #region Nested NodeFilter class

        class NodeFilter
        {
            private readonly string _nodeName;
            private readonly string _propName;
            private readonly string _propValue;

            public NodeFilter(string xpath)
            {
                _nodeName = xpath;

                int lbrack = xpath.IndexOf('[');
                if (lbrack >= 0)
                {
                    if (!xpath.EndsWith("]"))
                        throw new ArgumentException("Invalid property expression", nameof(xpath));

                    _nodeName = xpath.Substring(0, lbrack);
                    string filter = xpath.Substring(lbrack+1, xpath.Length - lbrack - 2);

                    int equals = filter.IndexOf('=');
                    if (equals < 0 || filter[0] != '@')
                        throw new ArgumentException("Invalid property expression", nameof(xpath));

                    _propName = filter.Substring(1, equals - 1).Trim();
                    _propValue = filter.Substring(equals + 1).Trim(new char[] { ' ', '"', '\'' });
                }
            }

            public bool Pass(TNode node)
            {
                if (node.Name != _nodeName)
                    return false;

                if (_propName == null)
                    return true;

                return node.Attributes[_propName] == _propValue;
            }
        }

        #endregion
    }

    /// <summary>
    /// Class used to represent a list of XmlResults
    /// </summary>
    public class NodeList : System.Collections.Generic.List<TNode>
    {
    }

    /// <summary>
    /// Class used to represent the attributes of a node
    /// </summary>
    public class AttributeDictionary : System.Collections.Generic.Dictionary<string, string>
    {
        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// Overridden to return null if attribute is not found.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Value of the attribute or null</returns>
        public new string this[string key]
        {
            get
            {
                string value;

                if (TryGetValue(key, out value))
                    return value;

                return null;
            }
        }
    }
}
