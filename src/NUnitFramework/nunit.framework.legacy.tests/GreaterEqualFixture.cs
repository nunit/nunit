// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Classic.Tests
{
    [TestFixture]
    public class GreaterEqualFixture
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
        public void GreaterOrEqual_Int32()
        {
            Legacy.ClassicAssert.GreaterOrEqual(_i1, _i1);
            Legacy.ClassicAssert.GreaterOrEqual(_i1, _i2);
        }

        [Test]
        public void GreaterOrEqual_UInt32()
        {
            Legacy.ClassicAssert.GreaterOrEqual(_u1, _u1);
            Legacy.ClassicAssert.GreaterOrEqual(_u1, _u2);
        }

        [Test]
        public void GreaterOrEqual_Long()
        {
            Legacy.ClassicAssert.GreaterOrEqual(_l1, _l1);
            Legacy.ClassicAssert.GreaterOrEqual(_l1, _l2);
        }

        [Test]
        public void GreaterOrEqual_ULong()
        {
            Legacy.ClassicAssert.GreaterOrEqual(_ul1, _ul1);
            Legacy.ClassicAssert.GreaterOrEqual(_ul1, _ul2);
        }

        [Test]
        public void GreaterOrEqual_Double()
        {
            Legacy.ClassicAssert.GreaterOrEqual(_d1, _d1, "double");
            Legacy.ClassicAssert.GreaterOrEqual(_d1, _d2, "double");
        }

        [Test]
        public void GreaterOrEqual_Decimal()
        {
            Legacy.ClassicAssert.GreaterOrEqual(_de1, _de1, "{0}", "decimal");
            Legacy.ClassicAssert.GreaterOrEqual(_de1, _de2, "{0}", "decimal");
        }

        [Test]
        public void GreaterOrEqual_Float()
        {
            Legacy.ClassicAssert.GreaterOrEqual(_f1, _f1, "float");
            Legacy.ClassicAssert.GreaterOrEqual(_f1, _f2, "float");
        }

        [Test]
        public void MixedTypes()
        {
            Legacy.ClassicAssert.GreaterOrEqual(5, 3L, "int to long");
            Legacy.ClassicAssert.GreaterOrEqual(5, 3.5f, "int to float");
            Legacy.ClassicAssert.GreaterOrEqual(5, 3.5d, "int to double");
            Legacy.ClassicAssert.GreaterOrEqual(5, 3U, "int to uint");
            Legacy.ClassicAssert.GreaterOrEqual(5, 3UL, "int to ulong");
            Legacy.ClassicAssert.GreaterOrEqual(5, 3M, "int to decimal");

            Legacy.ClassicAssert.GreaterOrEqual(5L, 3, "long to int");
            Legacy.ClassicAssert.GreaterOrEqual(5L, 3.5f, "long to float");
            Legacy.ClassicAssert.GreaterOrEqual(5L, 3.5d, "long to double");
            Legacy.ClassicAssert.GreaterOrEqual(5L, 3U, "long to uint");
            Legacy.ClassicAssert.GreaterOrEqual(5L, 3UL, "long to ulong");
            Legacy.ClassicAssert.GreaterOrEqual(5L, 3M, "long to decimal");

            Legacy.ClassicAssert.GreaterOrEqual(8.2f, 5, "float to int");
            Legacy.ClassicAssert.GreaterOrEqual(8.2f, 8L, "float to long");
            Legacy.ClassicAssert.GreaterOrEqual(8.2f, 3.5d, "float to double");
            Legacy.ClassicAssert.GreaterOrEqual(8.2f, 8U, "float to uint");
            Legacy.ClassicAssert.GreaterOrEqual(8.2f, 8UL, "float to ulong");
            Legacy.ClassicAssert.GreaterOrEqual(8.2f, 3.5M, "float to decimal");

            Legacy.ClassicAssert.GreaterOrEqual(8.2d, 5, "double to int");
            Legacy.ClassicAssert.GreaterOrEqual(8.2d, 5L, "double to long");
            Legacy.ClassicAssert.GreaterOrEqual(8.2d, 3.5f, "double to float");
            Legacy.ClassicAssert.GreaterOrEqual(8.2d, 8U, "double to uint");
            Legacy.ClassicAssert.GreaterOrEqual(8.2d, 8UL, "double to ulong");
            Legacy.ClassicAssert.GreaterOrEqual(8.2d, 3.5M, "double to decimal");

            Legacy.ClassicAssert.GreaterOrEqual(5U, 3, "uint to int");
            Legacy.ClassicAssert.GreaterOrEqual(5U, 3L, "uint to long");
            Legacy.ClassicAssert.GreaterOrEqual(5U, 3.5f, "uint to float");
            Legacy.ClassicAssert.GreaterOrEqual(5U, 3.5d, "uint to double");
            Legacy.ClassicAssert.GreaterOrEqual(5U, 3UL, "uint to ulong");
            Legacy.ClassicAssert.GreaterOrEqual(5U, 3M, "uint to decimal");

            Legacy.ClassicAssert.GreaterOrEqual(5ul, 3, "ulong to int");
            Legacy.ClassicAssert.GreaterOrEqual(5UL, 3L, "ulong to long");
            Legacy.ClassicAssert.GreaterOrEqual(5UL, 3.5f, "ulong to float");
            Legacy.ClassicAssert.GreaterOrEqual(5UL, 3.5d, "ulong to double");
            Legacy.ClassicAssert.GreaterOrEqual(5UL, 3U, "ulong to uint");
            Legacy.ClassicAssert.GreaterOrEqual(5UL, 3M, "ulong to decimal");

            Legacy.ClassicAssert.GreaterOrEqual(5M, 3, "decimal to int");
            Legacy.ClassicAssert.GreaterOrEqual(5M, 3L, "decimal to long");
            Legacy.ClassicAssert.GreaterOrEqual(5M, 3.5f, "decimal to float");
            Legacy.ClassicAssert.GreaterOrEqual(5M, 3.5d, "decimal to double");
            Legacy.ClassicAssert.GreaterOrEqual(5M, 3U, "decimal to uint");
            Legacy.ClassicAssert.GreaterOrEqual(5M, 3UL, "decimal to ulong");
        }

        [Test]
        public void NotGreaterOrEqual()
        {
            var expectedMessage =
                "  Expected: greater than or equal to 5" + Environment.NewLine +
                "  But was:  4" + Environment.NewLine;
            var ex = Framework.Assert.Throws<AssertionException>(() => Legacy.ClassicAssert.GreaterOrEqual(_i2, _i1));
            Framework.Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void NotGreaterEqualIComparable()
        {
            var expectedMessage =
                "  Expected: greater than or equal to Ignored" + Environment.NewLine +
                "  But was:  Explicit" + Environment.NewLine;
            var ex = Framework.Assert.Throws<AssertionException>(() => Legacy.ClassicAssert.GreaterOrEqual(_e1, _e2));
            Framework.Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void FailureMessage()
        {
            var ex = Framework.Assert.Throws<AssertionException>(() => Legacy.ClassicAssert.GreaterOrEqual(7, 99));

            var msg = ex?.Message;
            Framework.Assert.That(msg, Contains.Substring(TextMessageWriter.Pfx_Expected + "greater than or equal to 99"));
            Framework.Assert.That(msg, Contains.Substring(TextMessageWriter.Pfx_Actual + "7"));
        }
    }
}
