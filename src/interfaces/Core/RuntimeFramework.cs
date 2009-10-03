// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org.
// ****************************************************************

using System;
using System.Reflection;
using System.Collections;
using Microsoft.Win32;

namespace NUnit.Core
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
		/// <summary>Microsoft .NET Compact Framework</summary>
		NetCF,
		/// <summary>Microsoft Shared Source CLI</summary>
		SSCLI,
		/// <summary>Mono</summary>
		Mono
	}

	/// <summary>
	/// RuntimeFramework represents a particular version
	/// of a common language runtime implementation.
	/// </summary>
    [Serializable]
	public sealed class RuntimeFramework
    {
        #region Static and Instance Fields
        private static RuntimeFramework currentFramework;
        private static Version[] knownVersions = new Version[] {
            new Version( "1.0.3705" ),
            new Version( "1.1.4322" ),
            new Version( "2.0.50727" ) };

        private RuntimeType runtime;
		private Version version;
		private string displayName;
        #endregion

        #region Static Properties and Methods
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
                    RuntimeType runtime = monoRuntimeType != null
                        ? RuntimeType.Mono : RuntimeType.Net;

                    if (monoRuntimeType != null)
                    {
                        currentFramework = new RuntimeFramework(RuntimeType.Mono, Environment.Version);
                        MethodInfo getDisplayNameMethod = monoRuntimeType.GetMethod(
                            "GetDisplayName", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.ExactBinding);
                        if (getDisplayNameMethod != null)
                            currentFramework.displayName = (string)getDisplayNameMethod.Invoke(null, new object[0]);
                    }
                    else
                        currentFramework = new RuntimeFramework(runtime, Environment.Version);
                }

                return currentFramework;
            }
        }

        /// <summary>
        /// Return an array of well-known framework versions
        /// </summary>
        public static Version[] KnownVersions
        {
            get { return knownVersions; }
        }

        /// <summary>
        /// Gets an array of all available frameworks
        /// </summary>
        public static RuntimeFramework[] AvailableFrameworks
        {
            get 
            {
                ArrayList frameworks = new ArrayList();

                foreach (RuntimeFramework framework in GetAvailableFrameworks(RuntimeType.Net))
                    frameworks.Add(framework);

                foreach (RuntimeFramework framework in GetAvailableFrameworks(RuntimeType.Mono))
                    frameworks.Add(framework);

                return (RuntimeFramework[])frameworks.ToArray(typeof(RuntimeFramework)); 
            }
        }

        /// <summary>
        /// Parses a string representing a RuntimeFramework.
        /// The string may be just a RuntimeType name or just
        /// a Version or a hyphentated RuntimeType-Version or
        /// a Version prefixed by 'v'.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static RuntimeFramework Parse(string s)
        {
            RuntimeType runtime = RuntimeType.Any;
            Version version = new Version();

            string[] parts = s.Split(new char[] { '-' });
            if (parts.Length == 2)
            {
                runtime = (RuntimeType)System.Enum.Parse(typeof(RuntimeType), parts[0], true);
                string vstring = parts[1];
                if (runtime == RuntimeType.Mono && vstring == "1.0")
                    vstring = "1.1";
                if ( vstring != "" )
                    version = new Version(vstring);
            }
            else if (char.ToLower(s[0]) == 'v')
            {
                version = new Version(s.Substring(1));
            }
            else
            {
                runtime = (RuntimeType)System.Enum.Parse(typeof(RuntimeType), s, true);
            }

            return new RuntimeFramework(runtime, version);
        }

        /// <summary>
        /// Returns true if a particular target runtime is available.
        /// </summary>
        /// <param name="framework">The framework being sought</param>
        /// <returns>True if it's available, false if not</returns>
        public static bool IsAvailable(RuntimeFramework framework)
        {
            switch (framework.Runtime)
            {
                case RuntimeType.Mono:
                    return IsMonoInstalled();
                case RuntimeType.Net:
                    return CurrentFramework.Matches( framework ) || IsDotNetInstalled(framework.Version);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines whether Mono is installed].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if Mono is installed, otherwise, <c>false</c>.
        /// </returns>
        public static bool IsMonoInstalled()
        {
            if (CurrentFramework.IsMono) return true;

            // Don't know how to do this on linux yet, but it's not
            // a problem since we are only supporting Mono on Linux
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                return false;

            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\Novell\Mono");
            if (key == null)
                return false;

            string version = key.GetValue("DefaultCLR") as string;
            if (version == null || version == "")
                return false;

            key = key.OpenSubKey(version);
            if (key == null)
                return false;

            return true;
        }

        /// <summary>
        /// Determines whether the specified version of Microsoft .NET is installed.
        /// </summary>
        public static bool IsDotNetInstalled(Version version)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                return false;

            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\.NETFramework\policy\v" + version.ToString(2));
            if (key == null) return false;

            return version.Build < 0 || key.GetValue(version.Build.ToString()) != null;
        }

        /// <summary>
        /// Returns an array of all available frameworks of a given type,
        /// for example, all mono or all .NET frameworks.
        /// </summary>
        public static RuntimeFramework[] GetAvailableFrameworks(RuntimeType rt)
        {
            ArrayList frameworks = new ArrayList();

            if (rt == RuntimeType.Net)
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\.NETFramework\policy");
                if (key != null)
                {
                    foreach (string name in key.GetSubKeyNames())
                    {
                        if (name.StartsWith("v"))
                        {
                            RegistryKey key2 = key.OpenSubKey(name);
                            foreach (string build in key2.GetValueNames())
                                frameworks.Add( new RuntimeFramework(rt, new Version(name.Substring(1) + "." + build)));
                        }
                    }
                }
            }
            else if (rt == RuntimeType.Mono && IsMonoInstalled())
            {
                RuntimeFramework framework = new RuntimeFramework(rt, new Version(1,1,4322));
                framework.displayName = "Mono 1.0 Profile";
                frameworks.Add( framework );
                framework = new RuntimeFramework(rt, new Version(2, 0, 50727));
                framework.displayName = "Mono 2.0 Profile";
                frameworks.Add( framework );
            }
            // Code to list various versions of Mono - currently not used
            //else if (rt == RuntimeType.Mono)
            //{
            //    RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\Novell\Mono");
            //    if (key != null)
            //    {
            //        foreach (string name in key.GetSubKeyNames())
            //        {
            //            RuntimeFramework framework = new RuntimeFramework(rt, new Version(1, 0));
            //            framework.displayName = "Mono " + name + " - 1.0 Profile";
            //            frameworks.Add(framework);
            //            framework = new RuntimeFramework(rt, new Version(2, 0));
            //            framework.displayName = "Mono " + name + " - 2.0 Profile";
            //            frameworks.Add(framework);
            //        }
            //    }
            //}

            return (RuntimeFramework[])frameworks.ToArray(typeof(RuntimeFramework));
        }
        #endregion

        #region Constructor
        /// <summary>
		/// Construct from a runtime type and version
		/// </summary>
		/// <param name="runtime">The runtime type of the framework</param>
		/// <param name="version">The version of the framework</param>
		public RuntimeFramework( RuntimeType runtime, Version version)
		{
			this.runtime = runtime;
            this.version = version;
            this.displayName = DefaultDisplayName(runtime, version);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The type of this runtime framework
        /// </summary>
        public RuntimeType Runtime
        {
            get { return runtime; }
        }

        /// <summary>
        /// The version of this runtime framework
        /// </summary>
        public Version Version
        {
            get { return version; }
        }

        /// <summary>
        /// Returns the Display name for this framework
        /// </summary>
        public string DisplayName
        {
            get { return displayName; }
        }

        /// <summary>
        /// Indicates whether the current framework is available on
        /// this machine.
        /// </summary>
        //public bool IsAvailable
        //{
        //    get { return IsAvailable(this); }
        //}

        private static string DefaultDisplayName(RuntimeType runtime, Version version)
        {
            return runtime.ToString() + " " + version.ToString();
        }

        /// <summary>
        /// Indicates whether the current runtime is Mono
        /// </summary>
        public bool IsMono
        {
            get { return this.runtime == RuntimeType.Mono; }
        }

        /// <summary>
        /// Indicates whether the current runtime is Microsoft .NET
        /// </summary>
        public bool IsNet
        {
            get { return this.runtime == RuntimeType.Net; }
        }

        /// <summary>
        /// Indicates whether the current runtime is the .NET Compact Framework
        /// </summary>
        public bool IsNetCF
        {
            get { return this.runtime == RuntimeType.NetCF; }
        }

        /// <summary>
        /// Indicates whether the current runtime is the Shared Source CLI (Rotor)
        /// </summary>
        public bool IsSSCLI
        {
            get { return this.runtime == RuntimeType.SSCLI; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Overridden to return the short name of the framework
        /// </summary>
        /// <returns></returns>
		public override string ToString()
		{
            string vstring = version.ToString();

            switch (runtime)
            {
                case RuntimeType.Any:
                    if (vstring != "")
                        vstring = "v" + vstring;
                    return vstring;
                case RuntimeType.Mono:
                    if (vstring == "1.1")
                        vstring = "1.0";
                    break;
                case RuntimeType.Net:
                case RuntimeType.NetCF:
                case RuntimeType.SSCLI:
                default:
                    break;
            }

            return runtime.ToString().ToLower() + "-" + vstring;
		}

        /// <summary>
        /// Returns true if the current framework matches the
        /// one supplied as an argument. Two frameworks match
        /// if their runtime types are the same or either one
        /// is RuntimeType.Any and all specified version components
        /// are equal. Negative (i.e. unspecified) version
        /// components are ignored.
        /// </summary>
        /// <param name="other">The RuntimeFramework to be matched.</param>
        /// <returns>True on match, otherwise false</returns>
        public bool Matches(RuntimeFramework other)
        {
            return (   this.Runtime == RuntimeType.Any
                    || other.Runtime == RuntimeType.Any
                    || this.Runtime == other.Runtime )
                && this.Version.Major == other.Version.Major
                && this.Version.Minor == other.Version.Minor
                && (   this.Version.Build < 0 
                    || other.Version.Build < 0 
                    || this.Version.Build == other.Version.Build ) 
                && (   this.Version.Revision < 0
                    || other.Version.Revision < 0
                    || this.Version.Revision == other.Version.Revision );
        }

        /// <summary>
        /// If the build number of this instance is not specified, use
        /// the standard build number for the major and minor version.
        /// </summary>
        public void SpecifyBuild()
        {
            if (version.Build < 0)
                foreach (Version v in knownVersions)
                    if (version.Major == v.Major && version.Minor == v.Minor)
                        version = v;
        }
        #endregion
    }
}
