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
using System.Collections.Generic;
using System.Xml;

namespace NUnit.Engine.Runners
{
    using Internal;

    /// <summary>
    /// AbstractTestRunner is the base class for all runners
    /// within the NUnit Engine. It implements the ITestRunner
    /// interface, which is used by clients of the engine.
    /// </summary>
    public abstract class AbstractTestRunner : ITestEngineRunner
    {
        protected const string TEST_RUN_ELEMENT = "test-run";

        public AbstractTestRunner(ServiceContext services)
        {
            this.Services = services;
        }

        #region Properties

        protected ServiceContext Services { get; private set; }

        protected TestPackage TestPackage { get; set; }

        #endregion

        #region ITestEngineRunner Members

        /// <summary>
        /// Explore a loaded TestPackage and return information about
        /// the tests found.
        /// </summary>
        /// <param name="package">The TestPackage to be explored</param>
        /// <param name="filter">The TestFilter to be used to select tests</param>
        /// <returns>A TestEngineResult.</returns>
        public abstract TestEngineResult Explore(TestFilter filter);

        /// <summary>
        /// Load a TestPackage for possible execution. This is
        /// the internal implementation returning a TestEngineResult.
        /// </summary>
        /// <param name="package">The TestPackage to be loaded</param>
        /// <returns>A TestEngineResult.</returns>
        public abstract TestEngineResult Load(TestPackage package);

        /// <summary>
        /// Reload the currently loaded test package.
        /// </summary>
        /// <returns>A TestEngineResult.</returns>
        /// <exception cref="InvalidOperationException">If no package has been loaded</exception>
        public virtual TestEngineResult Reload()
        {
            if (this.TestPackage == null)
                throw new InvalidOperationException("MasterTestRunner: Reload called before Load");

            return Load(TestPackage);
        }

        /// <summary>
        /// Unload any loaded TestPackage. Overridden in
        /// derived classes to take any necessary action.
        /// </summary>
        public virtual void Unload()
        {
        }

        /// <summary>
        /// Count the test cases that would be run under
        /// the specified filter.
        /// </summary>
        /// <param name="filter">A TestFilter</param>
        /// <returns>The count of test cases</returns>
        public abstract int CountTestCases(TestFilter filter);

        /// <summary>
        /// Run the tests in a loaded TestPackage. This is the interal
        /// implementation, returning a TestEngineResult.
        /// </summary>
        /// <param name="listener">An ITestEventHandler to receive events</param>
        /// <param name="filter">A TestFilter used to select tests</param>
        /// <returns>A TestEngineResult giving the result of the test execution</returns>
        public abstract TestEngineResult Run(ITestEventHandler listener, TestFilter filter);

        /// <summary>
        /// Start a run of the tests in the loaded TestPackage. The tests are run
        /// asynchronously and the listener interface is notified as it progresses.
        /// </summary>
        /// <param name="listener">An ITestEventHandler to receive events</param>
        /// <param name="filter">A TestFilter used to select tests</param>
        public abstract void BeginRun(ITestEventHandler listener, TestFilter filter);

        /// <summary>
        /// Cancel the ongoing test run. If no  test is running,
        /// the call is ignored.
        /// </summary>
        public abstract void CancelRun();

        #endregion

        #region IDisposable Members

        public virtual void Dispose()
        {
            this.Unload();
        }

        #endregion

        #region Helper Methods

        protected bool IsProjectPackage(TestPackage package)
        {
            return package != null
                && package.FullName != null
                && package.FullName != string.Empty
                && Services.ProjectService.IsProjectFile(package.FullName);
        }

        #endregion
    }
}
