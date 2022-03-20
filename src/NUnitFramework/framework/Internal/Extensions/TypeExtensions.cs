// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

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
                foreach (var typeArg in type.GetGenericArguments())
                    if (!typeArg.IsSortable())
                        return false;
            }

            return true;
        }
    }
}
