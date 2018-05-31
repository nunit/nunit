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

#if !(NETSTANDARD1_6 || NETSTANDARD2_0)
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Win32;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Enumeration identifying a common language
    /// runtime implementation.
    /// </summary>
    public enum RuntimeType
    {
        /// <summary>Any supported runtime framework</summary>
        Any,
        /// <summary>Microsoft .NET Framework</summary>
        Net,
        /// <summary>Microsoft Shared Source CLI</summary>
        SSCLI,
        /// <summary>Mono</summary>
        Mono,
        /// <summary>MonoTouch</summary>
        MonoTouch
    }

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
            Type monoRuntimeType = Type.GetType("Mono.Runtime", false);
            Type monoTouchType = Type.GetType("MonoTouch.UIKit.UIApplicationDelegate,monotouch");
            bool isMonoTouch = monoTouchType != null;
            bool isMono = monoRuntimeType != null;

            RuntimeType runtime = isMonoTouch
                ? RuntimeType.MonoTouch
                : isMono
                    ? RuntimeType.Mono
                    : RuntimeType.Net;

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
            else /* It's windows */
            if (major == 2)
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\.NETFramework"))
                {
                    if (key != null)
                    {
                        string installRoot = key.GetValue("InstallRoot") as string;
                        if (installRoot != null)
                        {
                            if (Directory.Exists(Path.Combine(installRoot, "v3.5")))
                            {
                                major = 3;
                                minor = 5;
                            }
                            else if (Directory.Exists(Path.Combine(installRoot, "v3.0")))
                            {
                                major = 3;
                                minor = 0;
                            }
                        }
                    }
                }
            }
            else if (major == 4 && Type.GetType("System.Reflection.AssemblyMetadataAttribute") != null)
            {
                minor = 5;
            }

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
                    case RuntimeType.Net:
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
                    version = new Version(vstring);
            }
            else if (char.ToLower(s[0]) == 'v')
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
                return Runtime.ToString().ToLower();
            }
            else
            {
                string vstring = FrameworkVersion.ToString();
                if (Runtime == RuntimeType.Any)
                    return "v" + vstring;
                else
                    return Runtime.ToString().ToLower() + "-" + vstring;
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

            return FrameworkVersion.Major >= target.FrameworkVersion.Major && FrameworkVersion.Minor >= target.FrameworkVersion.Minor;
        }

        #endregion

        #region Helper Methods

        private static bool IsRuntimeTypeName(string name)
        {
            return Enum.GetNames( typeof(RuntimeType)).Any( item => item.ToLower() == name.ToLower() );
        }

        private static string GetDefaultDisplayName(RuntimeType runtime, Version version)
        {
            if (version == DefaultVersion)
                return runtime.ToString();
            else if (runtime == RuntimeType.Any)
                return "v" + version;
            else
                return runtime + " " + version;
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
#endif
