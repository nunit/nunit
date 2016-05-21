// ***********************************************************************
// Copyright (c) 2016 Charlie Poole
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
using NUnit.Engine.Extensibility;

namespace NUnit.Engine
{
    /// <summary>
    /// The IExtensionService interface allows a runner to manage extensions.
    /// </summary>
    public interface IExtensionService
    {
        #region Public Properties

        /// <summary>
        /// Gets an enumeration of all ExtensionPoints in the engine.
        /// </summary>
        IEnumerable<IExtensionPoint> ExtensionPoints { get; }

        /// <summary>
        /// Gets an enumeration of all installed Extensions.
        /// </summary>
        IEnumerable<IExtensionNode> Extensions { get; }

        /// <summary>
        /// Get an ExtensionPoint based on it's unique identifying path.
        /// </summary>
        IExtensionPoint GetExtensionPoint(string path);

        /// <summary>
        /// Get an enumeration of ExtensionNodes based on their identifying path.
        /// </summary>
        IEnumerable<IExtensionNode> GetExtensionNodes(string path);

        /// <summary>
        /// Enable or disable an extension
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="enabled"></param>
        void EnableExtension(string typeName, bool enabled);

        #endregion
    }
}

