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
            Type yType = y.GetType();

            if (equalityComparer.CompareProperties &&
                (TypeHelper.HasCompilerGeneratedEquals(xType) || TypeHelper.HasCompilerGeneratedEquals(yType)))
            {
                // For record types, when CompareProperties is requested, we ignore generated Equals method and compare by properties.
                return EqualMethodResult.TypesNotSupported;
            }

            bool xOverridesEqualsObject = OverridesEqualsObject(xType);
            bool yOverridesEqualsObject = OverridesEqualsObject(yType);

            if (xOverridesEqualsObject || yOverridesEqualsObject)
            {
                if (tolerance.HasVariance)
                    return EqualMethodResult.ToleranceNotSupported;

                bool result;
                if (xOverridesEqualsObject && yOverridesEqualsObject)
                {
                    result = x.Equals(y) || y.Equals(x);
                }
                else if (xOverridesEqualsObject)
                {
                    result = x.Equals(y);
                }
                else // yOverridesEqualsObject
                {
                    result = y.Equals(x);
                }

                return result ? EqualMethodResult.ComparedEqual : EqualMethodResult.ComparedNotEqual;
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
