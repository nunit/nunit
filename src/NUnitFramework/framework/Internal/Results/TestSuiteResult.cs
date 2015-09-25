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
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
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
        public override void AddResult(ITestResult result)
        {
            base.AddResult(result);

            this.AssertCount += result.AssertCount;
            this.passCount += result.PassCount;
            this.failCount += result.FailCount;
            this.skipCount += result.SkipCount;
            this.inconclusiveCount += result.InconclusiveCount;
        }
    }
}
