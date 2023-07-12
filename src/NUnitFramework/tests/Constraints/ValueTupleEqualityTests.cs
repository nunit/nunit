// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;

namespace NUnit.Framework.Tests.Constraints
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
