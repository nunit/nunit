// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;

namespace NUnit.Framework.Interfaces
{
    /// <summary>
    /// TNode represents a single node in the XML representation
    /// of a Test or TestResult. It replaces System.Xml.XmlNode and
    /// System.Xml.Linq.XElement, providing a minimal set of methods
    /// for operating on the XML in a platform-independent manner.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    // Disregarding naming convention for back-compat
    [DebuggerDisplay("{OuterXml}")]
    public sealed class TNode
    {
        private List<TNode>? _childNodes;
        private Dictionary<string, string>? _attributes;

        #region Constructors

        /// <summary>
        /// Constructs a new instance of TNode
        /// </summary>
        /// <param name="name">The name of the node</param>
        public TNode(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Constructs a new instance of TNode with a value
        /// </summary>
        /// <param name="name">The name of the node</param>
        /// <param name="value">The text content of the node</param>
        public TNode(string name, string? value) : this(name, value, false)
        {
        }

        /// <summary>
        /// Constructs a new instance of TNode with a value
        /// </summary>
        /// <param name="name">The name of the node</param>
        /// <param name="value">The text content of the node</param>
        /// <param name="valueIsCDATA">Flag indicating whether to use CDATA when writing the text</param>
        public TNode(string name, string? value, bool valueIsCDATA)
            : this(name)
        {
            Value = XmlExtensions.EscapeInvalidXmlCharacters(value);
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
        public string? Value { get; set; }

        /// <summary>
        /// Gets a flag indicating whether the value should be output using CDATA.
        /// </summary>
        public bool ValueIsCDATA { get; set; }

        /// <summary>
        /// Gets the dictionary of attributes
        /// </summary>
        public AttributeDictionary Attributes => new(this);

        /// <summary>
        /// Gets a list of child nodes
        /// </summary>
        public NodeList ChildNodes => new(this);

        /// <summary>
        /// Gets the first ChildNode
        /// </summary>
        public TNode? FirstChild => _childNodes?.Count == 0 ? null : ChildNodes[0];

        /// <summary>
        /// Gets the XML representation of this node.
        /// </summary>
        public string OuterXml
        {
            get
            {
                using var stringWriter = new StringWriter();
                using (var xmlWriter = XmlWriter.Create(stringWriter, XmlExtensions.FragmentWriterSettings))
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
            using var stringReader = new StringReader(xmlText);
            using var reader = XmlReader.Create(stringReader);

            // go to starting point
            reader.MoveToContent();

            TNode? root = null;
            TNode? current = null;
            var parents = new Stack<TNode>();

            while (!reader.EOF)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    // keep track of previous which will be promoted to parent stack if reader depth changes
                    var previous = current;
                    current = new TNode(reader.Name, reader.Value);

                    if (root is null)
                    {
                        // initialize root
                        root = current;
                        parents.Push(root);
                    }
                    else
                    {
                        if (reader.Depth > parents.Count)
                        {
                            parents.Push(previous!);
                        }

                        if (reader.Depth < parents.Count)
                        {
                            parents.Pop();
                        }

                        var parent = parents.Peek();
                        parent.AddChildNode(current);
                    }

                    var attributeCount = reader.AttributeCount;
                    if (attributeCount > 0)
                    {
                        for (var i = 0; i < attributeCount; i++)
                        {
                            reader.MoveToNextAttribute();
                            current.AddAttribute(reader.Name, reader.Value);
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (reader.Depth < parents.Count)
                    {
                        parents.Pop();
                    }
                }
                else if (reader.NodeType == XmlNodeType.Text)
                {
                    current!.Value = reader.Value;
                }
                else if (reader.NodeType == XmlNodeType.CDATA)
                {
                    current!.Value = reader.Value;
                    current.ValueIsCDATA = true;
                }

                reader.Read();
            }

            if (root is null)
            {
                throw new ArgumentException("Could not extract root element from " + xmlText);
            }

            return root;
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
            AddChildNode(childResult);
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
            TNode childResult = new TNode(name, value);
            AddChildNode(childResult);
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
            TNode childResult = new TNode(name, value, true);
            AddChildNode(childResult);
            return childResult;
        }

        /// <summary>
        /// Adds a child node to this node.
        /// </summary>
        /// <param name="node">The child node to add.</param>
        public void AddChildNode(TNode node)
        {
            _childNodes ??= new List<TNode>();
            _childNodes.Add(node);
        }

        /// <summary>
        /// Inserts a child nodeat the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="node" /> should be inserted.</param>
        /// <param name="node">The node to insert.</param>
        public void InsertChildNode(int index, TNode node)
        {
            _childNodes ??= new List<TNode>();
            _childNodes.Insert(index, node);
        }

        /// <summary>
        /// Adds an attribute with a specified name and value to the XmlNode.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        public void AddAttribute(string name, string value)
        {
            _attributes ??= new Dictionary<string, string>();
            _attributes.Add(name, XmlExtensions.EscapeInvalidXmlCharacters(value));
        }

        /// <summary>
        /// Finds a single descendant of this node matching an XPath
        /// specification. The format of the specification is
        /// limited to what is needed by NUnit and its tests.
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public TNode? SelectSingleNode(string xpath)
        {
            List<TNode> nodes = SelectNodes(xpath);

            return nodes.Count > 0
                ? nodes[0] as TNode
                : null;
        }

        /// <summary>
        /// Finds all descendants of this node matching an XPath
        /// specification. The format of the specification is
        /// limited to what is needed by NUnit and its tests.
        /// </summary>
        public List<TNode> SelectNodes(string xpath)
        {
            var nodeList = new List<TNode>();
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

            foreach (var pair in Attributes)
                writer.WriteAttributeString(pair.Key, pair.Value);

            if (Value is not null)
            {
                if (ValueIsCDATA)
                    writer.WriteCDataSafe(Value);
                else
                    writer.WriteString(Value);
            }

            var count = ChildNodes.Count;
            for (var i = 0; i < count; i++)
            {
                ChildNodes[i].WriteTo(writer);
            }

            writer.WriteEndElement();
        }

        #endregion

        #region Helper Methods

        private static TNode FromXml(XmlNode xmlNode)
        {
            TNode tNode = new TNode(xmlNode.Name, xmlNode.InnerText);

            if (xmlNode.Attributes is not null)
            {
                foreach (XmlAttribute attr in xmlNode.Attributes)
                    tNode.AddAttribute(attr.Name, attr.Value);
            }

            foreach (XmlNode child in xmlNode.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Element)
                    tNode.AddChildNode(FromXml(child));
            }

            return tNode;
        }

        private static List<TNode> ApplySelection(List<TNode> nodeList, string xpath)
        {
            Guard.ArgumentNotNullOrEmpty(xpath, nameof(xpath));
            if (xpath[0] == '/')
                throw new ArgumentException("XPath expressions starting with '/' are not supported", nameof(xpath));
            if (xpath.IndexOf("//", StringComparison.Ordinal) >= 0)
                throw new ArgumentException("XPath expressions with '//' are not supported", nameof(xpath));

            string head = xpath;
            string? tail = null;

            int slash = xpath.IndexOf('/');
            if (slash >= 0)
            {
                head = xpath.Substring(0, slash);
                tail = xpath.Substring(slash + 1);
            }

            List<TNode> resultNodes = new();
            NodeFilter filter = new NodeFilter(head);

            var nodeListCount = nodeList.Count;
            for (var i = 0; i < nodeListCount; i++)
            {
                var node = nodeList[i];
                var childNodesCount = node.ChildNodes.Count;
                for (var j = 0; j < childNodesCount; j++)
                {
                    var childNode = node.ChildNodes[j];
                    if (filter.Pass(childNode))
                        resultNodes.Add(childNode);
                }
            }

            return tail is not null
                ? ApplySelection(resultNodes, tail)
                : resultNodes;
        }

        #endregion

        #region Nested NodeFilter class

        private sealed class NodeFilter
        {
            private readonly string _nodeName;
            private readonly string? _propName;
            private readonly string? _propValue;

            private static readonly char[] TrimChars = { ' ', '"', '\'' };

            public NodeFilter(string xpath)
            {
                _nodeName = xpath;

                int lbrack = xpath.IndexOf('[');
                if (lbrack >= 0)
                {
                    if (!xpath.EndsWith("]", StringComparison.Ordinal))
                        throw new ArgumentException("Invalid property expression", nameof(xpath));

                    _nodeName = xpath.Substring(0, lbrack);
                    string filter = xpath.Substring(lbrack + 1, xpath.Length - lbrack - 2);

                    int equals = filter.IndexOf('=');
                    if (equals < 0 || filter[0] != '@')
                        throw new ArgumentException("Invalid property expression", nameof(xpath));

                    _propName = filter.Substring(1, equals - 1).Trim();
                    _propValue = filter.Substring(equals + 1).Trim(TrimChars);
                }
            }

            public bool Pass(TNode node)
            {
                if (node.Name != _nodeName)
                    return false;

                if (_propName is null)
                    return true;

                return node.Attributes[_propName] == _propValue;
            }
        }

        #endregion

        /// <summary>
        /// Class used to represent a list of XmlResults
        /// </summary>
        public readonly struct NodeList : IEnumerable<TNode>
        {
            private static readonly List<TNode> EmptyList = new();

            private readonly TNode _parent;

            internal NodeList(TNode parent)
            {
                _parent = parent;
            }

            /// <summary>
            /// Gets or sets the element at the specified index.
            /// </summary>
            public TNode this[int index]
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    if (_parent._childNodes is null || (uint)index >= (uint)_parent._childNodes.Count)
                        ThrowArgumentOutOfRangeException(index);

                    return _parent._childNodes![index];
                }
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            private static void ThrowArgumentOutOfRangeException(int index)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, "Index was out or range of valid values");
            }

