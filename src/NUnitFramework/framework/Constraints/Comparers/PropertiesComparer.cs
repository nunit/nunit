// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Linq;
using System.Reflection;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for two instances of the same type, comparing each property.
    /// </summary>
    internal static class PropertiesComparer
    {
        public static EqualMethodResult Equal(object x, object y, ref Tolerance tolerance, ComparisonState state, NUnitEqualityComparer equalityComparer)
        {
            Type xType = x.GetType();
            Type yType = y.GetType();

            if (xType != yType)
            {
                // Both operands need to be the same type.
                return EqualMethodResult.TypesNotSupported;
            }

            if (xType.IsPrimitive)
            {
                // We should never get here if the order in NUnitEqualityComparer is correct.
                // We don't do built-in value types
                return EqualMethodResult.TypesNotSupported;
            }

            PropertyInfo[] properties = xType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            if (properties.Length == 0 || properties.Length >= 32 || properties.Any(p => p.GetIndexParameters().Length > 0))
            {
                // We can't compare if there are no (or too many) properties.
                // We also can't deal with indexer properties as we don't know the range of valid values.
                return EqualMethodResult.TypesNotSupported;
            }

            ComparisonState comparisonState = state.PushComparison(x, y);

            uint redoWithoutTolerance = 0x0;
            if (tolerance.HasVariance)
            {
                for (int i = 0; i < properties.Length; i++)
                {
                    PropertyInfo property = properties[i];
                    object? xPropertyValue = property.GetValue(x, null);
                    object? yPropertyValue = property.GetValue(y, null);

                    EqualMethodResult result = equalityComparer.AreEqual(xPropertyValue, yPropertyValue, ref tolerance, comparisonState);
                    if (result == EqualMethodResult.ComparedNotEqual)
                        return result;
                    if (result == EqualMethodResult.ToleranceNotSupported)
                        redoWithoutTolerance |= 1U << i;
                }

                if (redoWithoutTolerance == (1U << properties.Length) - 1)
                    return EqualMethodResult.ToleranceNotSupported;
            }
            else
            {
                redoWithoutTolerance = (1U << properties.Length) - 1;
            }

            if (redoWithoutTolerance != 0)
            {
                Tolerance noTolerance = Tolerance.Exact;
                for (int i = 0; i < properties.Length; i++)
                {
                    if ((redoWithoutTolerance & (1U << i)) != 0)
                    {
                        PropertyInfo property = properties[i];
                        object? xPropertyValue = property.GetValue(x, null);
                        object? yPropertyValue = property.GetValue(y, null);

                        EqualMethodResult result = equalityComparer.AreEqual(xPropertyValue, yPropertyValue, ref noTolerance, comparisonState);
                        if (result == EqualMethodResult.ComparedNotEqual)
                            return result;
                    }
                }
            }

            return EqualMethodResult.ComparedEqual;
        }
    }
}
