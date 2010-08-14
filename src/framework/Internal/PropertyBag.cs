using System;
using System.Collections;
using NUnit.Framework.Api;

#if CLR_2_0 || CLR_4_0
using System.Collections.Generic;
#endif

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
#if CLR_2_0 || CLR_4_0
        private Dictionary<string, IList> inner = new Dictionary<string, IList>();
#else
        private IDictionary inner = new Hashtable();
#endif

        /// <summary>
        /// Adds a key/value pair to the property set
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        public void Add(string key, object value)
        {
#if CLR_2_0 || CLR_4_0
            IList list;
            if (!inner.TryGetValue(key, out list))
#else
            IList list = this[key];
            if (list == null)
#endif
            {
                list = new ObjectList();
                inner.Add(key, list);
            }
            list.Add(value);
        }

        /// <summary>
        /// Add multiple values to the property bag
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="values">A list of the values</param>
        //public void AddEach(string key, IList<object> values)
        //{
        //    foreach (object value in values)
        //        Add(key, value);
        //}

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
#if CLR_2_0 || CLR_4_0
            IList list;
            return inner.TryGetValue(key, out list) && list.Count > 0
                ? list[0]
                : null;
#else
            IList list = inner[key] as IList;
            return list != null && list.Count > 0
                ? list[0]
                : null;
#endif
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
#if CLR_2_0 || CLR_4_0
            IList list;
            if (inner.TryGetValue(key, out list))
                list.Remove(value);
#else
            IList list = inner[key] as IList;
            if (list != null)
                list.Remove(value);
#endif
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
#if CLR_2_0 || CLR_4_0
            return inner.ContainsKey(key);
#else
            return inner.Contains(key);
#endif
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
#if CLR_2_0 || CLR_4_0
            IList list;
            return inner.TryGetValue(key, out list) && list.Contains(value);
#else
            IList list = inner[key] as IList;
            return list != null && list.Contains(value);
#endif
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
#if CLR_2_0 || CLR_4_0
        public ICollection<string> Keys
#else
        public ICollection Keys
#endif
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
#if CLR_2_0 || CLR_4_0
                IList list;
                if (!inner.TryGetValue(key, out list))
#else
                IList list = inner[key] as IList;
                if (list == null)
#endif
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

        #region Nested PropertyBagEnumerator Class

#if CLR_2_0 || CLR_4_0
        public class PropertyBagEnumerator : IEnumerator<PropertyEntry>
        {
            private IEnumerator<KeyValuePair<string, IList>> innerEnum;
#else
        public class PropertyBagEnumerator : IEnumerator
        {
            private IEnumerator innerEnum;
#endif
            private PropertyBag bag;
            private IEnumerator valueEnum;

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
#if CLR_2_0 || CLR_4_0
                    valueEnum = innerEnum.Current.Value.GetEnumerator();
#else
                    DictionaryEntry entry = (DictionaryEntry)innerEnum.Current;
                    IList list = (IList)entry.Value;
                    valueEnum = list.GetEnumerator();
#endif
                }
            }

            private PropertyEntry GetCurrentEntry()
            {
                if (valueEnum == null)
                    throw new InvalidOperationException();

#if CLR_2_0 || CLR_4_0
                string key = innerEnum.Current.Key;
#else
                string key = (string)((DictionaryEntry)innerEnum.Current).Key;
#endif

                object value = valueEnum.Current;

                return new PropertyEntry(key, value);
            }

#if CLR_2_0 || CLR_4_0
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
#endif

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

#if CLR_2_0 || CLR_4_0
                    valueEnum = innerEnum.Current.Value.GetEnumerator();
#else
                    DictionaryEntry entry = (DictionaryEntry)innerEnum.Current;
                    IList list = (IList)entry.Value;
                    valueEnum = list.GetEnumerator();
#endif
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
