// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// Class used to guard against unexpected argument values
    /// or operations by throwing an appropriate exception.
    /// </summary>
    internal static class Guard
    {
        /// <summary>
        /// Throws an exception if an argument is null
        /// </summary>
        /// <param name="value">The value to be tested</param>
        /// <param name="name">The name of the argument</param>
        public static void ArgumentNotNull([NotNull] object? value, string name)
        {
            if (value is null)
                ThrowArgumentNullException(name);

            [DoesNotReturn]
            [MethodImpl(MethodImplOptions.NoInlining)]
            static void ThrowArgumentNullException(string name) => throw new ArgumentNullException(name, "Argument " + name + " must not be null");
        }

        /// <summary>
        /// Throws an exception if a string argument is null or empty
        /// </summary>
        /// <param name="value">The value to be tested</param>
        /// <param name="name">The name of the argument</param>
        public static void ArgumentNotNullOrEmpty([NotNull] string? value, string name)
        {
            ArgumentNotNull(value, name);

            if (value == string.Empty)
                ThrowArgumentNotNullOrEmpty(name);

            [DoesNotReturn]
            [MethodImpl(MethodImplOptions.NoInlining)]
            static void ThrowArgumentNotNullOrEmpty(string name) => throw new ArgumentException("Argument " + name + " must not be the empty string", name);
        }

        /// <summary>
        /// Throws an ArgumentOutOfRangeException if the specified condition is not met.
        /// </summary>
        /// <param name="condition">The condition that must be met</param>
        /// <param name="message">The exception message to be used</param>
        /// <param name="paramName">The name of the argument</param>
        public static void ArgumentInRange([DoesNotReturnIf(false)] bool condition, string message, string paramName)
        {
            if (!condition)
                ThrowArgumentOutOfRangeException(message, paramName);

            [DoesNotReturn]
            [MethodImpl(MethodImplOptions.NoInlining)]
            static void ThrowArgumentOutOfRangeException(string message, string paramName) => throw new ArgumentOutOfRangeException(paramName, message);
        }

        /// <summary>
        /// Throws an ArgumentException if the specified condition is not met.
        /// </summary>
        /// <param name="condition">The condition that must be met</param>
        /// <param name="message">The exception message to be used</param>
        /// <param name="paramName">The name of the argument</param>
        public static void ArgumentValid([DoesNotReturnIf(false)] bool condition, string message, string paramName)
        {
            if (!condition)
                ThrowArgumentException(message, paramName);

            [DoesNotReturn]
            [MethodImpl(MethodImplOptions.NoInlining)]
            static void ThrowArgumentException(string message, string paramName) => throw new ArgumentException(message, paramName);
        }

        /// <summary>
        /// Throws an InvalidOperationException if the specified condition is not met.
        /// </summary>
        /// <param name="condition">The condition that must be met</param>
        /// <param name="message">The exception message to be used</param>
        public static void OperationValid([DoesNotReturnIf(false)] bool condition, string message)
        {
            if (!condition)
                ThrowInvalidOperationException(message);

            [DoesNotReturn]
            [MethodImpl(MethodImplOptions.NoInlining)]
            static void ThrowInvalidOperationException(string message) => throw new InvalidOperationException(message);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the specified delegate is <c>async void</c>.
        /// </summary>
        public static void ArgumentNotAsyncVoid(Delegate @delegate, string paramName)
        {
            ArgumentNotAsyncVoid(@delegate.GetMethodInfo(), paramName);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the specified delegate is <c>async void</c>.
        /// </summary>
        public static void ArgumentNotAsyncVoid(MethodInfo method, string paramName)
        {
            if (method.ReturnType != typeof(void)) return;
            if (!AsyncToSyncAdapter.IsAsyncOperation(method)) return;

            ThrowArgumentNotAsyncVoid(paramName);

            [DoesNotReturn]
            [MethodImpl(MethodImplOptions.NoInlining)]
            static void ThrowArgumentNotAsyncVoid(string paramName) => throw new ArgumentException("Async void methods are not supported. Please use 'async Task' instead.", paramName);
        }
    }
}
