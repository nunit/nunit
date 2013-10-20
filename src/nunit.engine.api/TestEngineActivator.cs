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
    /// <summary>
    /// TestEngineActivator creates an instance of the test
    /// engine and returns an ITestEngine interface.
    /// </summary>
    public static class TestEngineActivator
    {
        private static readonly string DefaultAssemblyName = "nunit.engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=96d09a1eb7f44a77, processorArchitecture=MSIL";
        private static readonly string DefaultTypeName = "NUnit.Engine.TestEngine";

        #region Public Methods

        /// <summary>
        /// Create an instance of the test engine using default values
        /// for the assembly and type names.
        /// </summary>
        /// <returns>An ITestEngine.</returns>
        public static ITestEngine CreateInstance(string workDirectory, InternalTraceLevel traceLevel)
        {
            return CreateInstance(DefaultAssemblyName, DefaultTypeName, workDirectory, traceLevel);
        }

        /// <summary>
        /// Create an instance of the test engine using provided values
        /// for the assembly and type names. This method is intended
        /// for use in experimenting with alternate implementations.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly to be used.</param>
        /// <param name="typeName">The name of the Type to be used.</param>
        /// <returns>An ITestEngine.</returns>
        public static ITestEngine CreateInstance(string assemblyName, string typeName, string workDirectory, InternalTraceLevel traceLevel)
        {
            try
            {
                ITestEngine engine = (ITestEngine)
                    AppDomain.CurrentDomain.CreateInstanceAndUnwrap(assemblyName, typeName);
                engine.InitializeServices(workDirectory, traceLevel);
                return engine;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to load the test engine", ex);
            }
        }

        #endregion
    }
}
