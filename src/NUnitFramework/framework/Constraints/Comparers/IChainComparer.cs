// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Interface for comparing two <see cref="Object"/>s.
    /// </summary>
    internal interface IChainComparer
    {
        /// <summary>
        /// Method for comparing two objects with a tolerance.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <param name="tolerance">The tolerance to use when comparing the objects.</param>
        /// <param name="state">The evaluation state of the comparison.</param>
        /// <returns>
        ///     <see langword="null"/> if the objects cannot be compared using the method.
        ///     Otherwise the result of the comparison is returned.
        /// </returns>
        bool? Equal(object x, object y, ref Tolerance tolerance, ComparisonState state);
    }
}
