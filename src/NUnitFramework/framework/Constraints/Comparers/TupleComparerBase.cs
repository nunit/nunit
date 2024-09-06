// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Base class for comparators for tuples (both regular Tuples and ValueTuples).
    /// </summary>
    internal static class TupleComparerBase
    {
        public static EqualMethodResult Equal(object x, object y, ref Tolerance tolerance, ComparisonState state, NUnitEqualityComparer equalityComparer,
                                              Func<Type, bool> isCorrectType, Func<Type, string, object, object?> getValue)
        {
            Type xType = x.GetType();
            Type yType = y.GetType();

            if (!isCorrectType(xType) || !isCorrectType(yType))
                return EqualMethodResult.TypesNotSupported;

            int numberOfGenericArgs = xType.GetGenericArguments().Length;

            if (numberOfGenericArgs != yType.GetGenericArguments().Length)
                return EqualMethodResult.ComparedNotEqual;

            ComparisonState comparisonState = state.PushComparison(x, y);

            if (numberOfGenericArgs == 0) // Nothing to compare
                return EqualMethodResult.ComparedEqual;

            uint redoWithoutTolerance = 0x0;
            for (int i = 0; i < numberOfGenericArgs; i++)
            {
                string propertyName = i < 7 ? "Item" + (i + 1) : "Rest";
                object? xItem = getValue(xType, propertyName, x);
                object? yItem = getValue(yType, propertyName, y);

                EqualMethodResult result = equalityComparer.AreEqual(xItem, yItem, ref tolerance, comparisonState);
                if (result == EqualMethodResult.ComparedNotEqual)
                    return result;
                if (result == EqualMethodResult.ToleranceNotSupported)
                    redoWithoutTolerance |= 1U << i;
            }

            if (redoWithoutTolerance == (1U << numberOfGenericArgs) - 1)
                return EqualMethodResult.ToleranceNotSupported;

            if (redoWithoutTolerance != 0)
            {
                Tolerance noTolerance = Tolerance.Exact;
                for (int i = 0; i < numberOfGenericArgs; i++)
                {
                    if ((redoWithoutTolerance & (1U << i)) != 0)
                    {
                        string propertyName = i < 7 ? "Item" + (i + 1) : "Rest";
                        object? xItem = getValue(xType, propertyName, x);
                        object? yItem = getValue(yType, propertyName, y);

                        EqualMethodResult result = equalityComparer.AreEqual(xItem, yItem, ref noTolerance, comparisonState);
                        if (result == EqualMethodResult.ComparedNotEqual)
                            return result;
                    }
                }
            }

            return EqualMethodResult.ComparedEqual;
        }
    }
}
