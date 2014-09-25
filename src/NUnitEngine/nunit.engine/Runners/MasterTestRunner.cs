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
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Xml;
using NUnit.Common;
using NUnit.Engine.Internal;

namespace NUnit.Engine.Runners
{
    public class MasterTestRunner : AbstractTestRunner, ITestRunner
    {
        private ITestEngineRunner _realRunner;

        // Count of assemblies and projects passed in package
        private int _assemblyCount;
        private int _projectCount;

        public MasterTestRunner(ServiceContext services, TestPackage package) : base(services, package) { }

        public bool IsTestRunning { get; private set; }

        #region AbstractTestRunner Overrides

        /// <summary>
        /// Explore a loaded TestPackage and return information about
        /// the tests found.
        /// </summary>
        /// <param name="package">The TestPackage to be explored</param>
        /// <returns>A TestEngineResult.</returns>
        protected override TestEngineResult ExploreTests(TestFilter filter)
        {
            return _realRunner.Explore(filter).Aggregate(TEST_RUN_ELEMENT, TestPackage.Name, TestPackage.FullName);
        }

        /// <summary>
        /// Load a TestPackage for possible execution
        /// </summary>
        /// <param name="package">The TestPackage to be loaded</param>
        /// <returns>A TestEngineResult.</returns>
        protected override TestEngineResult LoadPackage()
        {
            PerformPackageSetup(TestPackage);
            return _realRunner.Load().Aggregate(TEST_RUN_ELEMENT, TestPackage.Name, TestPackage.FullName);
        }

        /// <summary>
        /// Unload any loaded TestPackage.
        /// </summary>
        public override void UnloadPackage()
        {
            if (_realRunner != null)
                _realRunner.Unload();
        }

        /// <summary>
        /// Count the test cases that would be run under
        /// the specified filter.
        /// </summary>
        /// <param name="filter">A TestFilter</param>
        /// <returns>The count of test cases</returns>
        protected override int CountTests(TestFilter filter)
        {
            return _realRunner.CountTestCases(filter);
        }

        /// <summary>
        /// Run the tests in the loaded TestPackage and return a test result. The tests
        /// are run synchronously and the listener interface is notified as it progresses.
        /// </summary>
        /// <param name="listener">An ITestEventHandler to receive events</param>
        /// <param name="filter">A TestFilter used to select tests</param>
        /// <returns>A TestEngineResult giving the result of the test execution</returns>
        protected override TestEngineResult RunTests(ITestEventListener listener, TestFilter filter)
        {
            IsTestRunning = true;

            if (listener != null)
                listener.OnTestEvent(string.Format("<start-run count='{0}'/>", CountTestCases(filter)));

            DateTime startTime = DateTime.UtcNow;
            long startTicks = Stopwatch.GetTimestamp();

            TestEngineResult result = _realRunner.Run(listener, filter).Aggregate("test-run", TestPackage.Name, TestPackage.FullName);

            result.Xml.InsertEnvironmentElement();

            double duration = (double)(Stopwatch.GetTimestamp() - startTicks) / Stopwatch.Frequency;
            result.Xml.AddAttribute("start-time", XmlConvert.ToString(startTime, "u"));
            result.Xml.AddAttribute("end-time", XmlConvert.ToString(DateTime.UtcNow, "u"));
            result.Xml.AddAttribute("duration", duration.ToString("0.000000", NumberFormatInfo.InvariantInfo));

            IsTestRunning = false;

            if (listener != null)
                listener.OnTestEvent(result.Xml.OuterXml);

            return result;
        }

        /// <summary>
        /// Cancel the ongoing test run. If no  test is running, the call is ignored.
        /// </summary>
        /// <param name="force">If true, cancel any ongoing test threads, otherwise wait for them to complete.</param>
        public override void StopRun(bool force)
        {
            _realRunner.StopRun(force);
        }

        #endregion

        #region ITestRunner Explicit Implementation

        // NOTE: Only those methods which differ from those in
        // ITestEngineRunner have an explicit implementation. 
        // Methods that are the same for both interfaces
        // use the class methods.

        /// <summary>
        /// Load a TestPackage for possible execution. The 
        /// explicit implemenation returns an ITestEngineResult
        /// for consumption by clients.
        /// </summary>
        /// <param name="package">The TestPackage to be loaded</param>
        /// <returns>An XmlNode representing the loaded assembly.</returns>
        XmlNode ITestRunner.Load()
        {
            return this.Load().Xml;
        }

        /// <summary>
        /// Reload the currently loaded test jpackage.
        /// </summary>
        /// <returns>An XmlNode representing the loaded package</returns>
        /// <exception cref="InvalidOperationException">If no package has been loaded</exception>
        XmlNode ITestRunner.Reload()
        {
            return this.Reload().Xml;
        }

        /// <summary>
        /// Run the tests in a loaded TestPackage. The explicit
        /// implementation returns an ITestEngineResult for use
        /// by external clients.
        /// </summary>
        /// <param name="listener">An ITestEventHandler to receive events</param>
        /// <param name="filter">A TestFilter used to select tests</param>
        /// <returns>An XmlNode giving the result of the test execution</returns>
        XmlNode ITestRunner.Run(ITestEventListener listener, TestFilter filter)
        {
            return this.Run(listener, filter).Xml;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        ITestRun ITestRunner.RunAsync(ITestEventListener listener, TestFilter filter)
        {
            var testRun = new TestRun(this);
            testRun.Start(listener, filter);
            return testRun;
        }

        /// <summary>
        /// Explore a loaded TestPackage and return information about
        /// the tests found.
        /// </summary>
        /// <param name="package">The TestPackage to be explored</param>
        /// <returns>An XmlNode representing the tests found.</returns>
        XmlNode ITestRunner.Explore(TestFilter filter)
        {
            return this.Explore(filter).Xml;
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Dispose of this object.
        /// </summary>
        public override void Dispose()
        {
            if (_realRunner != null)
                _realRunner.Dispose();
        }

        #endregion

        #region HelperMethods

        private void PerformPackageSetup(TestPackage package)
        {
            this.TestPackage = package;

            // Expand projects, updating the count of projects and assemblies
            ExpandProjects();

            // If there is more than one project or a mix of assemblies and 
            // projects, AggregatingTestRunner will call MakeTestRunner for
            // each project or assembly.
            _realRunner = _projectCount > 1 || _projectCount > 0 && _assemblyCount > 0
                ? new AggregatingTestRunner(Services, package)
                : Services.TestRunnerFactory.MakeTestRunner(package);
        }

        private void ExpandProjects()
        {
            if (TestPackage.TestFiles.Length > 0)
            {
                foreach (string testFile in TestPackage.TestFiles)
                {
                    TestPackage subPackage = new TestPackage(testFile);
                    if (Services.ProjectService.IsProjectFile(testFile))
                    {
                        Services.ProjectService.ExpandProjectPackage(subPackage);
                        _projectCount++;
                    }
                    else
                        _assemblyCount++;
                }
            }
            else
            {
                if (Services.ProjectService.IsProjectFile(TestPackage.FullName))
                {
                    Services.ProjectService.ExpandProjectPackage(TestPackage);
                    _projectCount++;
                }
                else
                    _assemblyCount++;
            }
        }

        #endregion
    }
}
