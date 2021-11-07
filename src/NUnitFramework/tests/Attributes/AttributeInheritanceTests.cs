// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal;
using NUnit.TestData.AttributeInheritanceData;
using NUnit.TestUtilities;

namespace NUnit.Framework.Attributes
{
    [TestFixture]
    public class AttributeInheritanceTests
    {
        [Test]
        public void InheritedFixtureAttributeIsRecognized()
        {
            Assert.That( TestBuilder.MakeFixture( typeof (When_collecting_test_fixtures) ) != null );
        }

        [Test]
        public void InheritedTestAttributeIsRecognized()
        {
            Test fixture = TestBuilder.MakeFixture( typeof( When_collecting_test_fixtures ) );
            Assert.AreEqual( 1, fixture.TestCaseCount );
        }
    }
}
