// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for two <see cref="DictionaryEntry"/>s.
    /// </summary>
    internal static class DictionaryEntriesComparer
    {
        public static EqualMethodResult Equal(object x, object y, ref Tolerance tolerance, ComparisonState state, NUnitEqualityComparer equalityComparer)
        {
            // Issue #70 - EquivalentTo isn't compatible with IgnoreCase for dictionaries
            if (x is not DictionaryEntry xDictionaryEntry || y is not DictionaryEntry yDictionaryEntry)
                return EqualMethodResult.TypesNotSupported;

            ComparisonState comparisonState = state.PushComparison(x, y);
            var keyTolerance = Tolerance.Exact;
            EqualMethodResult result = equalityComparer.AreEqual(xDictionaryEntry.Key, yDictionaryEntry.Key, ref keyTolerance, comparisonState);
            if (result == EqualMethodResult.ComparedEqual)
                result = equalityComparer.AreEqual(xDictionaryEntry.Value, yDictionaryEntry.Value, ref tolerance, comparisonState);
            return result;
        }
    }
}
