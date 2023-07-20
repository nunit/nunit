// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Classic.Tests
{
    [TestFixture]
    public class GreaterFixture
    {
        private readonly int _i1 = 5;
        private readonly int _i2 = 4;
        private readonly uint _u1 = 12345879;
        private readonly uint _u2 = 12345678;
        private readonly long _l1 = 12345879;
        private readonly long _l2 = 12345678;
        private readonly ulong _ul1 = 12345879;
        private readonly ulong _ul2 = 12345678;
        private readonly float _f1 = 3.543F;
        private readonly float _f2 = 2.543F;
        private readonly decimal _de1 = 53.4M;
        private readonly decimal _de2 = 33.4M;
        private readonly double _d1 = 4.85948654;
        private readonly double _d2 = 1.0;
        private readonly Enum _e1 = RunState.Explicit;
        private readonly Enum _e2 = RunState.Ignored;

        [Test]
        public void Greater()
        {
            Assert.Greater(_i1, _i2);
            Assert.Greater(_u1, _u2);
            Assert.Greater(_l1, _l2);
            Assert.Greater(_ul1, _ul2);
            Assert.Greater(_d1, _d2, "double");
            Assert.Greater(_de1, _de2, "{0}", "decimal");
            Assert.Greater(_f1, _f2, "float");
        }

        [Test]
        public void MixedTypes()
        {
            Assert.Greater(5, 3L, "int to long");
            Assert.Greater(5, 3.5f, "int to float");
            Assert.Greater(5, 3.5d, "int to double");
            Assert.Greater(5, 3U, "int to uint");
            Assert.Greater(5, 3UL, "int to ulong");
            Assert.Greater(5, 3M, "int to decimal");

            Assert.Greater(5L, 3, "long to int");
            Assert.Greater(5L, 3.5f, "long to float");
            Assert.Greater(5L, 3.5d, "long to double");
            Assert.Greater(5L, 3U, "long to uint");
            Assert.Greater(5L, 3UL, "long to ulong");
            Assert.Greater(5L, 3M, "long to decimal");

            Assert.Greater(8.2f, 5, "float to int");
            Assert.Greater(8.2f, 8L, "float to long");
            Assert.Greater(8.2f, 3.5d, "float to double");
            Assert.Greater(8.2f, 8U, "float to uint");
            Assert.Greater(8.2f, 8UL, "float to ulong");
            Assert.Greater(8.2f, 3.5M, "float to decimal");

            Assert.Greater(8.2d, 5, "double to int");
            Assert.Greater(8.2d, 5L, "double to long");
            Assert.Greater(8.2d, 3.5f, "double to float");
            Assert.Greater(8.2d, 8U, "double to uint");
            Assert.Greater(8.2d, 8UL, "double to ulong");
            Assert.Greater(8.2d, 3.5M, "double to decimal");

            Assert.Greater(5U, 3, "uint to int");
            Assert.Greater(5U, 3L, "uint to long");
            Assert.Greater(5U, 3.5f, "uint to float");
            Assert.Greater(5U, 3.5d, "uint to double");
            Assert.Greater(5U, 3UL, "uint to ulong");
            Assert.Greater(5U, 3M, "uint to decimal");

            Assert.Greater(5ul, 3, "ulong to int");
            Assert.Greater(5UL, 3L, "ulong to long");
            Assert.Greater(5UL, 3.5f, "ulong to float");
            Assert.Greater(5UL, 3.5d, "ulong to double");
            Assert.Greater(5UL, 3U, "ulong to uint");
            Assert.Greater(5UL, 3M, "ulong to decimal");

            Assert.Greater(5M, 3, "decimal to int");
            Assert.Greater(5M, 3L, "decimal to long");
            Assert.Greater(5M, 3.5f, "decimal to float");
            Assert.Greater(5M, 3.5d, "decimal to double");
            Assert.Greater(5M, 3U, "decimal to uint");
            Assert.Greater(5M, 3UL, "decimal to ulong");
        }

        [Test]
        public void NotGreaterWhenEqual()
        {
            var expectedMessage =
                "  Expected: greater than 5" + Environment.NewLine +
                "  But was:  5" + Environment.NewLine;
            var ex = Framework.Assert.Throws<AssertionException>(() => Assert.Greater(_i1, _i1));
            Framework.Assert.That(ex?.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void NotGreater()
        {
            var expectedMessage =
                "  Expected: greater than 5" + Environment.NewLine +
                "  But was:  4" + Environment.NewLine;
            var ex = Framework.Assert.Throws<AssertionException>(() => Assert.Greater(_i2, _i1));
            Framework.Assert.That(ex?.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void NotGreaterIComparable()
        {
            var expectedMessage =
                "  Expected: greater than Ignored" + Environment.NewLine +
                "  But was:  Explicit" + Environment.NewLine;
            var ex = Framework.Assert.Throws<AssertionException>(() => Assert.Greater(_e1, _e2));
            Framework.Assert.That(ex?.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void FailureMessage()
        {
            var expectedMessage =
                "  Expected: greater than 99" + Environment.NewLine +
                "  But was:  7" + Environment.NewLine;
            var ex = Framework.Assert.Throws<AssertionException>(() => Assert.Greater(7, 99));
            Framework.Assert.That(ex?.Message, Is.EqualTo(expectedMessage));
        }
    }
}
