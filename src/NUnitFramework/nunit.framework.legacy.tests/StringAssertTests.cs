// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Legacy.Tests
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
        public void NotCaseInsensitiveCompare()
        {
            StringAssert.AreNotEqualIgnoringCase("name", "NAMES");
        }

        [Test]
        public void NotCaseInsensitiveCompareFails()
        {
            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "not equal to \"Name\", ignoring case" + Environment.NewLine
                + TextMessageWriter.Pfx_Actual + "\"NAME\"" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => StringAssert.AreNotEqualIgnoringCase("Name", "NAME"));
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
        public void DoesNotMatch()
        {
            StringAssert.DoesNotMatch("a?b*c", "12ab456");
        }

        [Test]
        public void DoesNotMatchFails()
        {
            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "not String matching \"a?bc\"" + Environment.NewLine +
                TextMessageWriter.Pfx_Actual + "\"12a3bc45\"" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => StringAssert.DoesNotMatch("a?bc", "12a3bc45"));
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

        #region IsNullOrEmpty Tests

        [Test]
        public void IsNullOrEmpty_Null()
        {
            StringAssert.IsNullOrEmpty(null);
        }

        [Test]
        public void IsNullOrEmpty_Empty()
        {
            StringAssert.IsNullOrEmpty(string.Empty);
        }

        [Test]
        public void IsNullOrEmpty_EmptyString()
        {
            StringAssert.IsNullOrEmpty(string.Empty);
        }

        [Test]
        public void IsNullOrEmptyFails()
        {
            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "null or <empty>" + Environment.NewLine +
                TextMessageWriter.Pfx_Actual + "\"Hello\"" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => StringAssert.IsNullOrEmpty("Hello"));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void IsNullOrEmptyFails_Whitespace()
        {
            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "null or <empty>" + Environment.NewLine +
                TextMessageWriter.Pfx_Actual + "\" \"" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => StringAssert.IsNullOrEmpty(" "));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        #endregion

        #region IsNotNullNorEmpty Tests

        [Test]
        public void IsNotNullNorEmpty()
        {
            StringAssert.IsNotNullNorEmpty("Hello");
        }

        [Test]
        public void IsNotNullNorEmpty_Whitespace()
        {
            StringAssert.IsNotNullNorEmpty(" ");
            StringAssert.IsNotNullNorEmpty("  ");
            StringAssert.IsNotNullNorEmpty("\t");
        }

        [Test]
        public void IsNotNullNorEmptyFails_Null()
        {
            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "not null and not <empty>" + Environment.NewLine +
                TextMessageWriter.Pfx_Actual + "null" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => StringAssert.IsNotNullNorEmpty(null));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void IsNotNullNorEmptyFails_Empty()
        {
            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "not null and not <empty>" + Environment.NewLine +
                TextMessageWriter.Pfx_Actual + "<string.Empty>" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => StringAssert.IsNotNullNorEmpty(string.Empty));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        #endregion

        #region IsNullOrWhiteSpace Tests

        [Test]
        public void IsNullOrWhiteSpace_Null()
        {
            StringAssert.IsNullOrWhiteSpace(null);
        }

        [Test]
        public void IsNullOrWhiteSpace_Empty()
        {
            StringAssert.IsNullOrWhiteSpace(string.Empty);
        }

        [Test]
        public void IsNullOrWhiteSpace_Whitespace()
        {
            StringAssert.IsNullOrWhiteSpace(" ");
            StringAssert.IsNullOrWhiteSpace("  ");
            StringAssert.IsNullOrWhiteSpace("\t");
            StringAssert.IsNullOrWhiteSpace("\n");
            StringAssert.IsNullOrWhiteSpace("\r");
            StringAssert.IsNullOrWhiteSpace(" \t\r\n ");
        }

        [Test]
        public void IsNullOrWhiteSpaceFails()
        {
            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "white-space" + Environment.NewLine +
                TextMessageWriter.Pfx_Actual + "\"Hello\"" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => StringAssert.IsNullOrWhiteSpace("Hello"));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void IsNullOrWhiteSpaceFails_WithWhitespace()
        {
            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "white-space" + Environment.NewLine +
                TextMessageWriter.Pfx_Actual + "\"Hello World\"" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => StringAssert.IsNullOrWhiteSpace("Hello World"));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        #endregion

        #region IsNotNullNorWhiteSpace Tests

        [Test]
        public void IsNotNullNorWhiteSpace()
        {
            StringAssert.IsNotNullNorWhiteSpace("Hello");
        }

        [Test]
        public void IsNotNullNorWhiteSpace_WithWhitespace()
        {
            StringAssert.IsNotNullNorWhiteSpace("Hello World");
            StringAssert.IsNotNullNorWhiteSpace(" Hello");
            StringAssert.IsNotNullNorWhiteSpace("Hello ");
        }

        [Test]
        public void IsNotNullNorWhiteSpaceFails_Null()
        {
            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "not white-space" + Environment.NewLine +
                TextMessageWriter.Pfx_Actual + "null" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => StringAssert.IsNotNullNorWhiteSpace(null));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void IsNotNullNorWhiteSpaceFails_Empty()
        {
            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "not white-space" + Environment.NewLine +
                TextMessageWriter.Pfx_Actual + "<string.Empty>" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => StringAssert.IsNotNullNorWhiteSpace(string.Empty));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void IsNotNullNorWhiteSpaceFails_Whitespace()
        {
            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "not white-space" + Environment.NewLine +
                TextMessageWriter.Pfx_Actual + "\" \"" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => StringAssert.IsNotNullNorWhiteSpace(" "));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        #endregion
    }
}
