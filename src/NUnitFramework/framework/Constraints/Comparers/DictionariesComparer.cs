// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for two <see cref="IDictionary"/>s.
    /// </summary>
    internal sealed class DictionariesComparer : IChainComparer
    {
        private readonly NUnitEqualityComparer _equalityComparer;

        internal DictionariesComparer(NUnitEqualityComparer equalityComparer)
        {
            _equalityComparer = equalityComparer;
        }

        public bool? Equal(object x, object y, ref Tolerance tolerance, ComparisonState state)
        {
            if (!(x is IDictionary) || !(y is IDictionary))
                return null;

            IDictionary xDictionary = (IDictionary)x;
            IDictionary yDictionary = (IDictionary)y;

            if (xDictionary.Count != yDictionary.Count)
                return false;

            CollectionTally tally = new CollectionTally(_equalityComparer, xDictionary.Keys);
            tally.TryRemove(yDictionary.Keys);
            if ((tally.Result.MissingItems.Count > 0) || (tally.Result.ExtraItems.Count > 0))
                return false;

            foreach (object key in xDictionary.Keys)
                if (!_equalityComparer.AreEqual(xDictionary[key], yDictionary[key], ref tolerance, state.PushComparison(x, y)))
                    return false;

            return true;
        }
    }
}
