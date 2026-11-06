// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections;
using NUnit.Framework.Internal;

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

            // A dictionary key can't be null, so a null-equivalent key isn't treated as null either:
            // it must be a key of both dictionaries. NUnit equality still matches the other keys.
            NullEquivalents? nullEquivalents = state.NullEquivalents;
            if (nullEquivalents is not null &&
                (!OtherContainsAllNullEquivalentKeys(xDictionary, yDictionary, nullEquivalents) || !OtherContainsAllNullEquivalentKeys(yDictionary, xDictionary, nullEquivalents)))
            {
                return EqualMethodResult.ComparedNotEqual;
            }

            var tally = new CollectionTally<object>(KeysWithoutNullEquivalents(xDictionary, nullEquivalents), equalityComparer);
            tally.TryRemove(KeysWithoutNullEquivalents(yDictionary, nullEquivalents));
            if ((tally.Result.MissingItems.Count > 0) || (tally.Result.ExtraItems.Count > 0))
                return EqualMethodResult.ComparedNotEqual;

            ComparisonState comparisonState = state.PushComparison(x, y);
            foreach (object key in xDictionary.Keys)
            {
                EqualMethodResult result = equalityComparer.AreEqual(xDictionary[key], yDictionary[key], ref tolerance, comparisonState);
                if (result != EqualMethodResult.ComparedEqual)
                    return result;
            }

            return EqualMethodResult.ComparedEqual;
        }

        // True when every null-equivalent key of dictionary is also a key of other (or there are none).
        private static bool OtherContainsAllNullEquivalentKeys(IDictionary dictionary, IDictionary other, NullEquivalents nullEquivalents)
        {
            foreach (object? key in dictionary.Keys)
            {
                if (key is not null && nullEquivalents.IsNullEquivalent(key) && !other.Contains(key))
                    return false;
            }

            return true;
        }

        private static IEnumerable KeysWithoutNullEquivalents(IDictionary dictionary, NullEquivalents? nullEquivalents)
            => nullEquivalents is null ? dictionary.Keys : NonNullEquivalentKeys(dictionary, nullEquivalents);

        private static IEnumerable NonNullEquivalentKeys(IDictionary dictionary, NullEquivalents nullEquivalents)
        {
            foreach (object? key in dictionary.Keys)
            {
                if (key is null || !nullEquivalents.IsNullEquivalent(key))
                    yield return key;
            }
        }
    }
}
