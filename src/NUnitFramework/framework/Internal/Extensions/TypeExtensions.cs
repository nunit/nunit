// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Linq;

namespace NUnit.Framework.Internal.Extensions
{
    internal static class TypeExtensions
    {
        public static bool ImplementsIComparable(this Type type) =>
            type?.GetInterface("System.IComparable") != null;

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
    }
}
