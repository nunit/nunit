// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#if NETFRAMEWORK

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System
{
    /// <summary>
    /// Extension methods for <see cref="ArgumentOutOfRangeException"/>.
    /// </summary>
    internal static class ArgumentOutOfRangeExceptionExtensions
    {
        extension(ArgumentOutOfRangeException)
        {
            /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is negative.</summary>
            /// <param name="value">The argument to validate as non-negative.</param>
            /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
            public static void ThrowIfNegative(int value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
            {
                if (value < 0)
                {
                    ThrowNegative(value, paramName);
                }
            }

            /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is greater than or equal to <paramref name="other"/>.</summary>
            /// <param name="value">The argument to validate as less than <paramref name="other"/>.</param>
            /// <param name="other">The value to compare with <paramref name="value"/>.</param>
            /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
            public static void ThrowIfGreaterThanOrEqual<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
                where T : IComparable<T>
            {
                if (value.CompareTo(other) >= 0)
                {
                    ThrowGreaterEqual(value, other, paramName);
                }
            }
        }

        [DoesNotReturn]
        private static void ThrowNegative(int value, string? paramName) =>
            throw new ArgumentOutOfRangeException(paramName, value, "Value must be non-negative.");

        [DoesNotReturn]
        private static void ThrowGreaterEqual<T>(T value, T other, string? paramName) =>
            throw new ArgumentOutOfRangeException(paramName, value, $"Value must be less than {other}.");
    }
}

#endif
