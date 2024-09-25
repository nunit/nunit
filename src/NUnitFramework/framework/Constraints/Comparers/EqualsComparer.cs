// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Reflection;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for a type that overrides Equals
    /// </summary>
    internal static class EqualsComparer
    {
        public static EqualMethodResult Equal(object x, object y, ref Tolerance tolerance, ComparisonState state, NUnitEqualityComparer equalityComparer)
        {
            if (equalityComparer.CompareAsCollection && state.TopLevelComparison)
                return EqualMethodResult.TypesNotSupported;

            Type xType = x.GetType();

            if (equalityComparer.CompareProperties && TypeHelper.HasCompilerGeneratedEquals(xType))
            {
                // For record types, when CompareProperties is requested, we ignore generated Equals method and compare by properties.
                return EqualMethodResult.TypesNotSupported;
            }

            if (OverridesEqualsObject(xType))
            {
                if (tolerance.HasVariance)
                    return EqualMethodResult.ToleranceNotSupported;

                return x.Equals(y) ?
                    EqualMethodResult.ComparedEqual : EqualMethodResult.ComparedNotEqual;
            }

            return EqualMethodResult.TypesNotSupported;
        }

        private static readonly Type[] EqualsObjectParameterTypes = { typeof(object) };

        private static bool OverridesEqualsObject(Type type)
        {
            // Check for Equals(object) override
            var equalsObject = type.GetMethod(nameof(type.Equals), BindingFlags.Instance | BindingFlags.Public,
                                  null, EqualsObjectParameterTypes, null);
            return equalsObject is not null && equalsObject.DeclaringType != (type.IsValueType ? typeof(ValueType) : typeof(object));
        }
    }
}
