// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.TestData;
using NUnit.TestUtilities;

namespace NUnit.Framework.Attributes
{
    [TestFixture]
    [Author("Rob Prouse", "rob@prouse.org")]
    [TestOf(typeof(TestOfAttribute))]
    public class TestOfTests
    {
        private static readonly Type FixtureType = typeof(TestOfFixture);

        [Test]
        public void ReflectionTest()
        {
            Test testCase = TestBuilder.MakeTestCase(FixtureType, nameof(TestOfFixture.Method));
            Assert.That(testCase.RunState, Is.EqualTo(RunState.Runnable));
        }

        [Test]
        public void TestOf()
        {
            Test testCase = TestBuilder.MakeTestCase(FixtureType, nameof(TestOfFixture.Method));
            Assert.That(testCase.Properties.Get(PropertyNames.TestOf), Is.EqualTo("NUnit.Framework.TestOfAttribute"));
        }

        [Test]
        public void NoTestOf()
        {
            Test testCase = TestBuilder.MakeTestCase(FixtureType, nameof(TestOfFixture.NoTestOfMethod));
            Assert.That(testCase.Properties.Get(PropertyNames.TestOf), Is.Null);
        }

        [Test]
        public void FixtureTestOf()
        {
            var suite = new TestSuite("suite");
            suite.Add(TestBuilder.MakeFixture(FixtureType));

            var mockFixtureSuite = (TestSuite)suite.Tests[0];

            Assert.That(mockFixtureSuite.Properties.Get(PropertyNames.TestOf), Is.EqualTo("NUnit.Framework.TestOfAttribute"));
        }

        [Test]
        public void SeparateTestOfAttribute()
        {
            Test testCase = TestBuilder.MakeTestCase(FixtureType, nameof(TestOfFixture.SeparateTestOfTypeMethod));
            Assert.That(testCase.Properties.Get(PropertyNames.TestOf), Is.EqualTo("NUnit.Framework.TestOfAttribute"));
        }

        [Test]
        public void SeparateTestOfStringMethod()
        {
            Test testCase = TestBuilder.MakeTestCase(FixtureType, nameof(TestOfFixture.SeparateTestOfStringMethod));
            Assert.That(testCase.Properties.Get(PropertyNames.TestOf), Is.EqualTo("NUnit.Framework.TestOfAttribute"));
        }

        [Test]
        public void TestOfOnTestCase()
        {
            TestSuite parameterizedMethodSuite = TestBuilder.MakeParameterizedMethodSuite(FixtureType, "TestCaseWithTestOf");
            Assert.That(parameterizedMethodSuite.Properties.Get(PropertyNames.TestOf), Is.EqualTo("NUnit.Framework.TestAttribute"));
            var testCase = (Test)parameterizedMethodSuite.Tests[0];
            Assert.That(testCase.Properties.Get(PropertyNames.TestOf), Is.EqualTo("NUnit.Framework.TestCaseAttribute"));
        }

        [Test]
        public void TestOfAttributeMultipleTimes()
        {
            Test testCase = TestBuilder.MakeTestCase(FixtureType, "TestOfMultipleAttributesMethod");
            Assert.That(testCase.Properties[PropertyNames.TestOf], Is.EquivalentTo(
                new[] { "NUnit.Framework.TestOfAttribute", "NUnit.Framework.TestAttribute" }));
        }

        [Test]
        public void TestFixtureMultipleTestOfAttributes()
        {
            var suite = new TestSuite("suite");
            suite.Add(TestBuilder.MakeFixture(typeof(TestOfFixture)));
            var mockFixtureSuite = (TestSuite)suite.Tests[0];
            Assert.That(mockFixtureSuite.Properties[PropertyNames.TestOf], Is.EquivalentTo(
                new[] { "NUnit.Framework.TestOfAttribute", "NUnit.Framework.TestOfAttribute", "NUnit.Framework.TestFixtureAttribute" }));
        }
    }
}
