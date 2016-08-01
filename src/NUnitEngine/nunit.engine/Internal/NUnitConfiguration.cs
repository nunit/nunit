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
using Microsoft.Win32;

namespace NUnit.Engine.Internal
{
    /// <summary>
    /// Provides static methods for accessing configuration info
    /// </summary>
    public static class NUnitConfiguration
    {
        #region Public Properties

        #region EngineDirectory

        private static string _engineDirectory;
        public static string EngineDirectory
        {
            get
            {
                if (_engineDirectory == null)
                    _engineDirectory =
                        AssemblyHelper.GetDirectoryName(Assembly.GetExecutingAssembly());

                return _engineDirectory;
            }
        }

        #endregion

        #region MonoExePath

        private static string _monoExePath;
        public static string MonoExePath
        {
            get
            {
                if (_monoExePath == null && IsWindows())
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
                                    _monoExePath = Path.Combine(installDir, @"bin\mono.exe");
                            }
                        }
                    }
                }

                if (_monoExePath == null)
                    _monoExePath = "mono";

                return _monoExePath;
            }
        }

        private static bool IsWindows()
        {
            return Environment.OSVersion.Platform == PlatformID.Win32NT;
        }

        #endregion

        #region ApplicationDataDirectory

        private static string _applicationDirectory;
        public static string ApplicationDirectory
        {
            get
            {
                if (_applicationDirectory == null)
                {
                    _applicationDirectory = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "NUnit");
                }

                return _applicationDirectory;
            }
        }

        #endregion

        #endregion
    }
}
