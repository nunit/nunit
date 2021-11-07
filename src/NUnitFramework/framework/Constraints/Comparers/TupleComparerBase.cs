// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Reflection;
using NUnit.Compatibility;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Base class for comparators for tuples (both regular Tuples and ValueTuples).
    /// </summary>
    internal abstract class TupleComparerBase : IChainComparer
    {
        private readonly NUnitEqualityComparer _equalityComparer;

        internal TupleComparerBase(NUnitEqualityComparer equalityComparer)
        {
            _equalityComparer = equalityComparer;
        }

        protected abstract bool IsCorrectType(Type type);

        protected abstract object GetValue(Type type, string propertyName, object obj);

        public bool? Equal(object x, object y, ref Tolerance tolerance, ComparisonState state)
        {
            Type xType = x.GetType();
            Type yType = y.GetType();

            if (!IsCorrectType(xType) || !IsCorrectType(yType))
                return null;

            int numberOfGenericArgs = xType.GetGenericArguments().Length;

            if (numberOfGenericArgs != yType.GetGenericArguments().Length)
                return false;

            for (int i = 0; i < numberOfGenericArgs; i++)
            {
                string propertyName = i < 7 ? "Item" + (i + 1) : "Rest";
                object xItem = GetValue(xType, propertyName, x);
                object yItem = GetValue(yType, propertyName, y);

                bool comparison = _equalityComparer.AreEqual(xItem, yItem, ref tolerance, state.PushComparison(x, y));
                if (!comparison)
                    return false;
            }

            return true;
        }
    }
}
