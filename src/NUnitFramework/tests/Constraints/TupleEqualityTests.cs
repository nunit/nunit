// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class TupleEqualityTests
    {
        [Test]
        public void SucceedsWhenTuplesAreTheSame()
        {
            var tuple1 = Tuple.Create("Hello", 3);
            var tuple2 = Tuple.Create("Hello", 3);
            Assert.That(tuple1, Is.EqualTo(tuple2));
        }

        [Test]
        public void SucceedsWhenContentOfTuplesAreEquivalent()
        {
            var actual = Tuple.Create(1, 2);
            var expected = Tuple.Create(1.0, 2.0);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void SucceedsWhenContentOfTuplesAreEquivalentWith8Elements()
        {
            var tuple1 = Tuple.Create(1, 2, 3, 4, 5, 6, 7, 8);
            var tuple2 = Tuple.Create(1, 2, 3, 4, 5, 6, 7, 8.0f);
            Assert.That(tuple1, Is.EqualTo(tuple2));
        }

        [Test]
        public void SucceedsWhenContentOfTuplesAreEquivalentWith15Elements()
        {
            var tuple1LastElements = Tuple.Create(8, 9, 10, 11, 12, 13, 14, 15);
            var tuple1 = Tuple.Create(1, 2, 3, 4, 5, 6, 7, tuple1LastElements);

            var tuple2LastElements = Tuple.Create(8.0f, 9.0f, 10.0f, 11.0f, 12.0f, 13.0f, 14.0f, 15.0f);
            var tuple2 = Tuple.Create(1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, tuple2LastElements);

            Assert.That(tuple1, Is.EqualTo(tuple2));
        }

        [Test]
        public void FailsWhenTuplesAreOfDifferentLengths()
        {
            var tuple1 = Tuple.Create("Hello", 3);
            var tuple2 = Tuple.Create("Hello", 3, 1);
#pragma warning disable NUnit2021 // Incompatible types for EqualTo constraint
            Assert.That(tuple1, Is.Not.EqualTo(tuple2));
#pragma warning restore NUnit2021 // Incompatible types for EqualTo constraint
        }

        [Test]
        public void FailsWhenContentOfTuplesAreDifferent()
        {
            var tuple1 = Tuple.Create("Hello", 3);
            var tuple2 = Tuple.Create("Hello", 4);
            Assert.That(tuple1, Is.Not.EqualTo(tuple2));
        }

        [Test]
        public void FailsWhenContentOfTuplesAreDifferentWith8Elements()
        {
            var tuple1 = Tuple.Create(1, 2, 3, 4, 5, 6, 7, 8);
            var tuple2 = Tuple.Create(1, 2, 3, 4, 5, 6, 7, 9);
            Assert.That(tuple1, Is.Not.EqualTo(tuple2));
        }

        [Test]
        public void TupleElementsAreComparedUsingNUnitEqualityComparer()
        {
            var a = Tuple.Create(new Dictionary<string, string>());
            var b = Tuple.Create(new Dictionary<string, string>());
            Assert.That(a, Is.EqualTo(b));
        }
    }
}
