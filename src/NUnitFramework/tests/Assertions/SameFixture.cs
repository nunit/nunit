// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Tests.Assertions
{
    [TestFixture]
    public class SameFixture
    {
        [Test]
        public void Same()
        {
            string s1 = "S1";
            Assert.That(s1, Is.SameAs(s1));
        }

        [Test]
        public void SameFails()
        {
            Exception ex1 = new Exception("one");
            Exception ex2 = new Exception("two");
            var expectedMessage =
                "  Expected: same as <System.Exception: one>" + Environment.NewLine +
                "  But was:  <System.Exception: two>" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.That(ex2, Is.SameAs(ex1)));
            Assert.That(ex?.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void SameValueTypes()
        {
            int index = 2;
            var expectedMessage =
                "  Expected: same as 2" + Environment.NewLine +
                "  But was:  2" + Environment.NewLine;
#pragma warning disable NUnit2040 // Non-reference types for SameAs constraint
            var ex = Assert.Throws<AssertionException>(() => Assert.That(index, Is.SameAs(index)));
#pragma warning restore NUnit2040 // Non-reference types for SameAs constraint
            Assert.That(ex?.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void ShouldNotCallToStringOnClassForPassingTests()
        {
            var actual = new ThrowsIfToStringIsCalled(1);

            Assert.That(actual, Is.SameAs(actual));
        }
    }
}
