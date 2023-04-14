// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections;
using System.Collections.Specialized;
using System.Linq;

namespace NUnit.Framework.Internal.Extensions
{
    internal static class IEnumerableExtensions
    {
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
    }
}
