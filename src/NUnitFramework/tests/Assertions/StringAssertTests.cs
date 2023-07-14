// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.Assertions
{
    [TestFixture]
    public class StringAssertTests
    {
        [Test]
        public void Contains()
        {
            Classic.StringAssert.Contains( "abc", "abc" );
            Classic.StringAssert.Contains( "abc", "***abc" );
            Classic.StringAssert.Contains( "abc", "**abc**" );
        }

        [Test]
        public void ContainsFails()
        {
            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "String containing \"abc\"" + Environment.NewLine +
                TextMessageWriter.Pfx_Actual + "\"abxcdxbc\"" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Classic.StringAssert.Contains("abc", "abxcdxbc"));
            Assert.That(ex?.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void DoesNotContain()
        {
            Classic.StringAssert.DoesNotContain("x", "abc");
        }

        [Test]
        public void DoesNotContainFails()
        {
            Assert.Throws<AssertionException>(() => Classic.StringAssert.DoesNotContain("abc", "**abc**"));
        }

        [Test]
        public void StartsWith()
        {
            Classic.StringAssert.StartsWith( "abc", "abcdef" );
            Classic.StringAssert.StartsWith( "abc", "abc" );
        }

        [Test]
        public void StartsWithFails()
        {
            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "String starting with \"xyz\"" + Environment.NewLine +
                TextMessageWriter.Pfx_Actual + "\"abcxyz\"" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Classic.StringAssert.StartsWith("xyz", "abcxyz"));
            Assert.That(ex?.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void DoesNotStartWith()
        {
            Classic.StringAssert.DoesNotStartWith("x", "abc");
        }

        [Test]
        public void DoesNotStartWithFails()
        {
            Assert.Throws<AssertionException>(() => Classic.StringAssert.DoesNotStartWith("abc", "abc**"));
        }

        [Test]
        public void EndsWith()
        {
            Classic.StringAssert.EndsWith( "abc", "abc" );
            Classic.StringAssert.EndsWith( "abc", "123abc" );
        }

        [Test]
        public void EndsWithFails()
        {
            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "String ending with \"xyz\"" + Environment.NewLine +
                TextMessageWriter.Pfx_Actual + "\"abcdef\"" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Classic.StringAssert.EndsWith( "xyz", "abcdef" ));
            Assert.That(ex?.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void DoesNotEndWith()
        {
            Classic.StringAssert.DoesNotEndWith("x", "abc");
        }

        [Test]
        public void DoesNotEndWithFails()
        {
            Assert.Throws<AssertionException>(() => Classic.StringAssert.DoesNotEndWith("abc", "***abc"));
        }

        [Test]
        public void CaseInsensitiveCompare()
        {
            Classic.StringAssert.AreEqualIgnoringCase( "name", "NAME" );
        }

        [Test]
        public void CaseInsensitiveCompareFails()
        {
            var expectedMessage =
                "  Expected string length 4 but was 5. Strings differ at index 4." + Environment.NewLine
                + TextMessageWriter.Pfx_Expected + "\"Name\", ignoring case" + Environment.NewLine
                + TextMessageWriter.Pfx_Actual   + "\"NAMES\"" + Environment.NewLine
                + "  ---------------^" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Classic.StringAssert.AreEqualIgnoringCase("Name", "NAMES"));
            Assert.That(ex?.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void IsMatch()
        {
            Classic.StringAssert.IsMatch( "a?bc", "12a3bc45" );
        }

        [Test]
        public void IsMatchFails()
        {
            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "String matching \"a?b*c\"" + Environment.NewLine +
                TextMessageWriter.Pfx_Actual + "\"12ab456\"" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Classic.StringAssert.IsMatch("a?b*c", "12ab456"));
            Assert.That(ex?.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void DifferentEncodingsOfSameStringAreNotEqual()
        {
            string input = "Hello World";
            byte[] data = System.Text.Encoding.Unicode.GetBytes( input );
            string garbage = System.Text.Encoding.UTF8.GetString( data, 0, data.Length);

            Classic.Assert.AreNotEqual( input, garbage );
        }


        [Test]
        public void EqualsFailsWhenUsed()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => Classic.StringAssert.Equals(string.Empty, string.Empty));
            Assert.That(ex?.Message, Does.StartWith("StringAssert.Equals should not be used."));
        }

        [Test]
        public void ReferenceEqualsFailsWhenUsed()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => Classic.StringAssert.ReferenceEquals(string.Empty, string.Empty));
            Assert.That(ex?.Message, Does.StartWith("StringAssert.ReferenceEquals should not be used."));
        }
    }
}
