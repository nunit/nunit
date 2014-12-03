// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
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
using System.Globalization;
using System.Xml;

namespace NUnit.ConsoleRunner
{
    using Utilities;

    /// <summary>
    /// Summary description for ResultSummary.
    /// </summary>
    public class ResultSummary
    {
        #region Constructor

        public ResultSummary(XmlNode result)
        {
            if (result.Name != "test-run")
                throw new InvalidOperationException("Expected <test-run> as top-level element but was <" + result.Name + ">");

            InitializeCounters();

            Summarize(result);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of test cases for which results
        /// have been summarized. Any tests excluded by use of
        /// Category or Explicit attributes are not counted.
        /// </summary>
        public int TestCount { get; private set; }

        /// <summary>
        /// Returns the number of test cases actually run, which
        /// is the same as TestCount, less any Skipped, Ignored
        /// or NonRunnable tests.
        /// </summary>
        public int RunCount { get; private set; }

        /// <summary>
        /// Gets the count of passed tests
        /// </summary>
        public int PassCount { get; private set; }

        /// <summary>
        /// Gets the count of failed tests, excluding errors and invalid tests
        /// </summary>
        public int FailureCount { get; private set; }

        /// <summary>
        /// Returns the number of test cases that had an error.
        /// </summary>
        public int ErrorCount { get; private set; }

        /// <summary>
        /// Gets the count of inconclusive tests
        /// </summary>
        public int InconclusiveCount { get; private set; }

        /// <summary>
        /// Returns the number of test cases that were not runnable
        /// due to errors in the signature of the class or method.
        /// Such tests are also counted as Errors.
        /// </summary>
        public int InvalidCount { get; private set; }

        /// <summary>
        /// Gets the count of skipped tests, excluding ignored tests
        /// </summary>
        public int SkipCount { get; private set; }

        /// <summary>
        /// Gets the count of ignored tests
        /// </summary>
        public int IgnoreCount { get; private set; }

        #endregion

        #region Helper Methods

        private void InitializeCounters()
        {
            TestCount = 0;
            RunCount = 0;
            PassCount = 0;
            FailureCount = 0;
            ErrorCount = 0;
            InconclusiveCount = 0;
            SkipCount = 0;
            IgnoreCount = 0;
            InvalidCount = 0;
        }

        private void Summarize(XmlNode node)
        {
            switch (node.Name)
            {
                case "test-case":
                    TestCount++;

                    string status = node.GetAttribute("result");
                    string label = node.GetAttribute("label");

                    switch (status)
                    {
                        case "Passed":
                            PassCount++;
                            RunCount++;
                            break;
                        case "Failed":
                            RunCount++;
                            if (label == null)
                                FailureCount++;
                            else if (label == "Invalid")
                                InvalidCount++;
                            else
                                ErrorCount++;
                            break;
                        case "Inconclusive":
                            InconclusiveCount++;
                            RunCount++;
                            break;
                        case "Skipped":
                            if (label == "Ignored")
                                IgnoreCount++;
                            else
                                SkipCount++;
                            break;
                        default:
                            SkipCount++;
                            break;
                    }
                    break;

                //case "test-suite":
                //case "test-fixture":
                //case "method-group":
                default:
                    foreach (XmlNode childResult in node.ChildNodes)
                        Summarize(childResult);
                    break;
            }
        }

        #endregion
    }
}
