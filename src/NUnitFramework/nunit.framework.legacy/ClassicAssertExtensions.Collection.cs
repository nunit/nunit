// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;

// ReSharper disable once CheckNamespace
namespace NUnit.Framework
{
    /// <summary>
    /// C#14 static extension methods for CollectionAssert methods on Assert
    /// </summary>
    public static partial class ClassicAssertExtensions
    {
        extension(Assert)
        {
            #region AllItemsAreInstancesOfType

            /// <inheritdoc cref="Legacy.CollectionAssert.AllItemsAreInstancesOfType(IEnumerable, Type)"/>
            public static void AllItemsAreInstancesOfType(IEnumerable collection, Type expectedType)
                => Legacy.CollectionAssert.AllItemsAreInstancesOfType(collection, expectedType);
            /// <inheritdoc cref="Legacy.CollectionAssert.AllItemsAreInstancesOfType(IEnumerable, Type, string, object[])"/>
            public static void AllItemsAreInstancesOfType(IEnumerable collection, Type expectedType, string message, params object?[]? args)
                => Legacy.CollectionAssert.AllItemsAreInstancesOfType(collection, expectedType, message, args);

            #endregion

            #region AllItemsAreNotNull

            /// <inheritdoc cref="Legacy.CollectionAssert.AllItemsAreNotNull(IEnumerable)"/>
            public static void AllItemsAreNotNull(IEnumerable collection)
                => Legacy.CollectionAssert.AllItemsAreNotNull(collection);
            /// <inheritdoc cref="Legacy.CollectionAssert.AllItemsAreNotNull(IEnumerable, string, object[])"/>
            public static void AllItemsAreNotNull(IEnumerable collection, string message, params object?[]? args)
                => Legacy.CollectionAssert.AllItemsAreNotNull(collection, message, args);

            #endregion

            #region AllItemsAreUnique

            /// <inheritdoc cref="Legacy.CollectionAssert.AllItemsAreUnique(IEnumerable)"/>
            public static void AllItemsAreUnique(IEnumerable collection)
                => Legacy.CollectionAssert.AllItemsAreUnique(collection);
            /// <inheritdoc cref="Legacy.CollectionAssert.AllItemsAreUnique(IEnumerable, string, object[])"/>
            public static void AllItemsAreUnique(IEnumerable collection, string message, params object?[]? args)
                => Legacy.CollectionAssert.AllItemsAreUnique(collection, message, args);

            #endregion

            #region AreEquivalent

            /// <inheritdoc cref="Legacy.CollectionAssert.AreEquivalent(IEnumerable, IEnumerable)"/>
            public static void AreEquivalent(IEnumerable expected, IEnumerable actual)
                => Legacy.CollectionAssert.AreEquivalent(expected, actual);
            /// <inheritdoc cref="Legacy.CollectionAssert.AreEquivalent(IEnumerable, IEnumerable, string, object[])"/>
            public static void AreEquivalent(IEnumerable expected, IEnumerable actual, string message, params object?[]? args)
                => Legacy.CollectionAssert.AreEquivalent(expected, actual, message, args);

            #endregion

            #region AreNotEquivalent

            /// <inheritdoc cref="Legacy.CollectionAssert.AreNotEquivalent(IEnumerable, IEnumerable)"/>
            public static void AreNotEquivalent(IEnumerable expected, IEnumerable actual)
                => Legacy.CollectionAssert.AreNotEquivalent(expected, actual);
            /// <inheritdoc cref="Legacy.CollectionAssert.AreNotEquivalent(IEnumerable, IEnumerable, string, object[])"/>
            public static void AreNotEquivalent(IEnumerable expected, IEnumerable actual, string message, params object?[]? args)
                => Legacy.CollectionAssert.AreNotEquivalent(expected, actual, message, args);

            #endregion

            #region IsSubsetOf

            /// <inheritdoc cref="Legacy.CollectionAssert.IsSubsetOf(IEnumerable, IEnumerable)"/>
            public static void IsSubsetOf(IEnumerable subset, IEnumerable superset)
                => Legacy.CollectionAssert.IsSubsetOf(subset, superset);
            /// <inheritdoc cref="Legacy.CollectionAssert.IsSubsetOf(IEnumerable, IEnumerable, string, object[])"/>
            public static void IsSubsetOf(IEnumerable subset, IEnumerable superset, string message, params object?[]? args)
                => Legacy.CollectionAssert.IsSubsetOf(subset, superset, message, args);

