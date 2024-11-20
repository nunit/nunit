// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
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
            if (properties.Length == 0 || properties.Any(p => p.GetIndexParameters().Length > 0))
            {
                // We can't compare if there are no properties.
                // We also can't deal with indexer properties as we don't know the range of valid values.
                return EqualMethodResult.TypesNotSupported;
            }

            ComparisonState comparisonState = state.PushComparison(x, y);

            string declaringTypeName = xType.Name;

            BitArray redoWithoutTolerance = new BitArray(properties.Length);
            int toleranceNotSupportedCount = 0;

            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo property = properties[i];
                object? xPropertyValue = property.GetValue(x, null);
                object? yPropertyValue = property.GetValue(y, null);

                EqualMethodResult result = equalityComparer.AreEqual(xPropertyValue, yPropertyValue, ref tolerance, comparisonState);
                if (result == EqualMethodResult.ComparedNotEqual)
                {
                    return PropertyNotEqualResult(equalityComparer, i, declaringTypeName, property.Name, xPropertyValue, yPropertyValue);
                }

                if (result == EqualMethodResult.ToleranceNotSupported)
                {
                    redoWithoutTolerance.Set(i, true);
                    toleranceNotSupportedCount++;
                }
            }

            if (toleranceNotSupportedCount == properties.Length)
            {
                // If none of the properties supported the tolerance don't retry without it
                return EqualMethodResult.ToleranceNotSupported;
            }

            if (toleranceNotSupportedCount != 0)
            {
                Tolerance noTolerance = Tolerance.Exact;
                for (int i = 0; i < properties.Length; i++)
                {
                    if (redoWithoutTolerance.Get(i))
                    {
                        PropertyInfo property = properties[i];
                        object? xPropertyValue = property.GetValue(x, null);
                        object? yPropertyValue = property.GetValue(y, null);

                        EqualMethodResult result = equalityComparer.AreEqual(xPropertyValue, yPropertyValue, ref noTolerance, comparisonState);
                        if (result == EqualMethodResult.ComparedNotEqual)
                        {
                            return PropertyNotEqualResult(equalityComparer, i, declaringTypeName, property.Name, xPropertyValue, yPropertyValue);
                        }
                    }
                }
            }

            return EqualMethodResult.ComparedEqual;
        }

        private static EqualMethodResult PropertyNotEqualResult(NUnitEqualityComparer equalityComparer, int i, string declaringTypeName, string propertyName, object? xPropertyValue, object? yPropertyValue)
        {
            var fp = new NUnitEqualityComparer.FailurePoint
            {
                Position = i,
                PropertyName = $"{declaringTypeName}.{propertyName}",
                ExpectedHasData = true,
                ExpectedValue = xPropertyValue,
                ActualHasData = true,
                ActualValue = yPropertyValue
            };
            equalityComparer.FailurePoints.Insert(0, fp);
            return EqualMethodResult.ComparedNotEqual;
        }
    }
}
