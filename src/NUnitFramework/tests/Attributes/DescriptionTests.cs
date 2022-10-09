// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.TestData;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Attributes
{
    // TODO: Review to see if we need these tests

    [TestFixture]
    public class DescriptionTests
    {
        static readonly Type FixtureType = typeof(DescriptionFixture);

        [Test]
        public void ReflectionTest()
        {
            Test testCase = TestBuilder.MakeTestCase(FixtureType, nameof(DescriptionFixture.Method));
            Assert.AreEqual( RunState.Runnable, testCase.RunState );
        }

        [Test]
        public void Description()
        {
            Test testCase = TestBuilder.MakeTestCase(FixtureType, nameof(DescriptionFixture.Method));
            Assert.AreEqual("Test Description", testCase.Properties.Get(PropertyNames.Description));
        }

        [Test]
        public void NoDescription()
        {
            Test testCase = TestBuilder.MakeTestCase(FixtureType, nameof(DescriptionFixture.NoDescriptionMethod) );
            Assert.IsNull(testCase.Properties.Get(PropertyNames.Description));
        }

        [Test]
        public void LongDescription()
        {
            Test testCase = TestBuilder.MakeTestCase(FixtureType, nameof(DescriptionFixture.TestWithLongDescription));
            Assert.AreEqual("This is a really, really, really, really, really, really, really, really, really, really, really, really, really, really, really, really, really, really, really, really, really, really, really, really, really long description", testCase.Properties.Get(PropertyNames.Description));
        }

        [Test]
        public void FixtureDescription()
        {
            TestSuite suite = new TestSuite("suite");
            suite.Add( TestBuilder.MakeFixture( typeof( DescriptionFixture ) ) );

            TestSuite mockFixtureSuite = (TestSuite)suite.Tests[0];

            Assert.AreEqual("Fixture Description", mockFixtureSuite.Properties.Get(PropertyNames.Description));
        }

        [Test]
        public void SeparateDescriptionAttribute()
        {
            Test testCase = TestBuilder.MakeTestCase(FixtureType, nameof(DescriptionFixture.SeparateDescriptionMethod));
            Assert.AreEqual("Separate Description", testCase.Properties.Get(PropertyNames.Description));
        }

        [Test]
        public void DescriptionOnTestCase()
        {
            TestSuite parameterizedMethodSuite = TestBuilder.MakeParameterizedMethodSuite(FixtureType, nameof(DescriptionFixture.TestCaseWithDescription));
            Assert.AreEqual("method description", parameterizedMethodSuite.Properties.Get(PropertyNames.Description));
            Test testCase = (Test)parameterizedMethodSuite.Tests[0];
            Assert.AreEqual("case description", testCase.Properties.Get(PropertyNames.Description));
        }
    }
}
