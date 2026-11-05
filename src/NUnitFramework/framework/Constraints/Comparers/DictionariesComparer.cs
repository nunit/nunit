// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections;
using System.Collections.Generic;
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

            var tally = new CollectionTally<object>(xDictionary.Keys, equalityComparer);
            tally.TryRemove(yDictionary.Keys);
            if ((tally.Result.MissingItems.Count > 0) || (tally.Result.ExtraItems.Count > 0))
                return EqualMethodResult.ComparedNotEqual;

            ComparisonState comparisonState = state.PushComparison(x, y);
            bool hasNullEquivalentPredicate = TestExecutionContext.HasNullEquivalentPredicate;
            List<object?>? xNullEquivalentKeyValues = null;
            foreach (object key in xDictionary.Keys)
            {
                if (hasNullEquivalentPredicate && TestExecutionContext.IsNullOrNullEquivalent(key))
                {
                    (xNullEquivalentKeyValues ??= new List<object?>()).Add(xDictionary[key]);
                    continue;
                }

                EqualMethodResult result = equalityComparer.AreEqual(xDictionary[key], yDictionary[key], ref tolerance, comparisonState);
                if (result != EqualMethodResult.ComparedEqual)
                    return result;
            }

            if (xNullEquivalentKeyValues is null)
                return EqualMethodResult.ComparedEqual;

            // Null-equivalent keys all compare equal to each other, so neither dictionary's own key lookup can pair
            // them. Match their entries one-to-one by value instead. The key tally guarantees both sides have as many.
            var yNullEquivalentKeyValues = new List<object?>();
            foreach (object key in yDictionary.Keys)
            {
                if (TestExecutionContext.IsNullOrNullEquivalent(key))
                    yNullEquivalentKeyValues.Add(yDictionary[key]);
            }

            foreach (object? xValue in xNullEquivalentKeyValues)
            {
                int match = 0;
                while (match < yNullEquivalentKeyValues.Count &&
                       equalityComparer.AreEqual(xValue, yNullEquivalentKeyValues[match], ref tolerance, comparisonState) != EqualMethodResult.ComparedEqual)
                {
                    match++;
                }

                if (match == yNullEquivalentKeyValues.Count)
                    return EqualMethodResult.ComparedNotEqual;

                yNullEquivalentKeyValues.RemoveAt(match);
            }

            return EqualMethodResult.ComparedEqual;
        }
    }
}
