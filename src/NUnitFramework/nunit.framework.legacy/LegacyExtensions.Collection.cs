// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;

// ReSharper disable once CheckNamespace
namespace NUnit.Framework
{
    /// <summary>
    /// C#14 static extension methods for CollectionAssert methods on Assert
    /// </summary>
    public static class LegacyCollectionAssertExtensions
    {
        extension(Assert)
        {
            #region AllItemsAreInstancesOfType

            public static void AllItemsAreInstancesOfType(IEnumerable collection, Type expectedType)
                => Legacy.CollectionAssert.AllItemsAreInstancesOfType(collection, expectedType);

            public static void AllItemsAreInstancesOfType(IEnumerable collection, Type expectedType, string? message, params object?[]? args)
                => Legacy.CollectionAssert.AllItemsAreInstancesOfType(collection, expectedType, message ?? string.Empty, args);

            #endregion

            #region AllItemsAreNotNull

            public static void AllItemsAreNotNull(IEnumerable collection)
                => Legacy.CollectionAssert.AllItemsAreNotNull(collection);

            public static void AllItemsAreNotNull(IEnumerable collection, string? message, params object?[]? args)
                => Legacy.CollectionAssert.AllItemsAreNotNull(collection, message ?? string.Empty, args);

            #endregion

            #region AllItemsAreUnique

            public static void AllItemsAreUnique(IEnumerable collection)
                => Legacy.CollectionAssert.AllItemsAreUnique(collection);

            public static void AllItemsAreUnique(IEnumerable collection, string? message, params object?[]? args)
                => Legacy.CollectionAssert.AllItemsAreUnique(collection, message ?? string.Empty, args);

            #endregion

            #region AreEquivalent

            public static void AreEquivalent(IEnumerable expected, IEnumerable actual)
                => Legacy.CollectionAssert.AreEquivalent(expected, actual);

            public static void AreEquivalent(IEnumerable expected, IEnumerable actual, string? message, params object?[]? args)
                => Legacy.CollectionAssert.AreEquivalent(expected, actual, message ?? string.Empty, args);

            #endregion

            #region AreNotEquivalent

            public static void AreNotEquivalent(IEnumerable expected, IEnumerable actual)
                => Legacy.CollectionAssert.AreNotEquivalent(expected, actual);

            public static void AreNotEquivalent(IEnumerable expected, IEnumerable actual, string? message, params object?[]? args)
                => Legacy.CollectionAssert.AreNotEquivalent(expected, actual, message ?? string.Empty, args);

            #endregion

            #region IsSubsetOf

            public static void IsSubsetOf(IEnumerable subset, IEnumerable superset)
                => Legacy.CollectionAssert.IsSubsetOf(subset, superset);

            public static void IsSubsetOf(IEnumerable subset, IEnumerable superset, string? message, params object?[]? args)
                => Legacy.CollectionAssert.IsSubsetOf(subset, superset, message ?? string.Empty, args);

            #endregion

            #region IsNotSubsetOf

            public static void IsNotSubsetOf(IEnumerable subset, IEnumerable superset)
                => Legacy.CollectionAssert.IsNotSubsetOf(subset, superset);

            public static void IsNotSubsetOf(IEnumerable subset, IEnumerable superset, string? message, params object?[]? args)
                => Legacy.CollectionAssert.IsNotSubsetOf(subset, superset, message ?? string.Empty, args);

            #endregion

            #region IsSupersetOf

            public static void IsSupersetOf(IEnumerable superset, IEnumerable subset)
                => Legacy.CollectionAssert.IsSupersetOf(superset, subset);

            public static void IsSupersetOf(IEnumerable superset, IEnumerable subset, string? message, params object?[]? args)
                => Legacy.CollectionAssert.IsSupersetOf(superset, subset, message ?? string.Empty, args);

            #endregion

            #region IsNotSupersetOf

            public static void IsNotSupersetOf(IEnumerable superset, IEnumerable subset)
                => Legacy.CollectionAssert.IsNotSupersetOf(superset, subset);

            public static void IsNotSupersetOf(IEnumerable superset, IEnumerable subset, string? message, params object?[]? args)
                => Legacy.CollectionAssert.IsNotSupersetOf(superset, subset, message ?? string.Empty, args);

            #endregion

            #region DoesNotContain

            public static void DoesNotContain(IEnumerable collection, object actual)
                => Legacy.CollectionAssert.DoesNotContain(collection, actual);

            public static void DoesNotContain(IEnumerable collection, object actual, string? message, params object?[]? args)
                => Legacy.CollectionAssert.DoesNotContain(collection, actual, message ?? string.Empty, args);

            #endregion

            #region IsOrdered

            public static void IsOrdered(IEnumerable collection)
                => Legacy.CollectionAssert.IsOrdered(collection);

            public static void IsOrdered(IEnumerable collection, string? message, params object?[]? args)
                => Legacy.CollectionAssert.IsOrdered(collection, message ?? string.Empty, args);

            public static void IsOrdered(IEnumerable collection, IComparer comparer)
                => Legacy.CollectionAssert.IsOrdered(collection, comparer);

            public static void IsOrdered(IEnumerable collection, IComparer comparer, string? message, params object?[]? args)
                => Legacy.CollectionAssert.IsOrdered(collection, comparer, message ?? string.Empty, args);

            #endregion
        }
    }
}
