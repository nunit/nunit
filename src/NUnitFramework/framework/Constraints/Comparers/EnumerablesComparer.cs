// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;

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

            Type? commonElementType = GetCommonGenericElementType(x, y);
            bool canCache = commonElementType is not null
                && !equalityComparer.IsModified;

            NUnitEqualityComparer.EqualMethod? cachedMethod = null;
            bool checkedForCachedMethod = false;

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
                            object? expectedItem = expectedEnum.Current;
                            object? actualItem = actualEnum.Current;

                            if (object.ReferenceEquals(expectedItem, actualItem))
                            {
                                result = EqualMethodResult.ComparedEqual;
                            }
                            else if (expectedItem is null || actualItem is null)
                            {
                                result = EqualMethodResult.ComparedNotEqual;
                            }
                            else
                            {
                                if (canCache && !checkedForCachedMethod)
                                {
                                    checkedForCachedMethod = true;
                                    cachedMethod = equalityComparer.FindEqualMethod(expectedItem, actualItem, ref tolerance, comparisonState, out result);

                                    if (cachedMethod is null)
                                    {
                                        result = equalityComparer.AreEqual(expectedItem, actualItem, ref tolerance, comparisonState);
                                    }
                                }
                                else if (cachedMethod is not null)
                                {
                                    result = cachedMethod(expectedItem, actualItem, ref tolerance, comparisonState, equalityComparer);
                                    if (result == EqualMethodResult.TypesNotSupported)
                                    {
                                        result = equalityComparer.AreEqual(expectedItem, actualItem, ref tolerance, comparisonState);
                                    }
                                }
                                else
                                {
                                    result = equalityComparer.AreEqual(expectedItem, actualItem, ref tolerance, comparisonState);
                                }
                            }
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

        #region Helper Methods
        private static Type? GetCommonGenericElementType(object x, object y)
        {
            Type xType = x.GetType();
            Type yType = y.GetType();

            if (xType.IsArray && yType.IsArray)
            {
                Type? xElem = xType.GetElementType();
                Type? yElem = yType.GetElementType();
                if (xElem is not null && xElem == yElem && xElem != typeof(object))
                    return xElem;
            }

            Type? xInterface = FindGenericEnumerable(xType);
            Type? yInterface = FindGenericEnumerable(yType);

            if (xInterface is not null && yInterface is not null && xInterface == yInterface)
            {
                Type genericArg = xInterface.GetGenericArguments()[0];
                if (genericArg != typeof(object))
                    return genericArg;
            }

            return null;

            static Type? FindGenericEnumerable(Type type)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    return type;

                foreach (Type iface in type.GetInterfaces())
                {
                    if (iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                        return iface;
                }

                return null;
            }
        }
        #endregion
    }
}
