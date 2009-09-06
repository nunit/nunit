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

        /// <summary>
        /// The test can only be run explicitly
        /// </summary>
        //Explicit,

        /// <summary>
        /// The test has been skipped
        /// </summary>
        //Skipped,

        /// <summary>
        /// The test has been ignored
        /// </summary>
        Ignored

        /// <summary>
        /// The test has been executed
        /// </summary>
        //Executed
    }

    public interface ITest : System.IComparable
    {
        string Name { get; }
        string FullName { get; }

        RunState RunState { get; set; }
        string IgnoreReason { get; set; }
        int TestCaseCount { get; }

        System.Collections.IDictionary Properties { get; }

        TestResult Run();
        TestResult Run(TestListener listener);
    }
}
