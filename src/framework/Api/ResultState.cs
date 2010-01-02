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

namespace NUnit.Framework.Api
{
	/// <summary>
	/// The ResultState enum indicates the result of running a test
	/// </summary>
	public enum ResultState
	{
        /// <summary>
        /// The result is inconclusive
        /// </summary>
        Inconclusive,

        /// <summary>
        /// The test was not runnable.
        /// </summary>
		NotRunnable, 

        /// <summary>
        /// The test has been skipped. 
        /// </summary>
		Skipped,

        /// <summary>
        /// The test has been ignored.
        /// </summary>
		Ignored,

        /// <summary>
        /// The test succeeded
        /// </summary>
		Success,

        /// <summary>
        /// The test failed
        /// </summary>
		Failure,

        /// <summary>
        /// The test encountered an unexpected exception
        /// </summary>
		Error,

        /// <summary>
        /// The test was cancelled by the user
        /// </summary>
        Cancelled
	}

    /// <summary>
    /// The FailureSite enum indicates the stage of a test
    /// in which an error or failure occured.
    /// </summary>
    public enum FailureSite
    {
        /// <summary>
        /// Failure in the test itself
        /// </summary>
        Test,

        /// <summary>
        /// Failure in the SetUp method
        /// </summary>
        SetUp,

        /// <summary>
        /// Failure in the TearDown method
        /// </summary>
        TearDown,

        /// <summary>
        /// Failure of a parent test
        /// </summary>
        Parent,

        /// <summary>
        /// Failure of a child test
        /// </summary>
        Child
    }

}
