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
using System.Text;
using System.Xml;
using NUnit.Engine.Internal;

namespace NUnit.Engine.Runners
{
    /// <summary>
    /// AggregatingTestRunner runs tests using multiple
    /// subordinate runners and combines the results.
    /// </summary>
    public class AggregatingTestRunner : AbstractTestRunner
    {
        // The runners created by the derived class will (at least at the time
        // of writing this comment) be either TestDomainRunners or ProcessRunners.
        private List<ITestEngineRunner> _runners = new List<ITestEngineRunner>();

        public AggregatingTestRunner(ServiceContext services, TestPackage package) : base(services, package) { }

        #region AbstractTestRunner Overrides

        /// <summary>
        /// Explore a TestPackage and return information about
        /// the tests found.
        /// </summary>
        /// <param name="package">The TestPackage to be explored</param>
        /// <returns>A TestEngineResult.</returns>
        protected override TestEngineResult ExploreTests(TestFilter filter)
        {
            List<TestEngineResult> results = new List<TestEngineResult>();

            foreach (ITestEngineRunner runner in _runners)
                results.Add(runner.Explore(filter));

            TestEngineResult result = ResultHelper.Merge(results);

            return IsProjectPackage(this.TestPackage)
                ? result.MakePackageResult(TestPackage.Name, TestPackage.FullName)
                : result;
        }

        /// <summary>
        /// Load a TestPackage for possible execution
        /// </summary>
        /// <param name="package">The TestPackage to be loaded</param>
        /// <returns>A TestEngineResult.</returns>
        protected override TestEngineResult LoadPackage()
        {
            List<TestPackage> packages = new List<TestPackage>();

            foreach (string testFile in TestPackage.TestFiles)
            {
                TestPackage subPackage = new TestPackage(testFile);
                if (Services.ProjectService.IsProjectFile(testFile))
                    Services.ProjectService.ExpandProjectPackage(subPackage);
                foreach (string key in TestPackage.Settings.Keys)
                    subPackage.Settings[key] = TestPackage.Settings[key];
                packages.Add(subPackage);
            }

            List<TestEngineResult> results = new List<TestEngineResult>();

            foreach (TestPackage subPackage in packages)
            {
                var runner = CreateRunner(subPackage);
                _runners.Add(runner);
                results.Add(runner.Load());
            }

            return ResultHelper.Merge(results);
        }

        /// <summary>
        /// Unload any loaded TestPackages and clear the
        /// list of runners.
        /// </summary>
        public override void UnloadPackage()
        {
            foreach (ITestEngineRunner runner in _runners)
                runner.Unload();

            _runners.Clear();
        }

        /// <summary>
        /// Count the test cases that would be run under
        /// the specified filter.
        /// </summary>
        /// <param name="filter">A TestFilter</param>
        /// <returns>The count of test cases</returns>
        protected override int CountTests(TestFilter filter)
        {
            int count = 0;

            foreach (ITestEngineRunner runner in _runners)
                count += runner.CountTestCases(filter);

            return count;
        }

        /// <summary>
        /// Run the tests in a loaded TestPackage
        /// </summary>
        /// <param name="filter">A TestFilter used to select tests</param>
        /// <returns>
        /// A TestEngineResult giving the result of the test execution.
        /// </returns>
        protected override TestEngineResult RunTests(ITestEventListener listener, TestFilter filter)
        {
            List<TestEngineResult> results = new List<TestEngineResult>();

            foreach (ITestEngineRunner runner in _runners)
                results.Add(runner.Run(listener, filter));

            TestEngineResult result = ResultHelper.Merge(results);

            return IsProjectPackage(this.TestPackage)
                ? result.MakePackageResult(TestPackage.Name, TestPackage.FullName)
                : result;
        }

        /// <summary>
        /// Cancel the ongoing test run. If no  test is running, the call is ignored.
        /// </summary>
        /// <param name="force">If true, cancel any ongoing test threads, otherwise wait for them to complete.</param>
        public override void StopRun(bool force)
        {
            foreach (var runner in _runners)
                runner.StopRun(force);
        }

        #endregion

        protected virtual ITestEngineRunner CreateRunner(TestPackage package)
        {
            return Services.TestRunnerFactory.MakeTestRunner(package);
        }
    }
}
