// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// EmptyCollectionConstraint tests whether a collection is empty.
    /// </summary>
    public class EmptyCollectionConstraint : CollectionConstraint
    {
        /// <inheritdoc/>
        public override string Description => "<empty>";

        /// <summary>
        /// Check that the collection is empty
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        protected override bool Matches(IEnumerable collection)
        {
            return IsEmpty(collection);
        }
    }
}
