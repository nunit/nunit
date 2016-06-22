// ***********************************************************************
// Copyright (c) 2007-2015 Charlie Poole
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
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
using System.Security;
using System.Security.Policy;
using System.Security.Principal;
using NUnit.Common;
using NUnit.Engine.Internal;

namespace NUnit.Engine.Services
{
    /// <summary>
    /// The DomainManager class handles the creation and unloading
    /// of domains as needed and keeps track of all existing domains.
    /// </summary>
    public class DomainManager : Service
    {
        static Logger log = InternalTrace.GetLogger(typeof(DomainManager));

        private static readonly PropertyInfo TargetFrameworkNameProperty =
            typeof(AppDomainSetup).GetProperty("TargetFrameworkName", BindingFlags.Public | BindingFlags.Instance);

        private ISettings _settingsService;

        #region Create and Unload Domains
        /// <summary>
        /// Construct an application domain for running a test package
        /// </summary>
        /// <param name="package">The TestPackage to be run</param>
        public AppDomain CreateDomain( TestPackage package )
        {
            AppDomainSetup setup = CreateAppDomainSetup(package);

            string domainName = "test-domain-" + package.Name;
            // Setup the Evidence
            Evidence evidence = new Evidence(AppDomain.CurrentDomain.Evidence);
            if (evidence.Count == 0)
            {
                Zone zone = new Zone(SecurityZone.MyComputer);
                evidence.AddHost(zone);
                Assembly assembly = Assembly.GetExecutingAssembly();
                Url url = new Url(assembly.CodeBase);
                evidence.AddHost(url);
                Hash hash = new Hash(assembly);
                evidence.AddHost(hash);
            }

            log.Info("Creating AppDomain " + domainName);

            AppDomain runnerDomain = AppDomain.CreateDomain(domainName, evidence, setup);

            // Set PrincipalPolicy for the domain if called for in the settings
            if (_settingsService != null && _settingsService.GetSetting("Options.TestLoader.SetPrincipalPolicy", false))
            {
                runnerDomain.SetPrincipalPolicy(_settingsService.GetSetting(
                    "Options.TestLoader.PrincipalPolicy", 
                    PrincipalPolicy.UnauthenticatedPrincipal));
            }

            return runnerDomain;
        }

        // Made separate and internal for testing
        AppDomainSetup CreateAppDomainSetup(TestPackage package)
        {
            AppDomainSetup setup = new AppDomainSetup();

            if (package.SubPackages.Count == 1)
                package = package.SubPackages[0];

            //For parallel tests, we need to use distinct application name
            setup.ApplicationName = "Tests" + "_" + Environment.TickCount;

            string appBase = GetApplicationBase(package);
            setup.ApplicationBase = appBase;
            setup.ConfigurationFile = GetConfigFile(appBase, package);
            setup.PrivateBinPath = GetPrivateBinPath(appBase, package); 

            if (!string.IsNullOrEmpty(package.FullName))
            {
                // Setting the target framework is only supported when running with
                // multiple AppDomains, one per assembly.
                // TODO: Remove this limitation

                // .NET versions greater than v4.0 report as v4.0, so look at
                // the TargetFrameworkAttribute on the assembly if it exists
                // If property is null, .NET 4.5+ is not installed, so there is no need
                if (TargetFrameworkNameProperty != null)
                {
                    var frameworkName = package.GetSetting(PackageSettings.ImageTargetFrameworkName, "");
                    if (frameworkName != "")
                        TargetFrameworkNameProperty.SetValue(setup, frameworkName, null);
                }
            }

            if (package.GetSetting("ShadowCopyFiles", false))
            {
                setup.ShadowCopyFiles = "true";
                setup.ShadowCopyDirectories = setup.ApplicationBase;
            }
            else
                setup.ShadowCopyFiles = "false";

            return setup;
        }

        public void Unload(AppDomain domain)
        {
            new DomainUnloader(domain).Unload();
        }

        #endregion

        #region Nested DomainUnloader Class
        class DomainUnloader
        {
            private Thread _unloadThread;
            private AppDomain _domain;
            private Exception _unloadException;

            public DomainUnloader(AppDomain domain)
            {
                _domain = domain;
            }

            public void Unload()
            {
                _unloadThread = new Thread(new ThreadStart(UnloadOnThread));

                _unloadThread.Start();

                if (!_unloadThread.Join(30000))
                {
                    string msg = "Unable to unload AppDomain, Unload thread timed out";

                    log.Error(msg);
                    _unloadThread.Abort();

                    throw new NUnitEngineException(msg);
                }

                if (_unloadException != null)
                    throw new NUnitEngineException("Exception encountered unloading AppDomain", _unloadException);
            }

            private void UnloadOnThread()
            {
                bool shadowCopy = false;
                string domainName = "UNKNOWN";               

                try
                {
                    shadowCopy = _domain.ShadowCopyFiles;
                    domainName = _domain.FriendlyName;

                    // Uncomment to simulate an error in unloading
                    //throw new Exception("Testing: simulated unload error");

                    // Uncomment to simulate a timeout while unloading
                    //while (true) ;

                    AppDomain.Unload(_domain);
                }
                catch (Exception ex)
                {
                    _unloadException = ex;

                    // We assume that the tests did something bad and just leave
                    // the orphaned AppDomain "out there". 
                    // TODO: Something useful.
                    log.Error("Unable to unload AppDomain " + domainName, ex);
                }
            }
        }
        #endregion

