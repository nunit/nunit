// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// The PropertyNames class provides static constants for the
    /// standard property ids that NUnit uses on tests.
    /// </summary>
    public class PropertyNames
    {
        #region Internal Properties

        /// <summary>
        /// The FriendlyName of the AppDomain in which the assembly is running
        /// </summary>
        public const string AppDomain = "_APPDOMAIN";

        /// <summary>
        /// The selected strategy for joining parameter data into test cases
        /// </summary>
        public const string JoinType = "_JOINTYPE";

        /// <summary>
        /// The process ID of the executing assembly
        /// </summary>
        public const string ProcessId = "_PID";

        /// <summary>
        /// The stack trace from any data provider that threw
        /// an exception.
        /// </summary>
        public const string ProviderStackTrace = "_PROVIDERSTACKTRACE";

        /// <summary>
        /// The reason a test was not run
        /// </summary>
        public const string SkipReason = "_SKIPREASON";

        #endregion

        #region Standard Properties

        /// <summary>
        /// The author of the tests
        /// </summary>
        public const string Author = "Author";

        /// <summary>
        /// The ApartmentState required for running the test
        /// </summary>
        public const string ApartmentState = "ApartmentState";

        /// <summary>
        /// The categories applying to a test
        /// </summary>
        public const string Category = "Category";

        /// <summary>
        /// The Description of a test
        /// </summary>
        public const string Description = "Description";

        /// <summary>
        /// The number of threads to be used in running tests
        /// </summary>
        public const string LevelOfParallelism = "LevelOfParallelism";

        /// <summary>
        /// The maximum time in ms, above which the test is considered to have failed
        /// </summary>
        public const string MaxTime = "MaxTime";

        /// <summary>
        /// The ParallelScope associated with a test
        /// </summary>
        public const string ParallelScope = "ParallelScope";

        /// <summary>
        /// The number of times the test should be repeated
        /// </summary>
        public const string RepeatCount = "Repeat";

        /// <summary>
        /// Indicates that the test should be run on a separate thread
        /// </summary>
        public const string RequiresThread = "RequiresThread";

        /// <summary>
        /// The culture to be set for a test
        /// </summary>
        public const string SetCulture = "SetCulture";

        /// <summary>
        /// The UI culture to be set for a test
        /// </summary>
        public const string SetUICulture = "SetUICulture";

        /// <summary>
        /// The type that is under test
        /// </summary>
        public const string TestOf = "TestOf";

        /// <summary>
        /// The timeout value for the test
        /// </summary>
        public const string Timeout = "Timeout";

        /// <summary>
        /// The test will be ignored until the given date
        /// </summary>
        public const string IgnoreUntilDate = "IgnoreUntilDate";

        /// <summary>
        /// The optional Order the test will run in
        /// </summary>
        public const string Order = "Order";
        #endregion
    }
}
