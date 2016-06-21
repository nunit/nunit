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
    /// The ExtensionAttribute is used to identify a class that is intended
    /// to serve as an extension.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=false)]
    public class ExtensionAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NUnit.Engine.Extensibility.ExtensionAttribute"/> class.
        /// </summary>
        public ExtensionAttribute()
        {
            Enabled = true;
        }

        /// <summary>
        /// A unique string identifying the ExtensionPoint for which this Extension is 
        /// intended. This is an optional field provided NUnit is able to deduce the
        /// ExtensionPoint from the Type of the extension class.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// An optional description of what the extension does.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Flag indicating whether the extension is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        public bool Enabled { get; set; }

        /// <summary>
        /// The minimum Engine version for which this extension is designed
        /// </summary>
        public string EngineVersion { get; set; }
    }
}
