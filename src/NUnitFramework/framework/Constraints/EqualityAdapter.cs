// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// EqualityAdapter class handles all equality comparisons
    /// that use an <see cref="IEqualityComparer"/>, <see cref="IEqualityComparer{T}"/>
    /// or a <see cref="ComparisonAdapter"/>.
    /// </summary>
    public abstract class EqualityAdapter
    {
        /// <summary>
        /// Compares two objects, returning true if they are equal
        /// </summary>
        public abstract bool AreEqual(object? x, object? y);

        /// <summary>
        /// Compares two objects, within a tolerance returning true if they are equal
        /// </summary>
        public virtual bool AreEqual(object? x, object? y, ref Tolerance tolerance)
        {
            // For backwards compatibility all existing equality operators can continue
            // to use the existing AreEqual.  Attempting to use a tolerance when it's
            // not supported is now thrown to the caller, rather than silently ignored.
            if (!tolerance.HasVariance)
            {
                return AreEqual(x, y);
            }
            else
            {
                throw new InvalidOperationException("Tolerance is not supported for this comparison");
            }
        }

        /// <summary>
        /// Returns true if the two objects can be compared by this adapter.
        /// The base adapter cannot handle IEnumerables except for strings.
        /// </summary>
        public virtual bool CanCompare(object? x, object? y)
        {
            return (x is string || x is not IEnumerable) &&
                   (y is string || y is not IEnumerable);
        }

        #region Nested IComparer Adapter

        /// <summary>
        /// Returns an <see cref="EqualityAdapter"/> that wraps an <see cref="IComparer"/>.
        /// </summary>
        public static EqualityAdapter For(IComparer comparer)
        {
            return new ComparerAdapter(comparer);
        }

        /// <summary>
        /// <see cref="EqualityAdapter"/> that wraps an <see cref="IComparer"/>.
        /// </summary>
        private class ComparerAdapter : EqualityAdapter
        {
            private readonly IComparer _comparer;

            public ComparerAdapter(IComparer comparer)
            {
                _comparer = comparer;
            }

            public override bool AreEqual(object? x, object? y)
            {
                return _comparer.Compare(x, y) == 0;
            }
        }

        #endregion

        #region Nested IEqualityComparer Adapter

        /// <summary>
        /// Returns an <see cref="EqualityAdapter"/> that wraps an <see cref="IEqualityComparer"/>.
        /// </summary>
        public static EqualityAdapter For(IEqualityComparer comparer)
        {
            return new EqualityComparerAdapter(comparer);
        }

        private class EqualityComparerAdapter : EqualityAdapter
        {
            private readonly IEqualityComparer _comparer;

            public EqualityComparerAdapter(IEqualityComparer comparer)
            {
                _comparer = comparer;
            }

            public override bool AreEqual(object? x, object? y)
            {
                return _comparer.Equals(x, y);
            }
        }

        /// <summary>
        /// Returns an EqualityAdapter that uses a predicate function for items comparison.
        /// </summary>
        /// <typeparam name="TExpected"></typeparam>
        /// <typeparam name="TActual"></typeparam>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public static EqualityAdapter For<TExpected, TActual>(Func<TExpected, TActual, bool> comparison)
        {
            return new PredicateEqualityAdapter<TExpected, TActual>(comparison);
        }

        internal sealed class PredicateEqualityAdapter<TActual, TExpected> : EqualityAdapter
        {
            private readonly Func<TActual, TExpected, bool> _comparison;

            /// <summary>
            /// Returns true if the two objects can be compared by this adapter.
            /// The base adapter cannot handle IEnumerables except for strings.
            /// </summary>
            public override bool CanCompare(object? x, object? y)
            {
                return TypeHelper.CanCast<TExpected>(x) && TypeHelper.CanCast<TActual>(y);
            }

            /// <summary>
            /// Compares two objects, returning true if they are equal
            /// </summary>
            public override bool AreEqual(object? x, object? y)
            {
                // The CanCompare method ensures that the casts will succeed,
                // so we can safely cast here.
                // The null-forgiving operator is used here because the generic predicate is unconstrained.
                return _comparison.Invoke((TActual)y!, (TExpected)x!);
            }

            public PredicateEqualityAdapter(Func<TActual, TExpected, bool> comparison)
            {
                _comparison = comparison;
            }
        }

        #endregion

        #region Nested GenericEqualityAdapter<T>

        private abstract class GenericEqualityAdapter<T> : EqualityAdapter
        {
            /// <summary>
            /// Returns true if the two objects can be compared by this adapter.
            /// Generic adapter requires objects of the specified type.
            /// </summary>
            public override bool CanCompare(object? x, object? y)
            {
                return TypeHelper.CanCast<T>(x) && TypeHelper.CanCast<T>(y);
            }
        }

        #endregion

        #region Nested IEqualityComparer<T> Adapter

        /// <summary>
        /// Returns an <see cref="EqualityAdapter"/> that wraps an <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        public static EqualityAdapter For<T>(IEqualityComparer<T> comparer)
        {
            return new EqualityComparerAdapter<T>(comparer);
        }

        private class EqualityComparerAdapter<T> : GenericEqualityAdapter<T>
        {
            private readonly IEqualityComparer<T> _comparer;

            public EqualityComparerAdapter(IEqualityComparer<T> comparer)
            {
                _comparer = comparer;
            }

            public override bool AreEqual(object? x, object? y)
            {
                // The CanCompare method ensures that the casts will succeed,
                // so we can safely cast here.
                return _comparer.Equals((T?)x, (T?)y);
            }
        }

        #endregion

        #region Nested IComparer<T> Adapter

        /// <summary>
        /// Returns an <see cref="EqualityAdapter"/> that wraps an <see cref="IComparer{T}"/>.
        /// </summary>
        public static EqualityAdapter For<T>(IComparer<T> comparer)
        {
            return new ComparerAdapter<T>(comparer);
        }

        /// <summary>
        /// <see cref="EqualityAdapter"/> that wraps an <see cref="IComparer"/>.
        /// </summary>
        private class ComparerAdapter<T> : GenericEqualityAdapter<T>
        {
            private readonly IComparer<T> _comparer;

            public ComparerAdapter(IComparer<T> comparer)
            {
                _comparer = comparer;
            }

            public override bool AreEqual(object? x, object? y)
            {
                // The CanCompare method ensures that the casts will succeed,
                // so we can safely cast here.
                return _comparer.Compare((T?)x, (T?)y) == 0;
            }
        }

        #endregion

        #region Nested Comparison<T> Adapter

        /// <summary>
        /// Returns an <see cref="EqualityAdapter"/> that wraps a <see cref="Comparison{T}"/>.
        /// </summary>
        public static EqualityAdapter For<T>(Comparison<T> comparer)
        {
            return new ComparisonAdapter<T>(comparer);
        }

        private class ComparisonAdapter<T> : GenericEqualityAdapter<T>
        {
            private readonly Comparison<T> _comparer;

            public ComparisonAdapter(Comparison<T> comparer)
            {
                _comparer = comparer;
            }

            public override bool AreEqual(object? x, object? y)
            {
                // The CanCompare method ensures that the casts will succeed,
                // so we can safely cast here.
                // The null-forgiving operator is used here because the Comparison<T> delegate
                // is not declared as accepting null values despite the note to implementors:
                // https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.comparer-1.compare?view=net-10.0#notes-to-implementers
                // Comparing null with any reference type is allowed and does not generate an exception.
                return _comparer.Invoke((T)x!, (T)y!) == 0;
            }
        }

        #endregion
    }

    /// <summary>
    /// Generic version of <see cref="EqualityAdapter"/> making it cleaner to write adapters for specific types.
    /// </summary>
    /// <typeparam name="T">The type of the elements to compare.</typeparam>
    public abstract class EqualityAdapter<T> : EqualityAdapter
        where T : notnull
    {
        /// <inheritdoc />
        public sealed override bool CanCompare(object? x, object? y)
        {
            return TypeHelper.CanCast<T>(x) && TypeHelper.CanCast<T>(y);
        }

        /// <inheritdoc />
        public sealed override bool AreEqual(object? x, object? y)
        {
            if (x is null && y is null)
                return true;
            if (x is null || y is null)
                return false;
            return AreEqual((T)x, (T)y);
        }

        /// <inheritdoc />
        public sealed override bool AreEqual(object? x, object? y, ref Tolerance tolerance)
        {
            if (x is null && y is null)
                return true;
            if (x is null || y is null)
                return false;
            return AreEqual((T)x, (T)y, ref tolerance);
        }

        /// <summary>
        /// Compares two instances of <typeparamref name="T"/>, within a tolerance returning true if they are equal
        /// </summary>
        public virtual bool AreEqual(T x, T y, ref Tolerance tolerance)
        {
            if (!tolerance.HasVariance)
            {
                return AreEqual(x, y);
            }

            throw new InvalidOperationException("Tolerance is not supported for this comparison");
        }

        /// <summary>
        /// Compares two instances of <typeparamref name="T"/>, returning true if they are equal
        /// </summary>
        public abstract bool AreEqual(T x, T y);
    }
}
