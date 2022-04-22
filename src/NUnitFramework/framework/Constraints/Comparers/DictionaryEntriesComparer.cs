// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for two <see cref="DictionaryEntry"/>s.
    /// </summary>
    internal static class DictionaryEntriesComparer
    {
        public static bool? Equal(object x, object y, ref Tolerance tolerance, ComparisonState state, NUnitEqualityComparer equalityComparer)
        {
            // Issue #70 - EquivalentTo isn't compatible with IgnoreCase for dictionaries
            if (!(x is DictionaryEntry xDictionaryEntry) || !(y is DictionaryEntry yDictionaryEntry))
                return null;

            var keyTolerance = Tolerance.Exact;
            return equalityComparer.AreEqual(xDictionaryEntry.Key, yDictionaryEntry.Key, ref keyTolerance, state.PushComparison(x, y)) 
                && equalityComparer.AreEqual(xDictionaryEntry.Value, yDictionaryEntry.Value, ref tolerance, state.PushComparison(x, y));
        }
    }
}
