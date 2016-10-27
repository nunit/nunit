// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
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
using System.Collections.Generic;
#if PARALLEL
using System.Collections.Concurrent;
#endif
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
        public TestCaseResult(TestMethod test) : base(test) { }

        #region Overrides

        /// <summary>
        /// Gets the number of test cases that failed
        /// when running the test and all its children.
        /// </summary>
        public override int FailCount
        {
            get { return ResultState.Status == TestStatus.Failed ? 1 : 0; }
        }

        /// <summary>
        /// Gets the number of test cases that passed
        /// when running the test and all its children.
        /// </summary>
        public override int PassCount
        {
            get { return ResultState.Status == TestStatus.Passed ? 1 : 0; }
        }

        /// <summary>
        /// Gets the number of test cases that were skipped
        /// when running the test and all its children.
        /// </summary>
        public override int SkipCount
        {
            get { return ResultState.Status == TestStatus.Skipped ? 1 : 0; }
        }

        /// <summary>
        /// Gets the number of test cases that were inconclusive
        /// when running the test and all its children.
        /// </summary>
        public override int InconclusiveCount
        {
            get { return ResultState.Status == TestStatus.Inconclusive ? 1 : 0; }
        }

        /// <summary>
        /// Indicates whether this result has any child results.
        /// </summary>
        public override bool HasChildren
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the collection of child results.
        /// </summary>
        public override IEnumerable<ITestResult> Children
        {
            get { return new ITestResult[0]; }
        }

        #endregion
    }
}
