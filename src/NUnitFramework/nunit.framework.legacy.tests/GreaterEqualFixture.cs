// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Legacy.Tests
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
        private readonly Enum _e1 = RunState.Ignored;
        private readonly Enum _e2 = RunState.Explicit;

        [Test]
        public void GreaterOrEqual_Int32()
        {
            ClassicAssert.GreaterOrEqual(_i1, _i1);
            ClassicAssert.GreaterOrEqual(_i1, _i2, "Type: {0}", "int");
            Assert.That(() => ClassicAssert.GreaterOrEqual(_i2, _i1, "Type: {0}", "int"),
                        Throws.InstanceOf<AssertionException>().With.Message.Contains("int"));
        }

        [Test]
        public void GreaterOrEqual_UInt32()
        {
            ClassicAssert.GreaterOrEqual(_u1, _u1);
            ClassicAssert.GreaterOrEqual(_u1, _u2, "Type: {0}", "uint");
            Assert.That(() => ClassicAssert.GreaterOrEqual(_u2, _u1, "Type: {0}", "uint"),
                        Throws.InstanceOf<AssertionException>().With.Message.Contains("uint"));
        }

        [Test]
        public void GreaterOrEqual_Long()
        {
            ClassicAssert.GreaterOrEqual(_l1, _l1);
            ClassicAssert.GreaterOrEqual(_l1, _l2, "Type: {0}", "long");
            Assert.That(() => ClassicAssert.GreaterOrEqual(_l2, _l1, "Type: {0}", "long"),
                        Throws.InstanceOf<AssertionException>().With.Message.Contains("long"));
        }

        [Test]
        public void GreaterOrEqual_ULong()
        {
            ClassicAssert.GreaterOrEqual(_ul1, _ul1);
            ClassicAssert.GreaterOrEqual(_ul1, _ul2, "Type: {0}", "ulong");
            Assert.That(() => ClassicAssert.GreaterOrEqual(_ul2, _ul1, "Type: {0}", "ulong"),
                        Throws.InstanceOf<AssertionException>().With.Message.Contains("ulong"));
        }

        [Test]
        public void GreaterOrEqual_Double()
        {
            ClassicAssert.GreaterOrEqual(_d1, _d1);
            ClassicAssert.GreaterOrEqual(_d1, _d2, "Type: {0}", "double");
            Assert.That(() => ClassicAssert.GreaterOrEqual(_d2, _d1, "Type: {0}", "double"),
                        Throws.InstanceOf<AssertionException>().With.Message.Contains("double"));
        }

        [Test]
        public void GreaterOrEqual_Decimal()
        {
            ClassicAssert.GreaterOrEqual(_de1, _de1);
            ClassicAssert.GreaterOrEqual(_de1, _de2, "{0}", "decimal");
            Assert.That(() => ClassicAssert.GreaterOrEqual(_de2, _de1, "{0}", "decimal"),
                        Throws.InstanceOf<AssertionException>().With.Message.Contains("decimal"));
        }

        [Test]
        public void GreaterOrEqual_Float()
        {
            ClassicAssert.GreaterOrEqual(_f1, _f1);
            ClassicAssert.GreaterOrEqual(_f1, _f2, "Type: {0}", "float");
            Assert.That(() => ClassicAssert.GreaterOrEqual(_f2, _f1, "Type: {0}", "float"),
                        Throws.InstanceOf<AssertionException>().With.Message.Contains("float"));
        }

        [Test]
        public void GreaterOrEqual_IComparable()
        {
            ClassicAssert.GreaterOrEqual(_e1, _e1);
            ClassicAssert.GreaterOrEqual(_e1, _e2, "Type: {0}", "enum");
            Assert.That(() => ClassicAssert.GreaterOrEqual(_e2, _e1, "Type: {0}", "enum"),
                        Throws.InstanceOf<AssertionException>().With.Message.Contains("enum"));
        }

        [Test]
        public void MixedTypes()
        {
            ClassicAssert.GreaterOrEqual(5, 3L, "int to long");
            ClassicAssert.GreaterOrEqual(5, 3.5f, "int to float");
            ClassicAssert.GreaterOrEqual(5, 3.5d, "int to double");
            ClassicAssert.GreaterOrEqual(5, 3U, "int to uint");
            ClassicAssert.GreaterOrEqual(5, 3UL, "int to ulong");
            ClassicAssert.GreaterOrEqual(5, 3M, "int to decimal");

            ClassicAssert.GreaterOrEqual(5L, 3, "long to int");
            ClassicAssert.GreaterOrEqual(5L, 3.5f, "long to float");
            ClassicAssert.GreaterOrEqual(5L, 3.5d, "long to double");
            ClassicAssert.GreaterOrEqual(5L, 3U, "long to uint");
            ClassicAssert.GreaterOrEqual(5L, 3UL, "long to ulong");
            ClassicAssert.GreaterOrEqual(5L, 3M, "long to decimal");

            ClassicAssert.GreaterOrEqual(8.2f, 5, "float to int");
            ClassicAssert.GreaterOrEqual(8.2f, 8L, "float to long");
            ClassicAssert.GreaterOrEqual(8.2f, 3.5d, "float to double");
            ClassicAssert.GreaterOrEqual(8.2f, 8U, "float to uint");
            ClassicAssert.GreaterOrEqual(8.2f, 8UL, "float to ulong");
            ClassicAssert.GreaterOrEqual(8.2f, 3.5M, "float to decimal");

            ClassicAssert.GreaterOrEqual(8.2d, 5, "double to int");
            ClassicAssert.GreaterOrEqual(8.2d, 5L, "double to long");
            ClassicAssert.GreaterOrEqual(8.2d, 3.5f, "double to float");
            ClassicAssert.GreaterOrEqual(8.2d, 8U, "double to uint");
            ClassicAssert.GreaterOrEqual(8.2d, 8UL, "double to ulong");
            ClassicAssert.GreaterOrEqual(8.2d, 3.5M, "double to decimal");

            ClassicAssert.GreaterOrEqual(5U, 3, "uint to int");
            ClassicAssert.GreaterOrEqual(5U, 3L, "uint to long");
            ClassicAssert.GreaterOrEqual(5U, 3.5f, "uint to float");
            ClassicAssert.GreaterOrEqual(5U, 3.5d, "uint to double");
            ClassicAssert.GreaterOrEqual(5U, 3UL, "uint to ulong");
            ClassicAssert.GreaterOrEqual(5U, 3M, "uint to decimal");

            ClassicAssert.GreaterOrEqual(5ul, 3, "ulong to int");
            ClassicAssert.GreaterOrEqual(5UL, 3L, "ulong to long");
            ClassicAssert.GreaterOrEqual(5UL, 3.5f, "ulong to float");
            ClassicAssert.GreaterOrEqual(5UL, 3.5d, "ulong to double");
            ClassicAssert.GreaterOrEqual(5UL, 3U, "ulong to uint");
            ClassicAssert.GreaterOrEqual(5UL, 3M, "ulong to decimal");

            ClassicAssert.GreaterOrEqual(5M, 3, "decimal to int");
            ClassicAssert.GreaterOrEqual(5M, 3L, "decimal to long");
            ClassicAssert.GreaterOrEqual(5M, 3.5f, "decimal to float");
            ClassicAssert.GreaterOrEqual(5M, 3.5d, "decimal to double");
            ClassicAssert.GreaterOrEqual(5M, 3U, "decimal to uint");
            ClassicAssert.GreaterOrEqual(5M, 3UL, "decimal to ulong");
        }

        [Test]
        public void NotGreaterOrEqual()
        {
            var expectedMessage =
                "  Expected: greater than or equal to 5" + Environment.NewLine +
                "  But was:  4" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.GreaterOrEqual(_i2, _i1));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void NotGreaterEqualIComparable()
        {
            var expectedMessage =
                "  Expected: greater than or equal to Ignored" + Environment.NewLine +
                "  But was:  Explicit" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.GreaterOrEqual(_e2, _e1));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void FailureMessage()
        {
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.GreaterOrEqual(7, 99));

            var msg = ex?.Message;
            Assert.That(msg, Contains.Substring(TextMessageWriter.Pfx_Expected + "greater than or equal to 99"));
            Assert.That(msg, Contains.Substring(TextMessageWriter.Pfx_Actual + "7"));
        }
    }
}
