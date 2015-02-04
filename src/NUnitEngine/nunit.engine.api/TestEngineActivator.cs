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
using System.IO;
using System.Reflection;
using Microsoft.Win32;

namespace NUnit.Engine
{
    // This is a preliminary implementation, which requires a specific version of the engine and only
    // loads it if it can be found in the current AppBase and ProbingPath or in the GAC.
    //
    // TODO: Find the engine in established locations known to NUnit or stored in the registry.
    //
    // TODO: Find the best available version of the engine.

    /// <summary>
    /// TestEngineActivator creates an instance of the test engine and returns an ITestEngine interface.
    /// </summary>
    public static class TestEngineActivator
    {
        internal static readonly Version DefaultMinimumVersion = new Version(3, 0);
        internal static readonly Version DefaultMaximumVersion = new Version(255, 0);

        private const string DefaultAssemblyName = "nunit.engine.dll";
        internal const string DefaultTypeName = "NUnit.Engine.TestEngine";

        private const string NunitInstallRegKey = @"SOFTWARE\Nunit.org";

        #region Public Methods

        /// <summary>
        /// Create an instance of the test engine.
        /// </summary>
        /// <remarks>If private copy is false, the search order is the NUnit install directory for the current user, then
        /// the install directory for the local machine and finally the current AppDomain's ApplicationBase.</remarks>
        /// <param name="privateCopy">if set to <c>true</c> loads the engine found in the application base directory, 
        /// otherwise searches for the test engine with the highest version installed. Defaults to <c>true</c>.</param>
        /// <exception cref="NUnitEngineNotFoundException">Thrown when a test engine of the required minimum version is not found</exception>
        /// <returns>An <see cref="NUnit.Engine.ITestEngine"/></returns>
        public static ITestEngine CreateInstance(bool privateCopy = false)
        {
            return CreateInstance(DefaultMinimumVersion, DefaultMaximumVersion, privateCopy);
        }

        /// <summary>
        /// Create an instance of the test engine with a minimum version.
        /// </summary>
        /// <remarks>If private copy is false, the search order is the NUnit install directory for the current user, then
        /// the install directory for the local machine and finally the current AppDomain's ApplicationBase.</remarks>
        /// <param name="minVersion">The minimum version of the engine to return inclusive.</param>
        /// <param name="privateCopy">if set to <c>true</c> loads the engine found in the application base directory, 
        /// otherwise searches for the test engine with the highest version installed. Defaults to <c>true</c>.</param>
        /// <exception cref="NUnitEngineNotFoundException">Thrown when a test engine of the given minimum version is not found</exception>
        /// <returns>An <see cref="ITestEngine"/></returns>
        public static ITestEngine CreateInstance(Version minVersion, bool privateCopy = false)
        {
            return CreateInstance(minVersion, DefaultMaximumVersion, privateCopy);
        }

        /// <summary>
        /// Create an instance of the test engine with a minimum and maximum version.
        /// </summary>
        /// <remarks>If private copy is false, the search order is the NUnit install directory for the current user, then
        /// the install directory for the local machine and finally the current AppDomain's ApplicationBase.</remarks>
        /// <param name="minVersion">The minimum version of the engine to return inclusive.</param>
        /// <param name="maxVersion">The maximum version of the engine to return inclusive.</param>
        /// <param name="privateCopy">if set to <c>true</c> loads the engine found in the application base directory, 
        /// otherwise searches for the test engine with the highest version installed. Defaults to <c>true</c>.</param>
        /// <exception cref="NUnitEngineNotFoundException">Thrown when a test engine of the given minimum and maximum version is not found</exception>
        /// <returns>An <see cref="ITestEngine"/></returns>
        public static ITestEngine CreateInstance(Version minVersion, Version maxVersion, bool privateCopy = false)
        {
            try
            {
                Assembly engine = FindNewestEngine(minVersion, maxVersion, privateCopy);
                if (engine == null)
                {
                    throw new NUnitEngineNotFoundException(minVersion, maxVersion);
                }
                return (ITestEngine)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(engine.CodeBase, DefaultTypeName);
            }
            catch (NUnitEngineNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to load the test engine", ex);
            }
        }

        #endregion

        #region Private Methods

        private static Assembly FindNewestEngine(Version minVersion, Version maxVersion, bool privateCopy)
        {
            var newestVersionFound = new Version();
            Assembly newestAssemblyFound = null;
            string path;
            if (!privateCopy)
            {
                // Check the install for the current user
                path = FindEngineInRegistry(Registry.CurrentUser, NunitInstallRegKey);
                newestAssemblyFound = CheckPathForEngine(path, minVersion, maxVersion, ref newestVersionFound, null);

                // check the install for the local machine
                path = FindEngineInRegistry(Registry.LocalMachine, NunitInstallRegKey);
                newestAssemblyFound = CheckPathForEngine(path, minVersion, maxVersion, ref newestVersionFound, newestAssemblyFound);
            }
            // Check the Application BaseDirectory
            path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DefaultAssemblyName);
            return CheckPathForEngine(path, minVersion, maxVersion, ref newestVersionFound, newestAssemblyFound);
        }

        private static Assembly CheckPathForEngine(string path, Version minVersion, Version maxVersion, ref Version newestVersionFound, Assembly newestAssemblyFound)
        {
            try
            {
                if (path != null && File.Exists(path))
                {
                    var ass = Assembly.ReflectionOnlyLoadFrom(path);
                    var ver = ass.GetName().Version;
                    if (ver >= minVersion && ver <= maxVersion && ver > newestVersionFound)
                    {
                        newestVersionFound = ver;
                        newestAssemblyFound = ass;
                    }
                }
            }
            catch (Exception){}
            return newestAssemblyFound;
        }

        private static string FindEngineInRegistry(RegistryKey rootKey, string subKey)
        {
            try
            {
                using (var key = rootKey.OpenSubKey(subKey, false))
                {
                    if (key != null)
                    {
                        return key.GetValue("Engine") as string;
                    }
                }
            }
            catch (Exception) { }
            return null;
        }

        #endregion
    }
}
