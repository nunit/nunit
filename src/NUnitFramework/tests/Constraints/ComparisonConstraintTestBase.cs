// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Constraints;
using NUnit.Framework.Tests.TestUtilities.Comparers;

namespace NUnit.Framework.Tests.Constraints
{
    #region ComparisonConstraintTestBase

    public abstract class ComparisonConstraintTestBase : ConstraintTestBase
    {
        protected ComparisonConstraint ComparisonConstraint => (ComparisonConstraint)TheConstraint;

        [TestCase(null)]
        [TestCase("xxx")]
        public void InvalidDataThrowsArgumentException(object? data)
        {
            Assert.Throws<ArgumentException>(() => TheConstraint.ApplyTo(data));
        }

        [Test]
        public void UsesProvidedIComparer()
        {
            var comparer = new ObjectComparer();
            ComparisonConstraint.Using(comparer).ApplyTo(0);
            Assert.That(comparer.WasCalled, "Comparer was not called");
        }

        [Test]
        public void UsesProvidedGenericComparer()
        {
            var comparer = new GenericComparer<int>();
            ComparisonConstraint.Using(comparer).ApplyTo(0);
            Assert.That(comparer.WasCalled, "Comparer was not called");
        }

        [Test]
        public void UsesProvidedGenericComparison()
        {
            var comparer = new GenericComparison<int>();
            ComparisonConstraint.Using(comparer.Delegate).ApplyTo(0);
            Assert.That(comparer.WasCalled, "Comparer was not called");
        }

        [Test]
        public void UsesProvidedLambda()
        {
            Comparison<int> comparer = (x, y) => x.CompareTo(y);
            ComparisonConstraint.Using(comparer).ApplyTo(0);
        }
    }

    #endregion

    #region Comparison Test Classes

    internal class ClassWithIComparable : IComparable
    {
        private readonly int _val;

        public ClassWithIComparable(int val)
        {
            _val = val;
        }

        public int CompareTo(object? x)
        {
            if (x is ClassWithIComparable other)
                return _val.CompareTo(other._val);

            throw new ArgumentException();
        }
    }

    internal class ClassWithIComparableOfT : IComparable<ClassWithIComparableOfT>, IComparable<int>
    {
        private readonly int _val;

        public ClassWithIComparableOfT(int val)
        {
            _val = val;
        }

        public int CompareTo(ClassWithIComparableOfT? other)
        {
            if (other is null)
                return 1;
            return _val.CompareTo(other._val);
        }

        public int CompareTo(int other)
        {
            return _val.CompareTo(other);
        }
    }

    #endregion
}
