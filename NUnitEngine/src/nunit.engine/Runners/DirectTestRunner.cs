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
        protected TestPackage package;
        private List<IFrameworkDriver> drivers = new List<IFrameworkDriver>();
        private ServiceContext services;

        protected AppDomain TestDomain;

        protected ServiceContext Services
        {
            get { return services; }
        }

        public DirectTestRunner(ServiceContext services)
        {
            this.services = services;
        }

        #region ITestRunner Members

        /// <summary>
        /// Load a TestPackage for possible execution
        /// </summary>
        /// <param name="package">The TestPackage to be loaded</param>
        /// <returns>A TestEngineResult.</returns>
        public override TestEngineResult Load(TestPackage package)
        {
            this.package = package;
            TestEngineResult loadResult = new TestEngineResult();

            foreach (string testFile in package.TestFiles)
            {
                // TODO: Should get the appropriate driver for the file
                IFrameworkDriver driver = new NUnitFrameworkDriver(TestDomain);
                TestEngineResult driverResult = driver.Load(testFile, package.Settings);

                foreach (XmlNode node in driverResult.XmlNodes)
                    loadResult.Add(node);

                if (!loadResult.HasErrors)
                    drivers.Add(driver);
            }

            return loadResult;
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
        public override TestEngineResult Run(ITestEventHandler listener, ITestFilter filter)
        {
            List<TestEngineResult> results = new List<TestEngineResult>();

            foreach (NUnitFrameworkDriver driver in drivers)
                results.Add(driver.Run(this.package.Settings, listener));

            if (IsProjectPackage(this.package))
                return TestEngineResult.MakeProjectResult(this.package, results);
            else if (results.Count == 1)
                return results[0];
            else
                return TestEngineResult.Merge(results);
        }

        #endregion

        private bool IsProjectPackage(TestPackage package)
        {
            return package.FullName != null && package.FullName != string.Empty && services.ProjectService.IsProjectFile(package.FullName);
        }
    }
}
