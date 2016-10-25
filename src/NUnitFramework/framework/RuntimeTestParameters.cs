using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// TestParameters class holds any named parameters supplied to the test run
    /// </summary>
    public class RuntimeTestParameters
    {
        private static readonly IFormatProvider MODIFIED_INVARIANT_CULTURE = CreateModifiedInvariantCulture();

        private readonly Dictionary<string, string> _parameters = new Dictionary<string, string>();

        private Dictionary<Type, Dictionary<string, object>> _types;

#if NETCF
        private static readonly List<Type> ConvertibleTypes = new List<Type>
#else
        private static readonly HashSet<Type> ConvertibleTypes = new HashSet<Type>
#endif
            {
            typeof(object),
#if !PORTABLE
            typeof(DBNull),
#endif
            typeof(bool), typeof(char), typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int),
            typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal), typeof(DateTime), typeof(string)
            };

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
        /// Gets a flag indicating whether a parameter with the specified name exists.N
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
        public string this[string name]
        {
            get { return Get(name); }
        }

        /// <summary>
        /// Get method is a simple alternative to the indexer
        /// </summary>
        /// <param name="name">Name of the paramter</param>
        /// <returns>Value of the parameter or null if not present</returns>
        public string Get(string name)
        {
            return Exists(name) ? _parameters[name] : null;
        }

        /// <summary>
        /// Get the value of a parameter or a default string
        /// </summary>
        /// <param name="name">Name of the parameter</param>
        /// <param name="defaultValue">Default value of the parameter</param>
        /// <returns>Value of the parameter or default value if not present</returns>
        public string Get(string name, string defaultValue)
        {
            return Get(name) ?? defaultValue;
        }

        /// <summary>
        /// Get the value of a parameter or return a default
        /// </summary>
        /// <typeparam name="T">The return Type</typeparam>
        /// <param name="name">Name of the parameter</param>
        /// <returns>Value of the parameter or default value if not present</returns>
        public T Get<T>(string name)
        {
            return Get(name, default(T));
        }

        /// <summary>
        /// Get the value of a parameter or return a default
        /// </summary>
        /// <typeparam name="T">The return Type</typeparam>
        /// <param name="name">Name of the parameter</param>
        /// <param name="defaultValue">Default value of the parameter</param>
        /// <returns>Value of the parameter or default value if not present</returns>
        public T Get<T>(string name, T defaultValue)
        {
            return (T)Get(typeof(T), name, defaultValue);
        }

        /// <summary>
        /// Get the value of a parameter or return a default
        /// </summary>
        /// <param name="type">Type of parameter</param>
        /// <param name="name">Name of the parameter</param>
        /// <returns>Value of the parameter or default value if not present</returns>
        internal object Get(Type type, string name)
        {
            var typeWrapper = new TypeWrapper(type);
            return Get(type, name, typeWrapper.IsValueType ? Activator.CreateInstance(type) : null);
        }

        /// <summary>
        /// Get the value of a parameter or return a default
        /// </summary>
        /// <param name="type">Type of parameter</param>
        /// <param name="name">Name of the parameter</param>
        /// <param name="defaultValue">Default value of the parameter</param>
        /// <returns>Value of the parameter or default value if not present</returns>
        internal object Get(Type type, string name, object defaultValue)
        {
            if (type == typeof(string))
                return Get(name, defaultValue);

            Dictionary<string, object> dict;
            object value;
            if (_types != null && _types.TryGetValue(type, out dict) && dict.TryGetValue(name, out value))
                return value;

            string val = Get(name);
            if (val == null)
                return defaultValue;

            if (ConvertibleTypes.Contains(type))
                return Convert.ChangeType(val, type, MODIFIED_INVARIANT_CULTURE);

#if !PORTABLE && !SILVERLIGHT && !NETCF
            var converter = TypeDescriptor.GetConverter(type);
            if (converter.CanConvertTo(type) && converter.CanConvertFrom(typeof(string)))
                return converter.ConvertFromInvariantString(val);
#endif

            return new InvalidCastException("Cannot convert parameter to: " + type.FullName);
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

        /// <summary>
        /// Adds a typed parameter to the list
        /// </summary>
        /// <typeparam name="T">The value Type</typeparam>
        /// <param name="name">Name of the parameter</param>
        /// <param name="value">Value of the parameter</param>
        /// <returns>true of parameter added successfully, false if not</returns>
        internal bool Add<T>(string name, T value)
        {
            _parameters[name] = value.ToString();

            if (typeof(T) != typeof(string))
            {
                Dictionary<string, object> dict;
                if (_types == null || !_types.TryGetValue(typeof(T), out dict))
                {
                    if (_types == null)
                        Interlocked.CompareExchange(ref _types, new Dictionary<Type, Dictionary<string, object>>(), null);
                    _types.Add(typeof(T), dict = new Dictionary<string, object>());
                }
                dict[name] = value;
            }

            return true;
        }

        /// <summary>
        /// Adds a typed parameter to the list
        /// </summary>
        /// <typeparam name="T">The value Type</typeparam>
        /// <param name="name">Name of the parameter</param>
        /// <param name="value">Value of the parameter</param>
        /// <returns>true of parameter added successfully, false if not</returns>
        internal bool Add<T>(string name, string value)
        {
            return Add(typeof(T), name, value);
        }

        /// <summary>
        /// Adds a typed parameter to the list
        /// </summary>
        /// <param name="type">Type of parameter</param>
        /// <param name="name">Name of the parameter</param>
        /// <param name="value">Value of the parameter</param>
        /// <returns>true of parameter added successfully, false if not</returns>
        internal bool Add(Type type, string name, string value)
        {
            _parameters[name] = value;

            if (type != typeof(string))
            {
                Dictionary<string, object> dict;
                if (_types == null || !_types.TryGetValue(type, out dict))
                {
                    if (_types == null)
                        Interlocked.CompareExchange(ref _types, new Dictionary<Type, Dictionary<string, object>>(), null);
                    _types.Add(type, dict = new Dictionary<string, object>());
                }

                if (ConvertibleTypes.Contains(type))
                {
                    dict[name] = Convert.ChangeType(value, type, MODIFIED_INVARIANT_CULTURE);
                    return true;
                }

#if !PORTABLE && !SILVERLIGHT && !NETCF
                var converter = TypeDescriptor.GetConverter(type);
                if (converter.CanConvertTo(type) && converter.CanConvertFrom(typeof(string)))
                {
                    dict[name] = converter.ConvertFromInvariantString(value);
                    return true;
                }
#endif

                return false;
            }

            return true;
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