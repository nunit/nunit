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

            bool useCache = !equalityComparer.IsModified;
            Type? cachedType = null;
            NUnitEqualityComparer.EqualMethod? cachedMethod = null;

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
                        else if (useCache)
                        {
                            result = CompareItems(expectedEnum.Current, actualEnum.Current, ref tolerance, comparisonState, equalityComparer, ref cachedType, ref cachedMethod);
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

        /// <summary>
        /// Compares two items, reusing the comparer found for the previous items if they have the same type.
        /// </summary>
        private static EqualMethodResult CompareItems(object? expected, object? actual, ref Tolerance tolerance, ComparisonState state,
            NUnitEqualityComparer equalityComparer, ref Type? cachedType, ref NUnitEqualityComparer.EqualMethod? cachedMethod)
        {
            if (expected is null || actual is null ||
                expected.GetType() != actual.GetType() || state.DidCompare(expected, actual))
            {
                return equalityComparer.AreEqual(expected, actual, ref tolerance, state);
            }

            if (ReferenceEquals(expected, actual))
                return EqualMethodResult.ComparedEqual;

            if (expected.GetType() != cachedType)
            {
                cachedType = expected.GetType();
                cachedMethod = equalityComparer.FindEqualMethod(expected, actual, ref tolerance, state, out EqualMethodResult result);
                if (cachedMethod is not null)
                    return result;
            }

            return cachedMethod is not null
                ? cachedMethod(expected, actual, ref tolerance, state, equalityComparer)
                : equalityComparer.AreEqual(expected, actual, ref tolerance, state);
        }
    }
}
