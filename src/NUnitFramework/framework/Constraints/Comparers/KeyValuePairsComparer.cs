// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for two <see cref="KeyValuePair{TKey, TValue}"/>s.
    /// </summary>
    internal static class KeyValuePairsComparer
    {
        public static EqualMethodResult Equal(object x, object y, ref Tolerance tolerance, ComparisonState state, NUnitEqualityComparer equalityComparer)
        {
            // IDictionary<,> will eventually try to compare its key value pairs when using CollectionTally
            Type xType = x.GetType();
            Type yType = y.GetType();

            Type? xGenericTypeDefinition = xType.IsGenericType ? xType.GetGenericTypeDefinition() : null;
            Type? yGenericTypeDefinition = yType.IsGenericType ? yType.GetGenericTypeDefinition() : null;

            if (xGenericTypeDefinition != typeof(KeyValuePair<,>) ||
                yGenericTypeDefinition != typeof(KeyValuePair<,>))
            {
                return EqualMethodResult.TypesNotSupported;
            }

            ComparisonState comparisonState = state.PushComparison(x, y);

            var keyTolerance = Tolerance.Exact;
            object? xKey = xType.GetProperty("Key")!.GetValue(x, null);
            object? yKey = yType.GetProperty("Key")!.GetValue(y, null);
            EqualMethodResult result = equalityComparer.AreEqual(xKey, yKey, ref keyTolerance, comparisonState);
            if (result == EqualMethodResult.ComparedEqual)
            {
                object? xValue = xType.GetProperty("Value")!.GetValue(x, null);
                object? yValue = yType.GetProperty("Value")!.GetValue(y, null);

                result = equalityComparer.AreEqual(xValue, yValue, ref tolerance, comparisonState);
            }

            return result;
        }
    }
}
