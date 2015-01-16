// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
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

namespace NUnit.Common
{
    /// <summary>
    /// PackageSettings is a static class containing constant values that
    /// are used as keys in setting up a TestPackage. These values duplicate
    /// settings in the engine and framework.
    /// </summary>
    public static class PackageSettings
    {
        #region Settings taken from RunnerSettings in the engine

        /// <summary>
        /// The config to use in loading a project
        /// </summary>
        public const string ActiveConfig = "ActiveConfig";

        /// <summary>
        /// If true, the engine should determine the private bin
        /// path by examining the paths to all the tests.
        /// </summary>
        public const string AutoBinPath = "AutoBinPath";

        /// <summary>
        /// The ApplicationBase to use in loading the tests.
        /// </summary>
        public const string BasePath = "BasePath";

        /// <summary>
        /// The config file to use in running the tests 
        /// </summary>
        public const string ConfigurationFile = "ConfigurationFile";

        /// <summary>
        /// Indicates how to load tests across AppDomains
        /// </summary>
        public const string DomainUsage = "DomainUsage";

        /// <summary>
        /// The private binpath used to locate assemblies
        /// </summary>
        public const string PrivateBinPath = "PrivateBinPath";

        /// <summary>
        /// Indicates how to allocate assemblies to processes
        /// </summary>
        public const string ProcessModel = "ProcessModel";

        /// <summary>
        /// Indicates the desired runtime to use for the tests.
        /// </summary>
        public const string RuntimeFramework = "RuntimeFramework";

        /// <summary>
        /// Indicates the test should be run in a 32-bit process on a 64-bit system
        /// </summary>
        public const string RunAsX86 = "RunAsX86";

        /// <summary>
        /// Indicates that test runners should be disposed after the tests are executed
        /// </summary>
        public const string DisposeRunners = "DisposeRunners";

        #endregion

        #region Settings taken from DriverSettings in the framework

        /// <summary>
        /// Integer value in milliseconds for the default timeout value
        /// for test cases. If not specified, there is no timeout.
        /// </summary>
        public const string DefaultTimeout = "DefaultTimeout";

        /// <summary>
        /// An InternalTraceLevel enumeration value for this run. 
        /// </summary>
        public const string InternalTraceLevel = "InternalTraceLevel";

        /// <summary>
        /// A TextWriter to which the internal trace will be sent.
        /// </summary>
        public const string InternalTraceWriter = "InternalTraceWriter";

        /// <summary>
        /// A list of tests to be loaded. 
        /// </summary>
        // TODO: Remove?
        public const string LOAD = "LOAD";

        /// <summary>
        /// The number of test threads to run for the assembly.
        /// </summary>
        public const string NumberOfTestWorkers = "NumberOfTestWorkers";

        /// <summary>
        /// The random seed to be used for this assembly.
        /// </summary>
        public const string RandomSeed = "RandomSeed";

        /// <summary>
        /// If true, execution stops after the first error or failure.
        /// </summary>
        public const string StopOnError = "StopOnError";

        /// <summary>
        /// Full path of the directory to be used for work and result files.
        /// </summary>
        public const string WorkDirectory = "WorkDirectory";

        #endregion
    }
}
