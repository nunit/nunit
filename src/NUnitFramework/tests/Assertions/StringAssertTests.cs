// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Assertions
{
    [TestFixture]
    public class StringAssertTests
    {
        [Test]
        public void Contains()
        {
            StringAssert.Contains( "abc", "abc" );
            StringAssert.Contains( "abc", "***abc" );
            StringAssert.Contains( "abc", "**abc**" );
        }

        [Test]
        public void ContainsFails()
        {
            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "String containing \"abc\"" + System.Environment.NewLine +
                TextMessageWriter.Pfx_Actual + "\"abxcdxbc\"" + System.Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => StringAssert.Contains("abc", "abxcdxbc"));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
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
            StringAssert.StartsWith( "abc", "abcdef" );
            StringAssert.StartsWith( "abc", "abc" );
        }

        [Test]
        public void StartsWithFails()
        {
            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "String starting with \"xyz\"" + System.Environment.NewLine +
                TextMessageWriter.Pfx_Actual + "\"abcxyz\"" + System.Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => StringAssert.StartsWith("xyz", "abcxyz"));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
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
            StringAssert.EndsWith( "abc", "abc" );
            StringAssert.EndsWith( "abc", "123abc" );
        }

        [Test]
        public void EndsWithFails()
        {
            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "String ending with \"xyz\"" + System.Environment.NewLine +
                TextMessageWriter.Pfx_Actual + "\"abcdef\"" + System.Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => StringAssert.EndsWith( "xyz", "abcdef" ));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
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
            StringAssert.AreEqualIgnoringCase( "name", "NAME" );
        }

        [Test]
        public void CaseInsensitiveCompareFails()
        {
            var expectedMessage =
                "  Expected string length 4 but was 5. Strings differ at index 4." + System.Environment.NewLine
                + TextMessageWriter.Pfx_Expected + "\"Name\", ignoring case" + System.Environment.NewLine
                + TextMessageWriter.Pfx_Actual   + "\"NAMES\"" + System.Environment.NewLine
                + "  ---------------^" + System.Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => StringAssert.AreEqualIgnoringCase("Name", "NAMES"));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]		
        public void IsMatch()
        {
            StringAssert.IsMatch( "a?bc", "12a3bc45" );
        }

        [Test]
        public void IsMatchFails()
        {
            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "String matching \"a?b*c\"" + System.Environment.NewLine +
                TextMessageWriter.Pfx_Actual + "\"12ab456\"" + System.Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => StringAssert.IsMatch("a?b*c", "12ab456"));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void DifferentEncodingsOfSameStringAreNotEqual()
        {
            string input = "Hello World";
            byte[] data = System.Text.Encoding.Unicode.GetBytes( input );
            string garbage = System.Text.Encoding.UTF8.GetString( data, 0, data.Length);

            Assert.AreNotEqual( input, garbage );
        }


        [Test]
        public void EqualsFailsWhenUsed()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => StringAssert.Equals(string.Empty, string.Empty));
            Assert.That(ex.Message, Does.StartWith("StringAssert.Equals should not be used for Assertions"));
        }

        [Test]
        public void ReferenceEqualsFailsWhenUsed()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => StringAssert.ReferenceEquals(string.Empty, string.Empty));
            Assert.That(ex.Message, Does.StartWith("StringAssert.ReferenceEquals should not be used for Assertions"));
        }
    }
}
