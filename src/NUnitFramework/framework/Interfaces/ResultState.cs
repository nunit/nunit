// ***********************************************************************
// Copyright (c) 2007-2014 Charlie Poole
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

using System.Text;

namespace NUnit.Framework.Interfaces
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
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultState"/> class.
        /// </summary>
        /// <param name="status">The TestStatus.</param>
        public ResultState(TestStatus status) : this (status, string.Empty, FailureSite.Test)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultState"/> class.
        /// </summary>
        /// <param name="status">The TestStatus.</param>
        /// <param name="label">The label.</param>
        public ResultState(TestStatus status, string label) : this (status, label, FailureSite.Test)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultState"/> class.
        /// </summary>
        /// <param name="status">The TestStatus.</param>
        /// <param name="site">The stage at which the result was produced</param>
        public ResultState(TestStatus status, FailureSite site) : this(status, string.Empty, site)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultState"/> class.
        /// </summary>
        /// <param name="status">The TestStatus.</param>
        /// <param name="label">The label.</param>
        /// <param name="site">The stage at which the result was produced</param>
        public ResultState(TestStatus status, string label, FailureSite site)
        {
            Status = status;
            Label = label == null ? string.Empty : label;
            Site = site;
        }

        #endregion

        #region Predefined ResultStates

        /// <summary>
        /// The result is inconclusive
        /// </summary>
        public readonly static ResultState Inconclusive = new ResultState(TestStatus.Inconclusive);
        
        /// <summary>
        /// The test has been skipped. 
        /// </summary>
        public readonly static ResultState Skipped = new ResultState(TestStatus.Skipped);

        /// <summary>
        /// The test has been ignored.
        /// </summary>
        public readonly static ResultState Ignored = new ResultState(TestStatus.Skipped, "Ignored");

        /// <summary>
        /// The test was skipped because it is explicit
        /// </summary>
        public readonly static ResultState Explicit = new ResultState(TestStatus.Skipped, "Explicit");

        /// <summary>
        /// The test succeeded
        /// </summary>
        public readonly static ResultState Success = new ResultState(TestStatus.Passed);

        /// <summary>
        /// The test failed
        /// </summary>
        public readonly static ResultState Failure = new ResultState(TestStatus.Failed);

        /// <summary>
        /// The test encountered an unexpected exception
        /// </summary>
        public readonly static ResultState Error = new ResultState(TestStatus.Failed, "Error");

        /// <summary>
        /// The test was cancelled by the user
        /// </summary>
        public readonly static ResultState Cancelled = new ResultState(TestStatus.Failed, "Cancelled");

        /// <summary>
        /// The test was not runnable.
        /// </summary>
        public readonly static ResultState NotRunnable = new ResultState(TestStatus.Failed, "Invalid");

        /// <summary>
        /// A suite failed because one or more child tests failed or had errors
        /// </summary>
        public readonly static ResultState ChildFailure = ResultState.Failure.WithSite(FailureSite.Child);

        /// <summary>
        /// A suite failed in its OneTimeSetUp
        /// </summary>
        public readonly static ResultState SetUpFailure = ResultState.Failure.WithSite(FailureSite.SetUp);

        /// <summary>
        /// A suite had an unexpected exception in its OneTimeSetUp
        /// </summary>
        public readonly static ResultState SetUpError = ResultState.Error.WithSite(FailureSite.SetUp);

        /// <summary>
        /// A suite had an unexpected exception in its OneTimeDown
        /// </summary>
        public readonly static ResultState TearDownError = ResultState.Error.WithSite(FailureSite.TearDown);

        #endregion

        #region Properties

        /// <summary>
        /// Gets the TestStatus for the test.
        /// </summary>
        /// <value>The status.</value>
        public TestStatus Status { get; private set; }

        /// <summary>
        /// Gets the label under which this test result is
        /// categorized, if any.
        /// </summary>
        public string Label { get; private set; }

        /// <summary>
        /// Gets the stage of test execution in which
        /// the failure or other result took place.
        /// </summary>
        public FailureSite Site { get; private set; }

        /// <summary>
        /// Get a new ResultState, which is the same as the current
        /// one but with the FailureSite set to the specified value.
        /// </summary>
        /// <param name="site">The FailureSite to use</param>
        /// <returns>A new ResultState</returns>
        public ResultState WithSite(FailureSite site)
        {
            return new ResultState(this.Status, this.Label, site);
        }

        #endregion

        #region Equals Override

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var other = obj as ResultState;
            if (other == null) return false;

            return Status.Equals(other.Status) && Label.Equals(other.Label) && Site.Equals(other.Site);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return (int)Status << 8 + (int)Site ^ Label.GetHashCode(); ;
        }

        #endregion

        #region ToString Override

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder(Status.ToString());

            if (Label != null && Label.Length > 0)
                sb.AppendFormat(":{0}", Label);
            if (Site != FailureSite.Test)
                sb.AppendFormat("({0})", Site.ToString());

            return sb.ToString();
        }

        #endregion
    }

    /// <summary>
    /// The FailureSite enum indicates the stage of a test
    /// in which an error or failure occurred.
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
