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
	/// The ResultState class represents the outcome of running a test.
    /// It contains two pieces of information. The Status of the test
    /// is an enum indicating whether the test passed, failed, was
    /// skipped or was inconclusive. The Label provides a more
    /// detailed breakdown for use by client runners.
	/// </summary>
	public class ResultState
	{
        private TestStatus status;
        private string label;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultState"/> class.
        /// </summary>
        /// <param name="status">The TestStatus.</param>
        public ResultState(TestStatus status)
        {
            this.status = status;
            this.label = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultState"/> class.
        /// </summary>
        /// <param name="status">The TestStatus.</param>
        /// <param name="label">The label.</param>
        public ResultState(TestStatus status, string label)
        {
            this.status = status;
            this.label = label;
        }

        #endregion

        #region Predefined ResultStates

        /// <summary>
        /// The result is inconclusive
        /// </summary>
        public static ResultState Inconclusive = new ResultState(TestStatus.Inconclusive);

        /// <summary>
        /// The test was not runnable.
        /// </summary>
        public static ResultState NotRunnable = new ResultState(TestStatus.Skipped, "Invalid");

        /// <summary>
        /// The test has been skipped. 
        /// </summary>
        public static ResultState Skipped = new ResultState(TestStatus.Skipped);

        /// <summary>
        /// The test has been ignored.
        /// </summary>
        public static ResultState Ignored = new ResultState(TestStatus.Skipped, "Ignored");

        /// <summary>
        /// The test succeeded
        /// </summary>
        public static ResultState Success = new ResultState(TestStatus.Passed);

        /// <summary>
        /// The test failed
        /// </summary>
        public static ResultState Failure = new ResultState(TestStatus.Failed);

        /// <summary>
        /// The test encountered an unexpected exception
        /// </summary>
        public static ResultState Error = new ResultState(TestStatus.Failed, "Error");

        /// <summary>
        /// The test was cancelled by the user
        /// </summary>
        public static ResultState Cancelled = new ResultState(TestStatus.Failed, "Cancelled");
        
        #endregion

        #region Properties

        /// <summary>
        /// Gets the TestStatus for the test.
        /// </summary>
        /// <value>The status.</value>
        public TestStatus Status
        {
            get { return status; }
        }

        /// <summary>
        /// Gets the label under which this test resullt is
        /// categorized, if any.
        /// </summary>
        public string Label
        {
            get { return label; }
        }

        #endregion

        public override string ToString()
        {
            string s = status.ToString();
            if (label == null) return s;

            return string.Format("{0}:{1}", s, label);
        }
    }
}
