// ***********************************************************************
// Copyright (c) 2011 Charlie Poole
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
using System.Collections;
using NUnit.Framework.Assertions;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    public class ExactCountConstraintTests
    {
        private static readonly string[] names = new string[] { "Charlie", "Fred", "Joe", "Charlie" };

        [Test]
        public void ZeroItemsMatch()
        {
            Assert.That(names, new ExactCountConstraint(0, Is.EqualTo("Sam")));
            Assert.That(names, Has.Exactly(0).EqualTo("Sam"));
        }

        [Test]
        public void ZeroItemsMatchFails()
        {
            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "no item equal to \"Charlie\"" + Env.NewLine +
                TextMessageWriter.Pfx_Actual + "< \"Charlie\", \"Fred\", \"Joe\", \"Charlie\" >" + Env.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.That(names, new ExactCountConstraint(0, Is.EqualTo("Charlie"))));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void ExactlyOneItemMatches()
        {
            Assert.That(names, new ExactCountConstraint(1, Is.EqualTo("Fred")));
            Assert.That(names, Has.Exactly(1).EqualTo("Fred"));
        }

        [Test]
        public void ExactlyOneItemMatchFails()
        {
            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "exactly one item equal to \"Charlie\"" + Env.NewLine +
                TextMessageWriter.Pfx_Actual + "< \"Charlie\", \"Fred\", \"Joe\", \"Charlie\" >" + Env.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.That(names, new ExactCountConstraint(1, Is.EqualTo("Charlie"))));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void ExactlyTwoItemsMatch()
        {
            Assert.That(names, new ExactCountConstraint(2, Is.EqualTo("Charlie")));
            Assert.That(names, Has.Exactly(2).EqualTo("Charlie"));
        }

        [Test]
        public void ExactlyTwoItemsMatchFails()
        {
            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "exactly 2 items equal to \"Fred\"" + Env.NewLine +
                TextMessageWriter.Pfx_Actual + "< \"Charlie\", \"Fred\", \"Joe\", \"Charlie\" >" + Env.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.That(names, new ExactCountConstraint(2, Is.EqualTo("Fred"))));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }
    }
}