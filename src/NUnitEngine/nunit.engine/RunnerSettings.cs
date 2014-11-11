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

namespace NUnit.Engine
{
    /// <summary>
    /// RunnerSettings is a static class containing constant values that
    /// are used as keys in a TestPackage and are acted on by the engine.
    /// </summary>
    public static class RunnerSettings
    {
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
        /// Indicates that the tests should be run in a 32-bit process on a 64-bit system
        /// </summary>
        public const string RunAsX86 = "RunAsX86";
    }
}
