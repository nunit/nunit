// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Linq;

namespace NUnit.Framework.Internal.Extensions
{
    internal static class TypeExtensions
    {
        public static bool ImplementsIComparable(this Type type) =>
            type?.GetInterface("System.IComparable") is not null;

        public static bool ImplementsIComparable<T>() =>
            typeof(T).IsAssignableTo(typeof(IComparable));

        public static bool IsSortable(this Type type)
        {
            if (!type.ImplementsIComparable())
                return false;

            if (TypeHelper.IsTuple(type) || TypeHelper.IsValueTuple(type))
            {
                return type.GetGenericArguments()
                           .All(arg => arg.IsSortable());
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified type parameter T is sortable.
        /// </summary>
        /// <typeparam name="T">The type to check for sortability.</typeparam>
        /// <returns>true if T implements IComparable; otherwise, false.</returns>
        public static bool IsSortable<T>()
        {
            if (!ImplementsIComparable<T>())
                return false;

            var type = typeof(T);
            if (TypeHelper.IsTuple(type) || TypeHelper.IsValueTuple(type))
            {
                return type.GetGenericArguments()
                           .All(arg => arg.IsSortable());
            }

            return true;
        }
    }
}
