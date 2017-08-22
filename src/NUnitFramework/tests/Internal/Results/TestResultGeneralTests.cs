// ***********************************************************************
// Copyright (c) 2010-2016 Charlie Poole, Rob Prouse
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
using System.Linq;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Results
{
    public class TestResultGeneralTests : TestResultTests
    {
        protected const double EXPECTED_DURATION = 0.125;
        protected static readonly DateTime EXPECTED_START = new DateTime(1968, 4, 8, 15, 05, 30, 250, DateTimeKind.Utc);
        protected static readonly DateTime EXPECTED_END = EXPECTED_START.AddSeconds(EXPECTED_DURATION);

        private const string TEST_DESCRIPTION = "Test description";
        private const string TEST_CATEGORY = "Dubious";
        private const string TEST_PRIORITY = "low";

        private const string SUITE_DESCRIPTION = "Suite description";
        private const string SUITE_CATEGORY = "Fast";
        private const int SUITE_LEVEL = 3;

        [SetUp]
        public void SimulateTestRun()
        {
            _test.Properties.Set(PropertyNames.Description, TEST_DESCRIPTION);
            _test.Properties.Set(PropertyNames.Category, TEST_CATEGORY);
            _test.Properties.Set("Priority", TEST_PRIORITY);

            _suite.Properties.Set(PropertyNames.Description, SUITE_DESCRIPTION);
            _suite.Properties.Set(PropertyNames.Category, SUITE_CATEGORY);
            _suite.Properties.Set("Level", SUITE_LEVEL);

            _testResult.StartTime = EXPECTED_START;
            _testResult.Duration = EXPECTED_DURATION;
            _testResult.EndTime = EXPECTED_END;

            _suiteResult.StartTime = EXPECTED_START;
            _suiteResult.Duration = EXPECTED_DURATION;
            _suiteResult.EndTime = EXPECTED_END;
        }

        [Test]
        public void TestResult_Identification()
        {
            Assert.AreEqual(_test.Name, _testResult.Name);
            Assert.AreEqual(_test.FullName, _testResult.FullName);
        }

        [Test]
        public void TestResult_Properties()
        {
            Assert.That(_testResult.Test.Properties.Get(PropertyNames.Description), Is.EqualTo(TEST_DESCRIPTION));
            Assert.That(_testResult.Test.Properties.Get(PropertyNames.Category), Is.EqualTo(TEST_CATEGORY));
            Assert.That(_testResult.Test.Properties.Get("Priority"), Is.EqualTo(TEST_PRIORITY));
        }

        [Test]
        public void TestResult_Duration()
        {
            Assert.AreEqual(EXPECTED_START, _testResult.StartTime);
            Assert.AreEqual(EXPECTED_END, _testResult.EndTime);
            Assert.AreEqual(EXPECTED_DURATION, _testResult.Duration);
        }

        [Test]
        public void TestResult_MinimumDuration()
        {
            // Change duration to value less than minimum
            _testResult.Duration = TestResult.MIN_DURATION - 0.000001d;

            Assert.That(_testResult.Duration, Is.EqualTo(TestResult.MIN_DURATION));
        }

        [Test]
        public void SuiteResult_Identification()
        {
            Assert.AreEqual(_suite.Name, _suiteResult.Name);
            Assert.AreEqual(_suite.FullName, _suiteResult.FullName);
        }

        [Test]
        public void SuiteResult_Properties()
        {
            Assert.That(_suiteResult.Test.Properties.Get(PropertyNames.Description), Is.EqualTo(SUITE_DESCRIPTION));
            Assert.That(_suiteResult.Test.Properties.Get(PropertyNames.Category), Is.EqualTo(SUITE_CATEGORY));
            Assert.That(_suiteResult.Test.Properties.Get("Level"), Is.EqualTo(SUITE_LEVEL));
        }

        [Test]
        public void SuiteResult_Duration()
        {
            Assert.AreEqual(EXPECTED_START, _suiteResult.StartTime);
            Assert.AreEqual(EXPECTED_END, _suiteResult.EndTime);
            Assert.AreEqual(EXPECTED_DURATION, _suiteResult.Duration);
        }

        [Test]
        public void SuiteResult_MinimumDuration()
        {
            // Change duration to value less than minimum
            _suiteResult.Duration = TestResult.MIN_DURATION - 0.000001d;

            Assert.That(_suiteResult.Duration, Is.EqualTo(TestResult.MIN_DURATION));
        }

        [Test]
        public void TestResultXml_Identification()
        {
            TNode testNode = _testResult.ToXml(true);

            Assert.NotNull(testNode.Attributes["id"]);
            Assert.AreEqual("test-case", testNode.Name);
            Assert.AreEqual(_testResult.Name, testNode.Attributes["name"]);
            Assert.AreEqual(_testResult.FullName, testNode.Attributes["fullname"]);
        }

        [Test]
        public void TestResultXml_Properties()
        {
            TNode testNode = _testResult.ToXml(true);

            Assert.AreEqual(TEST_DESCRIPTION, testNode.SelectSingleNode("properties/property[@name='Description']").Attributes["value"]);
            Assert.AreEqual(TEST_CATEGORY, testNode.SelectSingleNode("properties/property[@name='Category']").Attributes["value"]);
            Assert.AreEqual(TEST_PRIORITY, testNode.SelectSingleNode("properties/property[@name='Priority']").Attributes["value"]);
        }

        [Test]
        public void TestResultXml_NoChildren()
        {
            TNode testNode = _testResult.ToXml(true);

            Assert.AreEqual(0, testNode.SelectNodes("test-case").Count);
        }

        [Test]
        public void TestResultXml_Duration()
        {
            TNode testNode = _testResult.ToXml(true);

            Assert.AreEqual(EXPECTED_START.ToString("u"), testNode.Attributes["start-time"]);
            Assert.AreEqual(EXPECTED_END.ToString("u"), testNode.Attributes["end-time"]);
            Assert.AreEqual(EXPECTED_DURATION.ToString("0.000000", CultureInfo.InvariantCulture), testNode.Attributes["duration"]);
        }

        [Test]
        public void SuiteResultXml_Identification()
        {
            TNode suiteNode = _suiteResult.ToXml(true);

            Assert.NotNull(suiteNode.Attributes["id"]);
            Assert.AreEqual("test-suite", suiteNode.Name);
            Assert.AreEqual(_suiteResult.Name, suiteNode.Attributes["name"]);
            Assert.AreEqual(_suiteResult.FullName, suiteNode.Attributes["fullname"]);
        }

        [Test]
        public void SuiteResultXml_Properties()
        {
            TNode suiteNode = _suiteResult.ToXml(true);

            Assert.AreEqual(SUITE_DESCRIPTION, suiteNode.SelectSingleNode("properties/property[@name='Description']").Attributes["value"]);
            Assert.AreEqual(SUITE_CATEGORY, suiteNode.SelectSingleNode("properties/property[@name='Category']").Attributes["value"]);
            Assert.AreEqual(SUITE_LEVEL.ToString(), suiteNode.SelectSingleNode("properties/property[@name='Level']").Attributes["value"]);
        }

        [Test]
        public void SuiteResultXml_Children()
        {
            TNode suiteNode = _suiteResult.ToXml(true);

            Assert.AreEqual(_suiteResult.Children.Count(), suiteNode.SelectNodes("test-case").Count);
        }

        [Test]
        public void SuiteResultXml_Duration()
        {
            TNode suiteNode = _suiteResult.ToXml(true);

            Assert.AreEqual(EXPECTED_START.ToString("u"), suiteNode.Attributes["start-time"]);
            Assert.AreEqual(EXPECTED_END.ToString("u"), suiteNode.Attributes["end-time"]);
            Assert.AreEqual(EXPECTED_DURATION.ToString("0.000000", CultureInfo.InvariantCulture), suiteNode.Attributes["duration"]);
        }
    }
}
