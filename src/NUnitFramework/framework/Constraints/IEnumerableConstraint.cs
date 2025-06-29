// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// Interface for enumerable constraints
    /// </summary>
    public interface IEnumerableConstraint : IConstraint
    {
        /// <summary>
        /// Applies the constraint to a collection, returning a ConstraintResult.
        /// </summary>
        /// <param name="actual">The value that the collection originated from</param>
        /// <param name="enumerable">The collection to be tested</param>
        /// <returns>A ConstraintResult</returns>
        ConstraintResult ApplyToEnumerable<TActual, TItem>(TActual actual, IEnumerable<TItem> enumerable);
    }
}
