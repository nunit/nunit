// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Classic.Tests
{
    [TestFixture]
    public class LessEqualFixture
    {
        private readonly int _i1 = 5;
        private readonly int _i2 = 8;
        private readonly uint _u1 = 12345678;
        private readonly uint _u2 = 12345879;
        private readonly long _l1 = 12345678;
        private readonly long _l2 = 12345879;
        private readonly ulong _ul1 = 12345678;
        private readonly ulong _ul2 = 12345879;
        private readonly float _f1 = 3.543F;
        private readonly float _f2 = 8.543F;
        private readonly decimal _de1 = 53.4M;
        private readonly decimal _de2 = 83.4M;
        private readonly double _d1 = 4.85948654;
        private readonly double _d2 = 8.0;
        private readonly Enum _e1 = RunState.Explicit;
        private readonly Enum _e2 = RunState.Ignored;

        [Test]
        public void LessOrEqual()
        {
            // Test equality check for all forms
            Assert.LessOrEqual(_i1, _i1);
            Assert.LessOrEqual(_i1, _i1, "int");
            Assert.LessOrEqual(_i1, _i1, "{0}", "int");
            Assert.LessOrEqual(_u1, _u1);
            Assert.LessOrEqual(_u1, _u1, "uint");
            Assert.LessOrEqual(_u1, _u1, "{0}", "uint");
            Assert.LessOrEqual(_l1, _l1);
            Assert.LessOrEqual(_l1, _l1, "long");
            Assert.LessOrEqual(_l1, _l1, "{0}", "long");
            Assert.LessOrEqual(_ul1, _ul1);
            Assert.LessOrEqual(_ul1, _ul1, "ulong");
            Assert.LessOrEqual(_ul1, _ul1, "{0}", "ulong");
            Assert.LessOrEqual(_d1, _d1);
            Assert.LessOrEqual(_d1, _d1, "double");
            Assert.LessOrEqual(_d1, _d1, "{0}", "double");
            Assert.LessOrEqual(_de1, _de1);
            Assert.LessOrEqual(_de1, _de1, "decimal");
            Assert.LessOrEqual(_de1, _de1, "{0}", "decimal");
            Assert.LessOrEqual(_f1, _f1);
            Assert.LessOrEqual(_f1, _f1, "float");
            Assert.LessOrEqual(_f1, _f1, "{0}", "float");

            // Testing all forms after seeing some bugs. CFP
            Assert.LessOrEqual(_i1, _i2);
            Assert.LessOrEqual(_i1, _i2, "int");
            Assert.LessOrEqual(_i1, _i2, "{0}", "int");
            Assert.LessOrEqual(_u1, _u2);
            Assert.LessOrEqual(_u1, _u2, "uint");
            Assert.LessOrEqual(_u1, _u2, "{0}", "uint");
            Assert.LessOrEqual(_l1, _l2);
            Assert.LessOrEqual(_l1, _l2, "long");
            Assert.LessOrEqual(_l1, _l2, "{0}", "long");
            Assert.LessOrEqual(_ul1, _ul2);
            Assert.LessOrEqual(_ul1, _ul2, "ulong");
            Assert.LessOrEqual(_ul1, _ul2, "{0}", "ulong");
            Assert.LessOrEqual(_d1, _d2);
            Assert.LessOrEqual(_d1, _d2, "double");
            Assert.LessOrEqual(_d1, _d2, "{0}", "double");
            Assert.LessOrEqual(_de1, _de2);
            Assert.LessOrEqual(_de1, _de2, "decimal");
            Assert.LessOrEqual(_de1, _de2, "{0}", "decimal");
            Assert.LessOrEqual(_f1, _f2);
            Assert.LessOrEqual(_f1, _f2, "float");
            Assert.LessOrEqual(_f1, _f2, "{0}", "float");
        }

        [Test]
        public void MixedTypes()
        {
            Assert.LessOrEqual(5, 8L, "int to long");
            Assert.LessOrEqual(5, 8.2f, "int to float");
            Assert.LessOrEqual(5, 8.2d, "int to double");
            Assert.LessOrEqual(5, 8U, "int to uint");
            Assert.LessOrEqual(5, 8UL, "int to ulong");
            Assert.LessOrEqual(5, 8M, "int to decimal");

            Assert.LessOrEqual(5L, 8, "long to int");
            Assert.LessOrEqual(5L, 8.2f, "long to float");
            Assert.LessOrEqual(5L, 8.2d, "long to double");
            Assert.LessOrEqual(5L, 8U, "long to uint");
            Assert.LessOrEqual(5L, 8UL, "long to ulong");
            Assert.LessOrEqual(5L, 8M, "long to decimal");

            Assert.LessOrEqual(3.5f, 5, "float to int");
            Assert.LessOrEqual(3.5f, 8L, "float to long");
            Assert.LessOrEqual(3.5f, 8.2d, "float to double");
            Assert.LessOrEqual(3.5f, 8U, "float to uint");
            Assert.LessOrEqual(3.5f, 8UL, "float to ulong");
            Assert.LessOrEqual(3.5f, 8.2M, "float to decimal");

            Assert.LessOrEqual(3.5d, 5, "double to int");
            Assert.LessOrEqual(3.5d, 5L, "double to long");
            Assert.LessOrEqual(3.5d, 8.2f, "double to float");
            Assert.LessOrEqual(3.5d, 8U, "double to uint");
            Assert.LessOrEqual(3.5d, 8UL, "double to ulong");
            Assert.LessOrEqual(3.5d, 8.2M, "double to decimal");

            Assert.LessOrEqual(5U, 8, "uint to int");
            Assert.LessOrEqual(5U, 8L, "uint to long");
            Assert.LessOrEqual(5U, 8.2f, "uint to float");
            Assert.LessOrEqual(5U, 8.2d, "uint to double");
            Assert.LessOrEqual(5U, 8UL, "uint to ulong");
            Assert.LessOrEqual(5U, 8M, "uint to decimal");

            Assert.LessOrEqual(5ul, 8, "ulong to int");
            Assert.LessOrEqual(5UL, 8L, "ulong to long");
            Assert.LessOrEqual(5UL, 8.2f, "ulong to float");
            Assert.LessOrEqual(5UL, 8.2d, "ulong to double");
            Assert.LessOrEqual(5UL, 8U, "ulong to uint");
            Assert.LessOrEqual(5UL, 8M, "ulong to decimal");

            Assert.LessOrEqual(5M, 8, "decimal to int");
            Assert.LessOrEqual(5M, 8L, "decimal to long");
            Assert.LessOrEqual(5M, 8.2f, "decimal to float");
            Assert.LessOrEqual(5M, 8.2d, "decimal to double");
            Assert.LessOrEqual(5M, 8U, "decimal to uint");
            Assert.LessOrEqual(5M, 8UL, "decimal to ulong");
        }

        [Test]
        public void NotLessOrEqual()
        {
            var expectedMessage =
                "  Expected: less than or equal to 5" + Environment.NewLine +
                "  But was:  8" + Environment.NewLine;
            var ex = Framework.Assert.Throws<AssertionException>(() => Assert.LessOrEqual(_i2, _i1));
            Framework.Assert.That(ex?.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void NotLessEqualIComparable()
        {
            var expectedMessage =
                "  Expected: less than or equal to Explicit" + Environment.NewLine +
                "  But was:  Ignored" + Environment.NewLine;
            var ex = Framework.Assert.Throws<AssertionException>(() => Assert.LessOrEqual(_e2, _e1));
            Framework.Assert.That(ex?.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void FailureMessage()
        {
            var expectedMessage =
                "  Expected: less than or equal to 4" + Environment.NewLine +
                "  But was:  9" + Environment.NewLine;
            var ex = Framework.Assert.Throws<AssertionException>(() => Assert.LessOrEqual(9, 4));
            Framework.Assert.That(ex?.Message, Is.EqualTo(expectedMessage));
        }
    }
}
