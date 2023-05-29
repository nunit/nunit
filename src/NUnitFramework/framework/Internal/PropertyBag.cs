// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// A PropertyBag represents a collection of name value pairs
    /// that allows duplicate entries with the same key. Methods
    /// are provided for adding a new pair as well as for setting
    /// a key to a single value. All keys are strings but values
    /// may be of any type. Null values are not permitted, since
    /// a null entry represents the absence of the key.
    /// </summary>
    public class PropertyBag : IPropertyBag
    {
        private readonly Dictionary<string, IList> _inner = new Dictionary<string, IList>();

        #region IPropertyBagMembers

        /// <summary>
        /// Adds a key/value pair to the property set
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        public void Add(string key, object value)
        {
            Guard.ArgumentNotNull(value, "value");

            if (!_inner.TryGetValue(key, out var list))
            {
                list = new List<object>();
                _inner.Add(key, list);
            }
            list.Add(value);
        }

        /// <summary>
        /// Sets the value for a key, removing any other
        /// values that are already in the property set.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Set(string key, object value)
        {
            // Guard against mystery exceptions later!
            Guard.ArgumentNotNull(key, nameof(key));
            Guard.ArgumentNotNull(value, nameof(value));

            IList list = new List<object>();
            list.Add(value);
            _inner[key] = list;
        }

        /// <summary>
        /// Gets a single value for a key, using the first
        /// one if multiple values are present and returning
        /// null if the value is not found.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object? Get(string key)
        {
            return _inner.TryGetValue(key, out var list) && list.Count > 0
                ? list[0]
                : null;
        }

        /// <summary>
        /// Gets a flag indicating whether the specified key has
        /// any entries in the property set.
        /// </summary>
        /// <param name="key">The key to be checked</param>
        /// <returns>
        /// True if their are values present, otherwise false
        /// </returns>
        public bool ContainsKey(string key)
        {
            return _inner.ContainsKey(key);
        }

        /// <summary>
        /// Tries to retrieve list of values.
        /// </summary>
        /// <param name="key">The key for which the values are to be retrieved</param>
        /// <param name="values">Values, if found</param>
        /// <returns>true if found</returns>
        public bool TryGet(string key, [NotNullWhen(true)] out IList? values)
        {
            return _inner.TryGetValue(key, out values);
        }

        /// <summary>
        /// Gets a collection containing all the keys in the property set
        /// </summary>
        /// <value></value>
        public ICollection<string> Keys => _inner.Keys;

        /// <summary>
        /// Gets or sets the list of values for a particular key
        /// </summary>
        public IList this[string key]
        {
            get
            {
                if (!_inner.TryGetValue(key, out var list))
                {
                    list = new List<object>();
                    _inner.Add(key, list);
                }
                return list;
            }
            set => _inner[key] = value;
        }

        #endregion

        #region IXmlNodeBuilder Members

        /// <summary>
        /// Returns an XmlNode representing the current PropertyBag.
        /// </summary>
        /// <param name="recursive">Not used</param>
        /// <returns>An XmlNode representing the PropertyBag</returns>
        public TNode ToXml(bool recursive)
        {
            return AddToXml(new TNode("dummy"), recursive);
        }

        /// <summary>
        /// Returns an XmlNode representing the PropertyBag after
        /// adding it as a child of the supplied parent node.
        /// </summary>
        /// <param name="parentNode">The parent node.</param>
        /// <param name="recursive">Not used</param>
        /// <returns></returns>
        public TNode AddToXml(TNode parentNode, bool recursive)
        {
            TNode properties = parentNode.AddElement("properties");

            // enumerating dictionary directly with struct enumerator which is fastest
            foreach (var pair in _inner)
            {
                // Use for-loop to avoid allocating the enumerator
                var list = pair.Value;
                var propertyCount = list.Count;
                for (var i = 0; i < propertyCount; i++)
                {
                    TNode prop = properties.AddElement("property");

                    // TODO: Format as string
                    prop.AddAttribute("name", pair.Key);
                    prop.AddAttribute("value", list[i]!.ToString()!);
                }
            }

            return properties;
        }

        #endregion
    }
}
