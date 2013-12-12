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
        private List<ITestRunner> runners = new List<ITestRunner>();

        public AggregatingTestRunner(ServiceContext services) : base(services) { }

        #region AbstractTestRunner Overrides

        /// <summary>
        /// Explore a TestPackage and return information about
        /// the tests found.
        /// </summary>
        /// <param name="package">The TestPackage to be explored</param>
        /// <returns>A TestEngineResult.</returns>
        public override TestEngineResult Explore(TestFilter filter)
        {
            List<TestEngineResult> results = new List<TestEngineResult>();

            // TODO: Eliminate need for implicit cast to AbstractTestRunner
            foreach (AbstractTestRunner runner in runners)
                results.Add(runner.Explore(filter));

            return MakePackageResult(results);
        }

        /// <summary>
        /// Load a TestPackage for possible execution
        /// </summary>
        /// <param name="package">The TestPackage to be loaded</param>
        /// <returns>A TestEngineResult.</returns>
        public override TestEngineResult Load(TestPackage package)
        {
            this.package = package;

            List<TestPackage> packages = new List<TestPackage>();

            foreach (string testFile in package.TestFiles)
            {
                TestPackage subPackage = new TestPackage(testFile);
                if (Services.ProjectService.IsProjectFile(testFile))
                    Services.ProjectService.ExpandProjectPackage(subPackage);
                foreach (string key in package.Settings.Keys)
                    subPackage.Settings[key] = package.Settings[key];
                packages.Add(subPackage);
            }

            List<TestEngineResult> results = new List<TestEngineResult>();

            foreach (TestPackage subPackage in packages)
            {
                //foreach (string key in package.Settings.Keys)
                //    subPackage.Settings[key] = package.Settings[key];

                AbstractTestRunner runner = CreateRunner(subPackage);
                runners.Add(runner);
                results.Add(runner.Load(subPackage));
            }

            return TestEngineResult.Wrap("load", results);
        }

        /// <summary>
        /// Unload any loaded TestPackages and clear the
        /// list of runners.
        /// </summary>
        public override void Unload()
        {
            foreach (ITestRunner runner in runners)
                runner.Unload();

            runners.Clear();
        }

        /// <summary>
        /// Count the test cases that would be run under
        /// the specified filter.
        /// </summary>
        /// <param name="filter">A TestFilter</param>
        /// <returns>The count of test cases</returns>
        public override int CountTestCases(TestFilter filter)
        {
            int count = 0;

            foreach (ITestRunner runner in runners)
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
        public override TestEngineResult Run(ITestEventHandler listener, TestFilter filter)
        {
            List<TestEngineResult> results = new List<TestEngineResult>();

            // TODO: Eliminate need for implicit cast to AbstractTestRunner
            foreach (AbstractTestRunner runner in runners)
                results.Add(runner.Run(listener, filter));

            return MakePackageResult(results);
        }

        /// <summary>
        /// Start a run of the tests in the loaded TestPackage. The tests are run
        /// asynchronously and the listener interface is notified as it progresses.
        /// </summary>
        /// <param name="listener">An ITestEventHandler to receive events</param>
        /// <param name="filter">A TestFilter used to select tests</param>
        public override void BeginRun(ITestEventHandler listener, TestFilter filter)
        {
            throw new NotImplementedException();
        }

        #endregion

        protected virtual AbstractTestRunner CreateRunner(TestPackage package)
        {
            return Services.TestRunnerFactory.MakeTestRunner(package) as AbstractTestRunner;
        }
    }
}
