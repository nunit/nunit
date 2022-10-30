// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.TestData
{
    [TestFixture]
    public class AssertCountFixture
    {
        public static readonly int ExpectedAssertCount = 5;

        [Test]
        public void BooleanAssert()
        {
            Assert.That(2 + 2 == 4);
        }
        [Test]
        public void ConstraintAssert()
        {
            Assert.That(2 + 2, Is.EqualTo(4));
        }
        [Test]
        public void ThreeAsserts()
        {
            Assert.That(2 + 2 == 4);
            Assert.That(2 + 2, Is.EqualTo(4));
            Assert.That(2 + 2, Is.EqualTo(5));
        }
    }
}
