using System;
using System.Collections.Generic;
using System.Text;

namespace NUnit.Framework
{
    /// <summary>
    /// TestParameters class holds any named parameters supplied to the test run
    /// </summary>
    public class TestParameters
    {
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
        /// Indexer is the only public access to the internal dictionary
        /// </summary>
        /// <param name="name">Name of the parameter</param>
        /// <returns>Value of the parameter or null if not present</returns>
        public string this[string name]
        {
            get { return _parameters.ContainsKey(name) ? _parameters[name] : null; }
        }

        /// <summary>
        /// Get method is a simple alternative to the indexer
        /// </summary>
        /// <param name="name">Name of the paramter</param>
        /// <returns>Value of the paramter or null if not present</returns>
        public string Get(string name)
        {
            return _parameters.ContainsKey(name) ? _parameters[name] : null;
        }

        public string Get(string name, string defaultValue)
        {
            return _parameters.ContainsKey(name) ? _parameters[name] : defaultValue;
        }
        
        public int Get(string name, int defaultValue)
        {
            return _parameters.ContainsKey(name) ? int.Parse(_parameters[name]) : defaultValue;
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
    }
}
