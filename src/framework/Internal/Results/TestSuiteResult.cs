// ***********************************************************************
// Copyright (c) 2010 Charlie Poole
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
using System.Xml;
using NUnit.Framework.Api;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// TestSuiteResult represents the result of running a TestSuite.
    /// </summary>
    public class TestSuiteResult : TestResult
    {
        private int passCount = 0;
        private int failCount = 0;
        private int skipCount = 0;
        private int inconclusiveCount = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestSuiteResult"/> class.
        /// </summary>
        /// <param name="test">The test to be used</param>
        public TestSuiteResult(TestSuite test)
            : base(test) 
        {
        }

        /// <summary>
        /// Add a child result
        /// </summary>
        /// <param name="result">The child result to be added</param>
        public void AddResult(TestResult result)
        {
            this.Children.Add(result);
            this.AssertCount += result.AssertCount;

            TestSuiteResult suiteResult = result as TestSuiteResult;
            if (suiteResult != null)
            {
                this.passCount += suiteResult.passCount;
                this.failCount += suiteResult.failCount;
                this.skipCount += suiteResult.skipCount;
                this.inconclusiveCount += suiteResult.inconclusiveCount;
            }

            switch (result.ResultState.Status)
            {
                case TestStatus.Passed:
                    if (suiteResult == null)
                        this.passCount++;

                    if (this.ResultState == ResultState.Inconclusive)
                        this.SetResult(ResultState.Success);

                    break;

                case TestStatus.Failed:
                    if (suiteResult == null)
                        this.failCount++;

                    if (this.ResultState.Status != TestStatus.Failed)
                        this.SetResult(ResultState.Failure, "Child test failed");

                    break;

                case TestStatus.Skipped:
                    if (suiteResult == null)
                        this.skipCount++;
                    
                    break;

                case TestStatus.Inconclusive:
                    if (suiteResult == null)
                        this.inconclusiveCount++;

                    break;
            }

            switch (result.ResultState.Status)
            {
                case TestStatus.Failed:
                    break;
                case TestStatus.Passed:
                    break;
            }
        }

        #region TestResult Overrides

        /// <summary>
        /// Adds the XML representation of the result as a child of the
        /// supplied parent node..
        /// </summary>
        /// <param name="parentNode">The parent node.</param>
        /// <param name="recursive">If true, descendant results are included</param>
        /// <returns></returns>
        public override XmlNode AddToXml(XmlNode parentNode, bool recursive)
        {
            XmlNode thisNode = base.AddToXml(parentNode, recursive);

            XmlHelper.AddAttribute(thisNode, "total", (passCount + failCount + skipCount + inconclusiveCount).ToString());
            XmlHelper.AddAttribute(thisNode, "passed", passCount.ToString());
            XmlHelper.AddAttribute(thisNode, "failed", failCount.ToString());
            XmlHelper.AddAttribute(thisNode, "inconclusive", inconclusiveCount.ToString());
            XmlHelper.AddAttribute(thisNode, "skipped", skipCount.ToString());
            XmlHelper.AddAttribute(thisNode, "asserts", this.AssertCount.ToString());

            if (recursive && HasChildren)
                foreach (TestResult child in Children)
                    child.AddToXml(thisNode, recursive);

            return thisNode;
        }

        #endregion
    }
}
