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
    public class DomainManager : IService
    {
        static Logger log = InternalTrace.GetLogger(typeof(DomainManager));

        // Default settings used if SettingsService is unavailable
        private string _shadowCopyPath = Path.Combine(NUnitConfiguration.NUnitBinDirectory, "ShadowCopyCache");
        private PrincipalPolicy _principalPolicy = PrincipalPolicy.UnauthenticatedPrincipal;
        private bool _setPrincipalPolicy = false;

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
            if (_setPrincipalPolicy)
                runnerDomain.SetPrincipalPolicy(_principalPolicy);

            return runnerDomain;
        }

        // Made separate and public for testing
        public AppDomainSetup CreateAppDomainSetup(TestPackage package)
        {
            // Debugger.Launch();
            AppDomainSetup setup = new AppDomainSetup();

            if (package.SubPackages.Count == 1)
                package = package.SubPackages[0];

            //For parallel tests, we need to use distinct application name
            setup.ApplicationName = "Tests" + "_" + Environment.TickCount;

            FileInfo testFile = package.FullName != null && package.FullName != string.Empty
                ? new FileInfo(package.FullName)
                : null;

            string appBase = package.GetSetting(PackageSettings.BasePath, string.Empty);
            string binPath = package.GetSetting(PackageSettings.PrivateBinPath, string.Empty);

            if (testFile != null)
            {
                if (appBase == null || appBase == string.Empty)
                    appBase = testFile.DirectoryName;
            }
            else
            {
                if (appBase == null || appBase == string.Empty)
                    appBase = GetCommonAppBase(package.SubPackages);
            }

            char lastChar = appBase[appBase.Length - 1];
            if (lastChar != Path.DirectorySeparatorChar && lastChar != Path.AltDirectorySeparatorChar)
                appBase += Path.DirectorySeparatorChar;

            setup.ApplicationBase = appBase;

            // TODO: Check whether Mono still needs full path to config file...
            string configurationFile;
            if (TryGetConfigFileName(
                package.GetSetting(PackageSettings.ConfigurationFile, string.Empty),
                testFile != null ? testFile.FullName : string.Empty,
                appBase,
                out configurationFile))
            {
                // TODO: What about config file for multiple assemblies?
                setup.ConfigurationFile = configurationFile;
            }
            
            if (package.GetSetting(PackageSettings.AutoBinPath, binPath == string.Empty))
                binPath = GetPrivateBinPath(appBase, package.SubPackages);

            setup.PrivateBinPath = binPath;

            if (package.GetSetting("ShadowCopyFiles", false))
            {
                setup.ShadowCopyFiles = "true";
                setup.ShadowCopyDirectories = appBase;
                setup.CachePath = GetCachePath();
            }
            else
                setup.ShadowCopyFiles = "false";
            return setup;
        }

        public void Unload(AppDomain domain)
        {
            new DomainUnloader(domain).Unload();
        }

        public static bool TryGetConfigFileName(string settingsConfigFile, string testFile, string appBaseDir, out string config)
        {
            if (string.IsNullOrEmpty(testFile))
            {
                config = null;
                return false;
            }

            if (!string.IsNullOrEmpty(settingsConfigFile))
            {
                config = settingsConfigFile;
                return true;
            }

            var configFile = testFile + ".config";
            var configFileName = Path.GetFileName(configFile);
            if (!StringComparer.InvariantCultureIgnoreCase.Equals(configFile, configFileName))
            {
                config = configFile;
                return true;
            }

            if (!string.IsNullOrEmpty(appBaseDir))
            {
                config = Path.Combine(appBaseDir, Path.GetFileName(configFile));
                return true;
            }

            config = configFile;
            return true;
        }

        #endregion

        #region Nested DomainUnloader Class
        class DomainUnloader
        {
            private Thread thread;
            private AppDomain domain;

            public DomainUnloader(AppDomain domain)
            {
                this.domain = domain;
            }

            public void Unload()
            {
                string domainName;
                try
                {
                    domainName = "UNKNOWN";//domain.FriendlyName;
                }
                catch (AppDomainUnloadedException)
                {
                    return;
                }

                log.Info("Unloading AppDomain " + domainName);

                thread = new Thread(new ThreadStart(UnloadOnThread));
                thread.Start();
                if (!thread.Join(30000))
                {
                    log.Error("Unable to unload AppDomain {0}, Unload thread timed out", domainName);
                    thread.Abort();
                }
            }

            private void UnloadOnThread()
            {
                bool shadowCopy = false;
                string cachePath = null;
                string domainName = "UNKNOWN";               

                try
                {
                    shadowCopy = domain.ShadowCopyFiles;
                    cachePath = domain.SetupInformation.CachePath;
                    domainName = domain.FriendlyName;

                    AppDomain.Unload(domain);
                }
                catch (Exception ex)
                {
                    // We assume that the tests did something bad and just leave
                    // the orphaned AppDomain "out there". 
                    // TODO: Something useful.
                    log.Error("Unable to unload AppDomain " + domainName, ex);
                }
                finally
                {
                    if (shadowCopy && cachePath != null)
                        DeleteCacheDir(new DirectoryInfo(cachePath));
                }
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Get the location for caching and delete any old cache info
        /// </summary>
        private string GetCachePath()
        {
            int processId = Process.GetCurrentProcess().Id;
            long ticks = DateTime.Now.Ticks;
            string cachePath = Path.Combine( _shadowCopyPath, processId.ToString() + "_" + ticks.ToString() ); 
                
            try 
            {
                DirectoryInfo dir = new DirectoryInfo(cachePath);		
                if(dir.Exists) dir.Delete(true);
            }
            catch( Exception ex)
            {
                throw new ApplicationException( 
                    string.Format( "Invalid cache path: {0}",cachePath ),
                    ex );
            }

            return cachePath;
        }

        /// <summary>
        /// Helper method to delete the cache dir. This method deals 
        /// with a bug that occurs when files are marked read-only
        /// and deletes each file separately in order to give better 
        /// exception information when problems occur.
        /// 
        /// TODO: This entire method is problematic. Should we be doing it?
        /// </summary>
        /// <param name="cacheDir"></param>
        private static void DeleteCacheDir( DirectoryInfo cacheDir )
        {
            //			Debug.WriteLine( "Modules:");
            //			foreach( ProcessModule module in Process.GetCurrentProcess().Modules )
            //				Debug.WriteLine( module.ModuleName );
            

            if(cacheDir.Exists)
            {
                foreach( DirectoryInfo dirInfo in cacheDir.GetDirectories() )
                    DeleteCacheDir( dirInfo );

                foreach( FileInfo fileInfo in cacheDir.GetFiles() )
                {
                    fileInfo.Attributes = FileAttributes.Normal;
                    try 
                    {
                        fileInfo.Delete();
                    }
                    catch( Exception ex )
                    {
                        Debug.WriteLine( string.Format( 
                            "Error deleting {0}, {1}", fileInfo.Name, ex.Message ) );
                    }
                }

                cacheDir.Attributes = FileAttributes.Normal;

                try
                {
                    cacheDir.Delete();
                }
                catch( Exception ex )
                {
                    Debug.WriteLine( string.Format( 
                        "Error deleting {0}, {1}", cacheDir.Name, ex.Message ) );
                }
            }
        }

        private bool IsTestDomain(AppDomain domain)
        {
            return domain.FriendlyName.StartsWith( "test-domain-" );
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

        public void DeleteShadowCopyPath()
        {
            if ( Directory.Exists( _shadowCopyPath ) )
                Directory.Delete( _shadowCopyPath, true );
        }
        #endregion

        #region IService Members

        public ServiceContext ServiceContext { get; set; }

        public ServiceStatus Status { get; private set; }

        public void StopService()
        {
            // TODO:  Add DomainManager.UnloadService implementation
            Status = ServiceStatus.Stopped;
        }

        public void StartService() 
        {
            try
            {
                // DomainManager has a soft dependency on the SettingsService.
                // If it's not available, default values are used.
                var settings = ServiceContext.GetService<ISettings>();
                if (settings != null)
                {
                    var pathSetting = settings.GetSetting("Options.TestLoader.ShadowCopyPath", "");
                    if (pathSetting != "")
                        _shadowCopyPath = Environment.ExpandEnvironmentVariables(pathSetting);

                    _setPrincipalPolicy = settings.GetSetting("Options.TestLoader.SetPrincipalPolicy", false);
                    _principalPolicy = settings.GetSetting("Options.TestLoader.PrincipalPolicy", PrincipalPolicy.UnauthenticatedPrincipal);
                }

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
