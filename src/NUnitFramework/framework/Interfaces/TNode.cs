// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
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
        private static readonly XmlWriterSettings _xmlWriterSettings = new()
        {
            ConformanceLevel = ConformanceLevel.Fragment
        };

        internal List<TNode>? _childNodes;
        internal Dictionary<string, string>? _attributes;

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
        public TNode(string name, string? value) : this(name, value, false) { }

        /// <summary>
        /// Constructs a new instance of TNode with a value
        /// </summary>
        /// <param name="name">The name of the node</param>
        /// <param name="value">The text content of the node</param>
        /// <param name="valueIsCDATA">Flag indicating whether to use CDATA when writing the text</param>
        public TNode(string name, string? value, bool valueIsCDATA)
            : this(name)
        {
            Value = EscapeInvalidXmlCharacters(value);
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
        public AttributeDictionary Attributes => new AttributeDictionary(this);

        /// <summary>
        /// Gets a list of child nodes
        /// </summary>
        public NodeList ChildNodes => new NodeList(this);

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
                using (var xmlWriter = XmlWriter.Create(stringWriter, _xmlWriterSettings))
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

                    if (root == null)
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

            if (root == null)
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
            _attributes.Add(name, EscapeInvalidXmlCharacters(value));
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

            if (Value != null)
                if (ValueIsCDATA)
                    WriteCDataTo(writer);
                else
                    writer.WriteString(Value);

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
                if (child.NodeType == XmlNodeType.Element)
                    tNode.AddChildNode(FromXml(child));

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

            List<TNode> resultNodes = new List<TNode>();
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

            return tail != null
                ? ApplySelection(resultNodes, tail)
                : resultNodes;
        }

        [return: NotNullIfNotNull("str")]
        private static string? EscapeInvalidXmlCharacters(string? str)
        {
            if (str == null) return null;

            // quick check when we expect valid input
            foreach (var c in str)
            {
                if (c < 0x20 || c > 0x7F)
                {
                    return EscapeInvalidXmlCharactersUnlikely(str);
                }
            }

            return str;
        }

        private static string EscapeInvalidXmlCharactersUnlikely(string str)
        {
            StringBuilder? builder = null;
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                if(c > 0x20 && c < 0x7F)
                {
                    // ASCII characters - break quickly for these
                    builder?.Append(c);
                }
                // From the XML specification: https://www.w3.org/TR/xml/#charsets
                // Char ::= #x9 | #xA | #xD | [#x20-#xD7FF] | [#xE000-#xFFFD] | [#x10000-#x10FFFF]
                // Any Unicode character, excluding the surrogate blocks, FFFE, and FFFF.
                else if (!(0x0 <= c && c <= 0x8) &&
                    c != 0xB &&
                    c != 0xC &&
                    !(0xE <= c && c <= 0x1F) &&
                    !(0x7F <= c && c <= 0x84) &&
                    !(0x86 <= c && c <= 0x9F) &&
                    !(0xD800 <= c && c <= 0xDFFF) &&
                    c != 0xFFFE &&
                    c != 0xFFFF)
                {
                    builder?.Append(c);
                }
                // Also check if the char is actually a high/low surrogate pair of two characters.
                // If it is, then it is a valid XML character (from above based on the surrogate blocks).
                else if (char.IsHighSurrogate(c) &&
                    i + 1 != str.Length &&
                    char.IsLowSurrogate(str[i + 1]))
                {
                    if (builder != null)
                    {
                        builder.Append(c);
                        builder.Append(str[i + 1]);
                    }
                    i++;
                }
                else
                {
                    // We keep the builder null so that we don't allocate a string
                    // when doing this conversion until we encounter a unicode character.
                    // Then, we allocate the rest of the string and escape the invalid
                    // character.
                    if (builder == null)
                    {
                        builder = new StringBuilder();
                        for (int index = 0; index < i; index++)
                            builder.Append(str[index]);
                    }
                    builder.Append(CharToUnicodeSequence(c));
                }
            }

            if (builder != null)
                return builder.ToString();
            else
                return str;
        }

        private static string CharToUnicodeSequence(char symbol)
        {
            return $"\\u{(int)symbol:x4}";
        }

        private void WriteCDataTo(XmlWriter writer)
        {
            int start = 0;
            string text = Value ?? throw new InvalidOperationException();

            while (true)
            {
                int illegal = text.IndexOf("]]>", start, StringComparison.Ordinal);
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
                    string filter = xpath.Substring(lbrack+1, xpath.Length - lbrack - 2);

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
    public readonly struct NodeList : IEnumerable<TNode>
    {
        private static readonly List<TNode> _emptyList = new();

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
                if (_parent._childNodes is null || (uint) index >= (uint) _parent._childNodes.Count)
                    ThrowArgumentOutOfRangeException(index);

                return _parent._childNodes![index];
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowArgumentOutOfRangeException(int index)
        {
            throw new ArgumentOutOfRangeException(nameof(index), index, "Index was out or range of valida values");
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
        public List<TNode>.Enumerator GetEnumerator() => _parent._childNodes?.GetEnumerator() ?? _emptyList.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IEnumerator<TNode> IEnumerable<TNode>.GetEnumerator() => GetEnumerator();
    }

    /// <summary>
    /// Class used to represent the attributes of a node
    /// </summary>
    public readonly struct AttributeDictionary
    {
        private static readonly Dictionary<string, string> _emptyDictionary = new();

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
        public Dictionary<string,string>.Enumerator GetEnumerator() => _parent._attributes?.GetEnumerator() ?? _emptyDictionary.GetEnumerator();

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
