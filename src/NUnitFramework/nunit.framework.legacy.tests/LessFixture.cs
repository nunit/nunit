// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Legacy.Tests
{
    [TestFixture]
    public class LessFixture
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
        public void Less()
        {
            // Testing all forms after seeing some bugs. CFP
            ClassicAssert.Less(_i1, _i2);
            ClassicAssert.Less(_i1, _i2, "int");
            ClassicAssert.Less(_i1, _i2, "{0}", "int");
            ClassicAssert.Less(_u1, _u2);
            ClassicAssert.Less(_u1, _u2, "uint");
            ClassicAssert.Less(_u1, _u2, "{0}", "uint");
            ClassicAssert.Less(_l1, _l2);
            ClassicAssert.Less(_l1, _l2, "long");
            ClassicAssert.Less(_l1, _l2, "{0}", "long");
            ClassicAssert.Less(_ul1, _ul2);
            ClassicAssert.Less(_ul1, _ul2, "ulong");
            ClassicAssert.Less(_ul1, _ul2, "{0}", "ulong");
            ClassicAssert.Less(_d1, _d2);
            ClassicAssert.Less(_d1, _d2, "double");
            ClassicAssert.Less(_d1, _d2, "{0}", "double");
            ClassicAssert.Less(_de1, _de2);
            ClassicAssert.Less(_de1, _de2, "decimal");
            ClassicAssert.Less(_de1, _de2, "{0}", "decimal");
            ClassicAssert.Less(_f1, _f2);
            ClassicAssert.Less(_f1, _f2, "float");
            ClassicAssert.Less(_f1, _f2, "{0}", "float");
            ClassicAssert.Less(_e1, _e2);
            ClassicAssert.Less(_e1, _e2, "enum");
            ClassicAssert.Less(_e1, _e2, "{0}", "enum");
        }

        [Test]
        public void LessFails()
        {
            Assert.That(() => ClassicAssert.Less(_i2, _i1, "{0}", "int"),
                        Throws.InstanceOf<AssertionException>().With.Message.Contains("int"));
            Assert.That(() => ClassicAssert.Less(_u2, _u1, "{0}", "uint"),
                        Throws.InstanceOf<AssertionException>().With.Message.Contains("uint"));
            Assert.That(() => ClassicAssert.Less(_l2, _l1, "{0}", "long"),
                Throws.InstanceOf<AssertionException>().With.Message.Contains("long"));
            Assert.That(() => ClassicAssert.Less(_ul2, _ul1, "{0}", "ulong"),
                Throws.InstanceOf<AssertionException>().With.Message.Contains("ulong"));
            Assert.That(() => ClassicAssert.Less(_d2, _d1, "{0}", "double"),
                Throws.InstanceOf<AssertionException>().With.Message.Contains("double"));
            Assert.That(() => ClassicAssert.Less(_de2, _de1, "{0}", "decimal"),
                Throws.InstanceOf<AssertionException>().With.Message.Contains("decimal"));
            Assert.That(() => ClassicAssert.Less(_f2, _f1, "{0}", "float"),
                Throws.InstanceOf<AssertionException>().With.Message.Contains("float"));
            Assert.That(() => ClassicAssert.Less(_e2, _e1, "{0}", "enum"),
                Throws.InstanceOf<AssertionException>().With.Message.Contains("enum"));
        }

        [Test]
        public void MixedTypes()
        {
            ClassicAssert.Less(5, 8L, "int to long");
            ClassicAssert.Less(5, 8.2f, "int to float");
            ClassicAssert.Less(5, 8.2d, "int to double");
            ClassicAssert.Less(5, 8U, "int to uint");
            ClassicAssert.Less(5, 8UL, "int to ulong");
            ClassicAssert.Less(5, 8M, "int to decimal");

            ClassicAssert.Less(5L, 8, "long to int");
            ClassicAssert.Less(5L, 8.2f, "long to float");
            ClassicAssert.Less(5L, 8.2d, "long to double");
            ClassicAssert.Less(5L, 8U, "long to uint");
            ClassicAssert.Less(5L, 8UL, "long to ulong");
            ClassicAssert.Less(5L, 8M, "long to decimal");

            ClassicAssert.Less(3.5f, 5, "float to int");
            ClassicAssert.Less(3.5f, 8L, "float to long");
            ClassicAssert.Less(3.5f, 8.2d, "float to double");
            ClassicAssert.Less(3.5f, 8U, "float to uint");
            ClassicAssert.Less(3.5f, 8UL, "float to ulong");
            ClassicAssert.Less(3.5f, 8.2M, "float to decimal");

            ClassicAssert.Less(3.5d, 5, "double to int");
            ClassicAssert.Less(3.5d, 5L, "double to long");
            ClassicAssert.Less(3.5d, 8.2f, "double to float");
            ClassicAssert.Less(3.5d, 8U, "double to uint");
            ClassicAssert.Less(3.5d, 8UL, "double to ulong");
            ClassicAssert.Less(3.5d, 8.2M, "double to decimal");

            ClassicAssert.Less(5U, 8, "uint to int");
            ClassicAssert.Less(5U, 8L, "uint to long");
            ClassicAssert.Less(5U, 8.2f, "uint to float");
            ClassicAssert.Less(5U, 8.2d, "uint to double");
            ClassicAssert.Less(5U, 8UL, "uint to ulong");
            ClassicAssert.Less(5U, 8M, "uint to decimal");

            ClassicAssert.Less(5ul, 8, "ulong to int");
            ClassicAssert.Less(5UL, 8L, "ulong to long");
            ClassicAssert.Less(5UL, 8.2f, "ulong to float");
            ClassicAssert.Less(5UL, 8.2d, "ulong to double");
            ClassicAssert.Less(5UL, 8U, "ulong to uint");
            ClassicAssert.Less(5UL, 8M, "ulong to decimal");

            ClassicAssert.Less(5M, 8, "decimal to int");
            ClassicAssert.Less(5M, 8L, "decimal to long");
            ClassicAssert.Less(5M, 8.2f, "decimal to float");
            ClassicAssert.Less(5M, 8.2d, "decimal to double");
            ClassicAssert.Less(5M, 8U, "decimal to uint");
            ClassicAssert.Less(5M, 8UL, "decimal to ulong");
        }

        [Test]
        public void NotLessWhenEqual()
        {
            var expectedMessage =
                "  Expected: less than 5" + Environment.NewLine +
                "  But was:  5" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.Less(_i1, _i1));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void NotLess()
        {
            var expectedMessage =
                "  Expected: less than 5" + Environment.NewLine +
                "  But was:  8" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.Less(_i2, _i1));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void NotLessIComparable()
        {
            var expectedMessage =
                "  Expected: less than Explicit" + Environment.NewLine +
                "  But was:  Ignored" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.Less(_e2, _e1));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void FailureMessage()
        {
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.Less(9, 4));

            var msg = ex?.Message;
            Assert.That(msg, Contains.Substring(TextMessageWriter.Pfx_Expected + "less than 4"));
            Assert.That(msg, Contains.Substring(TextMessageWriter.Pfx_Actual + "9"));
        }
    }
}
