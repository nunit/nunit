using System;
using NUnit.Framework.Api;

namespace NUnit.Framework.Internal
{
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

    /// <summary>
    /// Represents the result of running a test suite
    /// </summary>
    public class TestSuiteResult : TestResult
    {
        private int passCount = 0;
        private int failCount = 0;
        private int skipCount = 0;
        private int inconclusiveCount = 0;

        /// <summary>
        /// Construct a TestSuiteResult base on a TestSuite
        /// </summary>
        /// <param name="suite">The TestSuite to which the result applies</param>
        public TestSuiteResult(TestSuite suite) : base(suite) { }

        /// <summary>
        /// Gets the number of test cases that failed
        /// when running the test and all its children.
        /// </summary>
        public override int FailCount
        {
            get { return this.failCount; }
        }

        /// <summary>
        /// Gets the number of test cases that passed
        /// when running the test and all its children.
        /// </summary>
        public override int PassCount
        {
            get { return this.passCount; }
        }

        /// <summary>
        /// Gets the number of test cases that were skipped
        /// when running the test and all its children.
        /// </summary>
        public override int SkipCount
        {
            get { return this.skipCount; }
        }

        /// <summary>
        /// Gets the number of test cases that were inconclusive
        /// when running the test and all its children.
        /// </summary>
        public override int InconclusiveCount
        {
            get { return this.inconclusiveCount; }
        }

            /// <summary>
        /// Add a child result
        /// </summary>
        /// <param name="result">The child result to be added</param>
        public void AddResult(TestResult result)
        {
            this.Children.Add(result);

            this.assertCount += result.AssertCount;
            this.passCount += result.PassCount;
            this.failCount += result.FailCount;
            this.skipCount += result.SkipCount;
            this.inconclusiveCount += result.InconclusiveCount;

            // NOTE: We don't call SetResult from this
            // method to avoid double-counting of results.
            switch (result.ResultState.Status)
            {
                case TestStatus.Passed:

                    if (this.resultState.Status == TestStatus.Inconclusive)
                        this.resultState = ResultState.Success;

                    break;

                case TestStatus.Failed:

                    if (this.resultState.Status != TestStatus.Failed)
                    {
                        this.resultState = ResultState.Failure;
                        this.message = "One or more child tests had errors";
                    }

                    break;

                case TestStatus.Skipped:

                    switch (result.ResultState.Label)
                    {
                        case "Invalid":

                            if (this.ResultState != ResultState.NotRunnable && this.ResultState.Status != TestStatus.Failed)
                            {
                                this.resultState = ResultState.Failure;
                                this.message = "One or more child tests had errors";
                            }

                            break;

                        case "Ignored":

                            if (this.ResultState.Status == TestStatus.Inconclusive || this.ResultState.Status == TestStatus.Passed)
                            {
                                this.resultState = ResultState.Ignored;
                                this.message = "One or more child tests were ignored";
                            }

                            break;

                        default:

                            // Tests skipped for other reasons do not change the outcome
                            // of the containing suite when added.

                            break;
                    }

                    break;

                case TestStatus.Inconclusive:

                    // An inconclusive result does not change the outcome
                    // of the containing suite when added.

                    break;
            }
        }

        /// <summary>
        /// Set the test result based on the type of exception thrown
        /// and the site of the Failure.
        /// </summary>
        /// <param name="ex">The exception that was thrown</param>
        /// <param name="site">The FailureSite</param>
        public void RecordException(Exception ex, FailureSite site)
        {
            base.RecordException(ex);

            if (site == FailureSite.SetUp)
            {
                switch (ResultState.Status)
                {
                    case TestStatus.Skipped:
                        this.skipCount = this.test.TestCaseCount;
                        break;

                    case TestStatus.Failed:
                        this.failCount = this.test.TestCaseCount;
                        break;
                }
            }
        }
    }
}
