// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#if NETFRAMEWORK || NET6_0

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System
{
    /// <summary>
    /// Extension methods for <see cref="ArgumentException"/>.
    /// </summary>
    internal static class ArgumentExceptionExtensions
    {
        extension(ArgumentException)
        {
            /// <summary>Throws an exception if <paramref name="argument"/> is null or empty.</summary>
            /// <param name="argument">The string argument to validate as non-null and non-empty.</param>
            /// <param name="paramName">The name of the parameter with which <paramref name="argument"/> corresponds.</param>
            /// <exception cref="ArgumentNullException"><paramref name="argument"/> is null.</exception>
            /// <exception cref="ArgumentException"><paramref name="argument"/> is empty.</exception>
            public static void ThrowIfNullOrEmpty([NotNull] string? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
            {
                if (string.IsNullOrEmpty(argument))
                {
                    ThrowNullOrEmptyException(argument, paramName);
                }
            }

            /// <summary>Throws an exception if <paramref name="argument"/> is null, empty, or consists only of white-space characters.</summary>
            /// <param name="argument">The string argument to validate.</param>
            /// <param name="paramName">The name of the parameter with which <paramref name="argument"/> corresponds.</param>
            /// <exception cref="ArgumentNullException"><paramref name="argument"/> is null.</exception>
            /// <exception cref="ArgumentException"><paramref name="argument"/> is empty or consists only of white-space characters.</exception>
            public static void ThrowIfNullOrWhiteSpace([NotNull] string? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
            {
                if (string.IsNullOrWhiteSpace(argument))
                {
                    ThrowNullOrWhiteSpaceException(argument, paramName);
                }
            }
        }

        [DoesNotReturn]
        private static void ThrowNullOrEmptyException(string? argument, string? paramName)
        {
            ArgumentNullException.ThrowIfNull(argument, paramName);
            throw new ArgumentException("The value cannot be an empty string.", paramName);
        }

        [DoesNotReturn]
        private static void ThrowNullOrWhiteSpaceException(string? argument, string? paramName)
        {
            ArgumentNullException.ThrowIfNull(argument, paramName);
            throw new ArgumentException("The value cannot be an empty string or composed entirely of whitespace.", paramName);
        }
    }
}

#endif
