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
    using Internal;

    /// <summary>
    /// AbstractTestRunner is the base class for all runners
    /// within the NUnit Engine. It implements the ITestRunner
    /// interface, which is used by clients of the engine.
    /// </summary>
    public abstract class AbstractTestRunner : ITestRunner
    {
        protected const string TEST_RUN_ELEMENT = "test-run";
        private const string TEST_SUITE_ELEMENT = "test-suite";
        private const string PROJECT_SUITE_TYPE = "Project";

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
        ITestEngineResult ITestRunner.Run(ITestEventHandler listener, TestFilter filter)
        {
            return this.Run(listener, filter);
        }

        /// <summary>
        /// Explore a loaded TestPackage and return information about
        /// the tests found.
        /// </summary>
        /// <param name="package">The TestPackage to be explored</param>
        /// <returns>A TestEngineResult.</returns>
        ITestEngineResult ITestRunner.Explore(TestFilter filter)
        {
            return this.Explore(filter);
        }

        /// <summary>
        /// Start a run of the tests in the loaded TestPackage. The tests are run
        /// asynchronously and the listener interface is notified as it progresses.
        /// </summary>
        /// <param name="listener">An ITestEventHandler to receive events</param>
        /// <param name="filter">A TestFilter used to select tests</param>
        public abstract void BeginRun(ITestEventHandler listener, TestFilter filter);

        #endregion

        #region IDisposable Members

        public virtual void Dispose()
        {
            this.Unload();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Create the proper type of result for a TestPackage, depending on the 
        /// nature of the package and the number of results. If the package 
        /// represents an NUnit project, then we should wrap the individual
        /// assembly results in a project element, even if there is just one
        /// assembly. Otherwise, we simply merge multiple results into a single
        /// result for later aggregation by the caller. Delaying aggregation
        /// ensures that we don't re-create the XmlNodes multiple times as the
        /// results are serialized from one AppDomain or Process to another.
        /// </summary>
        protected TestEngineResult MakePackageResult(IList<TestEngineResult> results)
        {
            if (IsProjectPackage(this.package))
                return ResultHelper.Merge(results).Aggregate(TEST_SUITE_ELEMENT, PROJECT_SUITE_TYPE, package.Name, package.FullName);
            else if (results.Count == 1)
                return results[0];
            else
                return ResultHelper.Merge(results);
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
