// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for two <c>Tuple</c>s.
    /// </summary>
    internal static class TupleComparer
    {
        private static bool IsCorrectType(Type type)
        {
            return TypeHelper.IsTuple(type);
        }

        private static object? GetValue(Type type, string propertyName, object obj)
        {
            return type.GetProperty(propertyName)?.GetValue(obj, null);
        }

        public static bool? Equal(object x, object y, ref Tolerance tolerance, ComparisonState state, NUnitEqualityComparer equalityComparer)
            => TupleComparerBase.Equal(x, y, ref tolerance, state, equalityComparer, IsCorrectType, GetValue);
    }
}
