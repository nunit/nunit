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
using System.Collections.Generic;
using System.Reflection;
using NUnit.Compatibility;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Base class for comparators for tuples (both regular Tuples and ValueTuples).
    /// </summary>
    internal abstract class TupleComparerBase : ChainComparer
    {
        private readonly NUnitEqualityComparer _equalityComparer;

        protected TupleComparerBase(NUnitEqualityComparer equalityComparer)
        {
            _equalityComparer = equalityComparer;
        }

        public override bool? Equals(object x, object y, ref Tolerance tolerance)
        {
            if (!CanCompare(x) || !CanCompare(y)) return null;

            using (var xValues = GetPropertyValues(x).GetEnumerator())
            using (var yValues = GetPropertyValues(y).GetEnumerator())
            {
                while (xValues.MoveNext())
                {
                    if (!yValues.MoveNext())
                        return false;
                    if (!_equalityComparer.AreEqual(xValues.Current, yValues.Current, ref tolerance, topLevelComparison: false))
                        return false;
                }

                return !yValues.MoveNext();
            }
        }

        public override int GetHashCode(object obj)
        {
            return new HashCodeBuilder(_equalityComparer)
               .AppendAll(GetPropertyValues(obj))
               .GetHashCode();
        }

        protected IEnumerable<object> GetPropertyValues(object obj)
        {
            Type type = obj.GetType();
            int numberOfGenericArgs = type.GetGenericArguments().Length;

            for (int i = 0; i < numberOfGenericArgs; i++)
            {
                string propertyName = i < 7 ? "Item" + (i + 1) : "Rest";
                PropertyInfo propertyInfo = type.GetProperty(propertyName);
                yield return propertyInfo != null
                    ? propertyInfo.GetValue(obj, null)
                    : null;
            }
        }
    }
}
