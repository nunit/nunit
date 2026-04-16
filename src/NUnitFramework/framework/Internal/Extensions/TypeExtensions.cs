// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Linq;
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Internal.Extensions
{
    internal static class TypeExtensions
    {
        public static bool ImplementsIComparable(this Type type) =>
            type?.GetInterface("System.IComparable") is not null;

        public static bool ImplementsIComparable<T>() =>
            typeof(IComparable).IsAssignableFrom(typeof(T));

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

        public static bool CanUseDefaultEquality(this Type type)
        {
            if (type.IsPrimitive)
                return true;
            else if (type.IsEnum)
                return true;
            else if (type == typeof(string) || Numerics.IsNumericType(type) || type == typeof(DateTime))
                return true;

            if (TypeHelper.IsTuple(type) || TypeHelper.IsValueTuple(type))
            {
                return type.GetGenericArguments()
                           .All(arg => arg.CanUseDefaultEquality());
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified type parameter T is sortable.
        /// </summary>
        /// <typeparam name="T">The type to check for sortability.</typeparam>
        /// <returns>True if the type T can be inferred as sortable; otherwise, false.</returns>
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
