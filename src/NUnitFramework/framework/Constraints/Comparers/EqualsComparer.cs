// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Reflection;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for a type that overrides Equals
    /// </summary>
    internal static class EqualsComparer
    {
        public static bool? Equal(object x, object y, ref Tolerance tolerance, ComparisonState state, NUnitEqualityComparer equalityComparer)
        {
            if (equalityComparer.CompareAsCollection && state.TopLevelComparison)
                return null;
            if (tolerance is not null && tolerance.HasVariance)
                return null;

            Type xType = x.GetType();

            if (OverridesEqualsObject(xType))
                return x.Equals(y);

            return null;
        }

        private static readonly Type[] EqualsObjectParameterTypes = { typeof(object) };

        private static bool OverridesEqualsObject(Type type)
        {
            // Check for Equals(object) override
            var equalsObject = type.GetMethod(nameof(type.Equals), BindingFlags.Instance | BindingFlags.Public,
                                  null, EqualsObjectParameterTypes, null);
            return equalsObject is not null && equalsObject.DeclaringType != typeof(object);
        }
    }
}
