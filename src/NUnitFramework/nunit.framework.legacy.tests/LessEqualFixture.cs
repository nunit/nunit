// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Legacy.Tests
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
            ClassicAssert.LessOrEqual(_i1, _i1);
            ClassicAssert.LessOrEqual(_i1, _i1, "int");
            ClassicAssert.LessOrEqual(_i1, _i1, "{0}", "int");
            ClassicAssert.LessOrEqual(_u1, _u1);
            ClassicAssert.LessOrEqual(_u1, _u1, "uint");
            ClassicAssert.LessOrEqual(_u1, _u1, "{0}", "uint");
            ClassicAssert.LessOrEqual(_l1, _l1);
            ClassicAssert.LessOrEqual(_l1, _l1, "long");
            ClassicAssert.LessOrEqual(_l1, _l1, "{0}", "long");
            ClassicAssert.LessOrEqual(_ul1, _ul1);
            ClassicAssert.LessOrEqual(_ul1, _ul1, "ulong");
            ClassicAssert.LessOrEqual(_ul1, _ul1, "{0}", "ulong");
            ClassicAssert.LessOrEqual(_d1, _d1);
            ClassicAssert.LessOrEqual(_d1, _d1, "double");
            ClassicAssert.LessOrEqual(_d1, _d1, "{0}", "double");
            ClassicAssert.LessOrEqual(_de1, _de1);
            ClassicAssert.LessOrEqual(_de1, _de1, "decimal");
            ClassicAssert.LessOrEqual(_de1, _de1, "{0}", "decimal");
            ClassicAssert.LessOrEqual(_f1, _f1);
            ClassicAssert.LessOrEqual(_f1, _f1, "float");
            ClassicAssert.LessOrEqual(_f1, _f1, "{0}", "float");

            // Testing all forms after seeing some bugs. CFP
            ClassicAssert.LessOrEqual(_i1, _i2);
            ClassicAssert.LessOrEqual(_i1, _i2, "int");
            ClassicAssert.LessOrEqual(_i1, _i2, "{0}", "int");
            ClassicAssert.LessOrEqual(_u1, _u2);
            ClassicAssert.LessOrEqual(_u1, _u2, "uint");
            ClassicAssert.LessOrEqual(_u1, _u2, "{0}", "uint");
            ClassicAssert.LessOrEqual(_l1, _l2);
            ClassicAssert.LessOrEqual(_l1, _l2, "long");
            ClassicAssert.LessOrEqual(_l1, _l2, "{0}", "long");
            ClassicAssert.LessOrEqual(_ul1, _ul2);
            ClassicAssert.LessOrEqual(_ul1, _ul2, "ulong");
            ClassicAssert.LessOrEqual(_ul1, _ul2, "{0}", "ulong");
            ClassicAssert.LessOrEqual(_d1, _d2);
            ClassicAssert.LessOrEqual(_d1, _d2, "double");
            ClassicAssert.LessOrEqual(_d1, _d2, "{0}", "double");
            ClassicAssert.LessOrEqual(_de1, _de2);
            ClassicAssert.LessOrEqual(_de1, _de2, "decimal");
            ClassicAssert.LessOrEqual(_de1, _de2, "{0}", "decimal");
            ClassicAssert.LessOrEqual(_f1, _f2);
            ClassicAssert.LessOrEqual(_f1, _f2, "float");
            ClassicAssert.LessOrEqual(_f1, _f2, "{0}", "float");
        }

        [Test]
        public void MixedTypes()
        {
            ClassicAssert.LessOrEqual(5, 8L, "int to long");
            ClassicAssert.LessOrEqual(5, 8.2f, "int to float");
            ClassicAssert.LessOrEqual(5, 8.2d, "int to double");
            ClassicAssert.LessOrEqual(5, 8U, "int to uint");
            ClassicAssert.LessOrEqual(5, 8UL, "int to ulong");
            ClassicAssert.LessOrEqual(5, 8M, "int to decimal");

            ClassicAssert.LessOrEqual(5L, 8, "long to int");
            ClassicAssert.LessOrEqual(5L, 8.2f, "long to float");
            ClassicAssert.LessOrEqual(5L, 8.2d, "long to double");
            ClassicAssert.LessOrEqual(5L, 8U, "long to uint");
            ClassicAssert.LessOrEqual(5L, 8UL, "long to ulong");
            ClassicAssert.LessOrEqual(5L, 8M, "long to decimal");

            ClassicAssert.LessOrEqual(3.5f, 5, "float to int");
            ClassicAssert.LessOrEqual(3.5f, 8L, "float to long");
            ClassicAssert.LessOrEqual(3.5f, 8.2d, "float to double");
            ClassicAssert.LessOrEqual(3.5f, 8U, "float to uint");
            ClassicAssert.LessOrEqual(3.5f, 8UL, "float to ulong");
            ClassicAssert.LessOrEqual(3.5f, 8.2M, "float to decimal");

            ClassicAssert.LessOrEqual(3.5d, 5, "double to int");
            ClassicAssert.LessOrEqual(3.5d, 5L, "double to long");
            ClassicAssert.LessOrEqual(3.5d, 8.2f, "double to float");
            ClassicAssert.LessOrEqual(3.5d, 8U, "double to uint");
            ClassicAssert.LessOrEqual(3.5d, 8UL, "double to ulong");
            ClassicAssert.LessOrEqual(3.5d, 8.2M, "double to decimal");

            ClassicAssert.LessOrEqual(5U, 8, "uint to int");
            ClassicAssert.LessOrEqual(5U, 8L, "uint to long");
            ClassicAssert.LessOrEqual(5U, 8.2f, "uint to float");
            ClassicAssert.LessOrEqual(5U, 8.2d, "uint to double");
            ClassicAssert.LessOrEqual(5U, 8UL, "uint to ulong");
            ClassicAssert.LessOrEqual(5U, 8M, "uint to decimal");

            ClassicAssert.LessOrEqual(5ul, 8, "ulong to int");
            ClassicAssert.LessOrEqual(5UL, 8L, "ulong to long");
            ClassicAssert.LessOrEqual(5UL, 8.2f, "ulong to float");
            ClassicAssert.LessOrEqual(5UL, 8.2d, "ulong to double");
            ClassicAssert.LessOrEqual(5UL, 8U, "ulong to uint");
            ClassicAssert.LessOrEqual(5UL, 8M, "ulong to decimal");

            ClassicAssert.LessOrEqual(5M, 8, "decimal to int");
            ClassicAssert.LessOrEqual(5M, 8L, "decimal to long");
            ClassicAssert.LessOrEqual(5M, 8.2f, "decimal to float");
            ClassicAssert.LessOrEqual(5M, 8.2d, "decimal to double");
            ClassicAssert.LessOrEqual(5M, 8U, "decimal to uint");
            ClassicAssert.LessOrEqual(5M, 8UL, "decimal to ulong");
        }

        [Test]
        public void NotLessOrEqual()
        {
            var expectedMessage =
                "  Expected: less than or equal to 5" + Environment.NewLine +
                "  But was:  8" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.LessOrEqual(_i2, _i1));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void NotLessEqualIComparable()
        {
            var expectedMessage =
                "  Expected: less than or equal to Explicit" + Environment.NewLine +
                "  But was:  Ignored" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.LessOrEqual(_e2, _e1));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void FailureMessage()
        {
            var expectedMessage =
                "  Expected: less than or equal to 4" + Environment.NewLine +
                "  But was:  9" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.LessOrEqual(9, 4));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }
    }
}
