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

        bool TryGetAttribute([NotNull] string attributeName, out string value);
    }
}