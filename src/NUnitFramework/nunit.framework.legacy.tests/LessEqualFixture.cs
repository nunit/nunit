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
            Legacy.ClassicAssert.LessOrEqual(_i1, _i1);
            Legacy.ClassicAssert.LessOrEqual(_i1, _i1, "int");
            Legacy.ClassicAssert.LessOrEqual(_i1, _i1, "{0}", "int");
            Legacy.ClassicAssert.LessOrEqual(_u1, _u1);
            Legacy.ClassicAssert.LessOrEqual(_u1, _u1, "uint");
            Legacy.ClassicAssert.LessOrEqual(_u1, _u1, "{0}", "uint");
            Legacy.ClassicAssert.LessOrEqual(_l1, _l1);
            Legacy.ClassicAssert.LessOrEqual(_l1, _l1, "long");
            Legacy.ClassicAssert.LessOrEqual(_l1, _l1, "{0}", "long");
            Legacy.ClassicAssert.LessOrEqual(_ul1, _ul1);
            Legacy.ClassicAssert.LessOrEqual(_ul1, _ul1, "ulong");
            Legacy.ClassicAssert.LessOrEqual(_ul1, _ul1, "{0}", "ulong");
            Legacy.ClassicAssert.LessOrEqual(_d1, _d1);
            Legacy.ClassicAssert.LessOrEqual(_d1, _d1, "double");
            Legacy.ClassicAssert.LessOrEqual(_d1, _d1, "{0}", "double");
            Legacy.ClassicAssert.LessOrEqual(_de1, _de1);
            Legacy.ClassicAssert.LessOrEqual(_de1, _de1, "decimal");
            Legacy.ClassicAssert.LessOrEqual(_de1, _de1, "{0}", "decimal");
            Legacy.ClassicAssert.LessOrEqual(_f1, _f1);
            Legacy.ClassicAssert.LessOrEqual(_f1, _f1, "float");
            Legacy.ClassicAssert.LessOrEqual(_f1, _f1, "{0}", "float");

            // Testing all forms after seeing some bugs. CFP
            Legacy.ClassicAssert.LessOrEqual(_i1, _i2);
            Legacy.ClassicAssert.LessOrEqual(_i1, _i2, "int");
            Legacy.ClassicAssert.LessOrEqual(_i1, _i2, "{0}", "int");
            Legacy.ClassicAssert.LessOrEqual(_u1, _u2);
            Legacy.ClassicAssert.LessOrEqual(_u1, _u2, "uint");
            Legacy.ClassicAssert.LessOrEqual(_u1, _u2, "{0}", "uint");
            Legacy.ClassicAssert.LessOrEqual(_l1, _l2);
            Legacy.ClassicAssert.LessOrEqual(_l1, _l2, "long");
            Legacy.ClassicAssert.LessOrEqual(_l1, _l2, "{0}", "long");
            Legacy.ClassicAssert.LessOrEqual(_ul1, _ul2);
            Legacy.ClassicAssert.LessOrEqual(_ul1, _ul2, "ulong");
            Legacy.ClassicAssert.LessOrEqual(_ul1, _ul2, "{0}", "ulong");
            Legacy.ClassicAssert.LessOrEqual(_d1, _d2);
            Legacy.ClassicAssert.LessOrEqual(_d1, _d2, "double");
            Legacy.ClassicAssert.LessOrEqual(_d1, _d2, "{0}", "double");
            Legacy.ClassicAssert.LessOrEqual(_de1, _de2);
            Legacy.ClassicAssert.LessOrEqual(_de1, _de2, "decimal");
            Legacy.ClassicAssert.LessOrEqual(_de1, _de2, "{0}", "decimal");
            Legacy.ClassicAssert.LessOrEqual(_f1, _f2);
            Legacy.ClassicAssert.LessOrEqual(_f1, _f2, "float");
            Legacy.ClassicAssert.LessOrEqual(_f1, _f2, "{0}", "float");
        }

        [Test]
        public void MixedTypes()
        {
            Legacy.ClassicAssert.LessOrEqual(5, 8L, "int to long");
            Legacy.ClassicAssert.LessOrEqual(5, 8.2f, "int to float");
            Legacy.ClassicAssert.LessOrEqual(5, 8.2d, "int to double");
            Legacy.ClassicAssert.LessOrEqual(5, 8U, "int to uint");
            Legacy.ClassicAssert.LessOrEqual(5, 8UL, "int to ulong");
            Legacy.ClassicAssert.LessOrEqual(5, 8M, "int to decimal");

            Legacy.ClassicAssert.LessOrEqual(5L, 8, "long to int");
            Legacy.ClassicAssert.LessOrEqual(5L, 8.2f, "long to float");
            Legacy.ClassicAssert.LessOrEqual(5L, 8.2d, "long to double");
            Legacy.ClassicAssert.LessOrEqual(5L, 8U, "long to uint");
            Legacy.ClassicAssert.LessOrEqual(5L, 8UL, "long to ulong");
            Legacy.ClassicAssert.LessOrEqual(5L, 8M, "long to decimal");

            Legacy.ClassicAssert.LessOrEqual(3.5f, 5, "float to int");
            Legacy.ClassicAssert.LessOrEqual(3.5f, 8L, "float to long");
            Legacy.ClassicAssert.LessOrEqual(3.5f, 8.2d, "float to double");
            Legacy.ClassicAssert.LessOrEqual(3.5f, 8U, "float to uint");
            Legacy.ClassicAssert.LessOrEqual(3.5f, 8UL, "float to ulong");
            Legacy.ClassicAssert.LessOrEqual(3.5f, 8.2M, "float to decimal");

            Legacy.ClassicAssert.LessOrEqual(3.5d, 5, "double to int");
            Legacy.ClassicAssert.LessOrEqual(3.5d, 5L, "double to long");
            Legacy.ClassicAssert.LessOrEqual(3.5d, 8.2f, "double to float");
            Legacy.ClassicAssert.LessOrEqual(3.5d, 8U, "double to uint");
            Legacy.ClassicAssert.LessOrEqual(3.5d, 8UL, "double to ulong");
            Legacy.ClassicAssert.LessOrEqual(3.5d, 8.2M, "double to decimal");

            Legacy.ClassicAssert.LessOrEqual(5U, 8, "uint to int");
            Legacy.ClassicAssert.LessOrEqual(5U, 8L, "uint to long");
            Legacy.ClassicAssert.LessOrEqual(5U, 8.2f, "uint to float");
            Legacy.ClassicAssert.LessOrEqual(5U, 8.2d, "uint to double");
            Legacy.ClassicAssert.LessOrEqual(5U, 8UL, "uint to ulong");
            Legacy.ClassicAssert.LessOrEqual(5U, 8M, "uint to decimal");

            Legacy.ClassicAssert.LessOrEqual(5ul, 8, "ulong to int");
            Legacy.ClassicAssert.LessOrEqual(5UL, 8L, "ulong to long");
            Legacy.ClassicAssert.LessOrEqual(5UL, 8.2f, "ulong to float");
            Legacy.ClassicAssert.LessOrEqual(5UL, 8.2d, "ulong to double");
            Legacy.ClassicAssert.LessOrEqual(5UL, 8U, "ulong to uint");
            Legacy.ClassicAssert.LessOrEqual(5UL, 8M, "ulong to decimal");

            Legacy.ClassicAssert.LessOrEqual(5M, 8, "decimal to int");
            Legacy.ClassicAssert.LessOrEqual(5M, 8L, "decimal to long");
            Legacy.ClassicAssert.LessOrEqual(5M, 8.2f, "decimal to float");
            Legacy.ClassicAssert.LessOrEqual(5M, 8.2d, "decimal to double");
            Legacy.ClassicAssert.LessOrEqual(5M, 8U, "decimal to uint");
            Legacy.ClassicAssert.LessOrEqual(5M, 8UL, "decimal to ulong");
        }

        [Test]
        public void NotLessOrEqual()
        {
            var expectedMessage =
                "  Expected: less than or equal to 5" + Environment.NewLine +
                "  But was:  8" + Environment.NewLine;
            var ex = Framework.Assert.Throws<AssertionException>(() => Legacy.ClassicAssert.LessOrEqual(_i2, _i1));
            Framework.Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void NotLessEqualIComparable()
        {
            var expectedMessage =
                "  Expected: less than or equal to Explicit" + Environment.NewLine +
                "  But was:  Ignored" + Environment.NewLine;
            var ex = Framework.Assert.Throws<AssertionException>(() => Legacy.ClassicAssert.LessOrEqual(_e2, _e1));
            Framework.Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void FailureMessage()
        {
            var expectedMessage =
                "  Expected: less than or equal to 4" + Environment.NewLine +
                "  But was:  9" + Environment.NewLine;
            var ex = Framework.Assert.Throws<AssertionException>(() => Legacy.ClassicAssert.LessOrEqual(9, 4));
            Framework.Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }
    }
}
