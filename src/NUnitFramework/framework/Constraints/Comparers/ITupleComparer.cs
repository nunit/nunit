// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Runtime.CompilerServices;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Abstractions;
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
            // A class derived from Tuple inherits ITuple, but would not check any extra members declared in the derived class.
            // We can therefore not use ITuple to check for equality.
            // The derived class must correctly implement IStructuralEquatable.Equals.
            Type xType = x.GetType();
            Type yType = y.GetType();

            if (!xType.IsTupleOrValueTuple() || !yType.IsTupleOrValueTuple())
                return EqualMethodResult.TypesNotSupported;

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

            IBits redoWithoutTolerance = BitsFactory.Create(numberOfElements);
            for (int i = 0; i < numberOfElements; i++)
            {
                object? xItem = xTuple[i];
                object? yItem = yTuple[i];

                EqualMethodResult result = equalityComparer.AreEqual(xItem, yItem, ref tolerance, comparisonState);
                if (result == EqualMethodResult.ComparedNotEqual)
                    return result;
                if (result == EqualMethodResult.ToleranceNotSupported)
                    redoWithoutTolerance.Set(i);
            }

            if (redoWithoutTolerance.BitsSet == numberOfElements)
                return EqualMethodResult.ToleranceNotSupported;

            if (redoWithoutTolerance.BitsSet != 0)
            {
                Tolerance noTolerance = Tolerance.Exact;
                for (int i = 0; i < numberOfElements; i++)
                {
                    if (redoWithoutTolerance.Get(i))
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
