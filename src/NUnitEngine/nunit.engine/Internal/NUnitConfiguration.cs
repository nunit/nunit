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
using System.IO;
using System.Reflection;
using System.Configuration;
using Microsoft.Win32;

namespace NUnit.Engine.Internal
{
    /// <summary>
    /// Provides static methods for accessing the NUnit config
    /// file 
    /// </summary>
    public static class NUnitConfiguration
    {
        #region Public Properties

        #region BuildConfiguration
        public static string BuildConfiguration
        {
            get
            {
#if DEBUG
                    return "Debug";
#else
                    return "Release";
#endif
            }
        }
        #endregion

        #region NUnitLibDirectory
        private static string nunitLibDirectory;
        /// <summary>
        /// Gets the path to the lib directory for the version and build
        /// of NUnit currently executing.
        /// </summary>
        public static string NUnitLibDirectory
        {
            get {
                return nunitLibDirectory ??
                       (nunitLibDirectory = AssemblyHelper.GetDirectoryName(Assembly.GetExecutingAssembly()));
            }
        }
        #endregion

        #region NUnitBinDirectory
        private static string nunitBinDirectory;
        public static string NUnitBinDirectory
        {
            get
            {
                if (nunitBinDirectory == null)
                {
                    nunitBinDirectory = NUnitLibDirectory;
                    if (Path.GetFileName(nunitBinDirectory).ToLower() == "lib")
                        nunitBinDirectory = Path.GetDirectoryName(nunitBinDirectory);
                }

                return nunitBinDirectory;
            }
        }
        #endregion

        #region AddinDirectory
        private static string addinDirectory;
        public static string AddinDirectory
        {
            get { return addinDirectory ?? (addinDirectory = Path.Combine(NUnitBinDirectory, "addins")); }
        }
        #endregion

        #region TestAgentExePath
        //private static string testAgentExePath;
        //private static string TestAgentExePath
        //{
        //    get
        //    {
        //        if (testAgentExePath == null)
        //            testAgentExePath = Path.Combine(NUnitBinDirectory, "nunit-agent.exe");

        //        return testAgentExePath;
        //    }
        //}
        #endregion

        #region MonoExePath
        private static string monoExePath;
        public static string MonoExePath
        {
            get
            {
                if (monoExePath == null)
                {
                    string[] searchNames = IsWindows()
                        ? new[] { "mono.bat", "mono.cmd", "mono.exe" }
                        : new[] { "mono", "mono.exe" };
                    
                    monoExePath = FindOneOnPath(searchNames);

                    if (monoExePath == null && IsWindows())
                    {
                        RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\Novell\Mono");
                        if (key != null)
                        {
                            string version = key.GetValue("DefaultCLR") as string;
                            if (version != null)
                            {
                                key = key.OpenSubKey(version);
                                if (key != null)
                                {
                                    string installDir = key.GetValue("SdkInstallRoot") as string;
                                    if (installDir != null)
                                        monoExePath = Path.Combine(installDir, @"bin\mono.exe");
                                }
                            }
                        }
                    }

                    if (monoExePath == null)
                        return "mono";
                }

                return monoExePath;
            }
        }

        private static string FindOneOnPath(string[] names)
        {
            //foreach (string dir in Environment.GetEnvironmentVariable("path").Split(new char[] { Path.PathSeparator }))
            //    foreach (string name in names)
            //    {
            //        string fullPath = Path.Combine(dir, name);
            //        if (File.Exists(fullPath))
            //            return name;
            //    }

            return null;
        }

        private static bool IsWindows()
        {
            return Environment.OSVersion.Platform == PlatformID.Win32NT;
        }
        #endregion

        #region ApplicationDataDirectory
        private static string applicationDirectory;
        public static string ApplicationDirectory
        {
            get {
                return applicationDirectory ?? (applicationDirectory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "NUnit"));
            }
        }
        #endregion

        #region HelpUrl
        public static string HelpUrl
        {
            get
            {
                string helpUrl = ConfigurationManager.AppSettings["helpUrl"];

                if (helpUrl == null)
                {
                    helpUrl = "http://nunit.org";
                    string dir = Path.GetDirectoryName(NUnitBinDirectory);
                    if ( dir != null )
                    {
                        dir = Path.GetDirectoryName(dir);
                        if ( dir != null )
                        {
                            string localPath = Path.Combine(dir, @"doc/index.html");
                            if (File.Exists(localPath))
                            {
                                var uri = new UriBuilder {Scheme = "file", Host = "localhost", Path = localPath};
                                helpUrl = uri.ToString();
                            }
                        }
                    }
                }

                return helpUrl;
            }
        }
        #endregion

        #endregion
    }
}
