// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class NUnitComparerTests
    {
        private NUnitComparer _comparer;

        [SetUp]
        public void SetUp()
        {
            _comparer = new NUnitComparer();
        }

        [TestCase(4, 4)]
        [TestCase(4.0d, 4.0d)]
        [TestCase(4.0f, 4.0f)]
        [TestCase(4, 4.0d)]
        [TestCase(4, 4.0f)]
        [TestCase(4.0d, 4)]
        [TestCase(4.0d, 4.0f)]
        [TestCase(4.0f, 4)]
        [TestCase(4.0f, 4.0d)]
        [TestCase(null, null)]
        [TestCase((char)4, (char)4)]
        [TestCase((char)4, 4)]
        [TestCase(4, (char)4)]
        public void EqualItems(object x, object y)
        {
            Assert.That(_comparer.Compare(x, y), Is.EqualTo(0));
        }

        [TestCase(4, 2)]
        [TestCase(4.0d, 2.0d)]
        [TestCase(4.0f, 2.0f)]
        [TestCase(4, 2.0d)]
        [TestCase(4, 2.0f)]
        [TestCase(4.0d, 2)]
        [TestCase(4.0d, 2.0f)]
        [TestCase(4.0f, 2)]
        [TestCase(4.0f, 2.0d)]
        [TestCase(4, null)]
        [TestCase((char)4, (char)2)]
        [TestCase((char)4, 2)]
        [TestCase(4, (char)2)]
        public void UnequalItems(object greater, object lesser)
        {
            Assert.Multiple(() =>
            {
                Assert.That(_comparer.Compare(greater, lesser), Is.GreaterThan(0));
                Assert.That(_comparer.Compare(lesser, greater), Is.LessThan(0));
            });
        }

        [Test]
        public void Comparables()
        {
            var greater = new ClassWithIComparable(42);
            var lesser = new ClassWithIComparable(-42);

            Assert.Multiple(() =>
            {
                Assert.That(_comparer.Compare(greater, lesser), Is.GreaterThan(0));
                Assert.That(_comparer.Compare(lesser, greater), Is.LessThan(0));
            });
        }

        [Test]
        public void ComparablesOfT()
        {
            var greater = new ClassWithIComparableOfT(42);
            var lesser = new ClassWithIComparableOfT(-42);

            Assert.Multiple(() =>
            {
                Assert.That(_comparer.Compare(greater, lesser), Is.GreaterThan(0));
                Assert.That(_comparer.Compare(lesser, greater), Is.LessThan(0));
            });
        }

        [Test]
        public void ComparablesOfInt1()
        {
            int greater = 42;
            var lesser = new ClassWithIComparableOfT(-42);

            Assert.Multiple(() =>
            {
                Assert.That(_comparer.Compare(greater, lesser), Is.GreaterThan(0));
                Assert.That(_comparer.Compare(lesser, greater), Is.LessThan(0));
            });
        }

        [Test]
        public void ComparablesOfInt2()
        {
            var greater = new ClassWithIComparableOfT(42);
            int lesser = -42;

            Assert.Multiple(() =>
            {
                Assert.That(_comparer.Compare(greater, lesser), Is.GreaterThan(0));
                Assert.That(_comparer.Compare(lesser, greater), Is.LessThan(0));
            });
        }

        [Test]
        public void ComparablesOfInt3()
        {
            short greater = 42;
            var lesser = new ClassWithIComparableOfT(-42);

            Assert.Multiple(() =>
            {
                Assert.That(_comparer.Compare(greater, lesser), Is.GreaterThan(0));
                Assert.That(_comparer.Compare(lesser, greater), Is.LessThan(0));
            });
        }

        #region Comparison Test Classes

        private class ClassWithIComparable : IComparable
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

        private class ClassWithIComparableOfT : IComparable<ClassWithIComparableOfT>, IComparable<int>
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
}
