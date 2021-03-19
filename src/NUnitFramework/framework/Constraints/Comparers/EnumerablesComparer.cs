// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for two <see cref="IEnumerable"/>s.
    /// </summary>
    internal sealed class EnumerablesComparer : IChainComparer
    {
        private readonly NUnitEqualityComparer _equalityComparer;

        internal EnumerablesComparer(NUnitEqualityComparer equalityComparer)
        {
            _equalityComparer = equalityComparer;
        }

        public bool? Equal(object x, object y, ref Tolerance tolerance, ComparisonState state)
        {
            if (!(x is IEnumerable xIEnumerable) || !(y is IEnumerable yIEnumerable))
                return null;

            var expectedEnum = xIEnumerable.GetEnumerator();
            using (expectedEnum as IDisposable)
            {
                var actualEnum = yIEnumerable.GetEnumerator();
                using (actualEnum as IDisposable)
                {
                    for (int count = 0; ; count++)
                    {
                        bool expectedHasData = expectedEnum.MoveNext();
                        bool actualHasData = actualEnum.MoveNext();

                        if (!expectedHasData && !actualHasData)
                            return true;

                        if (expectedHasData != actualHasData ||
                            !_equalityComparer.AreEqual(expectedEnum.Current, actualEnum.Current, ref tolerance, state.PushComparison(x, y)))
                        {
                            NUnitEqualityComparer.FailurePoint fp = new NUnitEqualityComparer.FailurePoint();
                            fp.Position = count;
                            fp.ExpectedHasData = expectedHasData;
                            if (expectedHasData)
                                fp.ExpectedValue = expectedEnum.Current;
                            fp.ActualHasData = actualHasData;
                            if (actualHasData)
                                fp.ActualValue = actualEnum.Current;
                            _equalityComparer.FailurePoints.Insert(0, fp);
                            return false;
                        }
                    }
                }
            }
        }
    }
}
