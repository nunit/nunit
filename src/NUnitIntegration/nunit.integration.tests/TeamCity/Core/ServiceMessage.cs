// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
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

using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

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

        public bool TryGetAttribute(string attributeName, out string value)
        {
            Contract.Requires<ArgumentNullException>(attributeName != null);

            return _properties.TryGetValue(attributeName, out value);
        }

        public override string ToString()
        {
            return string.Format(
                "{0}{1} {2}{3}",
                ServiceMessageConstants.ServiceMessageOpen,
                Name,
                string.Join(" ", _properties.Select(i => string.Format("{0}=\"{1}\"", i.Key, i.Value)).ToArray()),
                ServiceMessageConstants.ServiceMessageClose);
        }
    }
}