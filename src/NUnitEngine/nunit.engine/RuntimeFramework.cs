// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.Win32;

namespace NUnit.Engine
{
    /// <summary>
    /// Enumeration identifying profile of .NET Framework
    /// </summary>
    public enum Profile
    {
        /// <summary>Profile Unspecified</summary>
        Unspecified,
        /// <summary>Client Profile</summary>
        Client,
        /// <summary>Full Profile</summary>
        Full
    }

    /// <summary>
    /// RuntimeFramework represents a particular version
    /// of a common language runtime implementation.
    /// </summary>
    [Serializable]
    public sealed class RuntimeFramework : IRuntimeFramework
    {
        #region Static and Instance Fields

        /// <summary>
        /// DefaultVersion is an empty Version, used to indicate that
        /// NUnit should select the CLR version to use for the test.
        /// </summary>
        public static readonly Version DefaultVersion = new Version(0, 0);

        private static RuntimeFramework currentFramework;
        private static RuntimeFramework[] availableFrameworks;

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
        public RuntimeFramework(RuntimeType runtime, Version version)
            : this(runtime, version, Profile.Unspecified)
        {
        }

        /// <summary>
        /// Construct from a runtime type, version and profile. If the version has
        /// two parts, it is taken as a framework version. If it has three
        /// or more, it is taken as a CLR version. In either case, the other
        /// version is deduced based on the runtime type and provided version.
        /// </summary>
        /// <param name="runtime">The runtime type of the framework</param>
        /// <param name="version">The version of the framework</param>
        public RuntimeFramework(RuntimeType runtime, Version version, Profile profile)
        {
            Runtime = runtime;

            if (version.Build < 0)
                InitFromFrameworkVersion(version);
            else
                InitFromClrVersion(version);

            Profile = profile;

            DisplayName = GetDefaultDisplayName(runtime, FrameworkVersion, profile);
        }

        private void InitFromFrameworkVersion(Version version)
        {
            this.FrameworkVersion = this.ClrVersion = version;

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
                                        this.ClrVersion = Runtime == RuntimeType.Mono
                                            ? new Version(1, 1, 4322)
                                            : new Version(1, 0, 3705);
                                        break;
                                    case 1:
                                        if (Runtime == RuntimeType.Mono)
                                            this.FrameworkVersion = new Version(1, 0);
                                        this.ClrVersion = new Version(1, 1, 4322);
                                        break;
                                    default:
                                        ThrowInvalidFrameworkVersion(version);
                                        break;
                                }
                                break;
                            case 2:
                            case 3:
                                this.ClrVersion = new Version(2, 0, 50727);
                                break;
                            case 4:
                                this.ClrVersion = new Version(4, 0, 30319);
                                break;
                            default:
                                ThrowInvalidFrameworkVersion(version);
                                break;
                        }
                        break;

