// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Classic.Tests
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
            Assert.Less(_i1, _i2);
            Assert.Less(_i1, _i2, "int");
            Assert.Less(_i1, _i2, "{0}", "int");
            Assert.Less(_u1, _u2, "uint");
            Assert.Less(_u1, _u2, "{0}", "uint");
            Assert.Less(_l1, _l2, "long");
            Assert.Less(_l1, _l2, "{0}", "long");
            Assert.Less(_ul1, _ul2, "ulong");
            Assert.Less(_ul1, _ul2, "{0}", "ulong");
            Assert.Less(_d1, _d2);
            Assert.Less(_d1, _d2, "double");
            Assert.Less(_d1, _d2, "{0}", "double");
            Assert.Less(_de1, _de2);
            Assert.Less(_de1, _de2, "decimal");
            Assert.Less(_de1, _de2, "{0}", "decimal");
            Assert.Less(_f1, _f2);
            Assert.Less(_f1, _f2, "float");
            Assert.Less(_f1, _f2, "{0}", "float");
        }

        [Test]
        public void MixedTypes()
        {
            Assert.Less(5, 8L, "int to long");
            Assert.Less(5, 8.2f, "int to float");
            Assert.Less(5, 8.2d, "int to double");
            Assert.Less(5, 8U, "int to uint");
            Assert.Less(5, 8UL, "int to ulong");
            Assert.Less(5, 8M, "int to decimal");

            Assert.Less(5L, 8, "long to int");
            Assert.Less(5L, 8.2f, "long to float");
            Assert.Less(5L, 8.2d, "long to double");
            Assert.Less(5L, 8U, "long to uint");
            Assert.Less(5L, 8UL, "long to ulong");
            Assert.Less(5L, 8M, "long to decimal");

            Assert.Less(3.5f, 5, "float to int");
            Assert.Less(3.5f, 8L, "float to long");
            Assert.Less(3.5f, 8.2d, "float to double");
            Assert.Less(3.5f, 8U, "float to uint");
            Assert.Less(3.5f, 8UL, "float to ulong");
            Assert.Less(3.5f, 8.2M, "float to decimal");

            Assert.Less(3.5d, 5, "double to int");
            Assert.Less(3.5d, 5L, "double to long");
            Assert.Less(3.5d, 8.2f, "double to float");
            Assert.Less(3.5d, 8U, "double to uint");
            Assert.Less(3.5d, 8UL, "double to ulong");
            Assert.Less(3.5d, 8.2M, "double to decimal");

            Assert.Less(5U, 8, "uint to int");
            Assert.Less(5U, 8L, "uint to long");
            Assert.Less(5U, 8.2f, "uint to float");
            Assert.Less(5U, 8.2d, "uint to double");
            Assert.Less(5U, 8UL, "uint to ulong");
            Assert.Less(5U, 8M, "uint to decimal");

            Assert.Less(5ul, 8, "ulong to int");
            Assert.Less(5UL, 8L, "ulong to long");
            Assert.Less(5UL, 8.2f, "ulong to float");
            Assert.Less(5UL, 8.2d, "ulong to double");
            Assert.Less(5UL, 8U, "ulong to uint");
            Assert.Less(5UL, 8M, "ulong to decimal");

            Assert.Less(5M, 8, "decimal to int");
            Assert.Less(5M, 8L, "decimal to long");
            Assert.Less(5M, 8.2f, "decimal to float");
            Assert.Less(5M, 8.2d, "decimal to double");
            Assert.Less(5M, 8U, "decimal to uint");
            Assert.Less(5M, 8UL, "decimal to ulong");
        }

        [Test]
        public void NotLessWhenEqual()
        {
            var expectedMessage =
                "  Expected: less than 5" + Environment.NewLine +
                "  But was:  5" + Environment.NewLine;
            var ex = Framework.Assert.Throws<AssertionException>(() => Assert.Less(_i1, _i1));
            Framework.Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void NotLess()
        {
            var expectedMessage =
                "  Expected: less than 5" + Environment.NewLine +
                "  But was:  8" + Environment.NewLine;
            var ex = Framework.Assert.Throws<AssertionException>(() => Assert.Less(_i2, _i1));
            Framework.Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void NotLessIComparable()
        {
            var expectedMessage =
                "  Expected: less than Explicit" + Environment.NewLine +
                "  But was:  Ignored" + Environment.NewLine;
            var ex = Framework.Assert.Throws<AssertionException>(() => Assert.Less(_e2, _e1));
            Framework.Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void FailureMessage()
        {
            var ex = Framework.Assert.Throws<AssertionException>(() => Assert.Less(9, 4));

            var msg = ex?.Message;
            Framework.Assert.That(msg, Contains.Substring(TextMessageWriter.Pfx_Expected + "less than 4"));
            Framework.Assert.That(msg, Contains.Substring(TextMessageWriter.Pfx_Actual + "9"));
        }
    }
}
