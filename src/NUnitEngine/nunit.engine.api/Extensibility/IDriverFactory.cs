// ***********************************************************************
// Copyright (c) 2009-2014 Charlie Poole
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

namespace NUnit.Engine.Extensibility
{
    /// <summary>
    /// Interface implemented by a Type that knows how to create a driver for a test assembly.
    /// </summary>
    public interface IDriverFactory
    {
        /// <summary>
        /// Gets a flag indicating whether a given AssemblyName
        /// represents a test framework supported by this factory.
        /// </summary>
        bool IsSupportedFramework(AssemblyName refAssembly);

        /// <summary>
        /// Gets a driver for a given test assembly and framework
        /// which it is already known to reference.
        /// </summary>
        /// <param name="domain">The domain in which the assembly will be loaded</param>
        /// <param name="frameworkAssemblyName">The name of the test framework reference</param>
        /// <param name="assemblyPath">The path to the test assembly</param>
        /// <param name="settings">A dictionary of settings to be used for this assembly</param>
        /// <returns></returns>
        IFrameworkDriver GetDriver(AppDomain domain, string frameworkAssemblyName, string assemblyPath, IDictionary<string, object> settings);
    }
}
