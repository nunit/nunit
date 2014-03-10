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

namespace NUnit.Framework.Api
{
    /// <summary>
    /// DriverSettings is a static class containing constant values that
    /// are used in communicating between the engine and the framework.
    /// </summary>
    public static class DriverSettings
    {
        /// <summary>
        /// If true, the framework will capture output to standard error
        /// and send it to the listener in a TestOutput call.
        /// </summary>
        public const string CaptureStandardError = "CaptureStandardError";

        /// <summary>
        /// If true, the framework will capture output to standard output
        /// and send it to the listener in a TestOutput call.
        /// </summary>
        public const string CaptureStandardOutput = "CaptureStandardOutput";

        /// <summary>
        /// The default log threshold to be captured. Defaults to "Error"
        /// Use "None" to turn off capture. Other values depend on the
        /// logging subsystem. Currently, log4net is supported.
        /// </summary>
        public const string DefaultLogThreshold = "DefaultLogThreshold";

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
    }
}
