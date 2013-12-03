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
        public XmlNode ToXml(bool recursive)
        {
            XmlNode topNode = XmlNode.CreateTopLevelElement("dummy");

            XmlNode thisNode = AddToXml(topNode, recursive);

            return thisNode;
        }

        /// <summary>
        /// Returns an XmlNode representing the PropertyBag after
        /// adding it as a child of the supplied parent node.
        /// </summary>
        /// <param name="parentNode">The parent node.</param>
        /// <param name="recursive">Not used</param>
        /// <returns></returns>
        public XmlNode AddToXml(XmlNode parentNode, bool recursive)
        {
            XmlNode properties = parentNode.AddElement("properties");

            foreach (string key in Keys)
            {
                foreach (object value in this[key])
                {
                    XmlNode prop = properties.AddElement("property");

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
