// ***********************************************************************
// Copyright (c) 2010 Charlie Poole
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
        public const string ProcessID = "_PID";

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
