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
using System.Collections.Generic;

namespace NUnit.Engine.Extensibility
{
    /// <summary>
    /// The ExtensionNode class represents a single extension being installed
    /// on a particular extension point. It stores information needed to
    /// activate the extension object on a just-in-time basis.
    /// </summary>
    public class ExtensionNode
    {
        private object _extensionObject;
        private Dictionary<string, List<string>> _properties = new Dictionary<string, List<string>>();


        /// <summary>
        /// Construct an ExtensionNode
        /// </summary>
        /// <param name="assemblyPath">The path to the assembly where this extension is found.</param>
        /// <param name="typeName">The full name of the Type of the extension object.</param>
        public ExtensionNode(string assemblyPath, string typeName)
        {
            AssemblyPath = assemblyPath;
            TypeName = typeName;
			Enabled = true; // By default
        }

        #region Properties

        /// <summary>
        /// Gets the path to the assembly where the extension is defined.
        /// </summary>
        public string AssemblyPath { get; private set; }

        /// <summary>
        /// Gets the full name of the Type of the assembly object.
        /// </summary>
        public string TypeName { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="NUnit.Engine.Extensibility.ExtensionNode"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        public bool Enabled	{ get; set; }

        /// <summary>
        /// Gets an object of the specified extension type, loading the Assembly
        /// and creating the object as needed. Note that this property always
        /// returns the same object. Use CreateExtensionObject if a new one is
        /// needed each time or to specify arguments.
        /// </summary>
        public object ExtensionObject
        {
            get
            {
                if (_extensionObject == null)
                    _extensionObject = CreateExtensionObject();

                return _extensionObject;
            }
        }

        /// <summary>
        /// Gets and sets the unique string identifying the ExtensionPoint for which 
        /// this Extension is intended. This identifier may be supplied by the attribute
        /// marking the extension or deduced by NUnit from the Type of the extension class.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// An optional description of what the extension does.
        /// </summary>
        public string Description { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a newly created extension object, created in the domain specified
        /// </summary>
        public object CreateExtensionObject(params object[] args)
        {
            return AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AssemblyPath, TypeName, false, 0, null, args, null, null, null);
        }

        public void AddProperty(string name, string val)
        {
            if (_properties.ContainsKey(name))
                _properties[name].Add(val);
            else
            {
                var list = new List<string>();
                list.Add(val);
                _properties.Add(name, list);
            }
        }

        public IEnumerable<string> GetProperties(string name)
        {
            return _properties.ContainsKey(name) ? _properties[name] : new List<string>();
        }

        public string GetProperty(string name)
        {
            return _properties.ContainsKey(name) ? _properties[name][0] : null;
        }

        #endregion
    }
}
