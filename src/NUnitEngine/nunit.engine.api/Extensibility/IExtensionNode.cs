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

namespace NUnit.Engine.Extensibility
{
    /// <summary>
    /// The IExtensionNode interface is implemented by a class that represents a 
    /// single extension being installed on a particular extension point.
    /// </summary>
    public interface IExtensionNode
    {
        /// <summary>
        /// Gets the full name of the Type of the extension object.
        /// </summary>
        string TypeName { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="NUnit.Engine.Extensibility.IExtensionNode"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        bool Enabled { get; }
    
        /// <summary>
        /// Gets the unique string identifying the ExtensionPoint for which 
        /// this Extension is intended. This identifier may be supplied by the attribute
        /// marking the extension or deduced by NUnit from the Type of the extension class.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Gets an optional description of what the extension does.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets a collection of the names of all this extension's properties
        /// </summary>
        IEnumerable<string> PropertyNames { get; }

        /// <summary>
        /// Gets a collection of the values of a particular named property
        /// If none are present, returns an empty enumerator.
        /// </summary>
        /// <param name="name">The property name</param>
        /// <returns>A collection of values</returns>
        IEnumerable<string> GetValues(string name);
    }
}
