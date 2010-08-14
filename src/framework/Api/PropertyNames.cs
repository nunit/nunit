using System;

namespace NUnit.Framework.Api
{
    /// <summary>
    /// The PropKey class provides static constants for the
    /// standard property names that NUnit uses on tests.
    /// </summary>
    public class PropertyNames
    {
        #region Properties Used on All Types of Test

        /// <summary>
        /// The Description of a test
        /// </summary>
        public static readonly string Description = "_DESCRIPTION";
        
        /// <summary>
        /// The reason a test was not run
        /// </summary>
        public static readonly string IgnoreReason = "_IGNOREREASON";

        /// <summary>
        /// The culture to be set for a test
        /// </summary>
        public static readonly string SetCulture = "_SETCULTURE";

        /// <summary>
        /// The UI culture to be set for a test
        /// </summary>
        public static readonly string SetUICulture = "_SETUICULTURE";

        /// <summary>
        /// The categories applying to a test
        /// </summary>
        public static readonly string Categories = "_CATEGORIES";

        /// <summary>
        /// Indicates that the test should be run on a separate thread
        /// </summary>
        public static readonly string RequiresThread = "RequiresThread";

        /// <summary>
        /// The ApartmentState required for running the test
        /// </summary>
        public static readonly string ApartmentState = "APARTMENT_STATE";

        /// <summary>
        /// The timeout value for the test
        /// </summary>
        public static readonly string Timeout = "Timeout";

        /// <summary>
        /// The number of times the test should be repeated
        /// </summary>
        public static readonly string RepeatCount = "Repeat";

        /// <summary>
        /// The maximum time in ms, above which the test is considered to have failed
        /// </summary>
        public static readonly string MaxTime = "MaxTime";

        /// <summary>
        /// The selected strategy for joining parameter data into test cases
        /// </summary>
        public static readonly string JoinType = "_JOINTYPE";

        #endregion

        #region Properties used on Test assemblies only

        /// <summary>
        /// The process ID of the executing assembly
        /// </summary>
        public static readonly string ProcessID = "_PID";
        
        /// <summary>
        /// The FriendlyName of the AppDomain in which the assembly is running
        /// </summary>
        public static readonly string AppDomain = "_APPDOMAIN";

        #endregion
    }
}
