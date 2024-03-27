// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.Constraints
{
    public class ContainsConstraintTests
    {
        private static readonly string NL = Environment.NewLine;

        [Test]
        public void HonorsIgnoreCaseForStringCollection()
        {
            var actualItems = new[] { "ABC", "def" };
            var constraint = new ContainsConstraint("abc").IgnoreCase;

            var result = constraint.ApplyTo(actualItems);

            Assert.That(result.IsSuccess);
        }

        [Test]
        public void HonorsIgnoreWhiteSpaceForStringCollection()
        {
            var actualItems = new[] { "ABC", "d e f" };
            var constraint = new ContainsConstraint("def").IgnoreWhiteSpace;

            var result = constraint.ApplyTo(actualItems);
            Assert.That(result.IsSuccess);
        }

        [Test]
        public void HonorsIgnoreWhiteSpaceForStringCollectionSearchItem()
        {
            var actualItems = new[] { "ABC", "d e f" };
            var constraint = new ContainsConstraint("A B C").IgnoreWhiteSpace;

            var result = constraint.ApplyTo(actualItems);
            Assert.That(result.IsSuccess);
        }

        [Test, SetCulture("en-US")]
        public void HonorsIgnoreCaseForString()
        {
            var actualString = "ABCdef";
            var constraint = new ContainsConstraint("abc").IgnoreCase;

            var result = constraint.ApplyTo(actualString);

            Assert.That(result.IsSuccess);
        }

        [Test]
        public void ContainsSubstringErrorMessage()
        {
            var actualString = "abc";
            var expected = "bcd";

            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "String containing \"bcd\"" + NL +
                TextMessageWriter.Pfx_Actual + "\"abc\"" + NL;

            var ex = Assert.Throws<AssertionException>(() => Assert.That(actualString, Does.Contain(expected)));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void ContainsSubstringIgnoreCaseErrorMessage()
        {
            var actualString = "abc";
            var expected = "bcd";

            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "String containing \"bcd\", ignoring case" + NL +
                TextMessageWriter.Pfx_Actual + "\"abc\"" + NL;

            var ex = Assert.Throws<AssertionException>(() => Assert.That(actualString, Does.Contain(expected).IgnoreCase));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void ContainsItemErrorMessage()
        {
            var actualItems = new[] { "a", "b" };
            var expected = "c";

            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "some item equal to \"c\"" + NL +
                TextMessageWriter.Pfx_Actual + "< \"a\", \"b\" >" + NL;

            var ex = Assert.Throws<AssertionException>(() => Assert.That(actualItems, Does.Contain(expected)));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void ContainsItemIgnoreCaseErrorMessage()
        {
            var actualItems = new[] { "a", "b" };
            var expected = "c";

            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "some item equal to \"c\", ignoring case" + NL +
                TextMessageWriter.Pfx_Actual + "< \"a\", \"b\" >" + NL;

            var ex = Assert.Throws<AssertionException>(() => Assert.That(actualItems, Does.Contain(expected).IgnoreCase));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }
    }
}
