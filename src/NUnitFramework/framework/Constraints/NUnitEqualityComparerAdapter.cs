// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// Adapter that allows <see cref="NUnitEqualityComparer"/> to be used as
    /// an <see cref="IEqualityComparer{T}"/> for sorting and searching operations.
    /// </summary>
    /// <typeparam name="T">The type of objects to compare.</typeparam>
    public sealed class NUnitEqualityComparerAdapter<T> : IEqualityComparer<T>
    {
        private readonly NUnitEqualityComparer _comparer;
        private Tolerance _tolerance;

        /// <summary>
        /// Initializes a new instance of the <see cref="NUnitEqualityComparerAdapter{T}"/> class.
        /// </summary>
        /// <param name="comparer">The <see cref="NUnitEqualityComparer"/> to adapt. If null, a new instance is created.</param>
        public NUnitEqualityComparerAdapter(NUnitEqualityComparer? comparer = null)
        {
            _comparer = comparer ?? new NUnitEqualityComparer();
            _tolerance = Tolerance.Default;
        }

        /// <summary>
        /// Gets or sets the tolerance to use in comparisons.
        /// </summary>
        public Tolerance Tolerance
        {
            get => _tolerance;
            set => _tolerance = value;
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>true if the objects are equal; otherwise, false.</returns>
        public bool Equals(T? x, T? y)
        {
            return _comparer.AreEqual(x, y, ref _tolerance);
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <param name="obj">The object for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object.</returns>
        public int GetHashCode(T? obj)
        {
            return obj?.GetHashCode() ?? 0;
        }
    }
}
