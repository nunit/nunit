// ***********************************************************************
// Copyright (c) 2016 Charlie Poole, Rob Prouse
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

#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace NUnit.Framework
{
    /// <summary>
    /// TestParameters class holds any named parameters supplied to the test run
    /// </summary>
    public class TestParameters
    {
        private static readonly IFormatProvider MODIFIED_INVARIANT_CULTURE = CreateModifiedInvariantCulture();

        private readonly Dictionary<string, string> _parameters = new Dictionary<string, string>();

        /// <summary>
        /// Gets the number of test parameters
        /// </summary>
        public int Count
        {
            get { return _parameters.Count; }
        }

        /// <summary>
        /// Gets a collection of the test parameter names
        /// </summary>
        public ICollection<string> Names
        {
            get { return _parameters.Keys; }
        }

        /// <summary>
        /// Gets a flag indicating whether a parameter with the specified name exists.
        /// </summary>
        /// <param name="name">Name of the parameter</param>
        /// <returns>True if it exists, otherwise false</returns>
        public bool Exists(string name)
        {
            return _parameters.ContainsKey(name);
        }

        /// <summary>
        /// Indexer provides access to the internal dictionary
        /// </summary>
        /// <param name="name">Name of the parameter</param>
        /// <returns>Value of the parameter or null if not present</returns>
        public string? this[string name]
        {
            get { return Get(name); }
        }

        /// <summary>
        /// Get method is a simple alternative to the indexer
        /// </summary>
        /// <param name="name">Name of the parameter</param>
        /// <returns>Value of the parameter or null if not present</returns>
        public string? Get(string name)
        {
            return Exists(name) ? _parameters[name] : null;
        }

        /// <summary>
        /// Get the value of a parameter or a default string
        /// </summary>
        /// <param name="name">Name of the parameter</param>
        /// <param name="defaultValue">Default value of the parameter</param>
        /// <returns>Value of the parameter or default value if not present</returns>
        [return: NotNullIfNotNull("defaultValue")]
        public string? Get(string name, string? defaultValue)
        {
            return Get(name) ?? defaultValue;
        }

        /// <summary>
        /// Get the value of a parameter or return a default
        /// </summary>
        /// <typeparam name="T">The return Type</typeparam>
        /// <param name="name">Name of the parameter</param>
        /// <param name="defaultValue">Default value of the parameter</param>
        /// <returns>Value of the parameter or default value if not present</returns>
        [return: NotNullIfNotNull("defaultValue")]
        public T Get<T>(string name, [MaybeNull] T defaultValue)
        {
            string? val = Get(name);
            return val != null ? (T)Convert.ChangeType(val, typeof(T), MODIFIED_INVARIANT_CULTURE) : defaultValue;
        }

        /// <summary>
        /// Adds a parameter to the list
        /// </summary>
        /// <param name="name">Name of the parameter</param>
        /// <param name="value">Value of the parameter</param>
        internal void Add(string name, string value)
        {
            _parameters[name] = value;
        }

        private static IFormatProvider CreateModifiedInvariantCulture()
        {
            var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();

            // Remove comma (,) as group separator since it may confuse developers in cultures
            // where comma is a decimal separator
            culture.NumberFormat.CurrencyGroupSeparator = string.Empty;
            culture.NumberFormat.NumberGroupSeparator = string.Empty;
            culture.NumberFormat.PercentGroupSeparator = string.Empty;

            return culture;
        }
    }
}
