// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class ValueTupleEqualityTests
    {
        [Test]
        public void SucceedsWhenTuplesAreEmpty()
        {
            ValueTuple tuple1 = ValueTuple.Create();
            ValueTuple tuple2 = ValueTuple.Create();
            Assert.That(tuple1, Is.EqualTo(tuple2));
        }

        [Test]
        public void SucceedsWhenSingleTuplesAreTheSame()
        {
            ValueTuple<int> tuple1 = ValueTuple.Create(3);
            ValueTuple<int> tuple2 = ValueTuple.Create(3);
            Assert.That(tuple1, Is.EqualTo(tuple2));
        }

        [Test]
        public void SucceedsWhenTuplesAreTheSame()
        {
            ValueTuple<string, int> tuple1 = ("Hello", 3);
            ValueTuple<string, int> tuple2 = ("Hello", 3);
            Assert.That(tuple1, Is.EqualTo(tuple2));
        }

        [Test]
        public void SucceedsWhenTuplesAreTheSameWithinTolerance()
        {
            ValueTuple<string, int> tuple1 = ("Hello", 3);
            ValueTuple<string, int> tuple2 = ("Hello", 4);
            Assert.That(tuple1, Is.EqualTo(tuple2).Within(1));
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
        public void FailsForTuplesWith32ElementsOrMore()
        {
            var tuple1 = (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, "13", 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32);
            var tuple2 = (2, 2, 4, 4, 6, 6, 8, 8, 10, 10, 12, 12, "13", 14, 16, 16, 18, 18, 20, 20, 22, 22, 24, 24, 26, 26, 28, 28, 30, 30, 32, 32);

            Assert.That(() => Assert.That(tuple2, Is.EqualTo(tuple1).Within(1)),
                Throws.InstanceOf<NotSupportedException>().With.Message.Contains(
                    "Tuples with 32 or more elements are not supported."));
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

        [Test]
        public void SucceedsWithStringModifiersIgnoreCaseAndIgnoreWhiteSpace()
        {
            var a = (2, 5, "HELLO ");
            var b = (2, 5, "hello");

            Assert.That(a, Is.EqualTo(b).IgnoreCase.IgnoreWhiteSpace);
        }

        [Test]
        public void SucceedsWithStringModifiersIgnoreCaseAndLineEnding()
        {
            var a = (2, 5, "HELLO\r");
            var b = (2, 5, "hello\n");

            Assert.That(a, Is.EqualTo(b).IgnoreCase.IgnoreLineEndingFormat);
        }
    }
}
