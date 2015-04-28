using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using NUnit.Integration.Tests.TeamCity.Core.Common;

namespace NUnit.Integration.Tests.TeamCity.Core
{
    internal sealed class ServiceMessage : IServiceMessage
    {
        private readonly Dictionary<string, string> _properties;        

        public ServiceMessage(
            [NotNull] string name, 
            [CanBeNull] string defaultValue = null,
            [CanBeNull] Dictionary<string, string> properties = null)
        {
            Contract.Requires<ArgumentNullException>(name != null);

            Name = name;
            DefaultValue = properties != null ? null : defaultValue;
            _properties = properties ?? new Dictionary<string, string>();
        }

        public string Name { get; private set; }

        public string DefaultValue { get; private set; }

        public IEnumerable<string> Attributes
        {
            get { return _properties.Keys; }
        }

        public string GetAttribute(string attributeName)
        {
            Contract.Requires<ArgumentNullException>(attributeName != null);

            string value;
            return _properties.TryGetValue(attributeName, out value) ? value : null;
        }

        public bool TryGetAttribute(string attributeName, out string value)
        {
            Contract.Requires<ArgumentNullException>(attributeName != null);

            return _properties.TryGetValue(attributeName, out value);
        }

        public override string ToString()
        {
            return string.Format(
                "{0}{1} {2} {3}",
                ServiceMessageConstants.ServiceMessageOpen,
                Name,
                string.Join(" ", _properties.Select(i => string.Format("{0}=\"{1}\"", i.Key, i.Value)).ToArray()),
                ServiceMessageConstants.ServiceMessageClose);
        }
    }
}