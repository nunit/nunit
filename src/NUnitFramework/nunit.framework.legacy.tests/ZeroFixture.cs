// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Legacy.Tests
{
    [TestFixture]
    public class ZeroFixture
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

        #region Zero

        [Test]
        public void Zero()
        {
            // Test equality check for all forms
            ClassicAssert.Zero(_i1);
            ClassicAssert.Zero(_i1, "int");
            ClassicAssert.Zero(_i1, "{0}", "int");
            ClassicAssert.Zero(_u1);
            ClassicAssert.Zero(_u1, "uint");
            ClassicAssert.Zero(_u1, "{0}", "uint");
            ClassicAssert.Zero(_l1);
            ClassicAssert.Zero(_l1, "long");
            ClassicAssert.Zero(_l1, "{0}", "long");
            ClassicAssert.Zero(_ul1);
            ClassicAssert.Zero(_ul1, "ulong");
            ClassicAssert.Zero(_ul1, "{0}", "ulong");
            ClassicAssert.Zero(_d1);
            ClassicAssert.Zero(_d1, "double");
            ClassicAssert.Zero(_d1, "{0}", "double");
            ClassicAssert.Zero(_de1);
            ClassicAssert.Zero(_de1, "decimal");
            ClassicAssert.Zero(_de1, "{0}", "decimal");
            ClassicAssert.Zero(_f1);
            ClassicAssert.Zero(_f1, "float");
            ClassicAssert.Zero(_f1, "{0}", "float");
        }

        [Test]
        public void ZeroFails()
        {
            Assert.That(() => ClassicAssert.Zero(_i2, "{0}", "int"),
                        Throws.InstanceOf<AssertionException>().With.Message.Contains("int"));
            Assert.That(() => ClassicAssert.Zero(_u2, "{0}", "uint"),
                        Throws.InstanceOf<AssertionException>().With.Message.Contains("uint"));
            Assert.That(() => ClassicAssert.Zero(_l2, "{0}", "long"),
                Throws.InstanceOf<AssertionException>().With.Message.Contains("long"));
            Assert.That(() => ClassicAssert.Zero(_ul2, "{0}", "ulong"),
                Throws.InstanceOf<AssertionException>().With.Message.Contains("ulong"));
            Assert.That(() => ClassicAssert.Zero(_d2, "{0}", "double"),
                Throws.InstanceOf<AssertionException>().With.Message.Contains("double"));
            Assert.That(() => ClassicAssert.Zero(_de2, "{0}", "decimal"),
                Throws.InstanceOf<AssertionException>().With.Message.Contains("decimal"));
            Assert.That(() => ClassicAssert.Zero(_f2, "{0}", "float"),
                Throws.InstanceOf<AssertionException>().With.Message.Contains("float"));
        }

        [Test]
        public void ZeroFailureMessage()
        {
            var expectedMessage =
                "  Expected: 0" + Environment.NewLine +
                "  But was:  9" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.Zero(9));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        #endregion
        #region NotZero

        [Test]
        public void NotZero()
        {
            // Test equality check for all forms
            ClassicAssert.NotZero(_i2);
            ClassicAssert.NotZero(_i2, "int");
            ClassicAssert.NotZero(_i2, "{0}", "int");
            ClassicAssert.NotZero(_u2);
            ClassicAssert.NotZero(_u2, "uint");
            ClassicAssert.NotZero(_u2, "{0}", "uint");
            ClassicAssert.NotZero(_l2);
            ClassicAssert.NotZero(_l2, "long");
            ClassicAssert.NotZero(_l2, "{0}", "long");
            ClassicAssert.NotZero(_ul2);
            ClassicAssert.NotZero(_ul2, "ulong");
            ClassicAssert.NotZero(_ul2, "{0}", "ulong");
            ClassicAssert.NotZero(_d2);
            ClassicAssert.NotZero(_d2, "double");
            ClassicAssert.NotZero(_d2, "{0}", "double");
            ClassicAssert.NotZero(_de2);
            ClassicAssert.NotZero(_de2, "decimal");
            ClassicAssert.NotZero(_de2, "{0}", "decimal");
            ClassicAssert.NotZero(_f2);
            ClassicAssert.NotZero(_f2, "float");
            ClassicAssert.NotZero(_f2, "{0}", "float");
        }

        [Test]
        public void NotZeroFails()
        {
            Assert.That(() => ClassicAssert.NotZero(_i1, "{0}", "int"),
                        Throws.InstanceOf<AssertionException>().With.Message.Contains("int"));
            Assert.That(() => ClassicAssert.NotZero(_u1, "{0}", "uint"),
                        Throws.InstanceOf<AssertionException>().With.Message.Contains("uint"));
            Assert.That(() => ClassicAssert.NotZero(_l1, "{0}", "long"),
                Throws.InstanceOf<AssertionException>().With.Message.Contains("long"));
            Assert.That(() => ClassicAssert.NotZero(_ul1, "{0}", "ulong"),
                Throws.InstanceOf<AssertionException>().With.Message.Contains("ulong"));
            Assert.That(() => ClassicAssert.NotZero(_d1, "{0}", "double"),
                Throws.InstanceOf<AssertionException>().With.Message.Contains("double"));
            Assert.That(() => ClassicAssert.NotZero(_de1, "{0}", "decimal"),
                Throws.InstanceOf<AssertionException>().With.Message.Contains("decimal"));
            Assert.That(() => ClassicAssert.NotZero(_f1, "{0}", "float"),
                Throws.InstanceOf<AssertionException>().With.Message.Contains("float"));
        }

        [Test]
        public void NotZeroFailureMessage()
        {
            var expectedMessage =
                "  Expected: not equal to 0" + Environment.NewLine +
                "  But was:  0" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.NotZero(0));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        #endregion
    }
}
