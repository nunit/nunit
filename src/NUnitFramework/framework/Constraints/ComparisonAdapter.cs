// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// ComparisonAdapter class centralizes all comparisons of
    /// values in NUnit, adapting to the use of any provided
    /// <see cref="IComparer"/>, <see cref="IComparer{T}"/>
    /// or <see cref="Comparison{T}"/>.
    /// </summary>
    public abstract class ComparisonAdapter
    {
        /// <summary>
        /// Gets the default ComparisonAdapter, which wraps an
        /// NUnitComparer object.
        /// </summary>
        public static ComparisonAdapter Default => new DefaultComparisonAdapter();

        /// <summary>
        /// Returns a ComparisonAdapter that wraps an <see cref="IComparer"/>
        /// </summary>
        public static ComparisonAdapter For(IComparer comparer)
        {
            return new ComparerAdapter(comparer);
        }

        /// <summary>
        /// Returns a ComparisonAdapter that wraps an <see cref="IComparer{T}"/>
        /// </summary>
        public static ComparisonAdapter For<T>(IComparer<T> comparer)
        {
            return new ComparerAdapter<T>(comparer);
        }

        /// <summary>
        /// Returns a ComparisonAdapter that wraps a <see cref="Comparison{T}"/>
        /// </summary>
        public static ComparisonAdapter For<T>(Comparison<T> comparer)
        {
            return new ComparisonAdapterForComparison<T>(comparer);
        }

        /// <summary>
        /// Compares two objects
        /// </summary>
        public abstract int Compare(object? expected, object? actual);

        private class DefaultComparisonAdapter : ComparerAdapter
        {
            /// <summary>
            /// Construct a default ComparisonAdapter
            /// </summary>
            public DefaultComparisonAdapter() : base( NUnitComparer.Default ) { }
        }

        private class ComparerAdapter : ComparisonAdapter
        {
            private readonly IComparer comparer;

            /// <summary>
            /// Construct a ComparisonAdapter for an <see cref="IComparer"/>
            /// </summary>
            public ComparerAdapter(IComparer comparer)
            {
                this.comparer = comparer;
            }

            /// <summary>
            /// Compares two objects
            /// </summary>
            /// <param name="expected"></param>
            /// <param name="actual"></param>
            /// <returns></returns>
            public override int Compare(object? expected, object? actual)
            {
                return comparer.Compare(expected, actual);
            }
        }

        /// <summary>
        /// ComparerAdapter extends <see cref="ComparisonAdapter"/> and
        /// allows use of an <see cref="IComparer{T}"/> or <see cref="Comparison{T}"/>
        /// to actually perform the comparison.
        /// </summary>
        private class ComparerAdapter<T> : ComparisonAdapter
        {
            private readonly IComparer<T> comparer;

            /// <summary>
            /// Construct a ComparisonAdapter for an <see cref="IComparer{T}"/>
            /// </summary>
            public ComparerAdapter(IComparer<T> comparer)
            {
                this.comparer = comparer;
            }

            /// <summary>
            /// Compare a Type T to an object
            /// </summary>
            public override int Compare(object? expected, object? actual)
            {
                if (!TypeHelper.TryCast(expected, out T? expectedCast))
                    throw new ArgumentException($"Cannot compare {expected?.ToString() ?? "null"}");

                if (!TypeHelper.TryCast(actual, out T? actualCast))
                    throw new ArgumentException($"Cannot compare to {actual?.ToString() ?? "null"}");

                return comparer.Compare(expectedCast, actualCast);
            }
        }

        private class ComparisonAdapterForComparison<T> : ComparisonAdapter
        {
            private readonly Comparison<T> comparison;

            /// <summary>
            /// Construct a ComparisonAdapter for a <see cref="Comparison{T}"/>
            /// </summary>
            public ComparisonAdapterForComparison(Comparison<T> comparer)
            {
                this.comparison = comparer;
            }

            /// <summary>
            /// Compare a Type T to an object
            /// </summary>
            public override int Compare(object? expected, object? actual)
            {
                if (!TypeHelper.TryCast(expected, out T? expectedCast))
                    throw new ArgumentException($"Cannot compare {expected?.ToString() ?? "null"}");

                if (!TypeHelper.TryCast(actual, out T? actualCast))
                    throw new ArgumentException($"Cannot compare to {actual?.ToString() ?? "null"}");

                return comparison.Invoke(expectedCast, actualCast);
            }
        }
    }
}
