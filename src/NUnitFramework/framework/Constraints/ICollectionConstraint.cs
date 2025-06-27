// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// Interface for collection constraints
    /// </summary>
    public interface ICollectionConstraint : IConstraint
    {
        /// <summary>
        /// Applies the constraint to a collection, returning a ConstraintResult.
        /// </summary>
        /// <param name="actual">The value that the collection originated from</param>
        /// <param name="collection">The collection to be tested</param>
        /// <returns>A ConstraintResult</returns>
        ConstraintResult ApplyToCollection<TActual, TItem>(TActual actual, IEnumerable<TItem> collection);
    }
}
