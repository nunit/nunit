// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Globalization;
using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.Internal.Results
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
            Test.Properties.Set(PropertyNames.Description, TEST_DESCRIPTION);
            Test.Properties.Set(PropertyNames.Category, TEST_CATEGORY);
            Test.Properties.Set("Priority", TEST_PRIORITY);

            Suite.Properties.Set(PropertyNames.Description, SUITE_DESCRIPTION);
            Suite.Properties.Set(PropertyNames.Category, SUITE_CATEGORY);
            Suite.Properties.Set("Level", SUITE_LEVEL);

            TestResult.StartTime = EXPECTED_START;
            TestResult.Duration = EXPECTED_DURATION;
            TestResult.EndTime = EXPECTED_END;

            SuiteResult.StartTime = EXPECTED_START;
            SuiteResult.Duration = EXPECTED_DURATION;
            SuiteResult.EndTime = EXPECTED_END;
        }

        [Test]
        public void TestResult_Identification()
        {
            Assert.That(TestResult.Name, Is.EqualTo(Test.Name));
            Assert.That(TestResult.FullName, Is.EqualTo(Test.FullName));
        }

        [Test]
        public void TestResult_Properties()
        {
            Assert.That(TestResult.Test.Properties.Get(PropertyNames.Description), Is.EqualTo(TEST_DESCRIPTION));
            Assert.That(TestResult.Test.Properties.Get(PropertyNames.Category), Is.EqualTo(TEST_CATEGORY));
            Assert.That(TestResult.Test.Properties.Get("Priority"), Is.EqualTo(TEST_PRIORITY));
        }

        [Test]
        public void TestResult_Duration()
        {
            Assert.That(TestResult.StartTime, Is.EqualTo(EXPECTED_START));
            Assert.That(TestResult.EndTime, Is.EqualTo(EXPECTED_END));
            Assert.That(TestResult.Duration, Is.EqualTo(EXPECTED_DURATION));
        }

        [Test]
        public void TestResult_MinimumDuration()
        {
            // Change duration to value less than minimum
            TestResult.Duration = TestResult.MIN_DURATION - 0.000001d;

            Assert.That(TestResult.Duration, Is.EqualTo(TestResult.MIN_DURATION));
        }

        [Test]
        public void SuiteResult_Identification()
        {
            Assert.That(SuiteResult.Name, Is.EqualTo(Suite.Name));
            Assert.That(SuiteResult.FullName, Is.EqualTo(Suite.FullName));
        }

        [Test]
        public void SuiteResult_Properties()
        {
            Assert.That(SuiteResult.Test.Properties.Get(PropertyNames.Description), Is.EqualTo(SUITE_DESCRIPTION));
            Assert.That(SuiteResult.Test.Properties.Get(PropertyNames.Category), Is.EqualTo(SUITE_CATEGORY));
            Assert.That(SuiteResult.Test.Properties.Get("Level"), Is.EqualTo(SUITE_LEVEL));
        }

        [Test]
        public void SuiteResult_Duration()
        {
            Assert.That(SuiteResult.StartTime, Is.EqualTo(EXPECTED_START));
            Assert.That(SuiteResult.EndTime, Is.EqualTo(EXPECTED_END));
            Assert.That(SuiteResult.Duration, Is.EqualTo(EXPECTED_DURATION));
        }

        [Test]
        public void SuiteResult_MinimumDuration()
        {
            // Change duration to value less than minimum
            SuiteResult.Duration = TestResult.MIN_DURATION - 0.000001d;

            Assert.That(SuiteResult.Duration, Is.EqualTo(TestResult.MIN_DURATION));
        }

        [Test]
        public void TestResultXml_Identification()
        {
            TNode testNode = TestResult.ToXml(true);

            Assert.Multiple(() =>
            {
                Assert.That(testNode.Attributes["id"], Is.Not.Null);
                Assert.That(testNode.Name, Is.EqualTo("test-case"));
                Assert.That(testNode.Attributes["name"], Is.EqualTo(TestResult.Name));
                Assert.That(testNode.Attributes["fullname"], Is.EqualTo(TestResult.FullName));
            });
        }

        [Test]
        public void TestResultXml_Properties()
        {
            TNode testNode = TestResult.ToXml(true);

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
            TNode testNode = TestResult.ToXml(true);

            Assert.That(testNode.SelectNodes("test-case"), Is.Empty);
        }

        [Test]
        public void TestResultXml_Duration()
        {
            TNode testNode = TestResult.ToXml(true);

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
            TNode suiteNode = SuiteResult.ToXml(true);

            Assert.Multiple(() =>
            {
                Assert.That(suiteNode.Attributes["id"], Is.Not.Null);
                Assert.That(suiteNode.Name, Is.EqualTo("test-suite"));
                Assert.That(suiteNode.Attributes["name"], Is.EqualTo(SuiteResult.Name));
                Assert.That(suiteNode.Attributes["fullname"], Is.EqualTo(SuiteResult.FullName));
            });
        }

        [Test]
        public void SuiteResultXml_Properties()
        {
            TNode suiteNode = SuiteResult.ToXml(true);

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
            TNode suiteNode = SuiteResult.ToXml(true);

            Assert.That(suiteNode.SelectNodes("test-case"), Has.Count.EqualTo(SuiteResult.Children.Count()));
        }

        [Test]
        public void SuiteResultXml_Duration()
        {
            TNode suiteNode = SuiteResult.ToXml(true);

            Assert.Multiple(() =>
            {
                Assert.That(suiteNode.Attributes["start-time"], Is.EqualTo(EXPECTED_START.ToString("o")));
                Assert.That(suiteNode.Attributes["end-time"], Is.EqualTo(EXPECTED_END.ToString("o")));
                Assert.That(suiteNode.Attributes["duration"], Is.EqualTo(EXPECTED_DURATION.ToString("0.000000", CultureInfo.InvariantCulture)));
            });
        }
    }
}
