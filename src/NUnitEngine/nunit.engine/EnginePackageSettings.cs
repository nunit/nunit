// ***********************************************************************
// Copyright (c) 2016 Charlie Poole
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

namespace NUnit.Engine
{
    /// <summary>
    /// EngineSettings contains constant values that
    /// are used as keys in setting up a TestPackage. 
    /// Values here are used in the engine, and set by the runner.
    /// Setting values may be a string, int or bool.
    /// </summary>
    public static class EnginePackageSettings
    {
        #region Engine Settings - Used by the Engine itself

        /// <summary>
        /// The name of the config to use in loading a project.
        /// If not specified, the first config found is used.
        /// </summary>
        public const string ActiveConfig = "ActiveConfig";

        /// <summary>
        /// Bool indicating whether the engine should determine the private
        /// bin path by examining the paths to all the tests. Defaults to
        /// true unless PrivateBinPath is specified.
        /// </summary>
        public const string AutoBinPath = "AutoBinPath";

        /// <summary>
        /// The ApplicationBase to use in loading the tests. If not 
        /// specified, and each assembly has its own process, then the
        /// location of the assembly is used. For multiple  assemblies
        /// in a single process, the closest common root directory is used.
        /// </summary>
        public const string BasePath = "BasePath";

        /// <summary>
        /// Path to the config file to use in running the tests. 
        /// </summary>
        public const string ConfigurationFile = "ConfigurationFile";

        /// <summary>
        /// Flag (bool) indicating whether tests are being debugged.
        /// </summary>
        public const string DebugTests = "DebugTests";

        /// <summary>
        /// Bool flag indicating whether a debugger should be launched at agent 
        /// startup. Used only for debugging NUnit itself.
        /// </summary>
        public const string DebugAgent = "DebugAgent";

        /// <summary>
        /// Indicates how to load tests across AppDomains. Values are:
        /// "Default", "None", "Single", "Multiple". Default is "Multiple"
        /// if more than one assembly is loaded in a process. Otherwise,
        /// it is "Single".
        /// </summary>
        public const string DomainUsage = "DomainUsage";

        /// <summary>
        /// The private binpath used to locate assemblies. Directory paths
        /// is separated by a semicolon. It's an error to specify this and
        /// also set AutoBinPath to true.
        /// </summary>
        public const string PrivateBinPath = "PrivateBinPath";

        /// <summary>
        /// The maximum number of test agents permitted to run simultneously. 
        /// Ignored if the ProcessModel is not set or defaulted to Multiple.
        /// </summary>
        public const string MaxAgents = "MaxAgents";

        /// <summary>
        /// Indicates how to allocate assemblies to processes. Values are:
        /// "Default", "Single", "Separate", "Multiple". Default is "Multiple"
        /// for more than one assembly, "Separate" for a single assembly.
        /// </summary>
        public const string ProcessModel = "ProcessModel";

        /// <summary>
        /// Indicates the desired runtime to use for the tests. Values 
        /// are strings like "net-4.5", "mono-4.0", etc. Default is to
        /// use the target framework for which an assembly was built.
        /// </summary>
        public const string RuntimeFramework = "RuntimeFramework";

        /// <summary>
        /// Bool flag indicating that the test should be run in a 32-bit process 
        /// on a 64-bit system. By default, NUNit runs in a 64-bit process on
        /// a 64-bit system. Ignored if set on a 32-bit system.
        /// </summary>
        public const string RunAsX86 = "RunAsX86";

        /// <summary>
        /// Indicates that test runners should be disposed after the tests are executed
        /// </summary>
        public const string DisposeRunners = "DisposeRunners";

        /// <summary>
        /// Bool flag indicating that the test assemblies should be shadow copied. 
        /// Defaults to false.
        /// </summary>
        public const string ShadowCopyFiles = "ShadowCopyFiles";

        /// <summary>
        /// Bool flag indicating that user profile should be loaded on test runner processes
        /// </summary>
        public const string LoadUserProfile = "LoadUserProfile";

        /// <summary>
        /// Flag (bool) indicating whether to pause execution of tests to allow
        /// the user to attach a debugger.
        /// </summary>
        public const string PauseBeforeRun = "PauseBeforeRun";

        /// <summary>
        /// The InternalTraceLevel for this run. Values are: "Default",
        /// "Off", "Error", "Warning", "Info", "Debug", "Verbose".
        /// Default is "Off". "Debug" and "Verbose" are synonyms.
        /// </summary>
        public const string InternalTraceLevel = "InternalTraceLevel";

        #endregion
    }
}
