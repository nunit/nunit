// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Compatibility;
using System.Reflection;
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
        public abstract bool AreEqual(object x, object y);

        /// <summary>
        /// Compares two objects, within a tolerance returning true if they are equal
        /// </summary>
        public virtual bool AreEqual(object x, object y, ref Tolerance tolerance)
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
        public virtual bool CanCompare(object x, object y)
        {
            return (x is string || !(x is IEnumerable)) &&
                   (y is string || !(y is IEnumerable));
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
        class ComparerAdapter : EqualityAdapter
        {
            private readonly IComparer comparer;

            public ComparerAdapter(IComparer comparer)
            {
                this.comparer = comparer;
            }

            public override bool AreEqual(object x, object y)
            {
                return comparer.Compare(x, y) == 0;
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

        class EqualityComparerAdapter : EqualityAdapter
        {
            private readonly IEqualityComparer comparer;

            public EqualityComparerAdapter(IEqualityComparer comparer)
            {
                this.comparer = comparer;
            }

            public override bool AreEqual(object x, object y)
            {
                return comparer.Equals(x, y);
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
            public override bool CanCompare(object x, object y)
            {
                return TypeHelper.CanCast<TExpected>(x) && TypeHelper.CanCast<TActual>(y);
            }

            /// <summary>
            /// Compares two objects, returning true if they are equal
            /// </summary>
            public override bool AreEqual(object x, object y)
            {
                return _comparison.Invoke((TActual)y, (TExpected)x);
            }

            public PredicateEqualityAdapter(Func<TActual, TExpected, bool> comparison)
            {
                _comparison = comparison;
            }
        }

        #endregion

        #region Nested GenericEqualityAdapter<T>

        abstract class GenericEqualityAdapter<T> : EqualityAdapter
        {
            /// <summary>
            /// Returns true if the two objects can be compared by this adapter.
            /// Generic adapter requires objects of the specified type.
            /// </summary>
            public override bool CanCompare(object x, object y)
            {
                return TypeHelper.CanCast<T>(x) && TypeHelper.CanCast<T>(y);
            }

            protected void CastOrThrow(object x, object y, out T xValue, out T yValue)
            {
                if (!TypeHelper.TryCast(x, out xValue))
                    throw new ArgumentException($"Cannot compare {x?.ToString() ?? "null"}");

                if (!TypeHelper.TryCast(y, out yValue))
                    throw new ArgumentException($"Cannot compare {y?.ToString() ?? "null"}");
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

        class EqualityComparerAdapter<T> : GenericEqualityAdapter<T>
        {
            private readonly IEqualityComparer<T> comparer;

            public EqualityComparerAdapter(IEqualityComparer<T> comparer)
            {
                this.comparer = comparer;
            }

            public override bool AreEqual(object x, object y)
            {
                CastOrThrow(x, y, out var xValue, out var yValue);
                return comparer.Equals(xValue, yValue);
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
        class ComparerAdapter<T> : GenericEqualityAdapter<T>
        {
            private readonly IComparer<T> comparer;

            public ComparerAdapter(IComparer<T> comparer)
            {
                this.comparer = comparer;
            }

            public override bool AreEqual(object x, object y)
            {
                CastOrThrow(x, y, out var xValue, out var yValue);
                return comparer.Compare(xValue, yValue) == 0;
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

        class ComparisonAdapter<T> : GenericEqualityAdapter<T>
        {
            private readonly Comparison<T> comparer;

            public ComparisonAdapter(Comparison<T> comparer)
            {
                this.comparer = comparer;
            }

            public override bool AreEqual(object x, object y)
            {
                CastOrThrow(x, y, out var xValue, out var yValue);
                return comparer.Invoke(xValue, yValue) == 0;
            }
        }

#endregion
    }
}
