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

namespace NUnit
{
    /// <summary>
    /// FrameworkPackageSettings is a static class containing constant values that
    /// are used as keys in setting up a TestPackage. These values are used in
    /// the framework, and set in the runner. Setting values may be a string, int or bool.
    /// </summary>
    public static class FrameworkPackageSettings
    {
        #region Settings used in Framework

        /// <summary>
        /// Flag (bool) indicating whether tests are being debugged.
        /// </summary>
        public const string DebugTests = "DebugTests";

        /// <summary>
        /// Flag (bool) indicating whether to pause execution of tests to allow
        /// the user to attache a debugger.
        /// </summary>
        public const string PauseBeforeRun = "PauseBeforeRun";

        /// <summary>
        /// The InternalTraceLevel for this run. Values are: "Default",
        /// "Off", "Error", "Warning", "Info", "Debug", "Verbose".
        /// Default is "Off". "Debug" and "Verbose" are synonyms.
        /// </summary>
        public const string InternalTraceLevel = "InternalTraceLevel";

        /// <summary>
        /// Full path of the directory to be used for work and result files.
        /// This path is provided to tests by the frameowrk TestContext.
        /// </summary>
        public const string WorkDirectory = "WorkDirectory";

        /// <summary>
        /// Integer value in milliseconds for the default timeout value
        /// for test cases. If not specified, there is no timeout except
        /// as specified by attributes on the tests themselves.
        /// </summary>
        public const string DefaultTimeout = "DefaultTimeout";

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
        /// The number of test threads to run for the assembly. If set to
        /// 1, a single queue is used. If set to 0, tests are executed
        /// directly, without queuing.
        /// </summary>
        public const string NumberOfTestWorkers = "NumberOfTestWorkers";

        /// <summary>
        /// The random seed to be used for this assembly. If specified
        /// as the value reported from a prior run, the framework should
        /// generate identical random values for tests as were used for
        /// that run, provided that no change has been made to the test
        /// assembly. Default is a random value itself.
        /// </summary>
        public const string RandomSeed = "RandomSeed";

        /// <summary>
        /// If true, execution stops after the first error or failure.
        /// </summary>
        public const string StopOnError = "StopOnError";

        /// <summary>
        /// If true, use of the event queue is suppressed and test events are synchronous.
        /// </summary>
        public const string SynchronousEvents = "SynchronousEvents";

        /// <summary>
        /// The default naming pattern used in generating test names
        /// </summary>
        public const string DefaultTestNamePattern = "DefaultTestNamePattern";

        /// <summary>
        /// Parameters to be passed on to the test
        /// </summary>
        public const string TestParameters = "TestParameters";

        #endregion
    }
}
