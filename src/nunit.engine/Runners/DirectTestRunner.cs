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
    /// DirectTestRunner is the abstract base for runners 
    /// that deal directly with a framework driver.
    /// </summary>
    public abstract class DirectTestRunner : AbstractTestRunner
    {
        private List<IFrameworkDriver> _drivers = new List<IFrameworkDriver>();

        public DirectTestRunner(ServiceContext services, TestPackage package) : base(services, package) { }

        #region Properties

        protected AppDomain TestDomain { get; set; }

        #endregion

        #region AbstractTestRunner Overrides

        /// <summary>
        /// Explore a TestPackage and return information about
        /// the tests found.
        /// </summary>
        /// <param name="package">The TestPackage to be explored</param>
        /// <returns>A TestEngineResult.</returns>
        protected override TestEngineResult ExploreTests(TestFilter filter)
        {
            TestEngineResult result = new TestEngineResult();

            foreach (IFrameworkDriver driver in _drivers)
                result.Add(driver.Explore(filter));

            return IsProjectPackage(this.TestPackage)
                ? result.MakePackageResult(TestPackage.Name, TestPackage.FullName)
                : result;
        }

        /// <summary>
        /// Load a TestPackage for exploration or execution
        /// </summary>
        /// <param name="package">The TestPackage to be loaded</param>
        /// <returns>A TestEngineResult.</returns>
        protected override TestEngineResult LoadPackage()
        {
            TestEngineResult result = new TestEngineResult();

            foreach (string testFile in TestPackage.TestFiles)
            {
                IFrameworkDriver driver = Services.DriverFactory.GetDriver(TestDomain, testFile, TestPackage.Settings);
                result.Add(driver.Load());
                _drivers.Add(driver);
            }

            return IsProjectPackage(TestPackage)
                ? result.MakePackageResult(TestPackage.Name, TestPackage.FullName)
                : result;
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

            foreach (IFrameworkDriver driver in _drivers)
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
        protected override TestEngineResult RunTests(ITestEventListener listener, TestFilter filter)
        {
            TestEngineResult result = new TestEngineResult();

            foreach (IFrameworkDriver driver in _drivers)
                result.Add(driver.Run(listener, filter));

            return IsProjectPackage(this.TestPackage)
                ? result.MakePackageResult(TestPackage.Name, TestPackage.FullName)
                : result;
        }

        /// <summary>
        /// Start a run of the tests in the loaded TestPackage. The tests are run
        /// asynchronously and the listener interface is notified as it progresses.
        /// </summary>
        /// <param name="listener">An ITestEventHandler to receive events</param>
        /// <param name="filter">A TestFilter used to select tests</param>
        protected override void RunTestsAsynchronously(ITestEventListener listener, TestFilter filter)
        {
            _listener = listener;
            _filter = filter;
            var threadStart = new System.Threading.ThreadStart(RunnerProc);
            System.Threading.Thread runnerThread = new System.Threading.Thread(threadStart);
            runnerThread.Start();
        }

        private ITestEventListener _listener;
        private TestFilter _filter;
        private void RunnerProc()
        {
            foreach (NUnitFrameworkDriver driver in _drivers)
                driver.Run(_listener, _filter);

            // TODO: Add something to indicate commpletion?
            //_listener.OnTestEvent("WHAT SHOULD GO HERE?");
        }

        /// <summary>
        /// Cancel the ongoing test run. If no test is running
        /// the call is ignored.
        /// </summary>
        public override void CancelRun()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
