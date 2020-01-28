// ***********************************************************************
// Copyright (c) 2018 Charlie Poole, Rob Prouse
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

namespace NUnit.Framework.Constraints
{
    public class ContainsConstraintTests
    {
        private readonly string NL = Environment.NewLine;

        [Test]
        public void HonorsIgnoreCaseForStringCollection()
        {
            var actualItems = new[] { "ABC", "def" };
            var constraint = new ContainsConstraint("abc").IgnoreCase;

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
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
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
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
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
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
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
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }
    }
}
