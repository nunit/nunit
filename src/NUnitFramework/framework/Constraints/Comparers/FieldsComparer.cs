// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for two instances of the same type, comparing each field.
    /// </summary>
    internal static class FieldsComparer
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

            if (xType.IsPrimitive || yType.IsPrimitive)
            {
                // We should never get here if the order in NUnitEqualityComparer is correct.
                // We don't do built-in value types
                return EqualMethodResult.TypesNotSupported;
            }

            FieldsComparerConfiguration configuration = equalityComparer.CompareFieldsConfiguration ?? FieldsComparerConfiguration.Default;

            FieldInfo[] fields = GetFieldsToCompare(xType, configuration.OnlyCompareInitAndReadonlyFields);

            if (fields.Length == 0)
            {
                // There is nothing to compare if there are no fields.
                return EqualMethodResult.TypesNotSupported;
            }

            var fieldNames = new HashSet<string>(fields.Select(p => p.Name));

            ComparisonState comparisonState = state.PushComparison(x, y);

            BitArray redoWithoutTolerance = new BitArray(fields.Length);
            int toleranceNotSupportedCount = 0;

            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];
                object? xFieldValue = field.GetValue(x);
                object? yFieldValue = field.GetValue(y);

                Tolerance toleranceToApply = tolerance;

                if (tolerance.IsUnsetOrDefault)
                {
                    if (xFieldValue is TimeSpan or DateTime or DateTimeOffset)
                    {
                        toleranceToApply = configuration.TimeSpanTolerance;
                    }
                    else if (Numerics.IsFloatingPointNumeric(xFieldValue))
                    {
                        toleranceToApply = configuration.FloatingPointTolerance;
                    }
                    else if (Numerics.IsFixedPointNumeric(xFieldValue))
                    {
                        toleranceToApply = configuration.FixedPointTolerance;
                    }
                }

                EqualMethodResult result = equalityComparer.AreEqual(xFieldValue, yFieldValue, ref toleranceToApply, comparisonState);

                if (result == EqualMethodResult.ComparedNotEqual || result == EqualMethodResult.TypesNotSupported)
                {
                    return FieldNotEqualResult(equalityComparer, i, xType.Name, field.Name, xFieldValue, yFieldValue);
                }

                if (result == EqualMethodResult.ToleranceNotSupported)
                {
                    redoWithoutTolerance.Set(i, true);
                    toleranceNotSupportedCount++;
                }
            }

            if (toleranceNotSupportedCount == fields.Length)
            {
                // If none of the properties supported the tolerance don't retry without it
                return EqualMethodResult.ToleranceNotSupported;
            }

            if (toleranceNotSupportedCount != 0)
            {
                Tolerance noTolerance = Tolerance.Exact;
                for (int i = 0; i < fields.Length; i++)
                {
                    if (redoWithoutTolerance.Get(i))
                    {
                        FieldInfo field = fields[i];
                        object? xFieldValue = field.GetValue(x);
                        object? yFieldValue = field.GetValue(y);

                        EqualMethodResult result = equalityComparer.AreEqual(xFieldValue, yFieldValue, ref noTolerance, comparisonState);
                        if (result == EqualMethodResult.ComparedNotEqual)
                        {
                            return FieldNotEqualResult(equalityComparer, i, xType.Name, field.Name, xFieldValue, yFieldValue);
                        }
                    }
                }
            }

            return EqualMethodResult.ComparedEqual;

            FieldInfo[] GetFieldsToCompare(Type type, bool onlyCompareInitAndReadonlyFields)
            {
                return type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                           .Where(field => field.IsInitOnly || !onlyCompareInitAndReadonlyFields)
                           .ToArray();
            }
        }

        private static EqualMethodResult FieldNotEqualResult(NUnitEqualityComparer equalityComparer, int i, string typeName, string fieldName, object? xFieldValue, object? yFieldValue)
        {
            var fp = new NUnitEqualityComparer.FailurePoint
            {
                Position = i,
                PropertyName = $"{typeName}.{fieldName}",
                ExpectedHasData = true,
                ExpectedValue = xFieldValue,
                ActualHasData = true,
                ActualValue = yFieldValue
            };
            equalityComparer.FailurePoints.Insert(0, fp);
            return EqualMethodResult.ComparedNotEqual;
        }
    }
}
