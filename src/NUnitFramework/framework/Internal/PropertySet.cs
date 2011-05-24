using System;
using NUnit.Framework.Api;

#if CLR_1_0 || CLR_1_1
using System.Collections;
#else
using System.Collections.Generic;
#endif

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// A PropertySet represents a collection of name value pairs.
    /// Duplicate entries with the same key are not allowed.
    /// All keys are strings but values may be of any type. Null
    /// values are not permitted, since a null entry represents
    /// the absence of the key.
    /// </summary>
    public class PropertySet : IPropertySet
    {
        /// <summary>
        /// Adds a key/value pair to the property set. Throws
        /// an InvalidArgumentException if the
        /// key is already present.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        public void Add(string key, object value)
        {
            inner.Add(key, value);
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            inner.Clear();
        }

        /// <summary>
        /// Removes the entry for a key from the property set
        /// </summary>
        /// <param name="key">The key for which the entries are to be removed</param>
        public void Remove(string key)
        {
            inner.Remove(key);
        }

        /// <summary>
        /// Get the number of key/value pairs in the property set
        /// </summary>
        /// <value></value>
        public int Count
        {
            get { return inner.Count; }
        }


#if CLR_1_0 || CLR_1_1
        public object this[string key]
        {
            get
            {
                return inner[key];
            }
            set
            {
                inner[key] = value;
            }
        }

        /// <summary>
        /// Gets a flag indicating whether the specified key is
        /// present in the property set.
        /// </summary>
        /// <param name="key">The key to be checked</param>
        /// <returns>
        /// True if their are values present, otherwise false
        /// </returns>
        public bool ContainsKey(string key)
        {
            return inner.Contains(key);
        }

        /// <summary>
        /// Gets a collection containing all the keys in the property set
        /// </summary>
        /// <value></value>
        public ICollection Keys
        {
            get { return inner.Keys; }
        }

        /// <summary>
        /// Gets an enumerator for all entries in the property set
        /// </summary>
        /// <returns></returns>
        public IDictionaryEnumerator GetEnumerator()
        {
            return inner.GetEnumerator();
        }

        private IDictionary inner = new Hashtable();
#else
        /// <summary>
        /// Gets or sets the value for a particular key
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        public object this[string key]
        {
            get
            {
                object result = null;
                inner.TryGetValue(key, out result);
                return result;
            }
            set
            {
                inner[key] = value;
            }
        }

        /// <summary>
        /// Gets a flag indicating whether the specified key is
        /// present in the property set.
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
        /// Gets a collection containing all the keys in the property set
        /// </summary>
        public ICollection<string> Keys
        {
            get { return inner.Keys; }
        }

        /// <summary>
        /// Gets an enumerator for all entries in the property set
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return inner.GetEnumerator();
        }

        private Dictionary<string, object> inner = new Dictionary<string, object>();
#endif
    }
}
