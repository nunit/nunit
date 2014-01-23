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
using System.Diagnostics;
using System.IO;
using System.Xml;
using NUnit.Engine.Internal;
using NUnit.Engine.Services;

namespace NUnit.Engine
{
    /// <summary>
    /// The TestEngine provides services that allow a client
    /// program to interact with NUnit in order to explore,
    /// load and run tests.
    /// </summary>
    public class TestEngine : ITestEngine
    {
        private ServiceContext services;

        public TestEngine()
        {
            this.services = new ServiceContext();
        }

        #region Public Properties

        public ServiceContext Services
        {
            get { return services; }
        }

        #endregion

        #region ITestEngine Members

        /// <summary>
        /// Access the public Engine Services
        /// </summary>
        IServiceLocator ITestEngine.Services
        {
            get { return Services; }
        }

        /// <summary>
        /// Create and initialize the standard set of services
        /// used in the Engine. This interface is not normally
        /// called by user code. Programs linking only to 
        /// only to the nunit.engine.api assembly are given a
        /// pre-initialized instance of TestEngine. Programs 
        /// that link directly to nunit.engine usually do so
        /// in order to perform custom initialization.
        /// </summary>
        public void InitializeServices(string workDirectory, InternalTraceLevel traceLevel)
        {
            SettingsService settingsService = new SettingsService("NUnit30Settings.xml", true);

            if (traceLevel == InternalTraceLevel.Default)
                traceLevel = (InternalTraceLevel)settingsService.GetSetting("Options.InternalTraceLevel", InternalTraceLevel.Off);

            if (traceLevel != InternalTraceLevel.Off)
            {
                var logName = string.Format("InternalTrace.{0}.log", Process.GetCurrentProcess().Id);
                InternalTrace.Initialize(Path.Combine(workDirectory, logName), traceLevel);
            }

            Services.Add(settingsService);
            services.Add(new RecentFilesService());
            Services.Add(new DomainManager());
            Services.Add(new ProjectService());
            Services.Add(new RuntimeFrameworkSelector());
            Services.Add(new DefaultTestRunnerFactory());
            Services.Add(new DriverFactory());
            Services.Add(new TestAgency());

            Services.ServiceManager.InitializeServices();
        }

        /// <summary>
        /// Returns an xml representation of the tests specified
        /// by a TestPackage.
        /// </summary>
        /// <param name="package">A TestPackage.</param>
        /// <returns>An XmlNode representing the tests.</returns>
        public ITestEngineResult Explore(TestPackage package, TestFilter filter)
        {
            using (ITestRunner runner = GetRunner())
            {
                ITestEngineResult loadResult = runner.Load(package);

                return loadResult.HasErrors
                    ? loadResult
                    : runner.Explore(filter);
            }
        }

        /// <summary>
        /// Runs tests specified in the test package, applying
        /// the supplied filter.
        /// </summary>
        /// <param name="package">A TestPackage.</param>
        /// <param name="filter">A TestFilter (currently ignored)</param>
        /// <returns>An XmlNode representing the test results.</returns>
        public ITestEngineResult Run(TestPackage package, ITestEventHandler listener, TestFilter filter)
        {
            using (ITestRunner runner = GetRunner())
            {
                ITestEngineResult loadResult = runner.Load(package);

                return loadResult.HasErrors
                    ? loadResult
                    : runner.Run(listener, filter);
            }
        }

        /// <summary>
        /// Returns a test runner for use by clients that need to load the
        /// tests once and run them multiple times.
        /// </summary>
        /// <returns>An ITestRunner.</returns>
        public ITestRunner GetRunner()
        {
            return new Runners.MasterTestRunner(Services);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Services.ServiceManager.StopAllServices();
        }

        #endregion
    }
}
