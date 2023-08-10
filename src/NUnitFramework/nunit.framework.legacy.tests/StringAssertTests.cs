// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal;
using NUnit.Framework.Legacy;

namespace NUnit.Framework.Tests.Assertions
{
    [TestFixture]
    public class StringAssertTests
    {
        [Test]
        public void Contains()
        {
            StringAssert.Contains("abc", "abc");
            StringAssert.Contains("abc", "***abc");
            StringAssert.Contains("abc", "**abc**");
        }

        [Test]
        public void ContainsFails()
        {
            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "String containing \"abc\"" + Environment.NewLine +
                TextMessageWriter.Pfx_Actual + "\"abxcdxbc\"" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => StringAssert.Contains("abc", "abxcdxbc"));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void DoesNotContain()
        {
            StringAssert.DoesNotContain("x", "abc");
        }

        [Test]
        public void DoesNotContainFails()
        {
            Assert.Throws<AssertionException>(() => StringAssert.DoesNotContain("abc", "**abc**"));
        }

        [Test]
        public void StartsWith()
        {
            StringAssert.StartsWith("abc", "abcdef");
            StringAssert.StartsWith("abc", "abc");
        }

        [Test]
        public void StartsWithFails()
        {
            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "String starting with \"xyz\"" + Environment.NewLine +
                TextMessageWriter.Pfx_Actual + "\"abcxyz\"" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => StringAssert.StartsWith("xyz", "abcxyz"));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void DoesNotStartWith()
        {
            StringAssert.DoesNotStartWith("x", "abc");
        }

        [Test]
        public void DoesNotStartWithFails()
        {
            Assert.Throws<AssertionException>(() => StringAssert.DoesNotStartWith("abc", "abc**"));
        }

        [Test]
        public void EndsWith()
        {
            StringAssert.EndsWith("abc", "abc");
            StringAssert.EndsWith("abc", "123abc");
        }

        [Test]
        public void EndsWithFails()
        {
            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "String ending with \"xyz\"" + Environment.NewLine +
                TextMessageWriter.Pfx_Actual + "\"abcdef\"" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => StringAssert.EndsWith("xyz", "abcdef"));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void DoesNotEndWith()
        {
            StringAssert.DoesNotEndWith("x", "abc");
        }

        [Test]
        public void DoesNotEndWithFails()
        {
            Assert.Throws<AssertionException>(() => StringAssert.DoesNotEndWith("abc", "***abc"));
        }

        [Test]
        public void CaseInsensitiveCompare()
        {
            StringAssert.AreEqualIgnoringCase("name", "NAME");
        }

        [Test]
        public void CaseInsensitiveCompareFails()
        {
            var expectedMessage =
                "  Expected string length 4 but was 5. Strings differ at index 4." + Environment.NewLine
                + TextMessageWriter.Pfx_Expected + "\"Name\", ignoring case" + Environment.NewLine
                + TextMessageWriter.Pfx_Actual + "\"NAMES\"" + Environment.NewLine
                + "  ---------------^" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => StringAssert.AreEqualIgnoringCase("Name", "NAMES"));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void IsMatch()
        {
            StringAssert.IsMatch("a?bc", "12a3bc45");
        }

        [Test]
        public void IsMatchFails()
        {
            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "String matching \"a?b*c\"" + Environment.NewLine +
                TextMessageWriter.Pfx_Actual + "\"12ab456\"" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => StringAssert.IsMatch("a?b*c", "12ab456"));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void DifferentEncodingsOfSameStringAreNotEqual()
        {
            string input = "Hello World";
            byte[] data = System.Text.Encoding.Unicode.GetBytes(input);
            string garbage = System.Text.Encoding.UTF8.GetString(data, 0, data.Length);

            ClassicAssert.AreNotEqual(input, garbage);
        }

        [Test]
        public void EqualsFailsWhenUsed()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => StringAssert.Equals(string.Empty, string.Empty));
            Assert.That(ex?.Message, Does.StartWith("StringAssert.Equals should not be used."));
        }

        [Test]
        public void ReferenceEqualsFailsWhenUsed()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => StringAssert.ReferenceEquals(string.Empty, string.Empty));
            Assert.That(ex?.Message, Does.StartWith("StringAssert.ReferenceEquals should not be used."));
        }
    }
}
