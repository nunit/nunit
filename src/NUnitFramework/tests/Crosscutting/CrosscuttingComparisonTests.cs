using System;
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Crosscutting
{
    public abstract partial class CrosscuttingComparisonTests
    {
        protected void AssertEquality(object first, object second)
        {
            RunComparisons(new ComparisonBehavior(first, second, ValueRelationship.Equality));
        }

        protected void AssertFirstIsGreater(object first, object second)
        {
            RunComparisons(new ComparisonBehavior(first, second, ValueRelationship.FirstIsGreater));
        }

        protected void AssertUnorderedInequality(object first, object second)
        {
            RunComparisons(new ComparisonBehavior(first, second, ValueRelationship.UnorderedInequality));
        }

        /// <summary>
        /// Exercises every API to which each test’s specified behavior should apply.
        /// </summary>
        protected void RunComparisons(ComparisonBehavior comparisonBehavior)
        {
            comparisonBehavior
                .Verify((x, y) =>
                {
                    var tolerance = Tolerance.Default;
                    return new NUnitEqualityComparer().AreEqual(x, y, ref tolerance);
                }, ComparisonType.Equal)

                .Verify((x, y) => new NUnitComparer().Compare(x, y) == 0, ComparisonType.Equal)
                .Verify((x, y) => new NUnitComparer().Compare(x, y) > 0, ComparisonType.Greater);

            comparisonBehavior
                .WithAssertionException
                .Verify((x, y) => Assert.That(x, Is.EqualTo(y)), ComparisonType.Equal)
                .Verify((x, y) => Assert.That(x, Is.LessThan(y)), ComparisonType.Less)
                .Verify((x, y) => Assert.That(x, Is.LessThanOrEqualTo(y)), ComparisonType.LessOrEqual)
                .Verify((x, y) => Assert.That(x, Is.AtMost(y)), ComparisonType.LessOrEqual)
                .Verify((x, y) => Assert.That(x, Is.GreaterThan(y)), ComparisonType.Greater)
                .Verify((x, y) => Assert.That(x, Is.GreaterThanOrEqualTo(y)), ComparisonType.GreaterOrEqual)
                .Verify((x, y) => Assert.That(x, Is.AtLeast(y)), ComparisonType.GreaterOrEqual)

                .Verify((x, y) => Assert.That(x, Is.AnyOf(y)), ComparisonType.Equal)

                .Verify((x, y) => Assert.That(new[] { y }, Contains.Item(x)), ComparisonType.Equal);

            // TODO: flesh out constraint syntaxes

            var message = string.Empty;

            comparisonBehavior
                .WithAssertionException
                .Verify(Assert.AreEqual, ComparisonType.Equal)
                .Verify(Assert.AreNotEqual, ComparisonType.Unequal)
                .Verify((x, y) => Assert.AreEqual(x, y, message), ComparisonType.Equal)
                .Verify((x, y) => Assert.AreNotEqual(x, y, message), ComparisonType.Unequal);

            comparisonBehavior
                .If<int>()
                .WithAssertionException
                .Verify(Assert.Less, ComparisonType.Less)
                .Verify(Assert.LessOrEqual, ComparisonType.LessOrEqual)
                .Verify(Assert.Greater, ComparisonType.Greater)
                .Verify(Assert.GreaterOrEqual, ComparisonType.GreaterOrEqual)
                .Verify((x, y) => Assert.Less(x, y, message), ComparisonType.Less)
                .Verify((x, y) => Assert.LessOrEqual(x, y, message), ComparisonType.LessOrEqual)
                .Verify((x, y) => Assert.Greater(x, y, message), ComparisonType.Greater)
                .Verify((x, y) => Assert.GreaterOrEqual(x, y, message), ComparisonType.GreaterOrEqual);

            comparisonBehavior
                .If<uint>()
                .WithAssertionException
                .Verify(Assert.Less, ComparisonType.Less)
                .Verify(Assert.LessOrEqual, ComparisonType.LessOrEqual)
                .Verify(Assert.Greater, ComparisonType.Greater)
                .Verify(Assert.GreaterOrEqual, ComparisonType.GreaterOrEqual)
                .Verify((x, y) => Assert.Less(x, y, message), ComparisonType.Less)
                .Verify((x, y) => Assert.LessOrEqual(x, y, message), ComparisonType.LessOrEqual)
                .Verify((x, y) => Assert.Greater(x, y, message), ComparisonType.Greater)
                .Verify((x, y) => Assert.GreaterOrEqual(x, y, message), ComparisonType.GreaterOrEqual);

            comparisonBehavior
                .If<long>()
                .WithAssertionException
                .Verify(Assert.Less, ComparisonType.Less)
                .Verify(Assert.LessOrEqual, ComparisonType.LessOrEqual)
                .Verify(Assert.Greater, ComparisonType.Greater)
                .Verify(Assert.GreaterOrEqual, ComparisonType.GreaterOrEqual)
                .Verify((x, y) => Assert.Less(x, y, message), ComparisonType.Less)
                .Verify((x, y) => Assert.LessOrEqual(x, y, message), ComparisonType.LessOrEqual)
                .Verify((x, y) => Assert.Greater(x, y, message), ComparisonType.Greater)
                .Verify((x, y) => Assert.GreaterOrEqual(x, y, message), ComparisonType.GreaterOrEqual);

            comparisonBehavior
                .If<ulong>()
                .WithAssertionException
                .Verify(Assert.Less, ComparisonType.Less)
                .Verify(Assert.LessOrEqual, ComparisonType.LessOrEqual)
                .Verify(Assert.Greater, ComparisonType.Greater)
                .Verify(Assert.GreaterOrEqual, ComparisonType.GreaterOrEqual)
                .Verify((x, y) => Assert.Less(x, y, message), ComparisonType.Less)
                .Verify((x, y) => Assert.LessOrEqual(x, y, message), ComparisonType.LessOrEqual)
                .Verify((x, y) => Assert.Greater(x, y, message), ComparisonType.Greater)
                .Verify((x, y) => Assert.GreaterOrEqual(x, y, message), ComparisonType.GreaterOrEqual);

            comparisonBehavior
                .If<decimal>()
                .WithAssertionException
                .Verify(Assert.Less, ComparisonType.Less)
                .Verify(Assert.LessOrEqual, ComparisonType.LessOrEqual)
                .Verify(Assert.Greater, ComparisonType.Greater)
                .Verify(Assert.GreaterOrEqual, ComparisonType.GreaterOrEqual)
                .Verify((x, y) => Assert.Less(x, y, message), ComparisonType.Less)
                .Verify((x, y) => Assert.LessOrEqual(x, y, message), ComparisonType.LessOrEqual)
                .Verify((x, y) => Assert.Greater(x, y, message), ComparisonType.Greater)
                .Verify((x, y) => Assert.GreaterOrEqual(x, y, message), ComparisonType.GreaterOrEqual);

            comparisonBehavior
                .If<double>()
                .WithAssertionException
                .Verify(Assert.Less, ComparisonType.Less)
                .Verify(Assert.LessOrEqual, ComparisonType.LessOrEqual)
                .Verify(Assert.Greater, ComparisonType.Greater)
                .Verify(Assert.GreaterOrEqual, ComparisonType.GreaterOrEqual)
                .Verify((x, y) => Assert.Less(x, y, message), ComparisonType.Less)
                .Verify((x, y) => Assert.LessOrEqual(x, y, message), ComparisonType.LessOrEqual)
                .Verify((x, y) => Assert.Greater(x, y, message), ComparisonType.Greater)
                .Verify((x, y) => Assert.GreaterOrEqual(x, y, message), ComparisonType.GreaterOrEqual);

            comparisonBehavior
                .If<float>()
                .WithAssertionException
                .Verify(Assert.Less, ComparisonType.Less)
                .Verify(Assert.LessOrEqual, ComparisonType.LessOrEqual)
                .Verify(Assert.Greater, ComparisonType.Greater)
                .Verify(Assert.GreaterOrEqual, ComparisonType.GreaterOrEqual)
                .Verify((x, y) => Assert.Less(x, y, message), ComparisonType.Less)
                .Verify((x, y) => Assert.LessOrEqual(x, y, message), ComparisonType.LessOrEqual)
                .Verify((x, y) => Assert.Greater(x, y, message), ComparisonType.Greater)
                .Verify((x, y) => Assert.GreaterOrEqual(x, y, message), ComparisonType.GreaterOrEqual);

            comparisonBehavior
                .If<IComparable>()
                .WithAssertionException
                .Verify(Assert.Less, ComparisonType.Less)
                .Verify(Assert.LessOrEqual, ComparisonType.LessOrEqual)
                .Verify(Assert.Greater, ComparisonType.Greater)
                .Verify(Assert.GreaterOrEqual, ComparisonType.GreaterOrEqual)
                .Verify((x, y) => Assert.Less(x, y, message), ComparisonType.Less)
                .Verify((x, y) => Assert.LessOrEqual(x, y, message), ComparisonType.LessOrEqual)
                .Verify((x, y) => Assert.Greater(x, y, message), ComparisonType.Greater)
                .Verify((x, y) => Assert.GreaterOrEqual(x, y, message), ComparisonType.GreaterOrEqual);

            comparisonBehavior
                .WithAssertionException
                .Verify((x, y) => CollectionAssert.AreEqual(new[] { x }, new[] { y }), ComparisonType.Equal)
                .Verify((x, y) => CollectionAssert.AreNotEqual(new[] { x }, new[] { y }), ComparisonType.Unequal)
                .Verify((x, y) => CollectionAssert.AreEquivalent(new[] { x }, new[] { y }), ComparisonType.Equal)
                .Verify((x, y) => CollectionAssert.AreNotEquivalent(new[] { x }, new[] { y }), ComparisonType.Unequal)
                .Verify((x, y) => CollectionAssert.Contains(new[] { x }, y), ComparisonType.Equal)
                .Verify((x, y) => CollectionAssert.DoesNotContain(new[] { x }, y), ComparisonType.Unequal)
                .Verify((x, y) => CollectionAssert.IsSubsetOf(new[] { x }, new[] { y }), ComparisonType.Equal)
                .Verify((x, y) => CollectionAssert.IsNotSubsetOf(new[] { x }, new[] { y }), ComparisonType.Unequal)
                .Verify((x, y) => CollectionAssert.IsSupersetOf(new[] { x }, new[] { y }), ComparisonType.Equal)
                .Verify((x, y) => CollectionAssert.IsNotSupersetOf(new[] { x }, new[] { y }), ComparisonType.Unequal)
                .Verify((x, y) => CollectionAssert.IsOrdered(new[] { x, y }), ComparisonType.LessOrEqual);
        }
    }
}
