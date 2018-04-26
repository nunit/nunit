// ***********************************************************************
// Copyright (c) 2018 Charlie Poole, Rob Prouse
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

using System.Collections;
using System.Collections.Generic;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// An <see cref="IEqualityComparer"/> that also supports equality comparisons
    /// with a <see cref="Tolerance"/>.
    /// </summary>
    internal abstract class ChainComparer : IEqualityComparer
    {
        /// <summary>
        /// Indicates whether this comparer can compare a given object
        /// to other objects.
        /// </summary>
        public virtual bool CanCompare(object obj)
        {
            return false;
        }

        /// <summary>Determines whether the specified objects are equal.</summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        public new bool Equals(object x, object y)
        {
            var tolerance = Tolerance.Default;
            return Equals(x, y, ref tolerance) ?? false;
        }

        /// <summary>
        /// Compares two objects for equality within a tolerance.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <param name="tolerance">The comparison tolerance .</param>
        /// <returns>true if the specified objects are equal within the given tolerance; otherwise, false.</returns>
        public abstract bool? Equals(object x, object y, ref Tolerance tolerance);

        /// <summary>Returns a hash code for the specified object.</summary>
        public virtual int GetHashCode(object obj)
        {
            return obj == null ? 0 : 1;
        }
    }

    /// <summary>
    /// An <see cref="IEqualityComparer{T}"/> that also supports equality comparisons
    /// with a <see cref="Tolerance"/>.
    /// </summary>
    internal abstract class ChainComparer<T> : ChainComparer, IEqualityComparer<T>
    {
        /// <summary>
        /// Indicates whether this comparer can compare a given object
        /// to other objects.
        /// </summary>
        public override bool CanCompare(object obj)
        {
            return obj is T;
        }

        /// <summary>Determines whether the specified objects are equal.</summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        public bool Equals(T x, T y)
        {
            var tolerance = Tolerance.Default;
            return Equals(x, y, ref tolerance);
        }

        /// <summary>
        /// Compares two objects for equality within a tolerance.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <param name="tolerance">The comparison tolerance .</param>
        /// <returns>true if the specified objects are equal within the given tolerance; otherwise, false.</returns>
        public sealed override bool? Equals(object x, object y, ref Tolerance tolerance)
        {
            return CanCompare(x) && CanCompare(y)
                ? Equals((T)x, (T)y, ref tolerance)
                : default(bool?);
        }

        /// <summary>
        /// Compares two objects for equality within a tolerance.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <param name="tolerance">The comparison tolerance .</param>
        /// <returns>true if the specified objects are equal within the given tolerance; otherwise, false.</returns>
        public abstract bool Equals(T x, T y, ref Tolerance tolerance);

        /// <summary>Returns a hash code for the specified object.</summary>
        public sealed override int GetHashCode(object obj)
        {
            return GetHashCode((T)obj);
        }

        /// <summary>Returns a hash code for the specified object.</summary>
        public abstract int GetHashCode(T obj);
    }
}
