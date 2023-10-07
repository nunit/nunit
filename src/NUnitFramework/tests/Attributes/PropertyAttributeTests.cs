// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.TestData.PropertyAttributeTests;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.Attributes
{
    [TestFixture]
    public class PropertyAttributeTests
    {
        private TestSuite _fixture;

        [SetUp]
        public void CreateFixture()
        {
            _fixture = TestBuilder.MakeFixture(typeof(FixtureWithProperties));
        }

        [Test]
        public void PropertyWithStringValue()
        {
            Test test1 = (Test)_fixture.Tests[0];
            Assert.That(test1.Properties["user"].Contains("Charlie"));
        }

        [Test]
        public void PropertiesWithNumericValues()
        {
            Test test2 = (Test)_fixture.Tests[1];
            Assert.That(test2.Properties.Get("X"), Is.EqualTo(10.0));
            Assert.That(test2.Properties.Get("Y"), Is.EqualTo(17.0));
        }

        [Test]
        public void PropertyWorksOnFixtures()
        {
            Assert.That(_fixture.Properties.Get("ClassUnderTest"), Is.EqualTo("SomeClass"));
        }

        [Test]
        public void CanDeriveFromPropertyAttribute()
        {
            Test test3 = (Test)_fixture.Tests[2];
            Assert.That(test3.Properties.Get("Priority"), Is.EqualTo(5));
        }

        [Test]
        public void CustomPropertyAttribute()
        {
            Test test4 = (Test)_fixture.Tests[3];
            Assert.That(test4.Properties.Get("CustomProperty"), Is.Not.Null);
        }

        [Test]
        public void ManyProperties()
        {
            Test test5 = (Test)_fixture.Tests[4];
            Assert.That(test5.Properties.Keys, Has.Count.EqualTo(6));
            Assert.That(test5.Properties.Get("A"), Is.EqualTo("A"));
            Assert.That(test5.Properties.Get("B"), Is.EqualTo("B"));
            Assert.That(test5.Properties.Get("C"), Is.EqualTo("C"));
            Assert.That(test5.Properties.Get("D"), Is.EqualTo("D"));
            Assert.That(test5.Properties.Get("E"), Is.EqualTo("E"));
            Assert.That(test5.Properties.Get("F"), Is.EqualTo("F"));
        }
    }
}
