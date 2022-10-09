// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#region Using Directives

#endregion

namespace NUnit.Framework.Attributes
{
    [TestFixture]
    [Author("Rob Prouse", "rob@prouse.org")]
    [TestOf(typeof(TestOfAttribute))]
    public class TestOfTests
    {
        static readonly Type FixtureType = typeof(TestOfFixture);

        [Test]
        public void ReflectionTest()
        {
            Test testCase = TestBuilder.MakeTestCase(FixtureType, nameof(TestOfFixture.Method));
            Assert.AreEqual(RunState.Runnable, testCase.RunState);
        }

        [Test]
        public void TestOf()
        {
            Test testCase = TestBuilder.MakeTestCase(FixtureType, nameof(TestOfFixture.Method));
            Assert.AreEqual("NUnit.Framework.TestOfAttribute", testCase.Properties.Get(PropertyNames.TestOf));
        }

        [Test]
        public void NoTestOf()
        {
            Test testCase = TestBuilder.MakeTestCase(FixtureType, nameof(TestOfFixture.NoTestOfMethod));
            Assert.IsNull(testCase.Properties.Get(PropertyNames.TestOf));
        }

        [Test]
        public void FixtureTestOf()
        {
            var suite = new TestSuite("suite");
            suite.Add(TestBuilder.MakeFixture(FixtureType));

            var mockFixtureSuite = (TestSuite)suite.Tests[0];

            Assert.AreEqual("NUnit.Framework.TestOfAttribute", mockFixtureSuite.Properties.Get(PropertyNames.TestOf));
        }

        [Test]
        public void SeparateTestOfAttribute()
        {
            Test testCase = TestBuilder.MakeTestCase(FixtureType, nameof(TestOfFixture.SeparateTestOfTypeMethod));
            Assert.AreEqual("NUnit.Framework.TestOfAttribute", testCase.Properties.Get(PropertyNames.TestOf));
        }

        [Test]
        public void SeparateTestOfStringMethod()
        {
            Test testCase = TestBuilder.MakeTestCase(FixtureType, nameof(TestOfFixture.SeparateTestOfStringMethod));
            Assert.AreEqual("NUnit.Framework.TestOfAttribute", testCase.Properties.Get(PropertyNames.TestOf));
        }

        [Test]
        public void TestOfOnTestCase()
        {
            TestSuite parameterizedMethodSuite = TestBuilder.MakeParameterizedMethodSuite(FixtureType, "TestCaseWithTestOf");
            Assert.AreEqual("NUnit.Framework.TestAttribute", parameterizedMethodSuite.Properties.Get(PropertyNames.TestOf));
            var testCase = (Test)parameterizedMethodSuite.Tests[0];
            Assert.AreEqual("NUnit.Framework.TestCaseAttribute", testCase.Properties.Get(PropertyNames.TestOf));
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
