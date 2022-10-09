// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;

namespace NUnit.Framework.Assertions
{
    [TestFixture]
    public class SameFixture
    {
        [Test]
        public void Same()
        {
            string s1 = "S1";
            Assert.AreSame(s1, s1);
        }

        [Test]
        public void SameFails()
        {
            Exception ex1 = new Exception( "one" );
            Exception ex2 = new Exception( "two" );
            var expectedMessage =
                "  Expected: same as <System.Exception: one>" + Environment.NewLine +
                "  But was:  <System.Exception: two>" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.AreSame(ex1, ex2));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void SameValueTypes()
        {
            int index = 2;
            var expectedMessage =
                "  Expected: same as 2" + Environment.NewLine +
                "  But was:  2" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.AreSame(index, index));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void ShouldNotCallToStringOnClassForPassingTests()
        {
            var actual = new ThrowsIfToStringIsCalled(1);

            Assert.AreSame(actual, actual);
        }
    }
}
