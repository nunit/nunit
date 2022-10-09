// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Base class for comparators for tuples (both regular Tuples and ValueTuples).
    /// </summary>
    internal static class TupleComparerBase
    {
        public static bool? Equal(object x, object y, ref Tolerance tolerance, ComparisonState state, NUnitEqualityComparer equalityComparer,
                                  Func<Type, bool> isCorrectType, Func<Type, string, object, object> getValue)
        {
            Type xType = x.GetType();
            Type yType = y.GetType();

            if (!isCorrectType(xType) || !isCorrectType(yType))
                return null;

            int numberOfGenericArgs = xType.GetGenericArguments().Length;

            if (numberOfGenericArgs != yType.GetGenericArguments().Length)
                return false;

            for (int i = 0; i < numberOfGenericArgs; i++)
            {
                string propertyName = i < 7 ? "Item" + (i + 1) : "Rest";
                object xItem = getValue(xType, propertyName, x);
                object yItem = getValue(yType, propertyName, y);

                bool comparison = equalityComparer.AreEqual(xItem, yItem, ref tolerance, state.PushComparison(x, y));
                if (!comparison)
                    return false;
            }

            return true;
        }
    }
}
