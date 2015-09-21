// ***********************************************************************
// Copyright (c) 2011 Charlie Poole
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
    /// The IProjectLoader interface is implemented by any class
    /// that knows how to load projects in a specific format.
    /// </summary>
    [TypeExtensionPoint(
        Description = "Recognizes and loads assemblies from various types of project formats.")]
    public interface IProjectLoader
    {
        /// <summary>
        /// Returns true if the file indicated is one that this
        /// loader knows how to load.
        /// </summary>
        /// <param name="path">The path of the project file</param>
        /// <returns>True if the loader knows how to load this file, otherwise false</returns>
        bool CanLoadFrom( string path );

        /// <summary>
        /// Loads a project of a known format.
        /// </summary>
        /// <param name="path">The path of the project file</param>
        /// <returns>An IProject interface to the loaded project or null if the project cannot be loaded</returns>
        IProject LoadFrom(string path);
    }
}
