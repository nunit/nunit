// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.TestData;
using NUnit.TestUtilities;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Attributes
{
    // TODO: Review to see if we need these tests

    [TestFixture]
    public class DescriptionTests
    {
        private static readonly Type FixtureType = typeof(DescriptionFixture);

        [Test]
        public void ReflectionTest()
        {
            Test testCase = TestBuilder.MakeTestCase(FixtureType, nameof(DescriptionFixture.Method));
            Assert.That( testCase.RunState, Is.EqualTo(RunState.Runnable));
        }

        [Test]
        public void Description()
        {
            Test testCase = TestBuilder.MakeTestCase(FixtureType, nameof(DescriptionFixture.Method));
            Assert.That(testCase.Properties.Get(PropertyNames.Description), Is.EqualTo("Test Description"));
        }

        [Test]
        public void NoDescription()
        {
            Test testCase = TestBuilder.MakeTestCase(FixtureType, nameof(DescriptionFixture.NoDescriptionMethod) );
            Assert.That(testCase.Properties.Get(PropertyNames.Description), Is.Null);
        }

        [Test]
        public void LongDescription()
        {
            Test testCase = TestBuilder.MakeTestCase(FixtureType, nameof(DescriptionFixture.TestWithLongDescription));
            Assert.That(testCase.Properties.Get(PropertyNames.Description), Is.EqualTo("This is a really, really, really, really, really, really, really, really, really, really, really, really, really, really, really, really, really, really, really, really, really, really, really, really, really long description"));
        }

        [Test]
        public void FixtureDescription()
        {
            TestSuite suite = new TestSuite("suite");
            suite.Add( TestBuilder.MakeFixture( typeof( DescriptionFixture ) ) );

            TestSuite mockFixtureSuite = (TestSuite)suite.Tests[0];

            Assert.That(mockFixtureSuite.Properties.Get(PropertyNames.Description), Is.EqualTo("Fixture Description"));
        }

        [Test]
        public void SeparateDescriptionAttribute()
        {
            Test testCase = TestBuilder.MakeTestCase(FixtureType, nameof(DescriptionFixture.SeparateDescriptionMethod));
            Assert.That(testCase.Properties.Get(PropertyNames.Description), Is.EqualTo("Separate Description"));
        }

        [Test]
        public void DescriptionOnTestCase()
        {
            TestSuite parameterizedMethodSuite = TestBuilder.MakeParameterizedMethodSuite(FixtureType, nameof(DescriptionFixture.TestCaseWithDescription));
            Assert.That(parameterizedMethodSuite.Properties.Get(PropertyNames.Description), Is.EqualTo("method description"));
            Test testCase = (Test)parameterizedMethodSuite.Tests[0];
            Assert.That(testCase.Properties.Get(PropertyNames.Description), Is.EqualTo("case description"));
        }
    }
}
