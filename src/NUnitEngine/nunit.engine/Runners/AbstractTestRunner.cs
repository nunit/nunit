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
using System.ComponentModel;
using NUnit.Engine.Services;

namespace NUnit.Engine.Runners
{
    /// <summary>
    /// AbstractTestRunner is the base class for all runners
    /// within the NUnit Engine. It implements the ITestRunner
    /// interface, which is used by clients of the engine and
    /// uses a Template pattern with abstract methods overridden
    /// by the derived runners.
    /// </summary>
    public abstract class AbstractTestRunner : ITestEngineRunner
    {
        protected const string TEST_RUN_ELEMENT = "test-run";

        public AbstractTestRunner(IServiceLocator services, TestPackage package)
        {
            Services = services;
            TestPackage = package;
            TestRunnerFactory = Services.GetService<ITestRunnerFactory>();
            ProjectService = Services.GetService<IProjectService>();
        }

        #region Properties

        /// <summary>
        /// Our Service Context
        /// </summary>
        protected IServiceLocator Services { get; private set; }

        protected IProjectService ProjectService { get; private set; }

        protected ITestRunnerFactory TestRunnerFactory { get; private set; }

        /// <summary>
        /// The TestPackage for which this is the runner
        /// </summary>
        protected TestPackage TestPackage { get; set; }

        /// <summary>
        /// The result of the last call to LoadPackage
        /// </summary>
        protected TestEngineResult LoadResult { get; set; }

        /// <summary>
        /// Gets an indicator of whether the package has been loaded.
        /// </summary>
        protected bool IsPackageLoaded
        {
            get { return LoadResult != null;  }
        }

        #endregion

        #region Abstract and Virtual Template Methods

        /// <summary>
        /// Loads the TestPackage for exploration or execution.
        /// </summary>
        /// <returns>A TestEngineResult.</returns>
        protected abstract TestEngineResult LoadPackage();

        /// <summary>
        /// Reload the currently loaded test package. Overridden
        /// in derived classes to take any additional action.
        /// </summary>
        /// <returns>A TestEngineResult.</returns>
        protected virtual TestEngineResult ReloadPackage()
        {
            return LoadPackage();
        }

        /// <summary>
        /// Unload any loaded TestPackage. Overridden in
        /// derived classes to take any necessary action.
        /// </summary>
        public virtual void UnloadPackage()
        {
        }

        /// <summary>
        /// Explores a previously loaded TestPackage and returns information
        /// about the tests found.
        /// </summary>
        /// <param name="filter">The TestFilter to be used to select tests</param>
        /// <returns>A TestEngineResult.</returns>
        protected abstract TestEngineResult ExploreTests(TestFilter filter);

        /// <summary>
        /// Count the test cases that would be run under the specified filter.
        /// </summary>
        /// <param name="filter">A TestFilter</param>
        /// <returns>The count of test cases.</returns>
        protected abstract int CountTests(TestFilter filter);

        /// <summary>
        /// Run the tests in the loaded TestPackage.
        /// </summary>
        /// <param name="listener">An ITestEventHandler to receive events</param>
        /// <param name="filter">A TestFilter used to select tests</param>
        /// <returns>A TestEngineResult giving the result of the test execution</returns>
        protected abstract TestEngineResult RunTests(ITestEventListener listener, TestFilter filter);

        /// <summary>
        /// Start a run of the tests in the loaded TestPackage, returning immediately.
        /// The tests are run asynchronously and the listener interface is notified 
        /// as it progresses.
        /// </summary>
        /// <param name="listener">An ITestEventHandler to receive events</param>
        /// <param name="filter">A TestFilter used to select tests</param>
        /// <returns>An <see cref="AsyncTestEngineResult"/> that will provide the result of the test execution</returns>
        protected virtual AsyncTestEngineResult RunTestsAsync(ITestEventListener listener, TestFilter filter)
        {
            var testRun = new AsyncTestEngineResult();

            using (var worker = new BackgroundWorker())
            {
                worker.DoWork += (s, ea) =>
                {
                    var result = RunTests(listener, filter);
                    testRun.SetResult(result);
                };
                worker.RunWorkerAsync();
            }

            return testRun;
        }

