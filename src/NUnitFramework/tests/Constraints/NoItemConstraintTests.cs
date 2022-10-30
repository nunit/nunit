// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class NoItemConstraintTests
    {
        private readonly string NL = Environment.NewLine;

        [Test]
        public void NoItemsAreNotNull()
        {
            object[] c = new object[] { 1, "hello", 3, Environment.NewLine };
            Assert.That(c, new NoItemConstraint(Is.Null));
        }

        [Test]
        public void NoItemsAreNotNullFails()
        {
            object[] c = new object[] { 1, "hello", null, 3 };
            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "no item null" + NL +
                TextMessageWriter.Pfx_Actual + "< 1, \"hello\", null, 3 >" + NL +
                "  First non-matching item at index [2]:  null" + NL;
            var ex = Assert.Throws<AssertionException>(() => Assert.That(c, new NoItemConstraint(Is.Null)));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void FailsWhenNotUsedAgainstAnEnumerable()
        {
            var notEnumerable = 42;
            TestDelegate act = () => Assert.That(notEnumerable, new NoItemConstraint(Is.Null));
            Assert.That(act, Throws.ArgumentException.With.Message.Contains("IEnumerable"));
        }
    }
}