        #region Helper Methods

        /// <summary>
        /// Figure out the ApplicationBase for a package
        /// </summary>
        /// <param name="package">The package</param>
        /// <returns>The ApplicationBase</returns>
        public static string GetApplicationBase(TestPackage package)
        {
            Guard.ArgumentNotNull(package, "package");

            var appBase = package.GetSetting(PackageSettings.BasePath, string.Empty);

            if (string.IsNullOrEmpty(appBase))
                appBase = string.IsNullOrEmpty(package.FullName)
                    ? GetCommonAppBase(package.SubPackages)
                    : Path.GetDirectoryName(package.FullName);

            if (!string.IsNullOrEmpty(appBase))
            {
                char lastChar = appBase[appBase.Length - 1];
                if (lastChar != Path.DirectorySeparatorChar && lastChar != Path.AltDirectorySeparatorChar)
                    appBase += Path.DirectorySeparatorChar;
            }

            return appBase;
        }

        public static string GetConfigFile(string appBase, TestPackage package)
        {
            Guard.ArgumentNotNullOrEmpty(appBase, "appBase");
            Guard.ArgumentNotNull(package, "package");

            // Use provided setting if available
            string configFile = package.GetSetting(PackageSettings.ConfigurationFile, string.Empty);
            if (configFile != string.Empty)
                return Path.Combine(appBase, configFile);
        
            // The ProjectService adds any project config to the settings.
            // So, at this point, we only want to handle assemblies or an
            // anonymous package created from the comnand-line.
            string fullName = package.FullName;
            if (IsExecutable(fullName))
                return fullName + ".config";

            // Command-line package gets no config unless it's a single assembly
            if (string.IsNullOrEmpty(fullName) && package.SubPackages.Count == 1)
            {
                fullName = package.SubPackages[0].FullName;
                if (IsExecutable(fullName))
                    return fullName + ".config";
            }

            // No config file will be specified
            return null;
        }

        private static bool IsExecutable(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            string ext = Path.GetExtension(fileName).ToLower();
            return ext == ".dll" || ext == ".exe";
        }

        public static string GetCommonAppBase(IList<TestPackage> packages)
        {
            var assemblies = new List<string>();
            foreach (var package in packages)
                assemblies.Add(package.FullName);

            return GetCommonAppBase(assemblies);
        }

        public static string GetCommonAppBase(IList<string> assemblies)
        {
            string commonBase = null;

            foreach (string assembly in assemblies)
            {
                string dir = Path.GetDirectoryName(Path.GetFullPath(assembly));
                if (commonBase == null)
                    commonBase = dir;
                else while (!PathUtils.SamePathOrUnder(commonBase, dir) && commonBase != null)
                        commonBase = Path.GetDirectoryName(commonBase);
            }

            return commonBase;
        }

        public static string GetPrivateBinPath(string basePath, string fileName)
        {
            return GetPrivateBinPath(basePath, new string[] { fileName });
        }

        public static string GetPrivateBinPath(string appBase, TestPackage package)
        {
            var binPath = package.GetSetting(PackageSettings.PrivateBinPath, string.Empty);

            if (package.GetSetting(PackageSettings.AutoBinPath, binPath == string.Empty))
                binPath = package.SubPackages.Count > 0
                    ? GetPrivateBinPath(appBase, package.SubPackages)
                    : package.FullName != null
                        ? GetPrivateBinPath(appBase, package.FullName)
                        : null;

            return binPath;
        }

        public static string GetPrivateBinPath(string basePath, IList<TestPackage> packages)
        {
            var assemblies = new List<string>();
            foreach (var package in packages)
                assemblies.Add(package.FullName);

            return GetPrivateBinPath(basePath, assemblies);
        }

        public static string GetPrivateBinPath(string basePath, IList<string> assemblies)
        {
            List<string> dirList = new List<string>();
            StringBuilder sb = new StringBuilder(200);

            foreach( string assembly in assemblies )
            {
                string dir = PathUtils.RelativePath(
                    Path.GetFullPath(basePath), 
                    Path.GetDirectoryName( Path.GetFullPath(assembly) ) );
                if ( dir != null && dir != string.Empty && dir != "." && !dirList.Contains( dir ) )
                {
                    dirList.Add( dir );
                    if ( sb.Length > 0 )
                        sb.Append( Path.PathSeparator );
                    sb.Append( dir );
                }
            }

            return sb.Length == 0 ? null : sb.ToString();
        }

        #endregion

        #region Service Overrides

        public override void StartService() 
        {
            try
            {
                // DomainManager has a soft dependency on the SettingsService.
                // If it's not available, default values are used.
                _settingsService = ServiceContext.GetService<ISettings>();

                Status = ServiceStatus.Started;
            }
            catch
            {
                Status = ServiceStatus.Error;
                throw;
            }
        }

        #endregion
    }
}
