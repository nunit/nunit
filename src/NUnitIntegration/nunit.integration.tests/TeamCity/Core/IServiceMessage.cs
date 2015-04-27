using System.Collections.Generic;

using JetBrains.Annotations;

namespace NUnit.Integration.Tests.TeamCity.Core
{
    /// <summary>
    /// Object representation of TeamCity service message
    /// </summary>
    internal interface IServiceMessage
    {
        /// <summary>
        /// Service message name, i.e. messageName in ##teamcity[messageName 'ddd']. 
        /// </summary>
        [NotNull]
        string Name { get; }

        /// <summary>
        /// For one-value service messages returns value, i.e. 'aaa' for ##teamcity[message 'aaa']
        /// or <code>null</code> otherwise, i.e. ##teamcity[message aa='aaa']
        /// </summary>
        [CanBeNull]
        string DefaultValue { get; }

        /// <summary>
        /// Emptry for one-value service messages, i.e. ##teamcity[message 'aaa'], returns all keys otherwise
        /// </summary>
        [NotNull]
        IEnumerable<string> Attributes { get; }

        /// <summary>
        /// Return a value for keys or <code>null</code>
        /// </summary>
        /// <param name="attributeName">Key to check for value</param>
        /// <returns>value of available or <code>null</code></returns>
        [CanBeNull]
        string GetAttribute([NotNull] string attributeName);

        bool TryGetAttribute([NotNull] string attributeName, out string value);
    }
}