// ***********************************************************************
// Copyright (c) 2011-2014 Charlie Poole
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
using System.Xml;

namespace NUnit.Engine
{
    /// <summary>
    /// ITestEngine represents an instance of the test engine.
    /// Clients wanting to discover, explore or run tests start
    /// require an instance of the engine, which is generally 
    /// acquired from the TestEngineActivator class.
    /// </summary>
    public interface ITestEngine : IDisposable
    {
        /// <summary>
        /// Gets the IServiceLocator interface, which gives access to
        /// certain services provided by the engine.
        /// </summary>
        IServiceLocator Services { get; }

        /// <summary>
        /// Create and initialize the standard set of services
        /// used in the Engine. This interface is not normally
        /// called by user code. Programs linking only to 
        /// only to the nunit.engine.api assembly are given a
        /// pre-initialized instance of TestEngine. Programs 
        /// that link directly to nunit.engine usually do so
        /// in order to perform custom initialization.
        /// </summary>
        /// <param name="workDirectory">The work directory currently in use</param>
        /// <param name="traceLevel">The level of internal tracing</param>
        void InitializeServices(string workDirectory, InternalTraceLevel traceLevel);

        /// <summary>
        /// Returns a test runner instance for use by clients in discovering,
        /// exploring and exeuting tests.
        /// </summary>
        /// <param name="package">The TestPackage for which the runner is intended.</param>
        /// <returns>An ITestRunner.</returns>
        ITestRunner GetRunner(TestPackage package);
    }
}
