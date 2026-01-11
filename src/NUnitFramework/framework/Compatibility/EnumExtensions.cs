// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt
#if NETFRAMEWORK
namespace System
{
    /// <summary>
    /// Provides a polyfill for the generic Enum.Parse{T} method which is available in.
    /// .NET Core and newer .NET versions but missing in .NET Framework.
    /// </summary>
    internal static partial class EnumExtensions
    {
        extension(Enum)
        {
            /// <summary>
            /// Converts the string representation of the name or numeric value of one or more.
            /// enumerated constants to an equivalent enumerated object.
            /// </summary>
            /// <typeparam name="T">The enum type to which to convert.</typeparam>
            /// <param name="value">The string representation of the enumeration name or underlying value.</param>
            /// <returns>An object of type T whose value is represented by value.</returns>
            public static T Parse<T>(string value)
                where T : struct
            {
                return (T)System.Enum.Parse(typeof(T), value);
            }
            /// <summary>
            /// Converts the string representation of the name or numeric value of one or more.
            /// enumerated constants to an equivalent enumerated object. A parameter specifies.
            /// whether the operation is case-insensitive.
            /// </summary>
            /// <typeparam name="T">The enum type to which to convert.</typeparam>
            /// <param name="value">The string representation of the enumeration name or underlying value.</param>
            /// <param name="ignoreCase">true to ignore case; false to regard case.</param>
            /// <returns>An object of type T whose value is represented by value.</returns>
            public static T Parse<T>(string value, bool ignoreCase)
                where T : struct
            {
                if (!typeof(T).IsEnum)
                    throw new ArgumentException($"{typeof(T)} is not an enum");

                return (T)System.Enum.Parse(typeof(T), value, ignoreCase);
            }

        }
    }
}
#endif