            #endregion

            #region IsNotSubsetOf

            /// <inheritdoc cref="Legacy.CollectionAssert.IsNotSubsetOf(IEnumerable, IEnumerable)"/>
            public static void IsNotSubsetOf(IEnumerable subset, IEnumerable superset)
                => Legacy.CollectionAssert.IsNotSubsetOf(subset, superset);
            /// <inheritdoc cref="Legacy.CollectionAssert.IsNotSubsetOf(IEnumerable, IEnumerable, string, object[])"/>
            public static void IsNotSubsetOf(IEnumerable subset, IEnumerable superset, string message, params object?[]? args)
                => Legacy.CollectionAssert.IsNotSubsetOf(subset, superset, message, args);

            #endregion

            #region IsSupersetOf

            /// <inheritdoc cref="Legacy.CollectionAssert.IsSupersetOf(IEnumerable, IEnumerable)"/>
            public static void IsSupersetOf(IEnumerable superset, IEnumerable subset)
                => Legacy.CollectionAssert.IsSupersetOf(superset, subset);
            /// <inheritdoc cref="Legacy.CollectionAssert.IsSupersetOf(IEnumerable, IEnumerable, string, object[])"/>
            public static void IsSupersetOf(IEnumerable superset, IEnumerable subset, string message, params object?[]? args)
                => Legacy.CollectionAssert.IsSupersetOf(superset, subset, message, args);

            #endregion

            #region IsNotSupersetOf

            /// <inheritdoc cref="Legacy.CollectionAssert.IsNotSupersetOf(IEnumerable, IEnumerable)"/>
            public static void IsNotSupersetOf(IEnumerable superset, IEnumerable subset)
                => Legacy.CollectionAssert.IsNotSupersetOf(superset, subset);
            /// <inheritdoc cref="Legacy.CollectionAssert.IsNotSupersetOf(IEnumerable, IEnumerable, string, object[])"/>
            public static void IsNotSupersetOf(IEnumerable superset, IEnumerable subset, string message, params object?[]? args)
                => Legacy.CollectionAssert.IsNotSupersetOf(superset, subset, message, args);

            #endregion

            #region DoesNotContain

            /// <inheritdoc cref="Legacy.CollectionAssert.DoesNotContain(IEnumerable, object)"/>
            public static void DoesNotContain(IEnumerable collection, object actual)
                => Legacy.CollectionAssert.DoesNotContain(collection, actual);
            /// <inheritdoc cref="Legacy.CollectionAssert.DoesNotContain(IEnumerable, object, string, object[])"/>
            public static void DoesNotContain(IEnumerable collection, object actual, string message, params object?[]? args)
                => Legacy.CollectionAssert.DoesNotContain(collection, actual, message, args);

            #endregion

            #region IsOrdered

            /// <inheritdoc cref="Legacy.CollectionAssert.IsOrdered(IEnumerable)"/>
            public static void IsOrdered(IEnumerable collection)
                => Legacy.CollectionAssert.IsOrdered(collection);
            /// <inheritdoc cref="Legacy.CollectionAssert.IsOrdered(IEnumerable, string, object[])"/>
            public static void IsOrdered(IEnumerable collection, string message, params object?[]? args)
                => Legacy.CollectionAssert.IsOrdered(collection, message, args);

            /// <inheritdoc cref="Legacy.CollectionAssert.IsOrdered(IEnumerable, IComparer)"/>
            public static void IsOrdered(IEnumerable collection, IComparer comparer)
                => Legacy.CollectionAssert.IsOrdered(collection, comparer);
            /// <inheritdoc cref="Legacy.CollectionAssert.IsOrdered(IEnumerable, IComparer, string, object[])"/>
            public static void IsOrdered(IEnumerable collection, IComparer comparer, string message, params object?[]? args)
                => Legacy.CollectionAssert.IsOrdered(collection, comparer, message, args);

            #endregion
        }
    }
}
