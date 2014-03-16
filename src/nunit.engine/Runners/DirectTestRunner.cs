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
    /// DirectTestRunner is the abstract base for runners 
    /// that deal directly with a framework driver.
    /// </summary>
    public abstract class DirectTestRunner : AbstractTestRunner
    {
        private List<IFrameworkDriver> drivers = new List<IFrameworkDriver>();
        protected AppDomain TestDomain;

        public DirectTestRunner(ServiceContext services) : base(services) { }

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

            foreach (IFrameworkDriver driver in drivers)
                results.Add(new TestEngineResult(driver.Explore(filter)));

            return MakePackageResult(results);
        }

        /// <summary>
        /// Load a TestPackage for exploration or execution
        /// </summary>
        /// <param name="package">The TestPackage to be loaded</param>
        /// <returns>A TestEngineResult.</returns>
        public override TestEngineResult Load(TestPackage package)
        {
            this.package = package;
            List<TestEngineResult> loadResults = new List<TestEngineResult>();

            foreach (string testFile in package.TestFiles)
            {
                IFrameworkDriver driver = Services.DriverFactory.GetDriver(TestDomain, testFile, package.Settings);
                TestEngineResult driverResult = new TestEngineResult(driver.Load());

                loadResults.Add(driverResult);
                drivers.Add(driver);
            }

            return MakePackageResult(loadResults);
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

            foreach (IFrameworkDriver driver in drivers)
                count += driver.CountTestCases(filter);

            return count;
        }

        /// <summary>
        /// Run the tests in a loaded TestPackage
        /// </summary>
        /// <param name="filter">A TestFilter used to select tests</param>
        /// <returns>
        /// A TestEngineResult giving the result of the test execution. The 
        /// top-level node of the result is &lt;direct-runner&gt; and wraps
        /// all the &lt;test-assembly&gt; elements returned by the drivers.
        /// </returns>
        public override TestEngineResult Run(ITestEventHandler listener, TestFilter filter)
        {
            List<TestEngineResult> results = new List<TestEngineResult>();

            foreach (NUnitFrameworkDriver driver in drivers)
                results.Add(new TestEngineResult(driver.Run(listener, filter)));

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
            _listener = listener;
            _filter = filter;
            var threadStart = new System.Threading.ThreadStart(RunnerProc);
            System.Threading.Thread runnerThread = new System.Threading.Thread(threadStart);
            runnerThread.Start();
        }

        private ITestEventHandler _listener;
        private TestFilter _filter;
        private void RunnerProc()
        {
            foreach (NUnitFrameworkDriver driver in drivers)
                driver.Run(_listener, _filter);
        }

        #endregion
    }
}
