// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Legacy.Tests
{
    [TestFixture]
    public class PositiveFixture
    {
        private readonly int _i1 = 0;
        private readonly int _i2 = 8;
        private readonly uint _u1 = 0;
        private readonly uint _u2 = 12345879;
        private readonly long _l1 = 0;
        private readonly long _l2 = 12345879;
        private readonly ulong _ul1 = 0;
        private readonly ulong _ul2 = 12345879;
        private readonly float _f1 = 0.0F;
        private readonly float _f2 = 8.543F;
        private readonly decimal _de1 = 0.0M;
        private readonly decimal _de2 = 83.4M;
        private readonly double _d1 = 0.0;
        private readonly double _d2 = 8.0;

        [Test]
        public void Positive()
        {
            // Test equality check for all forms
            ClassicAssert.Positive(_i2);
            ClassicAssert.Positive(_i2, "int");
            ClassicAssert.Positive(_i2, "{0}", "int");
            ClassicAssert.Positive(_u2);
            ClassicAssert.Positive(_u2, "uint");
            ClassicAssert.Positive(_u2, "{0}", "uint");
            ClassicAssert.Positive(_l2);
            ClassicAssert.Positive(_l2, "long");
            ClassicAssert.Positive(_l2, "{0}", "long");
            ClassicAssert.Positive(_ul2);
            ClassicAssert.Positive(_ul2, "ulong");
            ClassicAssert.Positive(_ul2, "{0}", "ulong");
            ClassicAssert.Positive(_d2);
            ClassicAssert.Positive(_d2, "double");
            ClassicAssert.Positive(_d2, "{0}", "double");
            ClassicAssert.Positive(_de2);
            ClassicAssert.Positive(_de2, "decimal");
            ClassicAssert.Positive(_de2, "{0}", "decimal");
            ClassicAssert.Positive(_f2);
            ClassicAssert.Positive(_f2, "float");
            ClassicAssert.Positive(_f2, "{0}", "float");
        }

        [Test]
        public void PositiveFails()
        {
            Assert.That(() => ClassicAssert.Positive(_i1, "{0}", "int"),
                        Throws.InstanceOf<AssertionException>().With.Message.Contains("int"));
            Assert.That(() => ClassicAssert.Positive(_u1, "{0}", "uint"),
                        Throws.InstanceOf<AssertionException>().With.Message.Contains("uint"));
            Assert.That(() => ClassicAssert.Positive(_l1, "{0}", "long"),
                Throws.InstanceOf<AssertionException>().With.Message.Contains("long"));
            Assert.That(() => ClassicAssert.Positive(_ul1, "{0}", "ulong"),
                Throws.InstanceOf<AssertionException>().With.Message.Contains("ulong"));
            Assert.That(() => ClassicAssert.Positive(_d1, "{0}", "double"),
                Throws.InstanceOf<AssertionException>().With.Message.Contains("double"));
            Assert.That(() => ClassicAssert.Positive(_de1, "{0}", "decimal"),
                Throws.InstanceOf<AssertionException>().With.Message.Contains("decimal"));
            Assert.That(() => ClassicAssert.Positive(_f1, "{0}", "float"),
                Throws.InstanceOf<AssertionException>().With.Message.Contains("float"));
        }

        [Test]
        public void PositiveFailureMessage()
        {
            var expectedMessage =
                "  Expected: greater than 0" + Environment.NewLine +
                "  But was:  0" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.Positive(0));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }
    }
}
