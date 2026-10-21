// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;

namespace NUnit.Framework.Tests.Constraints
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
        public void SucceedsWhenTuplesAreTheSameWithinTolerance()
        {
            var tuple1 = Tuple.Create("Hello", 3);
            var tuple2 = Tuple.Create("Hello", 4);
            Assert.That(tuple1, Is.EqualTo(tuple2).Within(1));
        }

        [Test]
        public void SucceedsWhenSingleTuplesAreTheSame()
        {
            var tuple1 = Tuple.Create(3);
            var tuple2 = Tuple.Create(3);
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

        [Test]
        public void CanUseToleranceIfSomeMemberSupportsTolerance()
        {
            var tuple1 = Tuple.Create(true, 1);
            var tuple2 = Tuple.Create(true, 2);
            Assert.That(tuple1, Is.EqualTo(tuple2).Within(1));
        }

        [Test]
        public void WillComplainAboutToleranceIfNoMemberSupportsTolerance()
        {
#pragma warning disable NUnit2047 // Incompatible types for Within constraint
            var tuple1 = Tuple.Create(true, "1");
            var tuple2 = Tuple.Create(true, "2");
            Assert.That(() => Assert.That(tuple1, Is.EqualTo(tuple2).Within(1)),
                        Throws.InstanceOf<NotSupportedException>().With.Message.Contains("Tolerance"));
#pragma warning restore NUnit2047 // Incompatible types for Within constraint
        }

        [Test]
        public void CanCompareTuplesWithDifferentTypes()
        {
            Tuple<int, int, int> intTuple = Tuple.Create(1, 2, 3);
            Tuple<double, double, double> doubleTuple = Tuple.Create(1.0, 2.0, 3.0);

            using (Assert.EnterMultipleScope())
            {
                // This is a side effect of using ITuple to compare members.
                // NUnit allows comparing Tuple<int> with Tuple<double> so why not allow ValueTuple.
#pragma warning disable NUnit2021 // Incompatible types for EqualTo constraint
                Assert.That(doubleTuple, Is.EqualTo(intTuple));
                Assert.That(intTuple, Is.EqualTo(doubleTuple));
#pragma warning restore NUnit2021 // Incompatible types for EqualTo constraint
            }
        }

        [Test]
        public void WillNotUseTupleComparerForDerivedTypes()
        {
            var tuple1 = new MyTuple("Tuple1", 1);
            var tuple2 = new MyTuple("Tuple2", 1);
            Assert.That(tuple1, Is.Not.EqualTo(tuple2));
        }

        private sealed class MyTuple : Tuple<int>, IStructuralEquatable
        {
            public MyTuple(string name, int item1)
                : base(item1)
            {
                Name = name;
            }

            public string Name { get; }

            public override string ToString()
            {
                return $"{Name}: {base.ToString()}";
            }

            // NUnit calls IStructuralEquatable.Equals before Object.Equals
            // We need to therefore implement IStructuralEquatable here
            // as otherwise the Tuple<>'s implementation will be used, which will ignore the Name property.
            bool IStructuralEquatable.Equals(object? other, IEqualityComparer comparer)
            {
                if (other is MyTuple otherTuple)
                {
                    return comparer.Equals(Name, otherTuple.Name) &&
                           comparer.Equals(Item1, otherTuple.Item1);
                }

                return false;
            }

            int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
            {
                return HashCode.Combine(
                    comparer.GetHashCode(Name),
                    comparer.GetHashCode(Item1));
            }

            public override bool Equals(object? obj)
            {
                if (obj is MyTuple other)
                {
                    return Name == other.Name && base.Equals(other);
                }

                return false;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Name, base.GetHashCode());
            }
        }
    }
}
