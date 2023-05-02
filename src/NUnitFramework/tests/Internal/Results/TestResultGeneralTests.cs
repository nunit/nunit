// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
            Assert.That(_testResult.Name, Is.EqualTo(_test.Name));
            Assert.That(_testResult.FullName, Is.EqualTo(_test.FullName));
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
            Assert.That(_testResult.StartTime, Is.EqualTo(EXPECTED_START));
            Assert.That(_testResult.EndTime, Is.EqualTo(EXPECTED_END));
            Assert.That(_testResult.Duration, Is.EqualTo(EXPECTED_DURATION));
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
            Assert.That(_suiteResult.Name, Is.EqualTo(_suite.Name));
            Assert.That(_suiteResult.FullName, Is.EqualTo(_suite.FullName));
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
            Assert.That(_suiteResult.StartTime, Is.EqualTo(EXPECTED_START));
            Assert.That(_suiteResult.EndTime, Is.EqualTo(EXPECTED_END));
            Assert.That(_suiteResult.Duration, Is.EqualTo(EXPECTED_DURATION));
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

            Assert.Multiple(() =>
            {
                Assert.That(testNode.Attributes["id"], Is.Not.Null);
                Assert.That(testNode.Name, Is.EqualTo("test-case"));
                Assert.That(testNode.Attributes["name"], Is.EqualTo(_testResult.Name));
                Assert.That(testNode.Attributes["fullname"], Is.EqualTo(_testResult.FullName));
            });
        }

        [Test]
        public void TestResultXml_Properties()
        {
            TNode testNode = _testResult.ToXml(true);

            Assert.Multiple(() =>
            {
                Assert.That(testNode.SelectSingleNode("properties/property[@name='Description']")?.Attributes["value"], Is.EqualTo(TEST_DESCRIPTION));
                Assert.That(testNode.SelectSingleNode("properties/property[@name='Category']")?.Attributes["value"], Is.EqualTo(TEST_CATEGORY));
                Assert.That(testNode.SelectSingleNode("properties/property[@name='Priority']")?.Attributes["value"], Is.EqualTo(TEST_PRIORITY));
            });
        }

        [Test]
        public void TestResultXml_NoChildren()
        {
            TNode testNode = _testResult.ToXml(true);

            Assert.That(testNode.SelectNodes("test-case"), Is.Empty);
        }

        [Test]
        public void TestResultXml_Duration()
        {
            TNode testNode = _testResult.ToXml(true);

            Assert.Multiple(() =>
            {
                Assert.That(testNode.Attributes["start-time"], Is.EqualTo(EXPECTED_START.ToString("o")));
                Assert.That(testNode.Attributes["end-time"], Is.EqualTo(EXPECTED_END.ToString("o")));
                Assert.That(testNode.Attributes["duration"], Is.EqualTo(EXPECTED_DURATION.ToString("0.000000", CultureInfo.InvariantCulture)));
            });
        }

        [Test]
        public void SuiteResultXml_Identification()
        {
            TNode suiteNode = _suiteResult.ToXml(true);

            Assert.Multiple(() =>
            {
                Assert.That(suiteNode.Attributes["id"], Is.Not.Null);
                Assert.That(suiteNode.Name, Is.EqualTo("test-suite"));
                Assert.That(suiteNode.Attributes["name"], Is.EqualTo(_suiteResult.Name));
                Assert.That(suiteNode.Attributes["fullname"], Is.EqualTo(_suiteResult.FullName));
            });
        }

        [Test]
        public void SuiteResultXml_Properties()
        {
            TNode suiteNode = _suiteResult.ToXml(true);

            Assert.Multiple(() =>
            {
                Assert.That(suiteNode.SelectSingleNode("properties/property[@name='Description']")?.Attributes["value"], Is.EqualTo(SUITE_DESCRIPTION));
                Assert.That(suiteNode.SelectSingleNode("properties/property[@name='Category']")?.Attributes["value"], Is.EqualTo(SUITE_CATEGORY));
                Assert.That(suiteNode.SelectSingleNode("properties/property[@name='Level']")?.Attributes["value"], Is.EqualTo(SUITE_LEVEL.ToString()));
            });
        }

        [Test]
        public void SuiteResultXml_Children()
        {
            TNode suiteNode = _suiteResult.ToXml(true);

            Assert.That(suiteNode.SelectNodes("test-case"), Has.Count.EqualTo(_suiteResult.Children.Count()));
        }

        [Test]
        public void SuiteResultXml_Duration()
        {
            TNode suiteNode = _suiteResult.ToXml(true);

            Assert.Multiple(() =>
            {
                Assert.That(suiteNode.Attributes["start-time"], Is.EqualTo(EXPECTED_START.ToString("o")));
                Assert.That(suiteNode.Attributes["end-time"], Is.EqualTo(EXPECTED_END.ToString("o")));
                Assert.That(suiteNode.Attributes["duration"], Is.EqualTo(EXPECTED_DURATION.ToString("0.000000", CultureInfo.InvariantCulture)));
            });
        }
    }
}
