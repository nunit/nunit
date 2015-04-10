﻿// ***********************************************************************
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
using System.Linq;
using NUnit.Common;
using NUnit.Engine.Internal;

namespace NUnit.Engine.Services
{
    public class RuntimeFrameworkService : IRuntimeFrameworkService, IService
    {
        static readonly Logger log = InternalTrace.GetLogger(typeof(RuntimeFrameworkService));

        /// <summary>
        /// Returns true if the runtime framework represented by
        /// the string passed as an argument is available.
        /// </summary>
        /// <param name="name">A string representing a framework, like 'net-4.0'</param>
        /// <returns>True if the framework is available, false if unavailable or nonexistent</returns>
        public bool IsAvailable(string name)
        {
            var requestedFramework = RuntimeFramework.Parse(name);
            return RuntimeFramework.AvailableFrameworks.Any(framework => FrameworksMatch(requestedFramework, framework));
        }

        private static readonly Version AnyVersion = new Version(0, 0);

        private static bool FrameworksMatch(RuntimeFramework f1, RuntimeFramework f2)
        {
            var rt1 = f1.Runtime;
            var rt2 = f2.Runtime;

            if (rt1 != RuntimeType.Any && rt2 != RuntimeType.Any && rt1 != rt2)
                return false;

            var v1 = f1.ClrVersion;
            var v2 = f2.ClrVersion;

            if (v1 == AnyVersion || v2 == AnyVersion)
                return true;

            return v1.Major == v2.Major &&
                   v1.Minor == v2.Minor &&
                   (v1.Build < 0 || v2.Build < 0 || v1.Build == v2.Build) &&
                   (v1.Revision < 0 || v2.Revision < 0 || v1.Revision == v2.Revision) &&
                   f1.FrameworkVersion.Major == f2.FrameworkVersion.Major &&
                   f1.FrameworkVersion.Minor == f2.FrameworkVersion.Minor;
        }

        /// <summary>
        /// Selects a target runtime framework for a TestPackage based on
        /// the settings in the package and the assemblies themselves.
        /// The package RuntimeFramework setting may be updated as a result
        /// and a string representing the selected runtime is returned.
        /// </summary>
        /// <param name="package">A TestPackage</param>
        /// <returns>A string representing the selected RuntimeFramework</returns>
        public string SelectRuntimeFramework(TestPackage package)
        {
            var currentFramework = RuntimeFramework.CurrentFramework;
            var frameworkSetting = package.GetSetting(PackageSettings.RuntimeFramework, "");
            var requestedFramework = frameworkSetting.Length > 0
                ? RuntimeFramework.Parse(frameworkSetting)
                : new RuntimeFramework(RuntimeType.Any, RuntimeFramework.DefaultVersion);

            log.Debug("Current framework is {0}", currentFramework);
            if (requestedFramework == null)
                log.Debug("No specific framework requested");
            else
                log.Debug("Requested framework is {0}", requestedFramework);

            var targetRuntime = requestedFramework.Runtime;
            var targetVersion = requestedFramework.FrameworkVersion;

            if (targetRuntime == RuntimeType.Any)
                targetRuntime = currentFramework.Runtime;

            if (targetVersion == RuntimeFramework.DefaultVersion)
            {
                if (ServiceContext.UserSettings.GetSetting("Options.TestLoader.RuntimeSelectionEnabled", true))
                {
                    foreach (var assembly in package.TestFiles)
                    {
                        // If the file is not an assembly or doesn't exist, then it can't
                        // contribute any information to the decision, so we skip it.
                        if (PathUtils.IsAssemblyFileType(assembly) && File.Exists(assembly))
                        {
                            using (var reader = new AssemblyReader(assembly))
                            {
                                if (!reader.IsValidPeFile)
                                    log.Debug("{0} is not a valid PE file", assembly);
                                else if (!reader.IsDotNetFile)
                                    log.Debug("{0} is not a managed assembly", assembly);
                                else
                                {
                                    if (reader.ShouldRun32Bit)
                                    {
                                        package.Settings[PackageSettings.RunAsX86] = true;
                                        log.Debug("Assembly {0} will be run x86", assembly);
                                    }

                                    var imageRuntimeVersion = reader.ImageRuntimeVersion;
                                    if (imageRuntimeVersion != null)
                                    {
                                        var v = new Version(imageRuntimeVersion.Substring(1));
                                        log.Debug("Assembly {0} uses version {1}", assembly, v);

                                        // TODO: We are doing two jobs here: (1) getting the
                                        // target version and (2) applying a policy that says
                                        // we run under the highest version of all assemblies.
                                        // We should implement the policy at a higher level.
                                        if (v > targetVersion) targetVersion = v;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                    targetVersion = RuntimeFramework.CurrentFramework.ClrVersion;

                var checkFramework = new RuntimeFramework(targetRuntime, targetVersion);
                if (!checkFramework.IsAvailable || !ServiceContext.TestAgency.IsRuntimeVersionSupported(targetVersion))
                {
                    log.Debug("Preferred version {0} is not installed or this NUnit installation does not support it", targetVersion);
                    if (targetVersion < currentFramework.FrameworkVersion)
                        targetVersion = currentFramework.FrameworkVersion;
                }
            }

            var targetFramework = new RuntimeFramework(targetRuntime, targetVersion);
            package.Settings[PackageSettings.RuntimeFramework] = targetFramework.ToString();

            log.Debug("Test will use {0} framework", targetFramework);

            return targetFramework.ToString();
        }

        #region IService Members

        public ServiceContext ServiceContext { get; set; }

        public void InitializeService()
        {
        }

        public void UnloadService()
        {
        }

        #endregion
    }
}