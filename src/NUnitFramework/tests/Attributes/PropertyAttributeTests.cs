// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.TestUtilities;
using NUnit.TestData.PropertyAttributeTests;

namespace NUnit.Framework.Attributes
{
	[TestFixture]
	public class PropertyAttributeTests
	{
		TestSuite fixture;

		[SetUp]
		public void CreateFixture()
		{
			fixture = TestBuilder.MakeFixture( typeof( FixtureWithProperties ) );
		}

		[Test]
		public void PropertyWithStringValue()
		{
			Test test1 = (Test)fixture.Tests[0];
			Assert.That( test1.Properties["user"].Contains("Charlie"));
		}

		[Test]
		public void PropertiesWithNumericValues()
		{
			Test test2 = (Test)fixture.Tests[1];
			Assert.AreEqual( 10.0, test2.Properties.Get("X") );
			Assert.AreEqual( 17.0, test2.Properties.Get("Y") );
		}

		[Test]
		public void PropertyWorksOnFixtures()
		{
			Assert.AreEqual( "SomeClass", fixture.Properties.Get("ClassUnderTest") );
		}

		[Test]
		public void CanDeriveFromPropertyAttribute()
		{
			Test test3 = (Test)fixture.Tests[2];
			Assert.AreEqual( 5, test3.Properties.Get("Priority") );
		}

        [Test]
        public void CustomPropertyAttribute()
        {
            Test test4 = (Test)fixture.Tests[3];
            Assert.IsNotNull(test4.Properties.Get("CustomProperty"));
        }

        [Test]
        public void ManyProperties()
        {
            Test test5 = (Test)fixture.Tests[4];
            Assert.AreEqual(6, test5.Properties.Keys.Count);
            Assert.AreEqual("A", test5.Properties.Get("A"));
            Assert.AreEqual("B", test5.Properties.Get("B"));
            Assert.AreEqual("C", test5.Properties.Get("C"));
            Assert.AreEqual("D", test5.Properties.Get("D"));
            Assert.AreEqual("E", test5.Properties.Get("E"));
            Assert.AreEqual("F", test5.Properties.Get("F"));
        }
    }
}