        /// <summary>
        /// Cancel the ongoing test run. If no  test is running, the call is ignored.
        /// </summary>
        /// <param name="force">If true, cancel any ongoing test threads, otherwise wait for them to complete.</param>
        public abstract void StopRun(bool force);

        #endregion

        #region ITestEngineRunner Members

        /// <summary>
        /// Explores the TestPackage and returns information about
        /// the tests found. Loads the package if not done previously.
        /// </summary>
        /// <param name="filter">The TestFilter to be used to select tests</param>
        /// <returns>A TestEngineResult.</returns>
        public TestEngineResult Explore(TestFilter filter)
        {
            EnsurePackageIsLoaded();

            return ExploreTests(filter);
        }

        /// <summary>
        /// Loads the TestPackage for exploration or execution, saving the result.
        /// </summary>
        /// <returns>A TestEngineResult.</returns>
        public TestEngineResult Load()
        {
            return LoadResult = LoadPackage();
        }

        /// <summary>
        /// Reload the currently loaded test package, saving the result.
        /// </summary>
        /// <returns>A TestEngineResult.</returns>
        /// <exception cref="InvalidOperationException">If no package has been loaded</exception>
        public TestEngineResult Reload()
        {
            if (this.TestPackage == null)
                throw new InvalidOperationException("MasterTestRunner: Reload called before Load");

            return LoadResult = ReloadPackage();
        }

        /// <summary>
        /// Unload any loaded TestPackage.
        /// </summary>
        public void Unload()
        {
            UnloadPackage();
            LoadResult = null;
        }

        /// <summary>
        /// Count the test cases that would be run under the specified
        /// filter, loading the TestPackage if it is not already loaded.
        /// </summary>
        /// <param name="filter">A TestFilter</param>
        /// <returns>The count of test cases.</returns>
        public int CountTestCases(TestFilter filter)
        {
            EnsurePackageIsLoaded();

            return CountTests(filter);
        }

        /// <summary>
        /// Run the tests in the TestPackage, loading the package
        /// if this has not already been done.
        /// </summary>
        /// <param name="listener">An ITestEventHandler to receive events</param>
        /// <param name="filter">A TestFilter used to select tests</param>
        /// <returns>A TestEngineResult giving the result of the test execution</returns>
        public TestEngineResult Run(ITestEventListener listener, TestFilter filter)
        {
            EnsurePackageIsLoaded();

            return RunTests(listener, filter);
        }

        /// <summary>
        /// Start a run of the tests in the loaded TestPackage. The tests are run
        /// asynchronously and the listener interface is notified as it progresses.
        /// </summary>
        /// <param name="listener">An ITestEventHandler to receive events</param>
        /// <param name="filter">A TestFilter used to select tests</param>
        /// <returns>An <see cref="AsyncTestEngineResult"/> that will provide the result of the test execution</returns>
        public AsyncTestEngineResult RunAsync(ITestEventListener listener, TestFilter filter)
        {
            EnsurePackageIsLoaded();

            return RunTestsAsync(listener, filter);
        }
        
        /// <summary>
        /// Start a run of the tests in the TestPackage. The tests are run
        /// asynchronously and the listener interface is notified as it progresses.
        /// Loads the TestPackage if not already loaded.
        /// </summary>
        /// <param name="listener">An ITestEventHandler to receive events</param>
        /// <param name="filter">A TestFilter used to select tests</param>
        public void StartRun(ITestEventListener listener, TestFilter filter)
        {
            RunAsync(listener, filter);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                    Unload();

                _disposed = true;
            }
        }

        #endregion

        #region Helper Methods

        protected bool IsProjectPackage(TestPackage package)
        {
            return package != null
                && package.FullName != null
                && package.FullName != string.Empty
                && ProjectService.CanLoadFrom(package.FullName);
        }

        private void EnsurePackageIsLoaded()
        {
            if (!IsPackageLoaded)
                LoadResult = LoadPackage();
        }

        #endregion
    }
}
