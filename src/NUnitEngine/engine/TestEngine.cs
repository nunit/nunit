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
using System.Collections;
using System.IO;
using System.Xml;
using NUnit.Engine.Api;

namespace NUnit.Engine
{
    /// <summary>
    /// The TestEngine provides services that allow a client
    /// program to interact with NUnit in order to explore,
    /// load and run tests.
    /// </summary>
    public class TestEngine : ITestEngine
    {
        ITestRunnerFactory factory;

        static TestEngine()
        {
            InitializeServices();
        }

        public TestEngine()
        {
            this.factory = new InProcessTestRunnerFactory();
        }

        #region Public Methods

        /// <summary>
        /// Returns an xml representation of the tests specified
        /// by a TestPackage.
        /// </summary>
        /// <param name="package">A TestPackage.</param>
        /// <returns>An XmlNode representing the tests.</returns>
        public XmlNode Explore(TestPackage package)
        {
            // TODO: We will need an agent or remote explorer
            // in the future in order to explore tests that
            // are located on a different machine.
            var driver = new FrameworkDriver(AppDomain.CurrentDomain);
            return driver.ExploreTests(package.TestFiles[0], new Hashtable());
        }

        /// <summary>
        /// Runs tests specified in the test package, applying
        /// the supplied filter.
        /// </summary>
        /// <param name="package">A TestPackage.</param>
        /// <param name="filter">A TestFilter (currently ignored)</param>
        /// <returns>An XmlNode representing the test results.</returns>
        public XmlNode Run(TestPackage package, TestFilter filter)
        {
            using (ITestRunner runner = GetRunner(package))
            {
                if (runner.Load(package))
                    return runner.Run(filter);

                return null;
            }
        }

        /// <summary>
        /// Returns a runner suitable for running tests in the specified package.
        /// </summary>
        /// <param name="package">The TestPackage for which a runner is needed.</param>
        /// <returns>An ITestRunner, which may be local or remote depending on the package settings.</returns>
        public ITestRunner GetRunner(TestPackage package)
        {
            return factory.MakeTestRunner(package);
        }

        #endregion

        #region Helper Methods

        private static void InitializeServices()
        {
            ServiceManager.Services.AddService(new DomainManager());

            ServiceManager.Services.InitializeServices();
        }

        #endregion
    }
}
