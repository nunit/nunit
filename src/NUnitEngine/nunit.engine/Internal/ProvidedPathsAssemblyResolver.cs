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
using System.Reflection;
using System.IO;
using System.Diagnostics;

namespace NUnit.Engine.Internal
{
    public class ProvidedPathsAssemblyResolver
    {
        static ILogger log = InternalTrace.GetLogger(typeof(ProvidedPathsAssemblyResolver));

        public ProvidedPathsAssemblyResolver()
        {
            _resolutionPaths = new List<string>();
        }

        public void Install()
        {
            Debug.Assert(AppDomain.CurrentDomain.IsDefaultAppDomain());
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
        }

        public void AddPath(string dirPath)
        {
            if (!_resolutionPaths.Contains(dirPath))
            {
                _resolutionPaths.Add(dirPath);
                log.Debug("Added path " + dirPath);
            }
        }

        public void AddPathFromFile(string filePath)
        {
            string dirPath = Path.GetDirectoryName(filePath);
            AddPath(dirPath);
        }

        public void RemovePath(string dirPath)
        {
            _resolutionPaths.Remove(dirPath);
        }

        public void RemovePathFromFile(string filePath)
        {
            string dirPath = Path.GetDirectoryName(filePath);
            RemovePath(dirPath);
        }

        Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            foreach (string path in _resolutionPaths)
            {
                string filename = new AssemblyName(args.Name).Name + ".dll";
                string fullPath = Path.Combine(path, filename);
                try
                {
                    if (File.Exists(fullPath))
                    {
                        return Assembly.LoadFrom(fullPath);
                    }
                }
                catch (Exception)
                {
                    // Resolution at this path failed. Do not interrupt the process; try the next path.
                }
            }

            return null;
        }

        List<string> _resolutionPaths;
    }
}
