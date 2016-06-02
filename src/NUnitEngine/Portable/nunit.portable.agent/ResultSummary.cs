// ***********************************************************************
// Copyright (c) 2016 Charlie Poole
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
using System.Globalization;
using System.Reflection;
using System.Xml.Linq;

namespace NUnit.Engine
{
    /// <summary>
    /// Summary description for ResultSummary.
    /// </summary>
    public class ResultSummary
    {
        IList<XElement> _results = new List<XElement>();

        #region Constructor

        /// <summary>
        /// Parses XML into test results
        /// </summary>
        public ResultSummary()
        {
            StartTime = DateTime.UtcNow;
            InitializeCounters();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Parses the given XML string into test results
        /// </summary>
        /// <param name="xml"></param>
        public void AddResult(string xml)
        {
            var element = XElement.Parse(xml);
            AddResult(element);
        }

        /// <summary>
        /// Parses the given <see cref="XElement"></see> into test results
        /// </summary>
        /// <param name="element"></param>
        public void AddResult(XElement element)
        {
            _results.Add(element);
        }

        /// <summary>
        /// Summarizes all of the results and returns the test result xml document
        /// </summary>
        /// <returns></returns>
        public XDocument GetTestResults()
        {
            InitializeCounters();
            Summarize(_results);
            var test = new XElement("test-run");
            test.Add(new XAttribute("id", "0"));
            test.Add(new XAttribute("testcasecount", TestCount));
            test.Add(new XAttribute("result", Result));
            test.Add(new XAttribute("total", TestCount));
            test.Add(new XAttribute("passed", PassCount));
            test.Add(new XAttribute("failed", FailedCount));
            test.Add(new XAttribute("inconclusive", InconclusiveCount));
            test.Add(new XAttribute("skipped", TotalSkipCount));
            test.Add(new XAttribute("asserts", AssertCount));

            test.Add(new XAttribute("portable-engine-version", typeof(ResultSummary).GetTypeInfo().Assembly.GetName().Version.ToString()));

            EndTime = DateTime.UtcNow;
            Duration = EndTime.Subtract(StartTime).TotalSeconds;

            test.Add(new XAttribute("start-time", StartTime.ToString("u")));
            test.Add(new XAttribute("end-time", EndTime.ToString("u")));
            test.Add(new XAttribute("duration", Duration.ToString("0.000000", NumberFormatInfo.InvariantInfo)));

            foreach (var result in _results)
                test.Add(result);
            
            return new XDocument(test);
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
        /// Returns the number of test cases actually run.
        /// </summary>
        public int RunCount
        {
            get { return PassCount + FailureCount + ErrorCount + InconclusiveCount; }
        }

        /// <summary>
        /// Returns the number of test cases not run for any reason.
        /// </summary>
        public int NotRunCount
        {
            get { return IgnoreCount + ExplicitCount + InvalidCount + SkipCount; }
        }

        /// <summary>
        /// Returns the number of failed test cases (including errors and invalid tests)
        /// </summary>
        public int FailedCount
        {
            get { return FailureCount + InvalidCount + ErrorCount; }
        }

        /// <summary>
        /// Returns the sum of SkipCount test cases, including ignored and explicit tests
        /// </summary>
        public int TotalSkipCount
        {
            get { return SkipCount + IgnoreCount + ExplicitCount; }
        }

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
        /// Gets the count of InconclusiveCount tests
        /// </summary>
        public int InconclusiveCount { get; private set; }

        /// <summary>
        /// Returns the number of test cases that were not runnable
        /// due to errors in the signature of the class or method.
        /// Such tests are also counted as Errors.
        /// </summary>
        public int InvalidCount { get; private set; }

        /// <summary>
        /// Gets the count of SkipCount tests, excluding ignored and explicit tests
        /// </summary>
        public int SkipCount { get; private set; }

        /// <summary>
        /// Gets the count of ignored tests
        /// </summary>
        public int IgnoreCount { get; private set; }

        /// <summary>
        /// Gets the count of tests not run because the are Explicit
        /// </summary>
        public int ExplicitCount { get; private set; }

        /// <summary>
        /// Gets the count of invalid assemblies
        /// </summary>
        public int InvalidAssemblies { get; private set; }

        /// <summary>
        /// An Unexpected error occurred
        /// </summary>
        public bool UnexpectedError { get; private set; }

        /// <summary>
        /// Gets the number of asserts
        /// </summary>
        public int AssertCount { get; private set; }

        /// <summary>
        /// The overall result of the test run
        /// </summary>
        public string Result { get; private set; }

        /// <summary>
        /// The start of the test run
        /// </summary>
        public DateTime StartTime { get; private set; }

        /// <summary>
        /// The end of the test run
        /// </summary>
        public DateTime EndTime { get; private set; }

        /// <summary>
        /// The length of the test run in seconds
        /// </summary>
        public double Duration { get; private set; }
  		  
        /// <summary>
        /// Invalid test fixture(s) were found
        /// </summary>
        public int InvalidTestFixtures { get; private set; }

        #endregion

        #region Helper Methods

        void InitializeCounters()
        {
            TestCount = 0;
            PassCount = 0;
            FailureCount = 0;
            ErrorCount = 0;
            InconclusiveCount = 0;
            SkipCount = 0;
            IgnoreCount = 0;
            ExplicitCount = 0;
            InvalidCount = 0;
            InvalidAssemblies = 0;
            InvalidTestFixtures = 0;
            AssertCount = 0;
        }

        void Summarize(IEnumerable<XElement> elements)
        {
            foreach (XElement element in elements)
                Summarize(element);
        }

        void Summarize(XElement element)
        {
            string type = element.Attribute("type")?.Value;
            string status = element.Attribute("result")?.Value;
            string label = element.Attribute("label")?.Value;

            switch (element.Name.ToString())
            {
                case "test-case":
                    TestCount++;

                    switch (status)
                    {
                        case "Passed":
                            PassCount++;
                            break;
                        case "Failed":
                            if (label == null)
                                FailureCount++;
                            else if (label == "Invalid")
                                InvalidCount++;
                            else
                                ErrorCount++;
                            break;
                        case "Inconclusive":
                            InconclusiveCount++;
                            break;
                        case "Skipped":
                            if (label == "Ignored")
                                IgnoreCount++;
                            else if (label == "Explicit")
                                ExplicitCount++;
                            else
                                SkipCount++;
                            break;
                        default:
                            SkipCount++;
                            break;
                    }
                    break;

                case "test-suite":
                    if (status == "Failed" && label == "Invalid")
                    {
                        if (type == "Assembly")
                            InvalidAssemblies++;
                        else
                            InvalidTestFixtures++;
                    }

                    if (type == "Assembly" && status == "Failed" && label == "Error")
                    {
                        InvalidAssemblies++;
                        UnexpectedError = true;
                    }

                    if (type == "Assembly")
                    {
                        var asserts = element.Attribute("asserts")?.Value;
                        if (!string.IsNullOrWhiteSpace(asserts))
                        {
                            int count = 0;
                            if (int.TryParse(asserts, out count))
                                AssertCount += count;
                        }
                    }

                    if (string.IsNullOrWhiteSpace(Result))
                    {
                        Result = status;
                    }
                    else
                    {
                        switch (status)
                        {
                            case "Passed":
                                if (Result != "Failed")
                                    Result = status;
                                break;
                            case "Failed":
                                Result = status;
                                break;
                        }
                    }

                    Summarize(element.Elements());
                    break;

                case "test-run":
                    Summarize(element.Elements());
                    break;
            }
        }

        #endregion
    }
}
