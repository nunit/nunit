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
using System.Collections.Generic;

namespace NUnit.Engine.Runners
{
    /// <summary>
    /// AbstractTestRunner is the base class for all runners
    /// within the NUnit Engine. It implements the ITestRunner
    /// interface, which is used by clients of the engine.
    /// </summary>
    public abstract class AbstractTestRunner : ITestRunner
    {
        protected ServiceContext services;
        protected TestPackage package;

        public AbstractTestRunner(ServiceContext services)
        {
            this.services = services;
        }

        protected ServiceContext Services
        {
            get { return services; }
        }

        #region Abstract and Virtual Methods

        /// <summary>
        /// Explore a TestPackage and return information about
        /// the tests found.
        /// </summary>
        /// <param name="package">The TestPackage to be explored</param>
        /// <returns>A TestEngineResult.</returns>
        public abstract TestEngineResult Explore(TestPackage package);

        /// <summary>
        /// Load a TestPackage for possible execution. This is
        /// the internal implementation returning a TestEngineResult.
        /// </summary>
        /// <param name="package">The TestPackage to be loaded</param>
        /// <returns>A TestEngineResult.</returns>
        public abstract TestEngineResult Load(TestPackage package);

        /// <summary>
        /// Unload any loaded TestPackage. Overridden in
        /// derived classes to take any ncessary action.
        /// </summary>
        public virtual void Unload()
        {
        }

        /// <summary>
        /// Run the tests in a loaded TestPackage. This is the interal
        /// implementation, returning a TestEngineResult.
        /// </summary>
        /// <param name="listener">An ITestEventHandler to receive events</param>
        /// <param name="filter">A TestFilter used to select tests</param>
        /// <returns>A TestEngineResult giving the result of the test execution</returns>
        public abstract TestEngineResult Run(ITestEventHandler listener, ITestFilter filter);

        #endregion

        #region ITestRunner Members

        /// <summary>
        /// Load a TestPackage for possible execution. The 
        /// explicit implemenation returns an ITestEngineResult
        /// for consumption by clients.
        /// </summary>
        /// <param name="package">The TestPackage to be loaded</param>
        /// <returns>An ITestEngineResult.</returns>
        ITestEngineResult ITestRunner.Load(TestPackage package)
        {
            return this.Load(package);
        }

        /// <summary>
        /// Run the tests in a loaded TestPackage. The explicit
        /// implementation returns an ITestEngineResult for use
        /// by external clients.
        /// </summary>
        /// <param name="listener">An ITestEventHandler to receive events</param>
        /// <param name="filter">A TestFilter used to select tests</param>
        /// <returns>An ITestEngineResult giving the result of the test execution</returns>
        ITestEngineResult ITestRunner.Run(ITestEventHandler listener, ITestFilter filter)
        {
            return this.Run(listener, filter);
        }

        /// <summary>
        /// Explore a TestPackage and return information about
        /// the tests found.
        /// </summary>
        /// <param name="package">The TestPackage to be explored</param>
        /// <returns>A TestEngineResult.</returns>
        ITestEngineResult ITestRunner.Explore(TestPackage package)
        {
            return this.Explore(package);
        }

        #endregion

        #region IDisposable Members

        public virtual void Dispose()
        {
            this.Unload();
        }

        #endregion

        #region Helper Methods

        protected TestEngineResult MakePackageResult(IList<TestEngineResult> results)
        {
            if (IsProjectPackage(this.package))
                return TestEngineResult.MakeProjectResult(this.package, results);
            else if (results.Count == 1)
                return results[0];
            else
                return TestEngineResult.Merge(results);
        }

        private bool IsProjectPackage(TestPackage package)
        {
            return package != null
                && package.FullName != null
                && package.FullName != string.Empty
                && services.ProjectService.IsProjectFile(package.FullName);
        }

        #endregion
    }
}
