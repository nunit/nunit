// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Tests.Assertions;

namespace NUnit.Framework.Legacy.Tests
{
    [TestFixture]
    public class SameFixture
    {
        private readonly string _s1 = "S1";
        private readonly string _s2 = "S2";

        [Test]
        public void NotSame()
        {
            string alsoS1 = _s1;
            Legacy.ClassicAssert.AreSame(_s1, alsoS1);
            Legacy.ClassicAssert.AreSame(_s1, alsoS1, "Different {0}", "string");
        }

        [Test]
        public void NotSameFails()
        {
            var expectedMessage =
                "  Expected: same as \"S1\"" + Environment.NewLine +
                "  But was:  \"S2\"" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Legacy.ClassicAssert.AreSame(_s1, _s2));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
            ex = Assert.Throws<AssertionException>(() => Legacy.ClassicAssert.AreSame(_s1, _s2, "Same {0}", "string"));
            Assert.That(ex?.Message, Does.Contain("Same string"));
        }

        [Test]
        public void ShouldNotCallToStringOnClassForPassingTests()
        {
            var expected = new ThrowsIfToStringIsCalled(1);
            var actual = expected;

            Legacy.ClassicAssert.AreSame(expected, actual);
        }
    }
}
