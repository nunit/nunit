// ***********************************************************************
// Copyright (c) 2008-2014 Charlie Poole
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
using System.Collections;
using System.IO;
using NUnit.Engine.Extensibility;

namespace NUnit.Engine.Services.ProjectLoaders
{
    /// <summary>
    /// Summary description for VSProjectLoader.
    /// </summary>
    [Extension]
    [ExtensionProperty("FileExtension", ".sln")]
    [ExtensionProperty("FileExtension", ".csproj")]
    [ExtensionProperty("FileExtension", ".vbproj")]
    [ExtensionProperty("FileExtension", ".vjsproj")]
    [ExtensionProperty("FileExtension", ".vcproj")]
    [ExtensionProperty("FileExtension", ".fsproj")]
    public class VisualStudioProjectLoader : IProjectLoader
    {
        #region IProjectLoader Members

        public bool CanLoadFrom(string path)
        {
            return VSProject.IsProjectFile(path)|| VSProject.IsSolutionFile(path);
        }

        public IProject LoadFrom(string path)
        {
            if (VSProject.IsProjectFile(path))
                return new VSProject(path);

            if (VSProject.IsSolutionFile(path))
                return new VSSolution(path);

            return null;
        }

        #endregion
    }
}
