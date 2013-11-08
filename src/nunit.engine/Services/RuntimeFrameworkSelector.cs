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
using NUnit.Engine.Internal;

namespace NUnit.Engine.Services
{
    public class RuntimeFrameworkSelector : IRuntimeFrameworkSelector, IService
    {
        static Logger log = InternalTrace.GetLogger(typeof(RuntimeFrameworkSelector));

        /// <summary>
        /// Selects a target runtime framework for a TestPackage based on
        /// the settings in the package and the assemblies themselves.
        /// The package RuntimeFramework setting may be updated as a 
        /// result and the selected runtime is returned.
        /// </summary>
        /// <param name="package">A TestPackage</param>
        /// <returns>The selected RuntimeFramework</returns>
        public RuntimeFramework SelectRuntimeFramework(TestPackage package)
        {
            RuntimeFramework currentFramework = RuntimeFramework.CurrentFramework;
            string frameworkSetting = package.GetSetting("RuntimeFramework", "");
            RuntimeFramework requestedFramework = frameworkSetting.Length > 0
                ? RuntimeFramework.Parse(frameworkSetting)
                : new RuntimeFramework(RuntimeType.Any, RuntimeFramework.DefaultVersion);

            log.Debug("Current framework is {0}", currentFramework);
            if (requestedFramework == null)
                log.Debug("No specific framework requested");
            else
                log.Debug("Requested framework is {0}", requestedFramework);

            RuntimeType targetRuntime = requestedFramework.Runtime;
            Version targetVersion = requestedFramework.FrameworkVersion;

            if (targetRuntime == RuntimeType.Any)
                targetRuntime = currentFramework.Runtime;

            if (targetVersion == RuntimeFramework.DefaultVersion)
            {
                if (ServiceContext.UserSettings.GetSetting("Options.TestLoader.RuntimeSelectionEnabled", true))
                    foreach (string assembly in package.TestFiles)
                    {
                        using (AssemblyReader reader = new AssemblyReader(assembly))
                        {
                            Version v = new Version(reader.ImageRuntimeVersion.Substring(1));
                            log.Debug("Assembly {0} uses version {1}", assembly, v);
                            if (v > targetVersion) targetVersion = v;
                        }
                    }
                else
                    targetVersion = RuntimeFramework.CurrentFramework.ClrVersion;

                RuntimeFramework checkFramework = new RuntimeFramework(targetRuntime, targetVersion);
                if (!checkFramework.IsAvailable || !ServiceContext.TestAgency.IsRuntimeVersionSupported(targetVersion))
                {
                    log.Debug("Preferred version {0} is not installed or this NUnit installation does not support it", targetVersion);
                    if (targetVersion < currentFramework.FrameworkVersion)
                        targetVersion = currentFramework.FrameworkVersion;
                }
            }

            RuntimeFramework targetFramework = new RuntimeFramework(targetRuntime, targetVersion);
            package.Settings["RuntimeFramework"] = targetFramework.ToString();

            log.Debug("Test will use {0} framework", targetFramework);

            return targetFramework;
        }

        #region IService Members

        private ServiceContext services;
        public ServiceContext ServiceContext
        {
            get { return services; }
            set { services = value; }
        }

        public void InitializeService()
        {
        }

        public void UnloadService()
        {
        }

        #endregion
    }
}
