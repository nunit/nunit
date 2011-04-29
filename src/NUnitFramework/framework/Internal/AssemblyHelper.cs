// ***********************************************************************
// Copyright (c) 2008 Charlie Poole
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

#if !NETCF
using System;
using System.Reflection;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// AssemblyHelper provides static methods for working 
    /// with assemblies.
    /// </summary>
    public class AssemblyHelper
    {
        #region GetAssemblyPath

        /// <summary>
        /// Gets the path from which an assembly was loaded.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The path.</returns>
        public static string GetAssemblyPath(Assembly assembly)
        {
            string path = assembly.CodeBase;
            Uri uri = new Uri(path);

            // If it wasn't loaded locally, use the Location
            if (!uri.IsFile)
                return assembly.Location;

            if (uri.IsUnc)
                return path.Substring(Uri.UriSchemeFile.Length + 1);


            int start = Uri.UriSchemeFile.Length + Uri.SchemeDelimiter.Length;

            if (path[start] == '/' && path[start + 2] == ':')
                ++start;

            return path.Substring(start);
        }

        #endregion

        #region GetDirectoryName

        /// <summary>
        /// Gets the path to the directory from which an assembly was loaded.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The path.</returns>
        public static string GetDirectoryName( Assembly assembly )
        {
            return System.IO.Path.GetDirectoryName(GetAssemblyPath(assembly));
        }

        #endregion
    }
}
#endif
