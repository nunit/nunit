// ***********************************************************************
// Copyright (c) 2007 Charlie Poole, Rob Prouse
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

#if !NET35
using System;
using System.Collections.Generic;

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class ValueTupleEqualityTests
    {
        [Test]
        public void SucceedsWhenTuplesAreTheSame()
        {
            ValueTuple<string, int> tuple1 = ("Hello", 3);
            ValueTuple<string, int> tuple2 = ("Hello", 3);
            Assert.That(tuple1, Is.EqualTo(tuple2));
        }

        [Test]
        public void SucceedsWhenContentOfTuplesAreEquivalent()
        {
            ValueTuple<string, int> tuple1 = ("Hello", 3);
            ValueTuple<string, int?> tuple2 = ("Hello", 3);
            Assert.That(tuple1, Is.EqualTo(tuple2));
        }

        [Test]
        public void SucceedsWhenContentOfTuplesAreEquivalentWith8Elements()
        {
            var tuple1 = (1, 2, 3, 4, 5, 6, 7, 8);
            var tuple2 = (1, 2, 3, 4, 5, 6, 7, 8.0f);

            Assert.That(tuple1, Is.EqualTo(tuple2));
        }

        [Test]
        public void SucceedsWhenContentOfTuplesAreEquivalentWith15Elements()
        {
            var tuple1 = (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15);
            var tuple2 = (1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f, 9.0f, 10.0f, 11.0f, 12.0f, 13.0f, 14.0f, 15.0f);

            Assert.That(tuple1, Is.EqualTo(tuple2));
        }

        [Test]
        public void FailsWhenTuplesAreOfDifferentLengths()
        {
            ValueTuple<string, int> tuple1 = ("Hello", 3);
            ValueTuple<string, int, int> tuple2 = ("Hello", 3, 1);
            Assert.That(tuple1, Is.Not.EqualTo(tuple2));
        }

        [Test]
        public void FailsWhenContentOfTuplesAreDifferent()
        {
            ValueTuple<string, int> tuple1 = ("Hello", 3);
            ValueTuple<string, int> tuple2 = ("Hello", 4);
            Assert.That(tuple1, Is.Not.EqualTo(tuple2));
        }

        [Test]
        public void FailsWhenContentOfTuplesAreDifferentWith8Elements()
        {
            var tuple1 = (1, 2, 3, 4, 5, 6, 7, 8);
            var tuple2 = (1, 2, 3, 4, 5, 6, 7, 9);
            Assert.That(tuple1, Is.Not.EqualTo(tuple2));
        }

        [Test]
        public void ValueTupleElementsAreComparedUsingNUnitEqualityComparer()
        {
            var a = ValueTuple.Create(new Dictionary<string, string>());
            var b = ValueTuple.Create(new Dictionary<string, string>());
            Assert.That(a, Is.EqualTo(b));
        }
    }
}
#endif
