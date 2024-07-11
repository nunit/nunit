// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for two <see cref="IEnumerable"/>s.
    /// </summary>
    internal static class EnumerablesComparer
    {
        public static EqualMethodResult Equal(object x, object y, ref Tolerance tolerance, ComparisonState state, NUnitEqualityComparer equalityComparer)
        {
            if (x is not IEnumerable xIEnumerable || y is not IEnumerable yIEnumerable)
                return EqualMethodResult.TypesNotSupported;

            var expectedEnum = xIEnumerable.GetEnumerator();
            using (expectedEnum as IDisposable)
            {
                var actualEnum = yIEnumerable.GetEnumerator();
                using (actualEnum as IDisposable)
                {
                    ComparisonState comparisonState = state.PushComparison(x, y);
                    for (int count = 0; ; count++)
                    {
                        bool expectedHasData = expectedEnum.MoveNext();
                        bool actualHasData = actualEnum.MoveNext();

                        if (!expectedHasData && !actualHasData)
                            return EqualMethodResult.ComparedEqual;

                        EqualMethodResult result;

                        if (expectedHasData != actualHasData)
                        {
                            result = EqualMethodResult.ComparedNotEqual;
                        }
                        else
                        {
                            result = equalityComparer.AreEqual(expectedEnum.Current, actualEnum.Current, ref tolerance, comparisonState);
                        }

                        if (result != EqualMethodResult.ComparedEqual)
                        {
                            NUnitEqualityComparer.FailurePoint fp = new NUnitEqualityComparer.FailurePoint();
                            fp.Position = count;
                            fp.ExpectedHasData = expectedHasData;
                            if (expectedHasData)
                                fp.ExpectedValue = expectedEnum.Current;
                            fp.ActualHasData = actualHasData;
                            if (actualHasData)
                                fp.ActualValue = actualEnum.Current;
                            equalityComparer.FailurePoints.Insert(0, fp);
                            return result;
                        }
                    }
                }
            }
        }
    }
}
