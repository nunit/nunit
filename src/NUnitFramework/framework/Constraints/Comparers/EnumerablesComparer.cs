// ***********************************************************************
// Copyright (c) 2009 Charlie Poole, Rob Prouse
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
using System.Collections;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for two <see cref="IEnumerable"/>s.
    /// </summary>
    internal sealed class EnumerablesComparer : ChainComparer<IEnumerable>
    {
        private readonly NUnitEqualityComparer _equalityComparer;

        internal EnumerablesComparer(NUnitEqualityComparer equalityComparer)
        {
            _equalityComparer = equalityComparer;
        }

        public override bool Equals(IEnumerable x, IEnumerable y, ref Tolerance tolerance)
        {
            if (ReferenceEquals(x, y))return true;
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;

            IEnumerator expectedEnum = x.GetEnumerator();
            IEnumerator actualEnum = y.GetEnumerator();
            try
            {
                for (int count = 0;; count++)
                {
                    bool expectedHasData = expectedEnum.MoveNext();
                    bool actualHasData = actualEnum.MoveNext();

                    if (!expectedHasData && !actualHasData)
                        return true;

                    if (expectedHasData != actualHasData ||
                        !_equalityComparer.AreEqual(expectedEnum.Current, actualEnum.Current, ref tolerance, topLevelComparison: false))
                    {
                        _equalityComparer.FailurePoints.Insert(0, 
                            new NUnitEqualityComparer.FailurePoint
                            {
                                Position = count,
                                ExpectedHasData = expectedHasData,
                                ExpectedValue = expectedHasData ? expectedEnum.Current : null,
                                ActualHasData = actualHasData,
                                ActualValue = actualHasData ? actualEnum.Current : null,
                            });
                        return false;
                    }
                }
            }
            finally
            {
                var expectedDisposable = expectedEnum as IDisposable;
                if (expectedDisposable != null) expectedDisposable.Dispose();

                var actualDisposable = actualEnum as IDisposable;
                if (actualDisposable != null) actualDisposable.Dispose();
            }
        }

        public override int GetHashCode(IEnumerable enumerable)
        {
            return new HashCodeBuilder(_equalityComparer)
               .AppendAll(enumerable)
               .GetHashCode();
        }
    }
}
