// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for two <see cref="IDictionary"/>s.
    /// </summary>
    internal static class DictionariesComparer
    {
        public static bool? Equal(object x, object y, ref Tolerance tolerance, ComparisonState state, NUnitEqualityComparer equalityComparer)
        {
            if (x is not IDictionary xDictionary || y is not IDictionary yDictionary)
                return null;

            if (xDictionary.Count != yDictionary.Count)
                return false;

            CollectionTally tally = new CollectionTally(equalityComparer, xDictionary.Keys);
            tally.TryRemove(yDictionary.Keys);
            if ((tally.Result.MissingItems.Count > 0) || (tally.Result.ExtraItems.Count > 0))
                return false;

            foreach (object key in xDictionary.Keys)
                if (!equalityComparer.AreEqual(xDictionary[key], yDictionary[key], ref tolerance, state.PushComparison(x, y)))
                    return false;

            return true;
        }
    }
}
