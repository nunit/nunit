// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.TestData.AttributeInheritanceData;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.Attributes
{
    [TestFixture]
    public class AttributeInheritanceTests
    {
        [Test]
        public void InheritedFixtureAttributeIsRecognized()
        {
            Assert.That(TestBuilder.MakeFixture(typeof(When_collecting_test_fixtures)) is not null);
        }

        [Test]
        public void InheritedTestAttributeIsRecognized()
        {
            Test fixture = TestBuilder.MakeFixture(typeof(When_collecting_test_fixtures));
            Assert.That(fixture.TestCaseCount, Is.EqualTo(1));
        }
    }
}
