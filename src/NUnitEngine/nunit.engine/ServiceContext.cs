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
    /// <summary>
    /// The ServiceContext is used by services, runners and
    /// external clients to locate the services they need through
    /// the IServiceLocator interface.
    /// 
    /// For internal use by runners and other services, individual
    /// properties are provided for common services as well.
    /// </summary>
    public class ServiceContext : IServiceLocator
    {
        #region Constructor

        public ServiceContext()
        {
            ServiceManager = new ServiceManager();
        }

        #endregion

        #region Service Properties

        #region ServiceManager

        public ServiceManager ServiceManager { get; private set; }

        #endregion

        #region DomainManager

        private DomainManager _domainManager;
        public DomainManager DomainManager
        {
            get
            {
                if (_domainManager == null)
                    _domainManager = GetService<DomainManager>();

                return _domainManager;
            }
        }

        #endregion

        #region UserSettings

        private ISettings _userSettings;
        public ISettings UserSettings
        {
            get
            {
                if (_userSettings == null)
                    _userSettings = GetService<ISettings>();

                return _userSettings;
            }
        }

        #endregion

        #region RecentFilesService
        private IRecentFiles _recentFiles;
        public IRecentFiles RecentFiles
        {
            get
            {
                if ( _recentFiles == null )
                    _recentFiles = GetService<IRecentFiles>();

                return _recentFiles;
            }
        }
        #endregion

        #region RuntimeFrameworkSelector

        // Note: the engine uses the RuntimeFrameworkService directly
        // while runners only have access to IRuntimeFrameworkService.
        private RuntimeFrameworkService _runtimeService;
        public RuntimeFrameworkService RuntimeFrameworkService
        {
            get
            {
                if (_runtimeService == null)
                    _runtimeService = GetService<RuntimeFrameworkService>();

                return _runtimeService;
            }
        }

        #endregion

        #region DriverFactory

        private IDriverService _driverFactory;
        public IDriverService DriverFactory
        {
            get
            {
                if (_driverFactory == null)
                    _driverFactory = GetService<IDriverService>();

                return _driverFactory;
            }
        }

        #endregion

        #region TestRunnerFactory

        private ITestRunnerFactory _testRunnerFactory;
        public ITestRunnerFactory TestRunnerFactory
        {
            get
            {
                if (_testRunnerFactory == null)
                    _testRunnerFactory = GetService<ITestRunnerFactory>();

                return _testRunnerFactory;
            }
        }

        #endregion

        #region TestAgency

        private TestAgency _agency;
        public TestAgency TestAgency
        {
            get
            {
                if (_agency == null)
                    _agency = GetService<TestAgency>();

                return _agency;
            }
        }

        #endregion

        #region ProjectService
        private ProjectService _projectService;
        public ProjectService ProjectService
        {
            get
            {
                if (_projectService == null)
                    _projectService = GetService<ProjectService>();

                return _projectService;
            }
        }
        #endregion

        #endregion

        #region Add Method

        public void Add(IService service)
        {
            ServiceManager.AddService(service);
            service.ServiceContext = this;
        }

        #endregion

        #region IServiceLocator Implementation

        public T GetService<T>() where T : class
        {
            return ServiceManager.GetService(typeof(T)) as T;
        }

        public object GetService(Type serviceType)
        {
            return ServiceManager.GetService(serviceType);
        }

        #endregion
    }
}
