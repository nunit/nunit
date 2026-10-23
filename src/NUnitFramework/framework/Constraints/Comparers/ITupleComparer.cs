// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Abstractions;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Base class for comparators for tuples (both regular Tuples and ValueTuples).
    /// </summary>
    internal static class ITupleComparer
    {
        public static EqualMethodResult Equal(object x, object y, ref Tolerance tolerance, ComparisonState state, NUnitEqualityComparer equalityComparer)
        {
            Type xType = x.GetType();
            Type yType = y.GetType();

            if (!xType.IsTupleOrValueTuple() || !yType.IsTupleOrValueTuple())
                return EqualMethodResult.TypesNotSupported;

            ITupleValuesSupplier? xTuple = TupleValuesSupplier.For(x);
            ITupleValuesSupplier? yTuple = TupleValuesSupplier.For(y);

            if (xTuple is null || yTuple is null)
                return EqualMethodResult.TypesNotSupported;

            int numberOfElements = xTuple.Length;

            if (numberOfElements != yTuple.Length)
                return EqualMethodResult.ComparedNotEqual;

            if (numberOfElements > 8)
            {
                // Tuples with more than 8 direct elements are not supported.
                // The 8th element is a nested tuple, we compare separately the 7 direct elements and the nested tuple.
                // If this is changed in later versions of .NET, this code should be updated accordingly.
                throw new InvalidOperationException($"Tuples with more than 8 direct elements are not supported. Found {numberOfElements} elements.");
            }

            ComparisonState comparisonState = state.PushComparison(x, y);

            if (numberOfElements == 0) // Nothing to compare
                return EqualMethodResult.ComparedEqual;

            uint redoWithoutTolerance = 0x0;
            for (int i = 0; i < numberOfElements; i++)
            {
                object? xItem = xTuple.GetValue(i);
                object? yItem = yTuple.GetValue(i);

                EqualMethodResult result = equalityComparer.AreEqual(xItem, yItem, ref tolerance, comparisonState);
                if (result == EqualMethodResult.ComparedNotEqual)
                    return result;
                if (result == EqualMethodResult.ToleranceNotSupported)
                {
                    redoWithoutTolerance |= 1U << i;
                }
            }

            if (redoWithoutTolerance == (1U << numberOfElements) - 1)
                return EqualMethodResult.ToleranceNotSupported;

            if (redoWithoutTolerance != 0)
            {
                Tolerance noTolerance = Tolerance.Exact;
                for (int i = 0; i < numberOfElements; i++)
                {
                    if ((redoWithoutTolerance & (1U << i)) != 0)
                    {
                        object? xItem = xTuple.GetValue(i);
                        object? yItem = yTuple.GetValue(i);

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
