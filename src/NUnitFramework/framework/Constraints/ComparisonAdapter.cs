// ***********************************************************************
// Copyright (c) 2009 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Compatibility;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// ComparisonAdapter class centralizes all comparisons of
    /// _values in NUnit, adapting to the use of any provided
    /// <see cref="IComparer"/>, <see cref="IComparer{T}"/>
    /// or <see cref="Comparison{T}"/>.
    /// </summary>
    public abstract class ComparisonAdapter
    {
        /// <summary>
        /// Gets the default ComparisonAdapter, which wraps an
        /// NUnitComparer object.
        /// </summary>
        public static ComparisonAdapter Default
        {
            get { return new DefaultComparisonAdapter(); }
        }

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
        public abstract int Compare(object expected, object actual);

        class DefaultComparisonAdapter : ComparerAdapter
        {
            /// <summary>
            /// Construct a default ComparisonAdapter
            /// </summary>
            public DefaultComparisonAdapter() : base( NUnitComparer.Default ) { }
        }

        class ComparerAdapter : ComparisonAdapter
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
            public override int Compare(object expected, object actual)
            {
                return comparer.Compare(expected, actual);
            }
        }

        /// <summary>
        /// ComparerAdapter extends <see cref="ComparisonAdapter"/> and
        /// allows use of an <see cref="IComparer{T}"/> or <see cref="Comparison{T}"/>
        /// to actually perform the comparison.
        /// </summary>
        class ComparerAdapter<T> : ComparisonAdapter
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
            public override int Compare(object expected, object actual)
            {
                if (!typeof(T).GetTypeInfo().IsAssignableFrom(expected.GetType().GetTypeInfo()))
                    throw new ArgumentException("Cannot compare " + expected.ToString());

                if (!typeof(T).GetTypeInfo().IsAssignableFrom(actual.GetType().GetTypeInfo()))
                    throw new ArgumentException("Cannot compare to " + actual.ToString());

                return comparer.Compare((T)expected, (T)actual);
            }
        }

        class ComparisonAdapterForComparison<T> : ComparisonAdapter
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
            public override int Compare(object expected, object actual)
            {
                if (!typeof(T).GetTypeInfo().IsAssignableFrom(expected.GetType().GetTypeInfo()))
                    throw new ArgumentException("Cannot compare " + expected.ToString());

                if (!typeof(T).GetTypeInfo().IsAssignableFrom(actual.GetType().GetTypeInfo()))
                    throw new ArgumentException("Cannot compare to " + actual.ToString());

                return comparison.Invoke((T)expected, (T)actual);
            }
        }
    }
}