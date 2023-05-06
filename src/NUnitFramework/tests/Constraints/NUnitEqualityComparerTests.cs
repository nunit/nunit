// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using NUnit.TestUtilities;

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class NUnitEqualityComparerTests
    {
        private Tolerance _tolerance;
        private NUnitEqualityComparer _comparer;

        [SetUp]
        public void Setup()
        {
            _tolerance = Tolerance.Default;
            _comparer = new NUnitEqualityComparer();
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
            Assert.That(_comparer.AreEqual(x, y, ref _tolerance));
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
            Assert.That(_comparer.AreEqual(greater, lesser, ref _tolerance), Is.False);
            Assert.That(_comparer.AreEqual(lesser, greater, ref _tolerance), Is.False);
        }

        [TestCase(double.PositiveInfinity, double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity, double.NegativeInfinity)]
        [TestCase(double.NaN, double.NaN)]
        [TestCase(float.PositiveInfinity, float.PositiveInfinity)]
        [TestCase(float.NegativeInfinity, float.NegativeInfinity)]
        [TestCase(float.NaN, float.NaN)]
        public void SpecialFloatingPointValuesCompareAsEqual(object x, object y)
        {
            Assert.That(_comparer.AreEqual(x, y, ref _tolerance));
        }

        [Test]
        public void SameDirectoriesAreEqual()
        {
            using (var testDir = new TestDirectory())
            {
                var one = new DirectoryInfo(testDir.Directory.FullName);
                var two = new DirectoryInfo(testDir.Directory.FullName);
                Assert.That(_comparer.AreEqual(one, two, ref _tolerance));
            }
        }

        [Test]
        public void DifferentDirectoriesAreNotEqual()
        {
            using (var one = new TestDirectory())
            using (var two = new TestDirectory())
            {
                Assert.That(_comparer.AreEqual(one, two, ref _tolerance), Is.False);
            }
        }

        [Test]
        public void CanCompareArrayContainingSelfToSelf()
        {
            object[] array = new object[1];
            array[0] = array;

            Assert.That(_comparer.AreEqual(array, array, ref _tolerance), Is.True);
        }

        [Test]
        public void IEquatableSuccess()
        {
            IEquatableWithoutEqualsOverridden x = new IEquatableWithoutEqualsOverridden(1);
            IEquatableWithoutEqualsOverridden y = new IEquatableWithoutEqualsOverridden(1);

            Assert.That(_comparer.AreEqual(x, y, ref _tolerance), Is.True);
        }

        [Test]
        public void IEquatableDifferentTypesSuccess_WhenActualImplementsIEquatable()
        {
            int x = 1;
            Int32IEquatable y = new Int32IEquatable(1);

            // y.Equals(x) is what gets actually called
            // TODO: This should work both ways
            Assert.That(_comparer.AreEqual(x, y, ref _tolerance), Is.True);
        }

        [Test]
        public void IEquatableDifferentTypesSuccess_WhenExpectedImplementsIEquatable()
        {
            int x = 1;
            Int32IEquatable y = new Int32IEquatable(1);

            // y.Equals(x) is what gets actually called
            // TODO: This should work both ways
            Assert.That(_comparer.AreEqual(y, x, ref _tolerance), Is.True);
        }

        [Test]
        public void IEquatableSuccess_WhenExplicitInterfaceImplementation()
        {
            IEquatableWithExplicitImplementation x = new IEquatableWithExplicitImplementation(1);
            IEquatableWithExplicitImplementation y = new IEquatableWithExplicitImplementation(1);

            Assert.That(_comparer.AreEqual(x, y, ref _tolerance), Is.True);
        }

        [Test]
        public void IEquatableFailure_WhenExplicitInterfaceImplementationWithDifferentValues()
        {
            IEquatableWithExplicitImplementation x = new IEquatableWithExplicitImplementation(1);
            IEquatableWithExplicitImplementation y = new IEquatableWithExplicitImplementation(2);

            Assert.That(_comparer.AreEqual(x, y, ref _tolerance), Is.False);
        }

        [Test]
        public void ReferenceEqualityHasPrecedenceOverIEquatable()
        {
            NeverEqualIEquatable z = new NeverEqualIEquatable();

            Assert.That(_comparer.AreEqual(z, z, ref _tolerance), Is.True);
        }

        [Test]
        public void IEquatableHasPrecedenceOverDefaultEquals()
        {
            NeverEqualIEquatableWithOverriddenAlwaysTrueEquals x = new NeverEqualIEquatableWithOverriddenAlwaysTrueEquals();
            NeverEqualIEquatableWithOverriddenAlwaysTrueEquals y = new NeverEqualIEquatableWithOverriddenAlwaysTrueEquals();

            Assert.That(_comparer.AreEqual(x, y, ref _tolerance), Is.False);
        }

        [Test]
        public void IEquatableHasPrecedenceOverEnumerableEquals()
        {
            var x = new EquatableWithEnumerableObject<int>(new[] { 1, 2, 3, 4, 5 }, 42);
            var y = new EnumerableObject<int>(new[] { 5, 4, 3, 2, 1 }, 42);
            var z = new EnumerableObject<int>(new[] { 1, 2, 3, 4, 5 }, 15);

            Assert.That(_comparer.AreEqual(x, y, ref _tolerance), Is.True);
            Assert.That(_comparer.AreEqual(y, x, ref _tolerance), Is.True);
            Assert.That(_comparer.AreEqual(x, z, ref _tolerance), Is.False);
            Assert.That(_comparer.AreEqual(z, x, ref _tolerance), Is.False);

            Assert.That(y, Is.EqualTo(x));
            Assert.That(x, Is.EqualTo(y));
            Assert.That(z, Is.Not.EqualTo(x));
            Assert.That(x, Is.Not.EqualTo(z));
        }

        [Test]
        public void IEquatableIsIgnoredAndEnumerableEqualsUsedWithAsCollection()
        {
            _comparer.CompareAsCollection = true;

            var x = new EquatableWithEnumerableObject<int>(new[] { 1, 2, 3, 4, 5 }, 42);
            var y = new EnumerableObject<int>(new[] { 5, 4, 3, 2, 1 }, 42);
            var z = new EnumerableObject<int>(new[] { 1, 2, 3, 4, 5 }, 15);

            Assert.That(_comparer.AreEqual(x, y, ref _tolerance), Is.False);
            Assert.That(_comparer.AreEqual(y, x, ref _tolerance), Is.False);
            Assert.That(_comparer.AreEqual(x, z, ref _tolerance), Is.True);
            Assert.That(_comparer.AreEqual(z, x, ref _tolerance), Is.True);

            Assert.That(y, Is.Not.EqualTo(x).AsCollection);
            Assert.That(x, Is.Not.EqualTo(y).AsCollection);
            Assert.That(z, Is.EqualTo(x).AsCollection);
            Assert.That(x, Is.EqualTo(z).AsCollection);
        }

        [Test]
        public void InheritingAndOverridingIEquatable()
        {
            var obj1 = new InheritingEquatableObject { SomeProperty = 1, OtherProperty = 2 };
            var obj2 = new InheritingEquatableObject { SomeProperty = 1, OtherProperty = 2 };
            var obj3 = new InheritingEquatableObject { SomeProperty = 1, OtherProperty = 3 };
            var obj4 = new InheritingEquatableObject { SomeProperty = 4, OtherProperty = 2 };

            Assert.That(obj1, Is.EqualTo(obj2));
            Assert.That(obj1, Is.Not.EqualTo(obj3));
            Assert.That(obj1, Is.Not.EqualTo(obj4));

            var n = new NUnitEqualityComparer();
            var tolerance = Tolerance.Exact;
            Assert.That(n.AreEqual(obj1, obj2, ref tolerance), Is.True);
            Assert.That(n.AreEqual(obj2, obj1, ref tolerance), Is.True);
            Assert.That(n.AreEqual(obj1, obj3, ref tolerance), Is.False);
            Assert.That(n.AreEqual(obj3, obj1, ref tolerance), Is.False);
            Assert.That(n.AreEqual(obj1, obj4, ref tolerance), Is.False);
            Assert.That(n.AreEqual(obj4, obj1, ref tolerance), Is.False);
        }

        [Test]
        public void ImplementingIEquatableDirectlyOnTheClass()
        {
            var obj1 = new EquatableObject { SomeProperty = 1 };
            var obj2 = new EquatableObject { SomeProperty = 1 };

            var n = new NUnitEqualityComparer();
            var tolerance = Tolerance.Exact;
            Assert.That(n.AreEqual(obj1, obj2, ref tolerance), Is.True);
            Assert.That(n.AreEqual(obj2, obj1, ref tolerance), Is.True);
        }

        [Test]
        public void ImplementingIEquatableOnABaseClassOrInterface()
        {
            var obj1 = new InheritedEquatableObject { SomeProperty = 1 };
            var obj2 = new InheritedEquatableObject { SomeProperty = 1 };

            var n = new NUnitEqualityComparer();
            var tolerance = Tolerance.Exact;
            Assert.That(n.AreEqual(obj1, obj2, ref tolerance), Is.True);
            Assert.That(n.AreEqual(obj2, obj1, ref tolerance), Is.True);
        }

        [Test]
        public void ImplementingIEquatableOnABaseClassOrInterfaceThroughInterface()
        {
            IEquatableObject obj1 = new InheritedEquatableObject { SomeProperty = 1 };
            IEquatableObject obj2 = new InheritedEquatableObject { SomeProperty = 1 };

            var n = new NUnitEqualityComparer();
            var tolerance = Tolerance.Exact;
            Assert.That(n.AreEqual(obj1, obj2, ref tolerance), Is.True);
            Assert.That(n.AreEqual(obj2, obj1, ref tolerance), Is.True);
        }

        [Test]
        public void CanHandleMultipleImplementationsOfIEquatable()
        {
            IEquatableObject obj1 = new InheritedEquatableObject { SomeProperty = 1 };
            IEquatableObject obj2 = new MultipleIEquatables { SomeProperty = 1 };
            var obj3 = new EquatableObject { SomeProperty = 1 };

            var n = new NUnitEqualityComparer();
            var tolerance = Tolerance.Exact;
            Assert.That(n.AreEqual(obj1, obj2, ref tolerance), Is.True);
            Assert.That(n.AreEqual(obj2, obj1, ref tolerance), Is.True);
            Assert.That(n.AreEqual(obj1, obj3, ref tolerance), Is.False);
            Assert.That(n.AreEqual(obj3, obj1, ref tolerance), Is.False);
            Assert.That(n.AreEqual(obj2, obj3, ref tolerance), Is.True);
            Assert.That(n.AreEqual(obj3, obj2, ref tolerance), Is.True);
        }

        [Test]
        public void IEnumeratorIsDisposed()
        {
            var enumeration = new EnumerableWithDisposeChecks<int>(new[] { 0, 1, 2, 3 });
            Assert.That(_comparer.AreEqual(enumeration, enumeration, ref _tolerance), Is.True);
            Assert.That(enumeration.EnumeratorsDisposed);
        }

        [TestCaseSource(nameof(GetRecursiveContainsTestCases))]
        public void SelfContainedItemFoundInCollection<T>(T x, ICollection y)
        {
            var equalityComparer = new NUnitEqualityComparer();
            var tolerance = Tolerance.Default;
            var equality = equalityComparer.AreEqual(x, y, ref tolerance);

            Assert.That(equality, Is.False);
            Assert.That(y, Contains.Item(x));
            Assert.That(y, Does.Contain(x));
        }

        [TestCaseSource(nameof(GetRecursiveComparerTestCases))]
        public void SelfContainedItemDoesntRecurseForever<T>(T x, ICollection y)
        {
            var equalityComparer = new NUnitEqualityComparer();
            var tolerance = Tolerance.Default;
            equalityComparer.ExternalComparers.Add(new DetectRecursionComparer(30));

            Assert.DoesNotThrow(() => equalityComparer.AreEqual(x, y, ref tolerance));
        }

        [Test]
        public void SelfContainedDuplicateItemsAreCompared()
        {
            var equalityComparer = new NUnitEqualityComparer();
            var equalInstance1 = new[] { 1 };
            var equalInstance2 = new[] { 1 };

            var x = new[] { equalInstance1, equalInstance1 };
            var y = new[] { equalInstance2, equalInstance2 };

            Assert.That(equalityComparer.AreEqual(x, y, ref _tolerance), Is.True);
        }

        private static IEnumerable<TestCaseData> GetRecursiveComparerTestCases()
        {
            // Separate from 'GetRecursiveContainsTestCases' until a stackoverflow issue in
            // 'MsgUtils.FormatValue()' can be fixed for the below cases
            foreach (var testCase in GetRecursiveContainsTestCases())
                yield return testCase;

            var dict = new Dictionary<object, object>();
            var dictItem = "nunit";

            dict[1] = dictItem;
            dict[2] = dict;

            yield return new TestCaseData(dictItem, dict);
        }

        private static IEnumerable<TestCaseData> GetRecursiveContainsTestCases()
        {
            var enumerable = new SelfContainer();
            var enumerableContainer = new[] { new SelfContainer(), enumerable };

            yield return new TestCaseData(enumerable, enumerableContainer);

            object itemB = 1;
            object[] itemBSet = new object[2];
            itemBSet[0] = itemB;
            itemBSet[1] = itemBSet;

            yield return new TestCaseData(itemB, itemBSet);
        }
    }

    internal class DetectRecursionComparer : EqualityAdapter
    {
        private readonly int _maxRecursion;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public DetectRecursionComparer(int maxRecursion)
        {
            var callerDepth = new StackTrace().FrameCount - 1;
            _maxRecursion = callerDepth + maxRecursion;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public override bool CanCompare(object x, object y)
        {
            var currentDepth = new StackTrace().FrameCount - 1;
            return currentDepth >= _maxRecursion;
        }

        public override bool AreEqual(object x, object y)
        {
            throw new InvalidOperationException("Recurses");
        }
    }

    internal class SelfContainer : IEnumerable
    {
        public IEnumerator GetEnumerator() { yield return this; }
    }

    internal class EnumerableWithDisposeChecks<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> _data;
        private readonly List<DisposableEnumerator<T>> _enumerators = new List<DisposableEnumerator<T>>();

        public EnumerableWithDisposeChecks(T[] data)
        {
            _data = data;
        }

        public bool EnumeratorsDisposed
        {
            get
            {
                foreach (var disposableEnumerator in _enumerators)
                {
                    if (!disposableEnumerator.Disposed)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            var enumerator = new DisposableEnumerator<T>(_data.GetEnumerator());
            _enumerators.Add(enumerator);
            return enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
        }
    }

    internal class DisposableEnumerator<T> : IEnumerator<T>
    {
        private bool _disposedValue = false;
        private readonly IEnumerator<T> _enumerator;

        public DisposableEnumerator(IEnumerator<T> enumerator)
        {
            _enumerator = enumerator;
        }

        public bool Disposed => _disposedValue;

        public T Current => _enumerator.Current;

        object? IEnumerator.Current => _enumerator.Current;

        public bool MoveNext()
        {
            return _enumerator.MoveNext();
        }

        public void Reset()
        {
            _enumerator.Reset();
        }

        public void Dispose()
        {
            _disposedValue = true;
        }

    }

    public class NeverEqualIEquatableWithOverriddenAlwaysTrueEquals : IEquatable<NeverEqualIEquatableWithOverriddenAlwaysTrueEquals>
    {
        public bool Equals(NeverEqualIEquatableWithOverriddenAlwaysTrueEquals? other)
        {
            return false;
        }

        public override bool Equals(object? obj)
        {
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class Int32IEquatable : IEquatable<int>
    {
        private readonly int _value;

        public Int32IEquatable(int value)
        {
            _value = value;
        }

        public bool Equals(int other)
        {
            return _value.Equals(other);
        }
    }

    public class NeverEqualIEquatable : IEquatable<NeverEqualIEquatable>
    {
        public bool Equals(NeverEqualIEquatable? other)
        {
            return false;
        }
    }

    public class IEquatableWithoutEqualsOverridden : IEquatable<IEquatableWithoutEqualsOverridden>
    {
        private readonly int _value;

        public IEquatableWithoutEqualsOverridden(int value)
        {
            _value = value;
        }

        public bool Equals(IEquatableWithoutEqualsOverridden? other)
        {
            return other is not null && _value.Equals(other._value);
        }
    }

    public class IEquatableWithExplicitImplementation : IEquatable<IEquatableWithExplicitImplementation>
    {
        private readonly int _value;

        public IEquatableWithExplicitImplementation(int value)
        {
            _value = value;
        }

        bool IEquatable<IEquatableWithExplicitImplementation>.Equals(IEquatableWithExplicitImplementation? other)
        {
            return other is not null && _value.Equals(other._value);
        }
    }

    public class EquatableObject : IEquatable<EquatableObject>
    {
        public int SomeProperty { get; set; }
        public bool Equals(EquatableObject? other)
        {
            if (other is null)
                return false;

            return SomeProperty == other.SomeProperty;
        }
    }

    public class InheritingEquatableObject : EquatableObject, IEquatable<InheritingEquatableObject>
    {
        public int OtherProperty { get; set; }
        public bool Equals(InheritingEquatableObject? other)
        {
            if (other is null)
                return false;

            return OtherProperty == other.OtherProperty && Equals((EquatableObject) other);
        }
    }

    public interface IEquatableObject : IEquatable<IEquatableObject>
    {
        int SomeProperty { get; set; }
    }

    public class InheritedEquatableObject : IEquatableObject
    {
        public int SomeProperty { get; set; }

        public bool Equals(IEquatableObject? other)
        {
            if (other is null)
                return false;

            return SomeProperty == other.SomeProperty;
        }
    }

    public class MultipleIEquatables : InheritedEquatableObject, IEquatable<EquatableObject>
    {
        public bool Equals(EquatableObject? other)
        {
            if (other is null)
                return false;

            return SomeProperty == other.SomeProperty;
        }
    }

    public class EnumerableObject<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> _enumerable;

        public EnumerableObject(IEnumerable<T> enumerable, int someProperty)
        {
            _enumerable = enumerable;
            SomeProperty = someProperty;
        }

        public int SomeProperty { get; }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _enumerable.GetEnumerator();
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class EquatableWithEnumerableObject<T> : IEnumerable<T>, IEquatable<EnumerableObject<T>>
    {
        private readonly IEnumerable<T> _enumerable;

        public EquatableWithEnumerableObject(IEnumerable<T> enumerable, int otherProperty)
        {
            _enumerable = enumerable;
            OtherProperty = otherProperty;
        }

        public int OtherProperty { get; }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _enumerable.GetEnumerator();
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(EnumerableObject<T>? other)
        {
            if (other is null)
                return false;

            return OtherProperty == other.SomeProperty;
        }
    }
}
