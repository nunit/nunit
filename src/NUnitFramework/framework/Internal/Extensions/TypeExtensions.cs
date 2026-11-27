// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Linq;
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Internal.Extensions
{
    internal static class TypeExtensions
    {
        public static bool ImplementsIComparable(this Type type) =>
            type.GetInterface("System.IComparable") is not null;

        public static bool ImplementsIComparable<T>() =>
            typeof(IComparable).IsAssignableFrom(typeof(T));

        public static bool IsSortable(this Type type)
        {
            if (!type.ImplementsIComparable())
                return false;

            if (type.IsTupleOrValueTuple())
            {
                return type.GetGenericArguments()
                           .All(arg => arg.IsSortable());
            }

            return true;
        }

        /// <summary>
        /// Whether a predicate can be added for <paramref name="type"/>. Types that NUnit compares with their
        /// default equality or with a dedicated constraint, or that <see cref="Constraints.EqualConstraint"/> rewrites,
        /// never call the predicate on some paths.
        /// </summary>
        public static bool CanBeNullEquivalent(this Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;
            if (type.CanUseDefaultEquality() || type == typeof(TimeSpan) || type == typeof(DateTimeOffset))
                return false;

            return !(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ArraySegment<>));
        }

        public static bool CanUseDefaultEquality(this Type type)
        {
            if (type.IsPrimitive)
                return true;
            else if (type.IsEnum)
                return true;
            else if (type == typeof(string) || Numerics.IsNumericType(type) || type == typeof(DateTime))
                return true;

            if (type.IsTupleOrValueTuple())
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
            if (type.IsTupleOrValueTuple())
            {
                return type.GetGenericArguments()
                           .All(arg => arg.IsSortable());
            }

            return true;
        }
    }
}
