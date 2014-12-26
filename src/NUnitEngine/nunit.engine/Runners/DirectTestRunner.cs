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
using NUnit.Engine.Internal;

namespace NUnit.Engine.Runners
{
    /// <summary>
    /// DirectTestRunner is the abstract base for runners 
    /// that deal directly with a framework driver.
    /// </summary>
    public abstract class DirectTestRunner : AbstractTestRunner
    {
        private readonly List<IFrameworkDriver> _drivers = new List<IFrameworkDriver>();

        public DirectTestRunner(ServiceContext services, TestPackage package) : base(services, package) { }

        #region Properties

        protected AppDomain TestDomain { get; set; }

        #endregion

        #region AbstractTestRunner Overrides

        /// <summary>
        /// Explores a previously loaded TestPackage and returns information
        /// about the tests found.
        /// </summary>
        /// <param name="filter">The TestFilter to be used to select tests</param>
        /// <returns>
        /// A TestEngineResult.
        /// </returns>
        protected override TestEngineResult ExploreTests(TestFilter filter)
        {
            var result = new TestEngineResult();

            foreach (IFrameworkDriver driver in _drivers)
                result.Add(driver.Explore(filter));

            return IsProjectPackage(TestPackage)
                ? result.MakePackageResult(TestPackage.Name, TestPackage.FullName)
                : result;
        }

        /// <summary>
        /// Load a TestPackage for exploration or execution
        /// </summary>
        /// <returns>A TestEngineResult.</returns>
        protected override TestEngineResult LoadPackage()
        {
            var result = new TestEngineResult();

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
        /// Run the tests in the loaded TestPackage.
        /// </summary>
        /// <param name="listener">An ITestEventHandler to receive events</param>
        /// <param name="filter">A TestFilter used to select tests</param>
        /// <returns>
        /// A TestEngineResult giving the result of the test execution
        /// </returns>
        protected override TestEngineResult RunTests(ITestEventListener listener, TestFilter filter)
        {
            var result = new TestEngineResult();

            foreach (IFrameworkDriver driver in _drivers)
                result.Add(driver.Run(listener, filter));

            return IsProjectPackage(TestPackage)
                ? result.MakePackageResult(TestPackage.Name, TestPackage.FullName)
                : result;
        }

        /// <summary>
        /// Cancel the ongoing test run. If no  test is running, the call is ignored.
        /// </summary>
        /// <param name="force">If true, cancel any ongoing test threads, otherwise wait for them to complete.</param>
        public override void StopRun(bool force)
        {
            foreach(var driver in _drivers)
                driver.StopRun(force);
        }

        #endregion
    }
}
