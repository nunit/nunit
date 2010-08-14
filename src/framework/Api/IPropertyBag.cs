using System;
using System.Collections;

#if CLR_2_0 || CLR_4_0
using System.Collections.Generic;
#endif

namespace NUnit.Framework.Api
{
    /// <summary>
    /// A PropertyBag represents a collection of name value pairs
    /// that allows duplicate entries with the same key. Methods
    /// are provided for adding a new pair as well as for setting
    /// a key to a single value. All keys are strings but values
    /// may be of any type. Null values are not permitted, since
    /// a null entry represents the absence of the key.
    /// </summary>
    public interface IPropertyBag : IEnumerable
    {
        /// <summary>
        /// Get the number of key/value pairs in the property bag
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Adds a key/value pair to the property bag
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        void Add(string key, object value);

        /// <summary>
        /// Add multiple values to the property bag
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="values">A list of the values</param>
        //void AddEach(string key, IList<object> values);

        /// <summary>
        /// Sets the value for a key, removing any other
        /// values that are already in the property set.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Set(string key, object value);

        /// <summary>
        /// Gets a single value for a key, using the first
        /// one if multiple values are present and returning
        /// null if the value is not found.
        /// </summary>
        /// <returns></returns>
        object Get(string key);

        /// <summary>
        ///  Removes all entries for a key from the property set.
        ///  If the key is not found, no error occurs.
        /// </summary>
        /// <param name="key">The key for which the entries are to be removed</param>
        void Remove(string key);

        /// <summary>
        /// Removes a single entry if present. If not found,
        /// no error occurs.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Remove(string key, object value);

        /// <summary>
        /// Removes a specific PropertyEntry. If the entry is not
        /// found, no errr occurs.
        /// </summary>
        /// <param name="entry">The property entry to remove</param>
        void Remove(PropertyEntry entry);

        /// <summary>
        /// Gets a flag indicating whether the specified key has
        /// any entries in the property set.
        /// </summary>
        /// <param name="key">The key to be checked</param>
        /// <returns>True if their are values present, otherwise false</returns>
        bool ContainsKey(string key);

        /// <summary>
        /// Gets a flag indicating whether the specified key and
        /// value are present in the property set.
        /// </summary>
        /// <param name="key">The key to be checked</param>
        /// <param name="value">The value to be checked</param>
        /// <returns>True if the key and value are present, otherwise false</returns>
        bool Contains(string key, object value);

        /// <summary>
        /// Gets a flag indicating whether the specified key and
        /// value are present in the property set.
        /// </summary>
        /// <param name="entry">The property entry to be checked</param>
        /// <returns>True if the entry is present, otherwise false</returns>
        bool Contains(PropertyEntry entry);

        /// <summary>
        /// Gets or sets the list of values for a particular key
        /// </summary>
        /// <param name="key">The key for which the values are to be retrieved or set</param>
        IList this[string key] { get; set; }

        /// <summary>
        /// Gets a collection containing all the keys in the property set
        /// </summary>
#if CLR_2_0 || CLR_4_0
        ICollection<string> Keys { get; }
#else
        ICollection Keys { get; }
#endif
    }

    /// <summary>
    /// The IPropertyEnumerator interface is defined as IEnumerator
    /// under .NET 1.0 and 1.1 and as IEnumerator&lt;PropertyEntry&gt;
    /// under later versions.
    /// </summary>
#if CLR_2_0 || CLR_4_0
    public interface IPropertyEnumerator : IEnumerator<PropertyEntry> { }
#else
    public interface IPropertyEnumerator : IEnumerator { }
#endif
}
