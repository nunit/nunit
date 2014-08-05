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

namespace NUnit.Engine
{
    // This is a preliminary implementation, which requires a specific version of the engine and only
    // loads it if it can be found in the current AppBase and ProbingPath or in the GAC.
    //
    // TODO: Find the engine in established locations known to NUnit or stored in the registry.
    //
    // TODO: Find the best available version of the engine.

    /// <summary>
    /// TestEngineActivator creates an instance of the test engine and returns an ITestEngine interface.
    /// </summary>
    public static class TestEngineActivator
    {
        private const string DefaultAssemblyName = "nunit.engine, Version=1.0.0.0, Culture=neutral, processorArchitecture=MSIL";
        private const string DefaultTypeName = "NUnit.Engine.TestEngine";

        #region Public Methods

        /// <summary>
        /// Create an instance of the test engine using default values for the assembly and type names.
        /// </summary>
        /// <returns>An ITestEngine.</returns>
        public static ITestEngine CreateInstance()
        {
            return CreateInstance(DefaultAssemblyName, DefaultTypeName);
        }

        /// <summary>
        /// Create an instance of the test engine using provided values for the assembly and type names.
        /// This method is intended for use in experimenting with alternative implementations.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly to be used.</param>
        /// <param name="typeName">The name of the Type to be used.</param>
        /// <returns>An ITestEngine.</returns>
        public static ITestEngine CreateInstance(string assemblyName, string typeName)
        {
            try
            {
                return (ITestEngine)AppDomain.CurrentDomain.CreateInstanceAndUnwrap(assemblyName, typeName);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to load the test engine", ex);
            }
        }

        #endregion
    }
}
