// ***********************************************************************
// Copyright (c) 2007-2016 Charlie Poole, Rob Prouse
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
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
#if NETSTANDARD2_0
using System.Runtime.Versioning;
#else
using Microsoft.Win32;
#endif

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// RuntimeFramework represents a particular version
    /// of a common language runtime implementation.
    /// </summary>
    [Serializable]
    public sealed class RuntimeFramework
    {
        // NOTE: This version of RuntimeFramework is for use
        // within the NUnit framework assembly. It is simpler
        // than the version in the test engine because it does
        // not need to know what frameworks are available,
        // only what framework is currently running.
        #region Static and Instance Fields

        /// <summary>
        /// DefaultVersion is an empty Version, used to indicate that
        /// NUnit should select the CLR version to use for the test.
        /// </summary>
        public static readonly Version DefaultVersion = new Version(0,0);

        private static readonly Lazy<RuntimeFramework> currentFramework = new Lazy<RuntimeFramework>(() =>
        {
            Type monoRuntimeType = null;
            Type monoTouchType = null;

            try
            {
                monoRuntimeType = Type.GetType("Mono.Runtime", false);
                monoTouchType = Type.GetType("MonoTouch.UIKit.UIApplicationDelegate,monotouch", false);
            }
            catch
            {
                //If exception thrown, assume no valid installation
            }

            bool isMonoTouch = monoTouchType != null;
            bool isMono = monoRuntimeType != null;
            bool isNetCore = !isMono && !isMonoTouch && IsNetCore();

            RuntimeType runtime = isMonoTouch
                ? RuntimeType.MonoTouch
                : isMono
                    ? RuntimeType.Mono
                    : isNetCore
                        ? RuntimeType.NetCore
                        : RuntimeType.NetFramework;

            int major = Environment.Version.Major;
            int minor = Environment.Version.Minor;

            if (isMono)
            {
                switch (major)
                {
                    case 1:
                        minor = 0;
                        break;
                    case 2:
                        major = 3;
                        minor = 5;
                        break;
                }
            }
            else if (isNetCore)
            {
                major = 0;
                minor = 0;
            }
            else /* It's windows */
#if NETSTANDARD2_0
            {
                minor = 5;
            }
#else
            if (major == 2)
            {
                // The only assembly we compile that can run on the v2 CLR uses .NET Framework 3.5.
                major = 3;
                minor = 5;
            }
            else if (major == 4 && Type.GetType("System.Reflection.AssemblyMetadataAttribute") != null)
            {
                minor = 5;
            }
#endif

            var currentFramework = new RuntimeFramework( runtime, new Version (major, minor) )
            {
                ClrVersion = Environment.Version
            };

            if (isMono)
            {
                MethodInfo getDisplayNameMethod = monoRuntimeType.GetMethod(
                    "GetDisplayName", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.ExactBinding);
                if (getDisplayNameMethod != null)
                    currentFramework.DisplayName = (string)getDisplayNameMethod.Invoke(null, new object[0]);
            }
            return currentFramework;
        });

        #endregion

        #region Constructor

        /// <summary>
        /// Construct from a runtime type and version. If the version has
        /// two parts, it is taken as a framework version. If it has three
        /// or more, it is taken as a CLR version. In either case, the other
        /// version is deduced based on the runtime type and provided version.
        /// </summary>
        /// <param name="runtime">The runtime type of the framework</param>
        /// <param name="version">The version of the framework</param>
        public RuntimeFramework( RuntimeType runtime, Version version)
        {
            Runtime = runtime;

            if (version.Build < 0)
                InitFromFrameworkVersion(version);
            else
                InitFromClrVersion(version);

            DisplayName = GetDefaultDisplayName(runtime, version);
        }

        private void InitFromFrameworkVersion(Version version)
        {
            FrameworkVersion = ClrVersion = version;

            if (version.Major > 0) // 0 means any version
                switch (Runtime)
                {
                    case RuntimeType.NetCore:
                        ClrVersion = new Version(4, 0, 30319);
                        break;

                    case RuntimeType.NetFramework:
                    case RuntimeType.Mono:
                    case RuntimeType.Any:
                        switch (version.Major)
                        {
                            case 1:
                                switch (version.Minor)
                                {
                                    case 0:
                                        ClrVersion = Runtime == RuntimeType.Mono
                                            ? new Version(1, 1, 4322)
                                            : new Version(1, 0, 3705);
                                        break;
                                    case 1:
                                        if (Runtime == RuntimeType.Mono)
                                            FrameworkVersion = new Version(1, 0);
                                        ClrVersion = new Version(1, 1, 4322);
                                        break;
                                    default:
                                        ThrowInvalidFrameworkVersion(version);
                                        break;
                                }
                                break;
                            case 2:
                            case 3:
                                ClrVersion = new Version(2, 0, 50727);
                                break;
                            case 4:
                                ClrVersion = new Version(4, 0, 30319);
                                break;
                            default:
                                ThrowInvalidFrameworkVersion(version);
                                break;
                        }
                        break;
                }
        }

        private static void ThrowInvalidFrameworkVersion(Version version)
        {
            throw new ArgumentException("Unknown framework version " + version, nameof(version));
        }

        private void InitFromClrVersion(Version version)
        {
            FrameworkVersion = new Version(version.Major, version.Minor);
            ClrVersion = version;
            if (Runtime == RuntimeType.Mono && version.Major == 1)
                FrameworkVersion = new Version(1, 0);
            if (Runtime == RuntimeType.NetFramework && version.Major == 4 && version.Minor == 5)
                ClrVersion = new Version(4, 0, 30319);
            if (Runtime == RuntimeType.NetCore)
                ClrVersion = new Version(4, 0, 30319);
        }

        #endregion

        #region Properties
        /// <summary>
        /// Static method to return a RuntimeFramework object
        /// for the framework that is currently in use.
        /// </summary>
        public static RuntimeFramework CurrentFramework
        {
            get
            {
                return currentFramework.Value;
            }
        }

        /// <summary>
        /// The type of this runtime framework
        /// </summary>
        public RuntimeType Runtime { get; }

        /// <summary>
        /// The framework version for this runtime framework
        /// </summary>
        public Version FrameworkVersion { get; private set; }

        /// <summary>
        /// The CLR version for this runtime framework
        /// </summary>
        public Version ClrVersion { get; private set; }

        /// <summary>
        /// Return true if any CLR version may be used in
        /// matching this RuntimeFramework object.
        /// </summary>
        public bool AllowAnyVersion
        {
            get { return ClrVersion == DefaultVersion; }
        }

        /// <summary>
        /// Returns the Display name for this framework
        /// </summary>
        public string DisplayName { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Parses a string representing a RuntimeFramework.
        /// The string may be just a RuntimeType name or just
        /// a Version or a hyphenated RuntimeType-Version or
        /// a Version prefixed by 'versionString'.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static RuntimeFramework Parse(string s)
        {
            RuntimeType runtime = RuntimeType.Any;
            Version version = DefaultVersion;

            string[] parts = s.Split('-');
            if (parts.Length == 2)
            {
                runtime = (RuntimeType)Enum.Parse(typeof(RuntimeType), parts[0], true);
                string vstring = parts[1];
                if (vstring != "")
                {
                    version = new Version(vstring);

                    if (runtime == RuntimeType.NetFramework && version.Major >= 5)
                        runtime = RuntimeType.NetCore;
                }
            }
            else if (s.StartsWith("v", StringComparison.OrdinalIgnoreCase))
            {
                version = new Version(s.Substring(1));
            }
            else if (IsRuntimeTypeName(s))
            {
                runtime = (RuntimeType)Enum.Parse(typeof(RuntimeType), s, true);
            }
            else
            {
                version = new Version(s);
            }

            return new RuntimeFramework(runtime, version);
        }

        /// <summary>
        /// Overridden to return the short name of the framework
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (AllowAnyVersion)
            {
                return GetShortName(Runtime, FrameworkVersion).ToLowerInvariant();
            }
            else
            {
                string vstring = FrameworkVersion.ToString();
                if (Runtime == RuntimeType.Any)
                    return "v" + vstring;
                else
                    return GetShortName(Runtime, FrameworkVersion).ToLowerInvariant() + "-" + vstring;
            }
        }

        /// <summary>
        /// Returns true if the current framework matches the
        /// one supplied as an argument. Two frameworks match
        /// if their runtime types are the same or either one
        /// is RuntimeType.Any and all specified version components
        /// are equal. Negative (i.e. unspecified) version
        /// components are ignored.
        /// </summary>
        /// <param name="target">The RuntimeFramework to be matched.</param>
        /// <returns>True on match, otherwise false</returns>
        public bool Supports(RuntimeFramework target)
        {
            if (Runtime != RuntimeType.Any
                && target.Runtime != RuntimeType.Any
                && Runtime != target.Runtime)
                return false;

            if (AllowAnyVersion || target.AllowAnyVersion)
                return true;

            if (!VersionsMatch(ClrVersion, target.ClrVersion))
                return false;

            if (FrameworkVersion.Major > target.FrameworkVersion.Major)
                return true;
            return FrameworkVersion.Major == target.FrameworkVersion.Major && FrameworkVersion.Minor >= target.FrameworkVersion.Minor;
        }

        #endregion

        #region Helper Methods

        private static bool IsNetCore()
        {
            if (Environment.Version.Major >= 5) return true;

#if NETSTANDARD2_0
            // Mono versions will throw a TypeLoadException when attempting to run the internal method, so we wrap it in a try/catch
            // block to stop any inlining in release builds and check whether the type exists
            Type runtimeInfoType = Type.GetType("System.Runtime.InteropServices.RuntimeInformation,System.Runtime.InteropServices.RuntimeInformation", false);
            if (runtimeInfoType != null)
            {
                try
                {
                    return IsNetCore_Internal();
                }
                catch (TypeLoadException) { }
            }
#endif

            return false;
        }

#if NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static bool IsNetCore_Internal()
        {
            // Mono versions will throw a TypeLoadException when attempting to run any method that uses RuntimeInformation
            // so we wrap it in a try/catch block in IsNetCore to catch it in case it ever gets this far
            if (System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription.StartsWith(".NET Core", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }
#endif

        private static bool IsRuntimeTypeName(string name)
        {
            return Enum.GetNames(typeof(RuntimeType)).Contains(name, StringComparer.OrdinalIgnoreCase);
        }

        private static string GetShortName(RuntimeType runtime, Version version)
        {
            return runtime == RuntimeType.NetFramework || (runtime == RuntimeType.NetCore && version.Major >= 5)
                ? "Net"
                : runtime.ToString();
        }

        private static string GetDefaultDisplayName(RuntimeType runtime, Version version)
        {
            if (version == DefaultVersion)
                return GetShortName(runtime, version);
            else if (runtime == RuntimeType.Any)
                return "v" + version;
            else
                return GetShortName(runtime, version) + " " + version;
        }

        private static bool VersionsMatch(Version v1, Version v2)
        {
            return v1.Major == v2.Major &&
                   v1.Minor == v2.Minor &&
                  (v1.Build < 0 || v2.Build < 0 || v1.Build == v2.Build) &&
                  (v1.Revision < 0 || v2.Revision < 0 || v1.Revision == v2.Revision);
        }

        #endregion
    }
}
