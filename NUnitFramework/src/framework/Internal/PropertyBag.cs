using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using NUnit.Framework.Api;

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
        private Dictionary<string, IList> inner = new Dictionary<string, IList>();

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
                list = new ObjectList();
                inner.Add(key, list);
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
            IList list = new ObjectList();
            list.Add(value);
            inner[key] = list;
        }

        /// <summary>
        /// Gets a single value for a key, using the first
        /// one if multiple values are present and returning
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
        /// Gets a single boolean value for a key, using the first
        /// one if multiple values are present and returning the
        /// default value if no entry is found.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public bool GetSetting(string key, bool defaultValue)
        {
            object value = Get(key);
            return value == null
                ? defaultValue
                : (bool)value;
        }

        /// <summary>
        /// Gets a single string value for a key, using the first
        /// one if multiple values are present and returning the
        /// default value if no entry is found.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string GetSetting(string key, string defaultValue)
        {
            object value = Get(key);
            return value == null
                ? defaultValue
                : (string)value;
        }

        /// <summary>
        /// Gets a single int value for a key, using the first
        /// one if multiple values are present and returning the
        /// default value if no entry is found.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public int GetSetting(string key, int defaultValue)
        {
            object value = Get(key);
            return value == null
                ? defaultValue
                : (int)value;
        }

        /// <summary>
        /// Gets a single Enum value for a key, using the first
        /// one if multiple values are present and returning the
        /// default value if no entry is found.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public Enum GetSetting(string key, Enum defaultValue)
        {
            object value = Get(key);
            return value == null
                ? defaultValue
                : (Enum)value;
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            inner.Clear();
        }

        /// <summary>
        /// Removes all entries for a key from the property set
        /// </summary>
        /// <param name="key">The key for which the entries are to be removed</param>
        public void Remove(string key)
        {
            inner.Remove(key);
        }

        /// <summary>
        /// Removes a single entry if present. If not found,
        /// no error occurs.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Remove(string key, object value)
        {
            IList list;
            if (inner.TryGetValue(key, out list))
                list.Remove(value);
        }

        /// <summary>
        /// Removes a specific PropertyEntry. If the entry is not
        /// found, no errr occurs.
        /// </summary>
        /// <param name="entry">The property entry to remove</param>
        public void Remove(PropertyEntry entry)
        {
            Remove(entry.Name, entry.Value);
        }

        /// <summary>
        /// Get the number of key/value pairs in the property set
        /// </summary>
        /// <value></value>
        public int Count
        {
            get 
            {
                int count = 0;

                foreach (string key in inner.Keys)
                    count += ((IList)inner[key]).Count;

                return count; 
            }
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
            return inner.ContainsKey(key);
        }

        /// <summary>
        /// Gets a flag indicating whether the specified key and
        /// value are present in the property set.
        /// </summary>
        /// <param name="key">The key to be checked</param>
        /// <param name="value">The value to be checked</param>
        /// <returns>
        /// True if the key and value are present, otherwise false
        /// </returns>
        public bool Contains(string key, object value)
        {
            IList list;
            return inner.TryGetValue(key, out list) && list.Contains(value);
        }

        /// <summary>
        /// Gets a flag indicating whether the specified key and
        /// value are present in the property set.
        /// </summary>
        /// <param name="entry">The property entry to be checked</param>
        /// <returns>
        /// True if the entry is present, otherwise false
        /// </returns>
        public bool Contains(PropertyEntry entry)
        {
            return Contains(entry.Name, entry.Value);
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
        /// Gets an enumerator for all properties in the property bag
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return new PropertyBagEnumerator(this);
        }

        /// <summary>
        /// Gets or sets the list of values for a particular key
        /// </summary>
        public IList this[string key]
        {
            get
            {
                IList list;
                if (!inner.TryGetValue(key, out list))
                {
                    list = new ObjectList();
                    inner.Add(key, list);
                }
                return list;
            }
            set
            {
                inner[key] = value;
            }
        }

        #region IXmlNodeBuilder Members

        /// <summary>
        /// Returns an XmlNode representating the current PropertyBag.
        /// </summary>
        /// <param name="recursive">Not used</param>
        /// <returns>An XmlNode representing the PropertyBag</returns>
        public System.Xml.XmlNode ToXml(bool recursive)
        {
            XmlNode topNode = XmlHelper.CreateTopLevelElement("dummy");

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
        public System.Xml.XmlNode AddToXml(System.Xml.XmlNode parentNode, bool recursive)
        {
            XmlNode properties = XmlHelper.AddElement(parentNode, "properties");

            foreach (string key in Keys)
            {
                foreach (object value in this[key])
                {
                    XmlNode prop = XmlHelper.AddElement(properties, "property");

                    // TODO: Format as string
                    XmlHelper.AddAttribute(prop, "name", key.ToString());
                    XmlHelper.AddAttribute(prop, "value", value.ToString());
                }
            }

            return properties;
        }

        #endregion

        #region Nested PropertyBagEnumerator Class

        /// <summary>
        /// TODO: Documentation needed for class
        /// </summary>
        public class PropertyBagEnumerator : IEnumerator<PropertyEntry>
        {
            private IEnumerator<KeyValuePair<string, IList>> innerEnum;
            private PropertyBag bag;
            private IEnumerator valueEnum;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="bag"></param>
            public PropertyBagEnumerator(PropertyBag bag)
            {
                this.bag = bag;
                
                Initialize();
            }

            private void Initialize()
            {
                innerEnum = bag.inner.GetEnumerator();
                valueEnum = null;

                if (innerEnum.MoveNext())
                {
                    valueEnum = innerEnum.Current.Value.GetEnumerator();
                }
            }

            private PropertyEntry GetCurrentEntry()
            {
                if (valueEnum == null)
                    throw new InvalidOperationException();

                string key = innerEnum.Current.Key;

                object value = valueEnum.Current;

                return new PropertyEntry(key, value);
            }

            #region IEnumerator<PropertyEntry> Members

            PropertyEntry IEnumerator<PropertyEntry>.Current
            {
                get 
                {
                    return GetCurrentEntry();
                }
            }

            #endregion

            #region IDisposable Members

            void IDisposable.Dispose()
            {
            }

            #endregion

            #region IEnumerator Members

            object IEnumerator.Current
            {
                get 
                {
                    return GetCurrentEntry();
                }
            }

            bool IEnumerator.MoveNext()
            {
                if (valueEnum == null)
                    return false;
                    
                while (!valueEnum.MoveNext())
                {
                    if (!innerEnum.MoveNext())
                    {
                        valueEnum = null;
                        return false;
                    }

                    valueEnum = innerEnum.Current.Value.GetEnumerator();
                }

                return true;
            }

            void IEnumerator.Reset()
            {
                Initialize();
            }

            #endregion
        }

        #endregion
    }
}
