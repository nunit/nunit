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
using NUnit.Engine.Services;

namespace NUnit.Engine
{
    public class ServiceContext
    {
        #region ServiceManager

        private IServiceManager serviceManager;
        public IServiceManager ServiceManager
        {
            get
            {
                if (serviceManager == null)
                    serviceManager = new ServiceManager();

                return serviceManager;
            }
        }

        #endregion

        #region DomainManager

        private DomainManager domainManager;
        public DomainManager DomainManager
        {
            get
            {
                if (domainManager == null)
                    domainManager = (DomainManager)ServiceManager.GetService(typeof(DomainManager));

                return domainManager;
            }
        }

        #endregion

        #region UserSettings

        private ISettings userSettings;
        public ISettings UserSettings
        {
            get 
            { 
                if ( userSettings == null )
                    userSettings = (ISettings)ServiceManager.GetService( typeof( ISettings ) );

        //        // Temporary fix needed to run TestDomain tests in test AppDomain
        //        // TODO: Figure out how to set up the test domain correctly
        //        if ( userSettings == null )
        //            userSettings = new SettingsService();

                return userSettings;
            }
        }

        #endregion

        #region RecentFilesService
        //        private static RecentFiles recentFiles;
        //        public static RecentFiles RecentFiles
        //        {
        //            get
        //            {
        //                if ( recentFiles == null )
        //                    recentFiles = (RecentFiles)ServiceManager.Services.GetService( typeof( RecentFiles ) );

        //                return recentFiles;
        //            }
        //        }
        #endregion

        #region TestRunnerFactory

        private ITestRunnerFactory testRunnerFactory;
        public ITestRunnerFactory TestRunnerFactory
        {
            get
            {
                if (testRunnerFactory == null)
                    testRunnerFactory = (ITestRunnerFactory)ServiceManager.GetService(typeof(ITestRunnerFactory));

                return testRunnerFactory;
            }
        }

        #endregion

        #region TestLoader
        //        private static TestLoader loader;
        //        public static TestLoader TestLoader
        //        {
        //            get
        //            {
        //                if ( loader == null )
        //                    loader = (TestLoader)ServiceManager.Services.GetService( typeof( TestLoader ) );

        //                return loader;
        //            }
        //        }
        #endregion

        #region TestAgency

        private TestAgency agency;
        public TestAgency TestAgency
        {
            get
            {
                if (agency == null)
                    agency = (TestAgency)ServiceManager.GetService(typeof(TestAgency));

                // Temporary fix needed to run ProcessRunner tests in test AppDomain
                // TODO: Figure out how to set up the test domain correctly
                //				if ( agency == null )
                //				{
                //					agency = new TestAgency();
                //					agency.Start();
                //				}

                return agency;
            }
        }

        #endregion

        #region ProjectService
        private ProjectService projectService;
        public ProjectService ProjectService
        {
            get
            {
                if (projectService == null)
                    projectService = (ProjectService)
                        ServiceManager.GetService(typeof(ProjectService));

                return projectService;
            }
        }
        #endregion

        public void Add(IService service)
        {
            ServiceManager.AddService(service);
            service.ServiceContext = this;
        }
    }
}
