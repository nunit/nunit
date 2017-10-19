// ***********************************************************************
// Copyright (c) 2011 Charlie Poole, Rob Prouse
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
                TextMessageWriter.Pfx_Expected + "no item equal to \"Charlie\"" + Environment.NewLine +
                TextMessageWriter.Pfx_Actual + "< \"Charlie\", \"Fred\", \"Joe\", \"Charlie\" >" + Environment.NewLine;
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
                TextMessageWriter.Pfx_Expected + "exactly one item equal to \"Charlie\"" + Environment.NewLine +
                TextMessageWriter.Pfx_Actual + "< \"Charlie\", \"Fred\", \"Joe\", \"Charlie\" >" + Environment.NewLine;
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
        public void ExactlyAndExactly()
        {
            Assert.That(names, Has.Exactly(2).EqualTo("Charlie").And.Exactly(1).EqualTo("Fred"));
            Assert.That(names, Has.Exactly(4).Items.And.Exactly(2).EqualTo("Charlie"));
            Assert.That(names, Has.Exactly(2).EqualTo("Charlie").And.Exactly(4).Items);
        }

        [Test]
        public void ExactlyOrExactly()
        {
            Assert.That(names, Has.Exactly(3).EqualTo("Fred").Or.Exactly(2).EqualTo("Charlie"));
        }

        [Test]
        public void ExactlyFollowedByOr()
        {
            Assert.That(names, Has.Exactly(3).EqualTo("Fred").Or.EqualTo("Charlie"));
        }

        [Test]
        public void ExactlyTwoItemsMatchFails()
        {
            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "exactly 2 items equal to \"Fred\"" + Environment.NewLine +
                TextMessageWriter.Pfx_Actual + "< \"Charlie\", \"Fred\", \"Joe\", \"Charlie\" >" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.That(names, new ExactCountConstraint(2, Is.EqualTo("Fred"))));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void ExactlyFourItemsNoPredicate()
        {
            Assert.That(names, new ExactCountConstraint(4));
        }

        [Test]
        public void ExactlyFourItemsNoopNoPredicate()
        {
            Assert.That(names, Has.Exactly(4).Items);
        }

        [Test]
        public void ExactlyOneItemNoPredicateFails()
        {
            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "exactly one item" + Environment.NewLine +
                TextMessageWriter.Pfx_Actual + "< \"Charlie\", \"Fred\", \"Joe\", \"Charlie\" >" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.That(names, new ExactCountConstraint(1)));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void ExactlyTwoItemsNoopMatch()
        {
            Assert.That(names, Has.Exactly(2).Items.EqualTo("Charlie"));
        }
        
        [Test]
        public void FailsWhenNotUsedAgainstAnEnumerable()
        {
            var notEnumerable = 42;
            TestDelegate act = () => Assert.That(notEnumerable, new ExactCountConstraint(1));
            Assert.That(act, Throws.ArgumentException.With.Message.Contains("IEnumerable"));
        }
    }
}