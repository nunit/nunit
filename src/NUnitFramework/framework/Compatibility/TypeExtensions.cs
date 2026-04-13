// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace System
{
#if NETFRAMEWORK
    internal static class TypeExtensions
    {
        /// <summary>
        /// Determines whether the specified type can be assigned a value of the specified <paramref name="baseType"/>.
        /// This extension method is provided for .NET Framework compatibility, as IsAssignableTo was introduced in .NET 5.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <param name="baseType">The base type to check assignability against.</param>
        /// <returns>true if <paramref name="type"/> can be assigned to <paramref name="baseType"/>; otherwise, false.</returns>
        public static bool IsAssignableTo(this Type type, Type baseType)
        {
            if (type is null || baseType is null)
                return false;

            return baseType.IsAssignableFrom(type);
        }
    }
#endif
}
