// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections;
using System.Collections.Generic;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for two <see cref="IDictionary"/>s.
    /// </summary>
    internal static class DictionariesComparer
    {
        public static EqualMethodResult Equal(object x, object y, ref Tolerance tolerance, ComparisonState state, NUnitEqualityComparer equalityComparer)
        {
            if (x is not IDictionary xDictionary || y is not IDictionary yDictionary)
                return EqualMethodResult.TypesNotSupported;

            if (xDictionary.Count != yDictionary.Count)
                return EqualMethodResult.ComparedNotEqual;

            var tally = new CollectionTally<object>(xDictionary.Keys, equalityComparer);
            tally.TryRemove(yDictionary.Keys);
            if ((tally.Result.MissingItems.Count > 0) || (tally.Result.ExtraItems.Count > 0))
                return EqualMethodResult.ComparedNotEqual;

            ComparisonState comparisonState = state.PushComparison(x, y);
            List<object>? usedKeys = null;
            foreach (object key in xDictionary.Keys)
            {
                if (!yDictionary.Contains(key))
                {
                    if (!TryFindMatchingEntry(key, xDictionary, yDictionary, ref usedKeys, ref tolerance, comparisonState, equalityComparer))
                        return EqualMethodResult.ComparedNotEqual;

                    continue;
                }

                EqualMethodResult result = equalityComparer.AreEqual(xDictionary[key], yDictionary[key], ref tolerance, comparisonState);
                if (result != EqualMethodResult.ComparedEqual)
                    return result;
            }

            return EqualMethodResult.ComparedEqual;
        }

        // The keys matched with NUnit equality (for example null-equivalent keys) but not with the dictionary's own
        // key equality. Find an unused entry whose key and value both match, skipping keys that match another key exactly.
        private static bool TryFindMatchingEntry(
            object key,
            IDictionary xDictionary,
            IDictionary yDictionary,
            ref List<object>? usedKeys,
            ref Tolerance tolerance,
            ComparisonState state,
            NUnitEqualityComparer equalityComparer)
        {
            foreach (object candidate in yDictionary.Keys)
            {
                if (xDictionary.Contains(candidate) || (usedKeys is not null && usedKeys.Exists(used => ReferenceEquals(used, candidate))))
                    continue;

                if (equalityComparer.AreEqual(key, candidate, ref tolerance, state) == EqualMethodResult.ComparedEqual &&
                    equalityComparer.AreEqual(xDictionary[key], yDictionary[candidate], ref tolerance, state) == EqualMethodResult.ComparedEqual)
                {
                    (usedKeys ??= new List<object>()).Add(candidate);
                    return true;
                }
            }

            return false;
        }
    }
}
