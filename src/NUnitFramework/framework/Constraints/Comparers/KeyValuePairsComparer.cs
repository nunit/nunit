// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Compatibility;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for two <see cref="KeyValuePair{TKey, TValue}"/>s.
    /// </summary>
    internal static class KeyValuePairsComparer
    {
        public static bool? Equal(object x, object y, ref Tolerance tolerance, ComparisonState state, NUnitEqualityComparer equalityComparer)
        {
            // IDictionary<,> will eventually try to compare its key value pairs when using CollectionTally
            Type xType = x.GetType();
            Type yType = y.GetType();

            Type xGenericTypeDefinition = xType.GetTypeInfo().IsGenericType ? xType.GetGenericTypeDefinition() : null;
            Type yGenericTypeDefinition = yType.GetTypeInfo().IsGenericType ? yType.GetGenericTypeDefinition() : null;

            if (xGenericTypeDefinition != typeof(KeyValuePair<,>) ||
                yGenericTypeDefinition != typeof(KeyValuePair<,>))
                return null;

            var keyTolerance = Tolerance.Exact;
            object xKey = xType.GetProperty("Key").GetValue(x, null);
            object yKey = yType.GetProperty("Key").GetValue(y, null);
            object xValue = xType.GetProperty("Value").GetValue(x, null);
            object yValue = yType.GetProperty("Value").GetValue(y, null);

            return equalityComparer.AreEqual(xKey, yKey, ref keyTolerance, state.PushComparison(x, y)) 
                && equalityComparer.AreEqual(xValue, yValue, ref tolerance, state.PushComparison(x, y));
        }
    }
}
