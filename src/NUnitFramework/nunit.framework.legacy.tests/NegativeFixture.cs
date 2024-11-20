// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Legacy.Tests
{
    [TestFixture]
    public class NegativeFixture
    {
        private readonly int _i1 = 0;
        private readonly int _i2 = -8;
        private readonly uint _u1 = 0;
        private readonly long _l1 = 0;
        private readonly long _l2 = -12345879;
        private readonly ulong _ul1 = 0;
        private readonly float _f1 = 0.0F;
        private readonly float _f2 = -8.543F;
        private readonly decimal _de1 = 0.0M;
        private readonly decimal _de2 = -83.4M;
        private readonly double _d1 = 0.0;
        private readonly double _d2 = -8.0;

        [Test]
        public void Negative()
        {
            // Test equality check for all forms
            ClassicAssert.Negative(_i2);
            ClassicAssert.Negative(_i2, "int");
            ClassicAssert.Negative(_i2, "{0}", "int");
            ClassicAssert.Negative(_l2);
            ClassicAssert.Negative(_l2, "long");
            ClassicAssert.Negative(_l2, "{0}", "long");
            ClassicAssert.Negative(_d2);
            ClassicAssert.Negative(_d2, "double");
            ClassicAssert.Negative(_d2, "{0}", "double");
            ClassicAssert.Negative(_de2);
            ClassicAssert.Negative(_de2, "decimal");
            ClassicAssert.Negative(_de2, "{0}", "decimal");
            ClassicAssert.Negative(_f2);
            ClassicAssert.Negative(_f2, "float");
            ClassicAssert.Negative(_f2, "{0}", "float");
        }

        [Test]
        public void NegativeFails()
        {
            Assert.That(() => ClassicAssert.Negative(_i1, "{0}", "int"),
                        Throws.InstanceOf<AssertionException>().With.Message.Contains("int"));
            Assert.That(() => ClassicAssert.Negative(_u1),
                        Throws.InstanceOf<AssertionException>());
            Assert.That(() => ClassicAssert.Negative(_u1, "{0}", "uint"),
                        Throws.InstanceOf<AssertionException>().With.Message.Contains("uint"));
            Assert.That(() => ClassicAssert.Negative(_l1, "{0}", "long"),
                Throws.InstanceOf<AssertionException>().With.Message.Contains("long"));
            Assert.That(() => ClassicAssert.Negative(_ul1),
                        Throws.InstanceOf<AssertionException>());
            Assert.That(() => ClassicAssert.Negative(_ul1, "{0}", "ulong"),
                Throws.InstanceOf<AssertionException>().With.Message.Contains("ulong"));
            Assert.That(() => ClassicAssert.Negative(_d1, "{0}", "double"),
                Throws.InstanceOf<AssertionException>().With.Message.Contains("double"));
            Assert.That(() => ClassicAssert.Negative(_de1, "{0}", "decimal"),
                Throws.InstanceOf<AssertionException>().With.Message.Contains("decimal"));
            Assert.That(() => ClassicAssert.Negative(_f1, "{0}", "float"),
                Throws.InstanceOf<AssertionException>().With.Message.Contains("float"));
        }

        [Test]
        public void NegativeFailureMessage()
        {
            var expectedMessage =
                "  Expected: less than 0" + Environment.NewLine +
                "  But was:  0" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.Negative(0));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }
    }
}
