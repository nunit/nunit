// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
        /// the user to attach a debugger.
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
        /// This path is provided to tests by the framework TestContext.
        /// </summary>
        public const string WorkDirectory = "WorkDirectory";

        /// <summary>
        /// Integer value in milliseconds for the default timeout value
        /// for test cases. If not specified, there is no timeout except
        /// as specified by attributes on the tests themselves.
        /// </summary>
        public const string DefaultTimeout = "DefaultTimeout";

        /// <summary>
        /// A string representing the default thread culture to be used for
        /// running tests. String should be a valid BCP-47 culture name. If
        /// culture is unset, tests run on the machine's default culture.
        /// </summary>
        public const string DefaultCulture = "DefaultCulture";

        /// <summary>
        /// A string representing the default thread UI culture to be used for
        /// running tests. String should be a valid BCP-47 culture name. If
        /// culture is unset, tests run on the machine's default culture.
        /// </summary>
        public const string DefaultUICulture = "DefaultUICulture";

        /// <summary>
        /// A TextWriter to which the internal trace will be sent.
        /// </summary>
        public const string InternalTraceWriter = "InternalTraceWriter";

        /// <summary>
        /// A list of tests to be loaded.
        /// </summary>
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
        /// If true, asserts in multiple asserts block will throw immediately.
        /// </summary>
        public const string DisableMultipleAssertsUnderDebugger = "DisableMultipleAssertsUnderDebugger";

        /// <summary>
        /// If true, use of the event queue is suppressed and test events are synchronous.
        /// </summary>
        public const string SynchronousEvents = "SynchronousEvents";

        /// <summary>
        /// The default naming pattern used in generating test names
        /// </summary>
        public const string DefaultTestNamePattern = "DefaultTestNamePattern";

        /// <summary>
        /// Parameters to be passed on to the tests, serialized to a single string which needs parsing. Obsoleted by <see cref="TestParametersDictionary"/>; kept for backward compatibility.
        /// </summary>
        public const string TestParameters = "TestParameters";

        /// <summary>
        /// Parameters to be passed on to the tests, already parsed into an IDictionary&lt;string, string>. Replaces <see cref="TestParameters"/>.
        /// </summary>
        public const string TestParametersDictionary = "TestParametersDictionary";

        /// <summary>
        /// If true, the tests will run on the same thread as the NUnit runner itself
        /// </summary>
        public const string RunOnMainThread = "RunOnMainThread";

        #endregion
    }
}
