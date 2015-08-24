// ***********************************************************************
// Copyright (c) 2008-2015 Charlie Poole
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
    /// ExtensionPoint represents a single point in the TestEngine
    /// that may be extended by user addins and extensions.
    /// </summary>
    public class ExtensionPoint
    {
        /// <summary>
        /// Construct an ExtensionPoint
        /// </summary>
        /// <param name="path">String that uniquely identifies the extension point.</param>
        /// <param name="type">Required type of any extension object.</param>
        public ExtensionPoint(string path, Type type)
        {
            Path = path;
            Type = type;
            Extensions = new List<ExtensionNode>();
        }

        /// <summary>
        /// Gets the unique path identifying this extension point.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Gets the Type of any extension object to be installed at this extension point.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Gets and sets the optional description of this extension point.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets a list of ExtensionNodes for extensions installed on this extension point.
        /// </summary>
        public List<ExtensionNode> Extensions { get; private set; }

        /// <summary>
        /// Install an extension at this extension point. If the
        /// extension node does not meet the requirements for
        /// this extension point, an exception is thrown.
        /// </summary>
        public void Install(ExtensionNode node)
        {
            if (node.Path != Path)
            {
                string msg = string.Format("Non-matching extension path. Expected {0} but got {1}.", Path, node.Path);
                throw new NUnitEngineException(msg);
            }

            // TODO: Verify that the type is correct using Cecil or Reflection 
            // depending on whether the assembly is pre-loaded. For now, it's not 
            // simple to verify the type without loading the extension, so we
            // let it throw at the time the object is accessed.

            Extensions.Add(node);
        }

        /// <summary>
        /// Removes an extension from this extension point. If the
        /// extension object is not present, the method returns
        /// without error.
        /// </summary>
        public void Remove(ExtensionNode extension)
        {
            Extensions.Remove(extension);
        }
    }
}
