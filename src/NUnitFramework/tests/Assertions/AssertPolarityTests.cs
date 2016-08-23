// ***********************************************************************
// Copyright (c) 2006 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Assertions
{
    [TestFixture]
    public class AssertPolarityTests
    {
        private readonly int i1 = 1;
        private readonly int i2 = -1;
        private readonly uint u1 = 123456789;
        private readonly long l1 = 123456789;
        private readonly long l2 = -12345879;
        private readonly ulong ul1 = 123456890;
        private readonly float f1 = 8.543F;
        private readonly float f2 = -8.543F;
        private readonly decimal de1 = 83.4M;
        private readonly decimal de2 = -83.4M;
        private readonly double d1 = 8.0;
        private readonly double d2 = -8.0;

        [Test]
        public void PositiveNumbersPassPositiveAssertion()
        {
            Assert.Positive(i1);
            Assert.Positive(l1);
            Assert.Positive(f1);
            Assert.Positive(de1);
            Assert.Positive(d1);
        }

        [Test]
        public void AssertNegativeNumbersFailPositiveAssertion()
        {
            Assert.Throws<AssertionException>(() => Assert.Positive(i2));
            Assert.Throws<AssertionException>(() => Assert.Positive(l2));
            Assert.Throws<AssertionException>(() => Assert.Positive(f2));
            Assert.Throws<AssertionException>(() => Assert.Positive(de2));
            Assert.Throws<AssertionException>(() => Assert.Positive(d2));
        }

        [Test]
        public void NegativeNumbersPassNegativeAssertion()
        {
            Assert.Negative(i2);
            Assert.Negative(l2);
            Assert.Negative(f2);
            Assert.Negative(de2);
            Assert.Negative(d2);
        }

        [Test]
        public void AssertPositiveNumbersFailNegativeAssertion()
        {
            Assert.Throws<AssertionException>(() => Assert.Negative(i1));
            Assert.Throws<AssertionException>(() => Assert.Negative(u1));
            Assert.Throws<AssertionException>(() => Assert.Negative(l1));
            Assert.Throws<AssertionException>(() => Assert.Negative(ul1));
            Assert.Throws<AssertionException>(() => Assert.Negative(f1));
            Assert.Throws<AssertionException>(() => Assert.Negative(de1));
            Assert.Throws<AssertionException>(() => Assert.Negative(d1));
        }

        [Test]
        public void IsNegativeWithMessageOverload()
        {
            Assert.That(
                () => Assert.Negative(1, "MESSAGE"),
                Throws.TypeOf<AssertionException>().With.Message.Contains("MESSAGE"));
        }

        [Test]
        public void IsPositiveWithMessageOverload()
        {
            Assert.That(
                () => Assert.Positive(-1, "MESSAGE"),
                Throws.TypeOf<AssertionException>().With.Message.Contains("MESSAGE"));
        }

        [Test]
        public void IsPositiveWithMessageOverloadPasses()
        {
            Assert.Positive(1, "Message");
        }

        [Test]
        public void IsNegativeWithMessageOverloadPasses()
        {
            Assert.Negative(-1, "Message");
        }

        [Test]
        public void ExpectedFailureMessageExistsForIsPositive()
        {
            var expectedMessage =
                "  Expected: greater than 0" + Environment.NewLine +
                "  But was:  -1" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.Positive(i2));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void ExpectedFailureMessageExistsForIsNegative()
        {
            var expectedMessage =
                "  Expected: less than 0" + Environment.NewLine +
                "  But was:  1" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.Negative(i1));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }
    }
}


