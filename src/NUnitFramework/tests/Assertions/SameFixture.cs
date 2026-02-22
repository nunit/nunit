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
            var ex1 = new Exception("one");
            var ex2 = new Exception("two");
            var expectedMessage =
                "  Expected: same as <System.Exception: one>" + Environment.NewLine +
                "  But was:  <System.Exception: two>" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.That(ex2, Is.SameAs(ex1)));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void SameUsingInternedConstStrings()
        {
            string s1 = "S1";
            string s2 = "S1";
            Assert.That(s1, Is.SameAs(s2));
        }

        [Test]
        public void SameUsingBoxedValueTypes()
        {
            object x = 1;
            Assert.That(x, Is.SameAs(x));
        }

        [TestCase(2, TypeArgs = [typeof(int)])]
        [TestCase(2, TypeArgs = [typeof(int?)])]
        public void SameValueTypes<T>(T index)
        {
            var expectedMessage =
                "  Expected: same as 2" + Environment.NewLine +
                "  But was:  2" + Environment.NewLine;
#pragma warning disable NUnit2040 // Non-reference types for SameAs constraint
            var ex = Assert.Throws<AssertionException>(() => Assert.That(index, Is.SameAs(index)));
#pragma warning restore NUnit2040 // Non-reference types for SameAs constraint
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void NullableValueTypesWithDefaultValue()
        {
            int? index = default;
#pragma warning disable NUnit2040 // Non-reference types for SameAs constraint
            Assert.That(index, Is.SameAs(index));
#pragma warning disable NUnit2040 // Non-reference types for SameAs constraint
        }

        [Test]
        public void ShouldNotCallToStringOnClassForPassingTests()
        {
            var actual = new ThrowsIfToStringIsCalled(1);

            Assert.That(actual, Is.SameAs(actual));
        }
    }
}
