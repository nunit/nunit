// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Runtime.CompilerServices;
using NUnit.Framework.Internal.Extensions;

namespace NUnit.Framework.Tests.Internal.Extensions
{
    [TestFixture]
    internal sealed class TupleAccessorTests
    {
        [Test]
        public void For_ReturnsNullForNullTuple()
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(TupleAccessor.For(null), Is.Null);
                Assert.That(TupleAccessor.For(3), Is.Null);
            }
        }

        [Test]
        public void For_ReturnNonNullForTuple()
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(TupleAccessor.For(Tuple.Create(1)), Is.Not.Null);
                Assert.That(TupleAccessor.For(Tuple.Create(1, 2)), Is.Not.Null);
                Assert.That(TupleAccessor.For(Tuple.Create(1, 2, 3)), Is.Not.Null);
                Assert.That(TupleAccessor.For(Tuple.Create(1, 2, 3, 4)), Is.Not.Null);
                Assert.That(TupleAccessor.For(Tuple.Create(1, 2, 3, 4, 5)), Is.Not.Null);
                Assert.That(TupleAccessor.For(Tuple.Create(1, 2, 3, 4, 5, 6)), Is.Not.Null);
                Assert.That(TupleAccessor.For(Tuple.Create(1, 2, 3, 4, 5, 6, 7)), Is.Not.Null);
                var tuple = Tuple.Create(1, 2, 3, 4, 5, 6, 7, 8);
                Assert.That(TupleAccessor.For(tuple), Is.Not.Null);
                Assert.That(TupleAccessor.For(tuple.Rest), Is.Not.Null);
            }
        }

        [Test]
        public void For_ReturnNonNullForValueTuple()
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(TupleAccessor.For(ValueTuple.Create(1)), Is.Not.Null);
                Assert.That(TupleAccessor.For((1, 2)), Is.Not.Null);
                Assert.That(TupleAccessor.For((1, 2, 3)), Is.Not.Null);
                Assert.That(TupleAccessor.For((1, 2, 3, 4)), Is.Not.Null);
                Assert.That(TupleAccessor.For((1, 2, 3, 4, 5)), Is.Not.Null);
                Assert.That(TupleAccessor.For((1, 2, 3, 4, 5, 6)), Is.Not.Null);
                Assert.That(TupleAccessor.For((1, 2, 3, 4, 5, 6, 7)), Is.Not.Null);
                var tuple = (1, 2, 3, 4, 5, 6, 7, 8);
                Assert.That(TupleAccessor.For(tuple), Is.Not.Null);
                Assert.That(TupleAccessor.For(tuple.Rest), Is.Not.Null);
            }
        }

        [Test]
        public void ITupleCanAccessTupleValues()
        {
            var rest = Tuple.Create(8, 9);
            var tuple = new Tuple<int, int, int, int, int, int, int, Tuple<int, int>>(1, 2, 3, 4, 5, 6, 7, rest);
            Assert.That(tuple.Rest, Is.EqualTo(rest));

            ITuple? accessor = TupleAccessor.For(tuple);
            Assert.That(accessor, Is.Not.Null);
            Assert.That(accessor.Length, Is.EqualTo(9));

            using (Assert.EnterMultipleScope())
            {
                Assert.That(accessor[0], Is.EqualTo(1));
                Assert.That(accessor[1], Is.EqualTo(2));
                Assert.That(accessor[2], Is.EqualTo(3));
                Assert.That(accessor[3], Is.EqualTo(4));
                Assert.That(accessor[4], Is.EqualTo(5));
                Assert.That(accessor[5], Is.EqualTo(6));
                Assert.That(accessor[6], Is.EqualTo(7));
                Assert.That(accessor[7], Is.EqualTo(8));
            }
        }

        [Test]
        public void ITupleCanAccessValueTupleValues()
        {
            var tuple = (1, 2, 3, 4, 5, 6, 7, 8, 9);
            ITuple? accessor = TupleAccessor.For(tuple);
            Assert.That(accessor, Is.Not.Null);
            Assert.That(accessor.Length, Is.EqualTo(9));

            using (Assert.EnterMultipleScope())
            {
                Assert.That(accessor[0], Is.EqualTo(1));
                Assert.That(accessor[1], Is.EqualTo(2));
                Assert.That(accessor[2], Is.EqualTo(3));
                Assert.That(accessor[3], Is.EqualTo(4));
                Assert.That(accessor[4], Is.EqualTo(5));
                Assert.That(accessor[5], Is.EqualTo(6));
                Assert.That(accessor[6], Is.EqualTo(7));
                Assert.That(accessor[7], Is.EqualTo(8));
                Assert.That(accessor[8], Is.EqualTo(9));
            }
        }
    }
}