            /// <summary>
            /// Gets the number of elements contained in the collection.
            /// </summary>
            public int Count
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _parent._childNodes?.Count ?? 0;
            }

            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public List<TNode>.Enumerator GetEnumerator() => _parent._childNodes?.GetEnumerator() ?? EmptyList.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            IEnumerator<TNode> IEnumerable<TNode>.GetEnumerator() => GetEnumerator();
        }

        /// <summary>
        /// Class used to represent the attributes of a node
        /// </summary>
        public readonly struct AttributeDictionary
        {
            private static readonly Dictionary<string, string> EmptyDictionary = new();

            private readonly TNode _parent;

            internal AttributeDictionary(TNode parent)
            {
                _parent = parent;
            }

            /// <summary>
            /// Gets or sets the value associated with the specified key.
            /// Overridden to return null if attribute is not found.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <returns>Value of the attribute or null</returns>
            public string? this[string key]
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    string? value = null;
                    _parent._attributes?.TryGetValue(key, out value);
                    return value;
                }
            }

            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Dictionary<string, string>.Enumerator GetEnumerator() => _parent._attributes?.GetEnumerator() ?? EmptyDictionary.GetEnumerator();

            /// <summary>
            /// Gets the number of key/value pairs contained in the <see cref="T:System.Collections.Generic.Dictionary`2" />.
            /// </summary>
            public int Count
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _parent._attributes?.Count ?? 0;
            }
        }
    }
}
