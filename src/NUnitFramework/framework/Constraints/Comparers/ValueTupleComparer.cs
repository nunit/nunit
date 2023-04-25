// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for two <c>ValueTuple</c>s.
    /// </summary>
    internal static class ValueTupleComparer
    {
        private static bool IsCorrectType(Type type)
        {
            return TypeHelper.IsValueTuple(type);
        }

        private static object? GetValue(Type type, string propertyName, object obj)
        {
            return type.GetField(propertyName)?.GetValue(obj);
        }

        public static bool? Equal(object x, object y, ref Tolerance tolerance, ComparisonState state, NUnitEqualityComparer equalityComparer)
            => TupleComparerBase.Equal(x, y, ref tolerance, state, equalityComparer, IsCorrectType, GetValue);
    }
}
