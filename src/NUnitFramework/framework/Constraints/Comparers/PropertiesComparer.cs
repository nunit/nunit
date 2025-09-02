// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
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

            PropertiesComparerConfiguration configuration = equalityComparer.ComparePropertiesConfiguration ?? PropertiesComparerConfiguration.Default;

            if (xType != yType && !configuration.AllowComparingDifferentTypes)
            {
                // Both operands need to be the same type.
                return EqualMethodResult.TypesNotSupported;
            }

            if (xType.IsPrimitive || yType.IsPrimitive)
            {
                // We should never get here if the order in NUnitEqualityComparer is correct.
                // We don't do built-in value types
                return EqualMethodResult.TypesNotSupported;
            }

            PropertyInfo[] xProperties = GetPropertiesToCompare(xType);
            HashSet<string> xPropertyNames = new HashSet<string>(xProperties.Select(p => p.Name));

            PropertyInfo[] yProperties = xProperties;
            HashSet<string> yPropertyNames = xPropertyNames;
            HashSet<string> allPropertyNames = xPropertyNames;

            if (xType != yType)
            {
                yProperties = GetPropertiesToCompare(yType);
                yPropertyNames = new HashSet<string>(yProperties.Select(p => p.Name));
                allPropertyNames = new HashSet<string>(xPropertyNames.Concat(yPropertyNames));
                UseProperties(yType, yPropertyNames);
                ExcludeProperties(yType, yPropertyNames);
            }

            UseProperties(xType, xPropertyNames);
            ExcludeProperties(xType, xPropertyNames);

            Dictionary<string, string>? propertyNameMap = GetPropertyNameMap();
            Dictionary<string, object?>? propertyNameToValueMap = GetPropertyToValueMap();

            if (configuration.OnlyCompareCommonProperties &&
                !ReferenceEquals(xPropertyNames, yPropertyNames))
            {
                xPropertyNames.IntersectWith(yPropertyNames);
                yPropertyNames = xPropertyNames;
            }

            if (xPropertyNames.Count != yPropertyNames.Count + (propertyNameToValueMap?.Count ?? 0) ||
                xPropertyNames.Count == 0)
            {
                // We can't compare if there are no properties left or the count is different.
                return EqualMethodResult.TypesNotSupported;
            }

            xProperties = xProperties.Where(p => xPropertyNames.Contains(p.Name)).ToArray();
            yProperties = yProperties.Where(p => yPropertyNames.Contains(p.Name)).ToArray();

            if (xProperties.Any(p => p.GetIndexParameters().Length > 0) ||
                yProperties.Any(p => p.GetIndexParameters().Length > 0))
            {
                // We also can't deal with indexer properties as we don't know the range of valid values.
                return EqualMethodResult.TypesNotSupported;
            }

            ComparisonState comparisonState = state.PushComparison(x, y);

            BitArray redoWithoutTolerance = new BitArray(xProperties.Length);
            int toleranceNotSupportedCount = 0;

            // We compare in 'x' order, but need to lookup corresponding yProperty.
            Dictionary<string, PropertyInfo> yPropertyDictionary = yProperties.ToDictionary(x => x.Name);
            for (int i = 0; i < xProperties.Length; i++)
            {
                (string xPropertyName, object? xPropertyValue, string yPropertyName, object? yPropertyValue) =
                    GetPropertyNamesAndValues(i);

                Tolerance toleranceToApply = tolerance;

                if (tolerance.IsUnsetOrDefault)
                {
                    if (xPropertyValue is TimeSpan or DateTime or DateTimeOffset)
                    {
                        toleranceToApply = configuration.TimeSpanTolerance;
                    }
                    else if (Numerics.IsFloatingPointNumeric(xPropertyValue))
                    {
                        toleranceToApply = configuration.FloatingPointTolerance;
                    }
                    else if (Numerics.IsFixedPointNumeric(xPropertyValue))
                    {
                        toleranceToApply = configuration.FixedPointTolerance;
                    }
                }

                EqualMethodResult result = equalityComparer.AreEqual(xPropertyValue, yPropertyValue, ref toleranceToApply, comparisonState);

                if (result == EqualMethodResult.ComparedNotEqual || result == EqualMethodResult.TypesNotSupported)
                {
                    return PropertyNotEqualResult(equalityComparer, i, xType.Name, xPropertyName, xPropertyValue, yType.Name, yPropertyName, yPropertyValue);
                }

                if (result == EqualMethodResult.ToleranceNotSupported)
                {
                    redoWithoutTolerance.Set(i, true);
                    toleranceNotSupportedCount++;
                }
            }

            if (toleranceNotSupportedCount == xProperties.Length)
            {
                // If none of the properties supported the tolerance don't retry without it
                return EqualMethodResult.ToleranceNotSupported;
            }

            if (toleranceNotSupportedCount != 0)
            {
                Tolerance noTolerance = Tolerance.Exact;
                for (int i = 0; i < xProperties.Length; i++)
                {
                    if (redoWithoutTolerance.Get(i))
                    {
                        (string xPropertyName, object? xPropertyValue, string yPropertyName, object? yPropertyValue) =
                            GetPropertyNamesAndValues(i);

                        EqualMethodResult result = equalityComparer.AreEqual(xPropertyValue, yPropertyValue, ref noTolerance, comparisonState);
                        if (result == EqualMethodResult.ComparedNotEqual)
                        {
                            return PropertyNotEqualResult(equalityComparer, i, xType.Name, xPropertyName, xPropertyValue, yType.Name, yPropertyName, yPropertyValue);
                        }
                    }
                }
            }

            return EqualMethodResult.ComparedEqual;

            PropertyInfo[] GetPropertiesToCompare(Type type)
            {
                var properties = new Dictionary<string, PropertyInfo>();
                foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    // If there's a property with the same name (in case of new keyword), let's decide which one to use
                    if (properties.TryGetValue(property.Name, out var existing))
                    {
                        // If the existing property is declared in the current type, keep the existing one
                        if (existing.DeclaringType == type)
                            continue;

                        if (
                            // If the property is declared in the current type, use it
                            property.DeclaringType == type
                            // Use the one declared in the most derived type (longest inheritance chain)
                            || existing.DeclaringType!.IsAssignableFrom(property.DeclaringType))
                        {
                            properties[property.Name] = property;
                        }
                    }
                    else
                    {
                        properties[property.Name] = property;
                    }
                }

                return properties.Values.ToArray();
            }

            void UseProperties(Type type, HashSet<string> propertyNames)
            {
                if (configuration.PropertyNamesToUseForType is null ||
                    !configuration.PropertyNamesToUseForType.TryGetValue(type, out HashSet<string>? propertyNamesToUse))
                {
                    propertyNamesToUse = configuration.PropertyNamesToUse;
                }

                if (propertyNamesToUse is not null)
                {
                    if (!propertyNamesToUse.IsSubsetOf(allPropertyNames))
                    {
                        throw new ArgumentException("The properties to use must all exist on the object.");
                    }

                    propertyNames.IntersectWith(propertyNamesToUse);
                }
            }

            void ExcludeProperties(Type type, HashSet<string> propertyNames)
            {
                if (configuration.PropertyNamesToExcludeForType is null ||
                    !configuration.PropertyNamesToExcludeForType.TryGetValue(type, out HashSet<string>? propertyNamesToExclude))
                {
                    propertyNamesToExclude = configuration.PropertyNamesToExclude;
                }

                if (propertyNamesToExclude is not null)
                {
                    if (!propertyNamesToExclude.IsSubsetOf(allPropertyNames))
                    {
                        throw new ArgumentException("The properties to exclude must all exist on the object.");
                    }

                    propertyNames.ExceptWith(propertyNamesToExclude);
                }
            }

            Dictionary<string, string>? GetPropertyNameMap()
            {
                if (configuration.PropertyNameMapForType is null ||
                    !configuration.PropertyNameMapForType.TryGetValue(xType, out Dictionary<string, string>? propertyNameMap))
                {
                    propertyNameMap = configuration.PropertyNameMap;
                }

                if (propertyNameMap is not null)
                {
                    // Verify that all Keys are in x and all Values are in y
                    if (!xPropertyNames.IsSupersetOf(propertyNameMap.Keys))
                    {
                        throw new ArgumentException($"The properties to map from must all exist on the type {xType.Name}.");
                    }
                    if (!yPropertyNames.IsSupersetOf(propertyNameMap.Values))
                    {
                        throw new ArgumentException($"The properties to map to must all exist on the type {yType.Name}.");
                    }
                }

                return propertyNameMap;
            }

            Dictionary<string, object?>? GetPropertyToValueMap()
            {
                if (configuration.PropertyNameToValueMapForType is null ||
                    !configuration.PropertyNameToValueMapForType.TryGetValue(xType, out Dictionary<string, object?>? propertyNameToValueMap))
                {
                    propertyNameToValueMap = null;
                }

                if (propertyNameToValueMap is not null)
                {
                    // Verify that all Keys are in x
                    if (!xPropertyNames.IsSupersetOf(propertyNameToValueMap.Keys))
                    {
                        throw new ArgumentException($"The properties to map from must all exist on the type {xType.Name}.");
                    }
                }

                return propertyNameToValueMap;
            }

            (string, object?, string, object?) GetPropertyNamesAndValues(int i)
            {
                PropertyInfo xProperty = xProperties[i];
                object? xPropertyValue = xProperty.GetValue(x, null);

                if (propertyNameMap is null ||
                    !propertyNameMap.TryGetValue(xProperty.Name, out string? yPropertyName))
                {
                    yPropertyName = xProperty.Name;
                }
                if (propertyNameToValueMap is null ||
                    !propertyNameToValueMap.TryGetValue(xProperty.Name, out object? yPropertyValue))
                {
                    PropertyInfo yProperty = yPropertyDictionary[yPropertyName];
                    yPropertyValue = yProperty.GetValue(y, null);
                }

                return (xProperty.Name, xPropertyValue, yPropertyName, yPropertyValue);
            }
        }

        private static EqualMethodResult PropertyNotEqualResult(NUnitEqualityComparer equalityComparer, int i, string xTypeName, string xPropertyName, object? xPropertyValue, string yTypeName, string yPropertyName, object? yPropertyValue)
        {
            var fp = new NUnitEqualityComparer.FailurePoint
            {
                Position = i,
                PropertyName = xTypeName == yTypeName ?
                    $"{xTypeName}.{xPropertyName}" :
                    $"{xTypeName}.{xPropertyName} => {yTypeName}.{yPropertyName}",
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
