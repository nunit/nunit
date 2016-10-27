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
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// A PropertyBag represents a collection of name value pairs
    /// that allows duplicate entries with the same key. Methods
    /// are provided for adding a new pair as well as for setting
    /// a key to a single value. All keys are strings but _values
    /// may be of any type. Null _values are not permitted, since
    /// a null entry represents the absence of the key.
    /// </summary>
    public class PropertyBag : IPropertyBag
    {
        private Dictionary<string, IList> inner = new Dictionary<string, IList>();

        #region IPropertyBagMembers

        /// <summary>
        /// Adds a key/value pair to the property set
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        public void Add(string key, object value)
        {
            IList list;
            if (!inner.TryGetValue(key, out list))
            {
                list = new List<object>();
                inner.Add(key, list);
            }
            list.Add(value);
        }

        /// <summary>
        /// Sets the value for a key, removing any other
        /// _values that are already in the property set.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Set(string key, object value)
        {
            // Guard against mystery exceptions later!
            Guard.ArgumentNotNull(key, "key");
            Guard.ArgumentNotNull(value, "value");

            IList list = new List<object>();
            list.Add(value);
            inner[key] = list;
        }

        /// <summary>
        /// Gets a single value for a key, using the first
        /// one if multiple _values are present and returning
        /// null if the value is not found.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Get(string key)
        {
            IList list;
            return inner.TryGetValue(key, out list) && list.Count > 0
                ? list[0]
                : null;
        }

        /// <summary>
        /// Gets a flag indicating whether the specified key has
        /// any entries in the property set.
        /// </summary>
        /// <param name="key">The key to be checked</param>
        /// <returns>
        /// True if their are _values present, otherwise false
        /// </returns>
        public bool ContainsKey(string key)
        {
            return inner.ContainsKey(key);
        }

        /// <summary>
        /// Gets a collection containing all the keys in the property set
        /// </summary>
        /// <value></value>
        public ICollection<string> Keys
        {
            get { return inner.Keys; }
        }

        /// <summary>
        /// Gets or sets the list of _values for a particular key
        /// </summary>
        public IList this[string key]
        {
            get
            {
                IList list;
                if (!inner.TryGetValue(key, out list))
                {
                    list = new List<object>();
                    inner.Add(key, list);
                }
                return list;
            }
            set
            {
                inner[key] = value;
            }
        }

        #endregion

        #region IXmlNodeBuilder Members

        /// <summary>
        /// Returns an XmlNode representating the current PropertyBag.
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

            foreach (string key in Keys)
            {
                foreach (object value in this[key])
                {
                    TNode prop = properties.AddElement("property");

                    // TODO: Format as string
                    prop.AddAttribute("name", key.ToString());
                    prop.AddAttribute("value", value.ToString());
                }
            }

            return properties;
        }

        #endregion
    }
}
