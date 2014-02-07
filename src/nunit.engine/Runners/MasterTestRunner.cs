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
using System.Globalization;
using System.Reflection;
using System.Xml;
using NUnit.Engine.Internal;

namespace NUnit.Engine.Runners
{
    public class MasterTestRunner : AbstractTestRunner, ITestRunner
    {
        private AbstractTestRunner realRunner;

        // Count of assemblies and projects passed in package
        private int assemblyCount;
        private int projectCount;

        public MasterTestRunner(ServiceContext services) : base(services) { }

        #region AbstractTestRunner Overrides

        /// <summary>
        /// Explore a loaded TestPackage and return information about
        /// the tests found.
        /// </summary>
        /// <param name="package">The TestPackage to be explored</param>
        /// <returns>A TestEngineResult.</returns>
        public override TestEngineResult Explore(TestFilter filter)
        {
            return this.realRunner.Explore(filter).Aggregate(TEST_RUN_ELEMENT, package.Name, package.FullName);
        }

        /// <summary>
        /// Load a TestPackage for possible execution
        /// </summary>
        /// <param name="package">The TestPackage to be loaded</param>
        /// <returns>A TestEngineResult.</returns>
        public override TestEngineResult Load(TestPackage package)
        {
            PerformPackageSetup(package);
            return this.realRunner.Load(package).Aggregate(TEST_RUN_ELEMENT, package.Name, package.FullName);
        }

        /// <summary>
        /// Unload any loaded TestPackage.
        /// </summary>
        public override void Unload()
        {
            if (this.realRunner != null)
                this.realRunner.Unload();
        }

        /// <summary>
        /// Count the test cases that would be run under
        /// the specified filter.
        /// </summary>
        /// <param name="filter">A TestFilter</param>
        /// <returns>The count of test cases</returns>
        public override int CountTestCases(TestFilter filter)
        {
            return realRunner.CountTestCases(filter);
        }

        /// <summary>
        /// Run the tests in the loaded TestPackage and return a test result. The tests
        /// are run synchronously and the listener interface is notified as it progresses.
        /// </summary>
        /// <param name="listener">An ITestEventHandler to receive events</param>
        /// <param name="filter">A TestFilter used to select tests</param>
        /// <returns>A TestEngineResult giving the result of the test execution</returns>
        public override TestEngineResult Run(ITestEventHandler listener, TestFilter filter)
        {
            DateTime startTime = DateTime.Now;

            TestEngineResult result = realRunner.Run(listener, filter).Aggregate("test-run", package.Name, package.FullName);

            result.Xml.InsertEnvironmentElement();

            result.Xml.AddAttribute("run-date", XmlConvert.ToString(startTime, "yyyy-MM-dd"));
            result.Xml.AddAttribute("start-time", XmlConvert.ToString(startTime, "HH:mm:ss"));

            return result;
        }

        /// <summary>
        /// Start a run of the tests in the loaded TestPackage. The tests are run
        /// asynchronously and the listener interface is notified as it progresses.
        /// </summary>
        /// <param name="listener">An ITestEventHandler to receive events</param>
        /// <param name="filter">A TestFilter used to select tests</param>
        public override void BeginRun(ITestEventHandler listener, TestFilter filter)
        {
            realRunner.BeginRun(listener, filter);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (this.realRunner != null)
                this.realRunner.Dispose();
        }

        #endregion

        #region HelperMethods

        private void PerformPackageSetup(TestPackage package)
        {
            this.package = package;

            // Expand projects, updating the count of projects and assemblies
            ExpandProjects();

            // If there is more than one project or a mix of assemblies and 
            // projects, AggregatingTestRunner will call MakeTestRunner for
            // each project or assembly.
            this.realRunner = projectCount > 1 || projectCount > 0 && assemblyCount > 0
                ? new AggregatingTestRunner(services)
                : (AbstractTestRunner)services.TestRunnerFactory.MakeTestRunner(package);
        }

        private void ExpandProjects()
        {
            if (package.TestFiles.Length > 0)
            {
                foreach (string testFile in package.TestFiles)
                {
                    TestPackage subPackage = new TestPackage(testFile);
                    if (services.ProjectService.IsProjectFile(testFile))
                    {
                        services.ProjectService.ExpandProjectPackage(subPackage);
                        projectCount++;
                    }
                    else
                        assemblyCount++;
                }
            }
            else
            {
                if (services.ProjectService.IsProjectFile(package.FullName))
                {
                    services.ProjectService.ExpandProjectPackage(package);
                    projectCount++;
                }
                else
                    assemblyCount++;
            }
        }

        #endregion
    }
}
