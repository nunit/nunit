// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
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

namespace NUnit.Engine.Extensibility
{
    /// <summary>
    /// The ExtensionNode class represents a single extension being installed
    /// on a particular extension point. It stores information needed to
    /// activate the extension object on a just-in-time basis.
    /// </summary>
    public class ExtensionNode
    {
        object _extensionObject;
        string _assemblyPath;
        string _typeName;

        /// <summary>
        /// Construct an ExtensionNode
        /// </summary>
        /// <param name="assemblyPath">The path to the assembly where this extension is found.</param>
        /// <param name="typeName">The full name of the Type of the extension object.</param>
        public ExtensionNode(string assemblyPath, string typeName)
        {
            _assemblyPath = assemblyPath;
            _typeName = typeName;
        }
        /// <summary>
        /// Gets the path to the assembly where the extension is defined.
        /// </summary>
        public string AssemblyPath
        {
            get { return _assemblyPath;  }
        }

        /// <summary>
        /// Gets the full name of the Type of the assembly object.
        /// </summary>
        public string TypeName
        {
            get { return _typeName;  }
        }

        /// <summary>
        /// Gets an object of the specified extension type.
        /// </summary>
        public object ExtensionObject
        {
            get
            {
                if (_extensionObject == null)
                    _extensionObject = AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(_assemblyPath, _typeName);

                return _extensionObject;
            }
        }

        /// <summary>
        /// Gets and sets the unique path identifying the ExtensionPoint for which
        /// this Extension node is intended.
        /// </summary>
        /// <remarks>
        /// Currently, the Path must be supplied before the extensin can actually be used 
        /// at an extension point. In a future release, we may be able to deduce the 
        /// desired ExtensionPoint using the Type of the extension object.
        /// </remarks>
        public string Path { get; set; }
    }
}
