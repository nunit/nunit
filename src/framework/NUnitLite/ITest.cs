// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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

namespace NUnitLite
{
	/// <summary>
	/// The RunState enum indicates whether a test
    /// has been or can be executed.
	/// </summary>
    public enum RunState
    {
        /// <summary>
        /// The test is not runnable
        /// </summary>
        NotRunnable,

        /// <summary>
        /// The test is runnable
        /// </summary>
        Runnable,

        ///// <summary>
        ///// The test can only be run explicitly
        ///// </summary>
        //Explicit,

        ///// <summary>
        ///// The test has been skipped
        ///// </summary>
        //Skipped,

        /// <summary>
        /// The test has been ignored
        /// </summary>
        Ignored

        ///// <summary>
        ///// The test has been executed
        ///// </summary>
        //Executed
    }

    /// <summary>
    /// Interface representing a test
    /// </summary>
    public interface ITest
    {
        /// <summary>
        /// Gets the name of the test.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }

        /// <summary>
        /// Gets the full name of the test.
        /// </summary>
        /// <value>The full name.</value>
        string FullName { get; }

        /// <summary>
        /// Gets or sets the run state of the test.
        /// </summary>
        /// <value>The state of the run.</value>
        RunState RunState { get; set; }

        /// <summary>
        /// Gets or sets the ignore reason.
        /// </summary>
        /// <value>The ignore reason.</value>
        string IgnoreReason { get; set; }

        /// <summary>
        /// Gets the test case count.
        /// </summary>
        /// <value>The test case count.</value>
        int TestCaseCount { get; }

        /// <summary>
        /// Gets the properties of the test.
        /// </summary>
        /// <value>The properties dictionary.</value>
        System.Collections.IDictionary Properties { get; }

        /// <summary>
        /// Runs this test.
        /// </summary>
        /// <returns>A TestResult</returns>
        TestResult Run();

        /// <summary>
        /// Runs this test
        /// </summary>
        /// <param name="listener">A TestListener to handle test events</param>
        /// <returns>A TestResult</returns>
        TestResult Run(TestListener listener);
    }
}
