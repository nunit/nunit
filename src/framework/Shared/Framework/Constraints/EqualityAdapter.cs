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
#if CLR_2_0
using System.Collections.Generic;
#endif

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// EqualityAdapter class handles all equality comparisons
    /// that use an IEqualityComparer, IEqualityComparer&lt;T&gt;
    /// or a ComparisonAdapter.
    /// </summary>
    public abstract class EqualityAdapter
    {
        /// <summary>
        /// Compares two objects, returning true if they are equal
        /// </summary>
        public abstract bool ObjectsEqual(object x, object y);

        /// <summary>
        /// Returns an EqualityAdapter that wraps an IComparer.
        /// </summary>
        public static EqualityAdapter For(IComparer comparer)
        {
            return new ComparisonAdapterAdapter(ComparisonAdapter.For(comparer));
        }

#if CLR_2_0
        /// <summary>
        /// Returns an EqualityAdapter that wraps an IEqualityComparer.
        /// </summary>
        public static EqualityAdapter For(IEqualityComparer comparer)
        {
            return new EqualityComparerAdapter(comparer);
        }

        /// <summary>
        /// Returns an EqualityAdapter that wraps an IEqualityComparer&lt;T&gt;.
        /// </summary>
        public static EqualityAdapter For<T>(IEqualityComparer<T> comparer)
        {
            return new EqualityComparerAdapter<T>(comparer);
        }

        /// <summary>
        /// Returns an EqualityAdapter that wraps an IComparer&lt;T&gt;.
        /// </summary>
        public static EqualityAdapter For<T>(IComparer<T> comparer)
        {
            return new ComparisonAdapterAdapter( ComparisonAdapter.For(comparer) );
        }

        /// <summary>
        /// Returns an EqualityAdapter that wraps a Comparison&lt;T&gt;.
        /// </summary>
        public static EqualityAdapter For<T>(Comparison<T> comparer)
        {
            return new ComparisonAdapterAdapter( ComparisonAdapter.For(comparer) );
        }

        class EqualityComparerAdapter : EqualityAdapter
        {
            private IEqualityComparer comparer;

            public EqualityComparerAdapter(IEqualityComparer comparer)
            {
                this.comparer = comparer;
            }

            public override bool ObjectsEqual(object x, object y)
            {
                return comparer.Equals(x, y);
            }
        }

        class EqualityComparerAdapter<T> : EqualityAdapter
        {
            private IEqualityComparer<T> comparer;

            public EqualityComparerAdapter(IEqualityComparer<T> comparer)
            {
                this.comparer = comparer;
            }

            public override bool ObjectsEqual(object x, object y)
            {
                if (!typeof(T).IsAssignableFrom(x.GetType()))
                    throw new ArgumentException("Cannot compare " + x.ToString());

                if (!typeof(T).IsAssignableFrom(y.GetType()))
                    throw new ArgumentException("Cannot compare to " + y.ToString());

                return comparer.Equals((T)x, (T)y);
            }
        }
#endif

        class ComparisonAdapterAdapter : EqualityAdapter
        {
            private ComparisonAdapter comparer;

            public ComparisonAdapterAdapter(ComparisonAdapter comparer)
            {
                this.comparer = comparer;
            }

            public override bool ObjectsEqual(object x, object y)
            {
                return comparer.Compare(x, y) == 0;
            }
        }
    }
}