                    case RuntimeType.Silverlight:
                        this.ClrVersion = version.Major >= 4
                            ? new Version(4, 0, 60310)
                            : new Version(2, 0, 50727);
                        break;
                }
        }

        private static void ThrowInvalidFrameworkVersion(Version version)
        {
            throw new ArgumentException("Unknown framework version " + version.ToString(), "version");
        }

        private void InitFromClrVersion(Version version)
        {
            this.FrameworkVersion = new Version(version.Major, version.Minor);
            this.ClrVersion = version;
            if (Runtime == RuntimeType.Mono && version.Major == 1)
                this.FrameworkVersion = new Version(1, 0);
        }

        #endregion

        #region IRuntimeFramework Explicit Implementation

        /// <summary>
        /// Gets the inique Id for this runtime, such as "net-4.5"
        /// </summary>
        string IRuntimeFramework.Id
        {
            get { return this.ToString(); }
        }

        /// <summary>
        /// Gets the display name of the framework, such as ".NET 4.5"
        /// </summary>
        string IRuntimeFramework.DisplayName
        {
            get { return this.DisplayName; }
        }

        /// <summary>
        /// Gets the framework version: usually contains two components, Major
        /// and Minor, which match the corresponding CLR components, but not always.
        /// </summary>
        Version IRuntimeFramework.FrameworkVersion
        {
            get { return this.FrameworkVersion; }
        }

        /// <summary>
        /// Gets the Version of the CLR for this framework
        /// </summary>
        Version IRuntimeFramework.ClrVersion
        {
            get { return this.ClrVersion; }
        }

        /// <summary>
        /// Gets a string representing the particular profile installed,
        /// or null if there is no profile. Currently. the only defined 
        /// values are Full and Client.
        /// </summary>
        string IRuntimeFramework.Profile
        {
            get { return Profile.ToString(); }
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
                if (currentFramework == null)
                {
                    Type monoRuntimeType = Type.GetType("Mono.Runtime", false);
                    bool isMono = monoRuntimeType != null;

                    RuntimeType runtime = isMono
                        ? RuntimeType.Mono
                        : Environment.OSVersion.Platform == PlatformID.WinCE
                            ? RuntimeType.NetCF
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
                            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\.NETFramework");
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
                        else if (major == 4 && Type.GetType("System.Reflection.AssemblyMetadataAttribute") != null)
                        {
                            minor = 5;
                        }

                    currentFramework = new RuntimeFramework(runtime, new Version(major, minor));
                    currentFramework.ClrVersion = Environment.Version;

                    if (isMono)
                    {
                        MethodInfo getDisplayNameMethod = monoRuntimeType.GetMethod(
                            "GetDisplayName", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.ExactBinding);
                        if (getDisplayNameMethod != null)
                            currentFramework.DisplayName = (string)getDisplayNameMethod.Invoke(null, new object[0]);
                    }
                }

                return currentFramework;
            }
        }

        /// <summary>
        /// Gets an array of all available frameworks
        /// </summary>
        // TODO: Special handling for netcf
        public static RuntimeFramework[] AvailableFrameworks
        {
            get
            {
                if (availableFrameworks == null)
                {
                    List<RuntimeFramework> frameworks = new List<RuntimeFramework>();

                    AppendDotNetFrameworks(frameworks);
                    AppendDefaultMonoFramework(frameworks);
                    // NYI
                    //AppendMonoFrameworks(frameworks);

                    availableFrameworks = frameworks.ToArray();
                }

                return availableFrameworks;
            }
        }

        /// <summary>
        /// Returns true if the current RuntimeFramework is available.
        /// In the current implementation, only Mono and Microsoft .NET
        /// are supported.
        /// </summary>
        /// <returns>True if it's available, false if not</returns>
        public bool IsAvailable
        {
            get
            {
                foreach (RuntimeFramework framework in AvailableFrameworks)
                    if (framework.Supports(this))
                        return true;

                return false;
            }
        }

        /// <summary>
        /// The type of this runtime framework
        /// </summary>
        public RuntimeType Runtime { get; private set; }

        /// <summary>
        /// The framework version for this runtime framework
        /// </summary>
        public Version FrameworkVersion { get; private set; }

        /// <summary>
        /// The CLR version for this runtime framework
        /// </summary>
        public Version ClrVersion { get; private set; }

        /// <summary>
        /// The .NET Framework Profile for this framwork, where relevant
        /// </summary>
        public Profile Profile { get; private set; }

        /// <summary>
        /// Return true if any CLR version may be used in
        /// matching this RuntimeFramework object.
        /// </summary>
        public bool AllowAnyVersion
        {
            get { return this.ClrVersion == DefaultVersion; }
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
        /// a Version prefixed by 'v'.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static RuntimeFramework Parse(string s)
        {
            RuntimeType runtime = RuntimeType.Any;
            Version version = DefaultVersion;

            string[] parts = s.Split(new char[] { '-' });
            if (parts.Length == 2)
            {
                runtime = (RuntimeType)System.Enum.Parse(typeof(RuntimeType), parts[0], true);
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
                runtime = (RuntimeType)System.Enum.Parse(typeof(RuntimeType), s, true);
            }
            else
            {
                version = new Version(s);
            }

            return new RuntimeFramework(runtime, version);
        }

        /// <summary>
        /// Returns the best available framework that matches a target framework.
        /// If the target framework has a build number specified, then an exact
        /// match is needed. Otherwise, the matching framework with the highest
        /// build number is used.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static RuntimeFramework GetBestAvailableFramework(RuntimeFramework target)
        {
            RuntimeFramework result = target;

            if (target.ClrVersion.Build < 0)
            {
                foreach (RuntimeFramework framework in AvailableFrameworks)
                    if (framework.Supports(target) &&
                        framework.ClrVersion.Build > result.ClrVersion.Build)
                    {
                        result = framework;
                    }
            }

            return result;
        }

        /// <summary>
        /// Overridden to return the short name of the framework
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.AllowAnyVersion)
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
        /// <returns><c>true</c> on match, otherwise <c>false</c></returns>
        public bool Supports(RuntimeFramework target)
        {
            if (this.Runtime != RuntimeType.Any
                && target.Runtime != RuntimeType.Any
                && this.Runtime != target.Runtime)
                return false;

            if (this.AllowAnyVersion || target.AllowAnyVersion)
                return true;

            return VersionsMatch(this.ClrVersion, target.ClrVersion)
                && this.FrameworkVersion.Major >= target.FrameworkVersion.Major
                && this.FrameworkVersion.Minor >= target.FrameworkVersion.Minor;
        }

        #endregion

        #region Helper Methods

        private static bool IsRuntimeTypeName(string name)
        {
            foreach (string item in Enum.GetNames(typeof(RuntimeType)))
                if (item.ToLower() == name.ToLower())
                    return true;

            return false;
        }

        private static string GetDefaultDisplayName(RuntimeType runtime, Version version, Profile profile)
        {
            string displayName;

            if (version == DefaultVersion)
                displayName = runtime.ToString();
            else if (runtime == RuntimeType.Any)
                displayName = "v" + version.ToString();
            else
                displayName = runtime.ToString() + " " + version.ToString();

            if (profile != Profile.Unspecified && profile != Profile.Full)
                displayName += " - " + profile.ToString();

            return displayName;
        }

        private static bool VersionsMatch(Version v1, Version v2)
        {
            return v1.Major == v2.Major &&
                   v1.Minor == v2.Minor &&
                  (v1.Build < 0 || v2.Build < 0 || v1.Build == v2.Build) &&
                  (v1.Revision < 0 || v2.Revision < 0 || v1.Revision == v2.Revision);
        }

        private static void AppendMonoFrameworks(List<RuntimeFramework> frameworks)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                AppendAllMonoFrameworks(frameworks);
            else
                AppendDefaultMonoFramework(frameworks);
        }

        private static void AppendAllMonoFrameworks(List<RuntimeFramework> frameworks)
        {
            // TODO: Find multiple installed Mono versions under Linux
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                // Use registry to find alternate versions
                RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\Novell\Mono");
                if (key == null) return;

                foreach (string version in key.GetSubKeyNames())
                {
                    RegistryKey subKey = key.OpenSubKey(version);
                    if (subKey == null) continue;

                    string monoPrefix = subKey.GetValue("SdkInstallRoot") as string;

                    AppendMonoFramework(frameworks, monoPrefix, version);
                }
            }
            else
                AppendDefaultMonoFramework(frameworks);
        }

        // This method works for Windows and Linux but currently
        // is only called under Linux.
        private static void AppendDefaultMonoFramework(List<RuntimeFramework> frameworks)
        {
            string monoPrefix = null;
            string version = null;

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\Novell\Mono");
                if (key != null)
                {
                    version = key.GetValue("DefaultCLR") as string;
                    if (version != null && version != "")
                    {
                        key = key.OpenSubKey(version);
                        if (key != null)
                            monoPrefix = key.GetValue("SdkInstallRoot") as string;
                    }
                }
            }
            else // Assuming we're currently running Mono - change if more runtimes are added
            {
                string libMonoDir = Path.GetDirectoryName(typeof(object).Assembly.Location);
                monoPrefix = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(libMonoDir)));
            }

            AppendMonoFramework(frameworks, monoPrefix, version);
        }

        private static void AppendMonoFramework(List<RuntimeFramework> frameworks, string monoPrefix, string version)
        {
            if (monoPrefix != null)
            {
                string displayFmt = version != null
                    ? "Mono " + version + " - {0} Profile"
                    : "Mono {0} Profile";

                if (File.Exists(Path.Combine(monoPrefix, "lib/mono/1.0/mscorlib.dll")))
                {
                    RuntimeFramework framework = new RuntimeFramework(RuntimeType.Mono, new Version(1, 1, 4322));
                    framework.DisplayName = string.Format(displayFmt, "1.0");
                    frameworks.Add(framework);
                }

                if (File.Exists(Path.Combine(monoPrefix, "lib/mono/2.0/mscorlib.dll")))
                {
                    RuntimeFramework framework = new RuntimeFramework(RuntimeType.Mono, new Version(2, 0, 50727));
                    framework.DisplayName = string.Format(displayFmt, "2.0");
                    frameworks.Add(framework);
                }

                if (Directory.Exists(Path.Combine(monoPrefix, "lib/mono/3.5")))
                {
                    RuntimeFramework framework = new RuntimeFramework(RuntimeType.Mono, new Version(2, 0, 50727));
                    framework.FrameworkVersion = new Version(3,5);
                    framework.DisplayName = string.Format(displayFmt, "3.5");
                    frameworks.Add(framework);
                }

                if (File.Exists(Path.Combine(monoPrefix, "lib/mono/4.0/mscorlib.dll")))
                {
                    RuntimeFramework framework = new RuntimeFramework(RuntimeType.Mono, new Version(4, 0, 30319));
                    framework.DisplayName = string.Format(displayFmt, "4.0");
                    frameworks.Add(framework);
                }

                if (File.Exists(Path.Combine(monoPrefix, "lib/mono/4.5/mscorlib.dll")))
                {
                    RuntimeFramework framework = new RuntimeFramework(RuntimeType.Mono, new Version(4, 0, 30319));
                    framework.FrameworkVersion = new Version(4,5);
                    framework.DisplayName = string.Format(displayFmt, "4.5");
                    frameworks.Add(framework);
                }
            }
        }

        private static void AppendDotNetFrameworks(List<RuntimeFramework> frameworks)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                // Handle Version 1.0, using a different registry key
                AppendExtremelyOldDotNetFrameworkVersions(frameworks);

                RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\NET Framework Setup\NDP");
                if (key != null)
                {
                    foreach (string name in key.GetSubKeyNames())
                    {
                        if (name.StartsWith("v") && name != "v4.0") // v4.0 is a duplicate, legacy key
                        {
                            var versionKey = key.OpenSubKey(name);
                            if (versionKey == null) continue;

                            if (name.StartsWith("v4", StringComparison.Ordinal))
                                // Version 4 and 4.5
                                AppendDotNetFourFrameworkVersions(frameworks, versionKey);
                            else
                                // Versions 1.1 through 3.5
                                AppendOlderDotNetFrameworkVersion(frameworks, versionKey, new Version(name.Substring(1)));
                        }
                    }
                }
            }
        }

        private static void AppendExtremelyOldDotNetFrameworkVersions(List<RuntimeFramework> frameworks)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\.NETFramework\policy\v1.0");
            if (key != null)
                foreach (string build in key.GetValueNames())
                    frameworks.Add(new RuntimeFramework(RuntimeType.Net, new Version("1.0." + build)));
        }

        private static void AppendOlderDotNetFrameworkVersion(List<RuntimeFramework> frameworks, RegistryKey versionKey, Version version)
        {
            if (CheckInstallDword(versionKey))
                frameworks.Add(new RuntimeFramework(RuntimeType.Net, version));
        }

        // Note: this method cannot be generalized past V4, because (a)  it has
        // specific code for detecting .NET 4.5 and (b) we don't know what
        // microsoft will do in the future
        private static void AppendDotNetFourFrameworkVersions(List<RuntimeFramework> frameworks, RegistryKey versionKey)
        {
            foreach (Profile profile in new Profile[] { Profile.Full, Profile.Client })
            {
                var profileKey = versionKey.OpenSubKey(profile.ToString());
                if (profileKey == null) continue;
                
                if (CheckInstallDword(profileKey))
                {
                    var framework = new RuntimeFramework(RuntimeType.Net, new Version(4, 0), profile);
                    framework.Profile = profile;
                    frameworks.Add(framework);

                    var release = (int)profileKey.GetValue("Release", 0);
                    if (release > 0)
                    {
                        framework = new RuntimeFramework(RuntimeType.Net, new Version(4, 5));
                        frameworks.Add(framework);
                    }
                    return;     //If full profile found, return and don't check for client profile
                }
            }
        }

        private static bool CheckInstallDword(RegistryKey key)
        {
            return ((int)key.GetValue("Install", 0) == 1);
        }

        #endregion
    }
}
