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
    /// TypeExtensionPointAttribute is used to bind an extension point
    /// to a class or interface.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple=true, Inherited=false)]
    public class TypeExtensionPointAttribute : Attribute
    {
        /// <summary>
        /// Construct a TypeExtensionPointAttribute, specifying the path.
        /// </summary>
        /// <param name="path">A unique string identifying the extension point.</param>
        public TypeExtensionPointAttribute(string path)
        {
            Path = path;
        }

        /// <summary>
        /// Construct an TypeExtensionPointAttribute, without specifying the path.
        /// The extension point will use a path constructed based on the interface
        /// or class to which the attribute is applied.
        /// </summary>
        public TypeExtensionPointAttribute()
        {

        }

        /// <summary>
        /// The unique string identifying this ExtensionPoint. This identifier
        /// is typically formatted as a path using '/' and the set of extension 
        /// points is sometimes viewed as forming a tree.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// An optional description of the purpose of the ExtensionPoint
        /// </summary>
        public string Description { get; set; }
    }
}
