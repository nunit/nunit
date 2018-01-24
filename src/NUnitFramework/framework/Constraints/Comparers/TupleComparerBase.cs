// ***********************************************************************
// Copyright (c) 2017 Charlie Poole, Rob Prouse
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
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

        public bool? Equal(object x, object y, ref Tolerance tolerance, bool topLevelComparison = true)
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

                bool comparison = _equalityComparer.AreEqual(xItem, yItem, ref tolerance, false);
                if (!comparison)
                    return false;
            }

            return true;
        }
    }
}
