// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Represents the result of running a single test case.
    /// </summary>
    public class TestCaseResult : TestResult
    {
        /// <summary>
        /// Construct a TestCaseResult based on a TestMethod
        /// </summary>
        /// <param name="test">A TestMethod to which the result applies.</param>
        public TestCaseResult(TestMethod test) : base(test)
        {
        }

        #region Overrides

        /// <summary>
        /// Gets the number of test cases that failed
        /// when running the test and all its children.
        /// </summary>
        public override int FailCount => ResultState.Status == TestStatus.Failed ? 1 : 0;

        /// <summary>
        /// Gets the number of test cases that executed
        /// when running the test and all its children.
        /// </summary>
        public override int TotalCount => 1;

        /// <summary>
        /// Gets the number of test cases that had warnings
        /// when running the test and all its children.
        /// </summary>
        public override int WarningCount => ResultState.Status == TestStatus.Warning ? 1 : 0;

        /// <summary>
        /// Gets the number of test cases that passed
        /// when running the test and all its children.
        /// </summary>
        public override int PassCount => ResultState.Status == TestStatus.Passed ? 1 : 0;

        /// <summary>
        /// Gets the number of test cases that were skipped
        /// when running the test and all its children.
        /// </summary>
        public override int SkipCount => ResultState.Status == TestStatus.Skipped ? 1 : 0;

        /// <summary>
        /// Gets the number of test cases that were inconclusive
        /// when running the test and all its children.
        /// </summary>
        public override int InconclusiveCount => ResultState.Status == TestStatus.Inconclusive ? 1 : 0;

        /// <summary>
        /// Indicates whether this result has any child results.
        /// </summary>
        public override bool HasChildren => false;

        /// <summary>
        /// Gets the collection of child results.
        /// </summary>
        public override IEnumerable<ITestResult> Children => Array.Empty<ITestResult>();

        #endregion
    }
}
