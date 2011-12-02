using System;
using System.Collections.Generic;

namespace NUnit.Framework.Api
{
    /// <summary>
    /// A PropertySet represents a collection of name value pairs.
    /// Duplicate entries with the same key are not allowed.
    /// All keys are strings but values may be of any type. Null 
    /// values are not permitted, since a null entry represents 
    /// the absence of the key.
    /// </summary>
    public interface IPropertySet
    {
        /// <summary>
        /// Get the number of key/value pairs in the property set
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets or sets the value for a particular key
        /// </summary>
        /// <param name="key">The key for which the value is to be retrieved or set</param>
        /// <returns></returns>
        object this[string key] { get; set; }

        /// <summary>
        /// Adds a key/value pair to the property set. Throws
        /// an InvalidArgumentException if the
        /// key is already present.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        void Add(string key, object value);

        /// <summary>
        ///  Removes the entry for a key from the property set
        /// </summary>
        /// <param name="key">The key for which the entries are to be removed</param>
        void Remove(string key);

        /// <summary>
        /// Gets a flag indicating whether the specified key is
        /// present in the property set.
        /// </summary>
        /// <param name="key">The key to be checked</param>
        /// <returns>True if their are values present, otherwise false</returns>
        bool ContainsKey(string key);

        /// <summary>
        /// Gets a collection containing all the keys in the property set
        /// </summary>
        ICollection<string> Keys { get; }

        /// <summary>
        /// Gets an enumerator for all entries in the property set
        /// </summary>
        /// <returns></returns>
        IEnumerator<KeyValuePair<string, object>> GetEnumerator();
    }
}
