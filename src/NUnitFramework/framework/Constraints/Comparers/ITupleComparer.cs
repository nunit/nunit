// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Runtime.CompilerServices;
using NUnit.Framework.Internal.Extensions;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Base class for comparators for tuples (both regular Tuples and ValueTuples).
    /// </summary>
    internal static class ITupleComparer
    {
        public static EqualMethodResult Equal(object x, object y, ref Tolerance tolerance, ComparisonState state, NUnitEqualityComparer equalityComparer)
        {
            ITuple? xTuple = TupleAccessor.For(x);
            ITuple? yTuple = TupleAccessor.For(y);

            if (xTuple is null || yTuple is null)
                return EqualMethodResult.TypesNotSupported;

            int numberOfElements = xTuple.Length;

            if (numberOfElements != yTuple.Length)
                return EqualMethodResult.ComparedNotEqual;

            ComparisonState comparisonState = state.PushComparison(x, y);

            if (numberOfElements == 0) // Nothing to compare
                return EqualMethodResult.ComparedEqual;

            if (numberOfElements >= 32)
                throw new NotSupportedException($"Tuples with 32 or more elements are not supported. Found {numberOfElements} elements.");

            uint redoWithoutTolerance = 0x0;
            for (int i = 0; i < numberOfElements; i++)
            {
                object? xItem = xTuple[i];
                object? yItem = yTuple[i];

                EqualMethodResult result = equalityComparer.AreEqual(xItem, yItem, ref tolerance, comparisonState);
                if (result == EqualMethodResult.ComparedNotEqual)
                    return result;
                if (result == EqualMethodResult.ToleranceNotSupported)
                    redoWithoutTolerance |= 1U << i;
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
                        object? xItem = xTuple[i];
                        object? yItem = yTuple[i];

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
