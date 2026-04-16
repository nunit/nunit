// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace NUnit.Framework.Internal.Extensions
{
    internal static class IEnumerableExtensions
    {
        /// <summary>
        /// Determines whether the specified enumerable collection's items can be inferred as sortable.
        /// </summary>
        /// <param name="collection">The collection to check (null collections are considered not sortable).</param>
        /// <returns>True if the items in the collection can be inferred as sortable.</returns>
        public static bool IsSortable(this IEnumerable? collection)
        {
            if (collection is null)
                return false;

            if (collection is StringCollection)
                return true;

            var collectionType = collection.GetType();

            var @interface = collectionType
                .GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType &&
                    i.Namespace == "System.Collections.Generic" &&
                    i.Name == "IEnumerable`1");

            var itemType = @interface?.GetGenericArguments()
                                     .FirstOrDefault();

            if (itemType is null)
                return false;

            return itemType.IsSortable();
        }

        /// <summary>
        /// Determines whether the specified generic enumerable collection's item type is sortable.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to check (null collections are considered not sortable).</param>
        /// <returns>True if the item type T can be inferred as sortable.</returns>
        public static bool IsSortable<T>(this IEnumerable<T>? collection)
        {
            return collection is not null && TypeExtensions.IsSortable<T>();
        }
    }
}
