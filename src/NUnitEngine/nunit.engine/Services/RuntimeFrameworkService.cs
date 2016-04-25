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
using Mono.Cecil;
using NUnit.Common;
using NUnit.Engine.Internal;

namespace NUnit.Engine.Services
{
    public class RuntimeFrameworkService : Service, IRuntimeFrameworkService, IAvailableRuntimes
    {
        static Logger log = InternalTrace.GetLogger(typeof(RuntimeFrameworkService));

        /// <summary>
        /// Gets a list of available runtimes.
        /// </summary>
        public IList<IRuntimeFramework> AvailableRuntimes
        {
            get { return RuntimeFramework.AvailableFrameworks; }
        }

        /// <summary>
        /// Returns true if the runtime framework represented by
        /// the string passed as an argument is available.
        /// </summary>
        /// <param name="name">A string representing a framework, like 'net-4.0'</param>
        /// <returns>True if the framework is available, false if unavailable or nonexistent</returns>
        public bool IsAvailable(string name)
        {
            var requestedFramework = RuntimeFramework.Parse(name);
            foreach (var framework in RuntimeFramework.AvailableFrameworks)
                if (FrameworksMatch(requestedFramework, framework))
                    return true;

            return false;
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
            // Start by examining the provided settings
            RuntimeFramework currentFramework = RuntimeFramework.CurrentFramework;
            string frameworkSetting = package.GetSetting(PackageSettings.RuntimeFramework, "");
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

            // Examine the package 
            ApplyImageData(package);

            // Modify settings if necessary
            targetVersion = package.GetSetting(PackageSettings.ImageRuntimeVersion, targetVersion);
            RuntimeFramework checkFramework = new RuntimeFramework(targetRuntime, targetVersion);
            if (!checkFramework.IsAvailable)
            {
                log.Debug("Preferred version {0} is not installed or this NUnit installation does not support it", targetVersion);
                if (targetVersion < currentFramework.FrameworkVersion)
                    targetVersion = currentFramework.FrameworkVersion;
            }

            RuntimeFramework targetFramework = new RuntimeFramework(targetRuntime, targetVersion);
            package.Settings[PackageSettings.RuntimeFramework] = targetFramework.ToString();

            log.Debug("Test will use {0} framework", targetFramework);

            return targetFramework.ToString();
        }

        #region Helper Methods

        /// <summary>
        /// Use Mono.Cecil to get information about all assemblies and
        /// apply it to the package using special internal keywords.
        /// </summary>
        /// <param name="package"></param>
        private static void ApplyImageData(TestPackage package)
        {
            string packageName = package.FullName;

            Version targetVersion = new Version(0, 0);
            string frameworkName = null;
            bool requiresX86 = false;
            bool requiresAssemblyResolver = false;

            // We are doing two jobs here: (1) in the else clause (below)
            // we get information about a single assembly and record it,
            // (2) in the if clause, we recursively examine all subpackages
            // and then apply policies for promulgating each setting to 
            // a containing package. We could implement the policy part at
            // a higher level, but it seems simplest to do it right here.
            if (package.SubPackages.Count > 0)
            {
                foreach (var subPackage in package.SubPackages)
                {
                    ApplyImageData(subPackage);

                    // Collect the highest version required
                    Version v = subPackage.GetSetting(PackageSettings.ImageRuntimeVersion, new Version(0, 0));
                    if (v > targetVersion) targetVersion = v;

                    // Collect highest framework name 
                    // TODO: This assumes lexical ordering is valid - check it
                    string fn = subPackage.GetSetting(PackageSettings.ImageTargetFrameworkName, "");
                    if (fn != "")
                    {
                        if (frameworkName == null || fn.CompareTo(frameworkName) < 0)
                            frameworkName = fn;
                    }

                    // If any assembly requires X86, then the aggregate package requires it
                    if (subPackage.GetSetting(PackageSettings.ImageRequiresX86, false))
                        requiresX86 = true;

                    if (subPackage.GetSetting(PackageSettings.ImageRequiresDefaultAppDomainAssemblyResolver, false))
                        requiresAssemblyResolver = true;
                }
            }
            else if (File.Exists(packageName) && PathUtils.IsAssemblyFileType(packageName))
            {
                var assemblyDef = AssemblyDefinition.ReadAssembly(packageName);
                var module = assemblyDef.MainModule;

                var NativeEntryPoint = (ModuleAttributes)16;
                var mask = ModuleAttributes.Required32Bit | NativeEntryPoint;

                if (module.Architecture != TargetArchitecture.AMD64 &&
                    module.Architecture != TargetArchitecture.IA64 &&
                    (module.Attributes & mask) != 0)
                {
                    requiresX86 = true;
                    log.Debug("Assembly {0} will be run x86", packageName);
                }

                targetVersion = new Version(module.RuntimeVersion.Substring(1));
                log.Debug("Assembly {0} uses version {1}", packageName, targetVersion);

                foreach (var attr in assemblyDef.CustomAttributes)
                {
                    if (attr.AttributeType.FullName == "System.Runtime.Versioning.TargetFrameworkAttribute")
                    {
                        frameworkName = attr.ConstructorArguments[0].Value as string;
                    }
                    else if (attr.AttributeType.FullName == "NUnit.Framework.TestAssemblyDirectoryResolveAttribute")
                    {
                        requiresAssemblyResolver = true;
                    }
                }
            }

            if (targetVersion.Major > 0)
                package.Settings[PackageSettings.ImageRuntimeVersion] = targetVersion;

            if (!string.IsNullOrEmpty(frameworkName))
                package.Settings[PackageSettings.ImageTargetFrameworkName] = frameworkName;

            package.Settings[PackageSettings.ImageRequiresX86] = requiresX86;
            if (requiresX86)
                package.Settings[PackageSettings.RunAsX86] = true;

            package.Settings[PackageSettings.ImageRequiresDefaultAppDomainAssemblyResolver] = requiresAssemblyResolver;
        }

        #endregion
    }
}
