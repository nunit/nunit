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
using NUnit.Compatibility;
using System.Reflection;

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
        /// Returns true if the two objects can be compared by this adapter.
        /// The base adapter cannot handle IEnumerables except for strings.
        /// </summary>
        public virtual bool CanCompare(object x, object y)
        {
            if (x is string && y is string)
                return true;

            if (x is IEnumerable || y is IEnumerable)
                return false;

            return true;
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
            private IComparer comparer;

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
            private IEqualityComparer comparer;

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

        internal class PredicateEqualityAdapter<TActual, TExpected> : EqualityAdapter
        {
            private readonly Func<TActual, TExpected, bool> _comparison;

            /// <summary>
            /// Returns true if the two objects can be compared by this adapter.
            /// The base adapter cannot handle IEnumerables except for strings.
            /// </summary>
            public override bool CanCompare(object x, object y)
            {
                return true;
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
                return typeof(T).GetTypeInfo().IsAssignableFrom(x.GetType().GetTypeInfo())
                    && typeof(T).GetTypeInfo().IsAssignableFrom(y.GetType().GetTypeInfo());
            }

            protected void ThrowIfNotCompatible(object x, object y)
            {
                if (!typeof(T).GetTypeInfo().IsAssignableFrom(x.GetType().GetTypeInfo()))
                    throw new ArgumentException("Cannot compare " + x.ToString());

                if (!typeof(T).GetTypeInfo().IsAssignableFrom(y.GetType().GetTypeInfo()))
                    throw new ArgumentException("Cannot compare " + y.ToString());
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
            private IEqualityComparer<T> comparer;

            public EqualityComparerAdapter(IEqualityComparer<T> comparer)
            {
                this.comparer = comparer;
            }

            public override bool AreEqual(object x, object y)
            {
                ThrowIfNotCompatible(x, y);
                return comparer.Equals((T)x, (T)y);
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
            private IComparer<T> comparer;

            public ComparerAdapter(IComparer<T> comparer)
            {
                this.comparer = comparer;
            }

            public override bool AreEqual(object x, object y)
            {
                ThrowIfNotCompatible(x, y);
                return comparer.Compare((T)x, (T)y) == 0;
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
            private Comparison<T> comparer;

            public ComparisonAdapter(Comparison<T> comparer)
            {
                this.comparer = comparer;
            }

            public override bool AreEqual(object x, object y)
            {
                ThrowIfNotCompatible(x, y);
                return comparer.Invoke((T)x, (T)y) == 0;
            }
        }

#endregion
    }
}
