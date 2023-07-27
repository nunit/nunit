// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Reflection;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for two instances of the same type, comparing each property.
    /// </summary>
    internal static class PropertiesComparer
    {
        public static bool? Equal(object x, object y, ref Tolerance tolerance, ComparisonState state, NUnitEqualityComparer equalityComparer)
        {
            Type xType = x.GetType();
            Type yType = y.GetType();

            if (xType != yType)
            {
                return null; // Both operands need to be the same type.
            }

            if (xType.IsPrimitive)
            {
                // We should never get here if the order in NUnitEqualityComparer is correct.
                return null; // We don't do built-in value types
            }

            PropertyInfo[] properties = xType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            if (properties.Length == 0)
            {
                return null;    // We can't compare if there are no properties.
            }

            foreach (var property in properties)
            {
                object? xPropertyValue = property.GetValue(x, null);
                object? yPropertyValue = property.GetValue(y, null);
                if (!equalityComparer.AreEqual(xPropertyValue, yPropertyValue, ref tolerance, state.PushComparison(x, y)))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
