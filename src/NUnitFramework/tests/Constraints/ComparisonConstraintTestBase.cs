// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections;
using NUnit.TestUtilities.Comparers;

namespace NUnit.Framework.Constraints
{
    #region ComparisonConstraintTestBase

    public abstract class ComparisonConstraintTestBase : ConstraintTestBase
    {
        protected ComparisonConstraint ComparisonConstraint;

        [TestCase(null)]
        [TestCase("xxx")]
        public void InvalidDataThrowsArgumentException(object data)
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

    class ClassWithIComparable : IComparable
    {
        private readonly int val;

        public ClassWithIComparable(int val)
        {
            this.val = val;
        }

        public int CompareTo(object x)
        {
            ClassWithIComparable other = x as ClassWithIComparable;
            if (x is ClassWithIComparable)
                return val.CompareTo(other.val);

            throw new ArgumentException();
        }
    }

    class ClassWithIComparableOfT : IComparable<ClassWithIComparableOfT>, IComparable<int>
    {
        private readonly int val;

        public ClassWithIComparableOfT(int val)
        {
            this.val = val;
        }

        public int CompareTo(ClassWithIComparableOfT other)
        {
            return val.CompareTo(other.val);
        }

        public int CompareTo(int other)
        {
            return val.CompareTo(other);
        }
    }

    #endregion
}
