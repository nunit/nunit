// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

// ReSharper disable once CheckNamespace
namespace NUnit.Framework
{
    /// <summary>
    /// C#14 static extension methods for type assertions
    /// </summary>
    public static class LegacyAssertTypeExtensions
    {
        extension(Assert)
        {
            #region IsAssignableFrom

            public static void IsAssignableFrom(Type expected, object? actual, string? message = null, params object?[]? args)
                => Legacy.ClassicAssert.IsAssignableFrom(expected, actual, message ?? string.Empty, args);

            public static void IsAssignableFrom(Type expected, object? actual)
                => Legacy.ClassicAssert.IsAssignableFrom(expected, actual);

            public static void IsAssignableFrom<TExpected>(object? actual, string? message = null, params object?[]? args)
                => Legacy.ClassicAssert.IsAssignableFrom<TExpected>(actual, message ?? string.Empty, args);

            public static void IsAssignableFrom<TExpected>(object? actual)
                => Legacy.ClassicAssert.IsAssignableFrom<TExpected>(actual);

            #endregion

            #region IsNotAssignableFrom

            public static void IsNotAssignableFrom(Type expected, object? actual, string? message = null, params object?[]? args)
                => Legacy.ClassicAssert.IsNotAssignableFrom(expected, actual, message ?? string.Empty, args);

            public static void IsNotAssignableFrom(Type expected, object? actual)
                => Legacy.ClassicAssert.IsNotAssignableFrom(expected, actual);

            public static void IsNotAssignableFrom<TExpected>(object? actual, string? message = null, params object?[]? args)
                => Legacy.ClassicAssert.IsNotAssignableFrom<TExpected>(actual, message ?? string.Empty, args);

            public static void IsNotAssignableFrom<TExpected>(object? actual)
                => Legacy.ClassicAssert.IsNotAssignableFrom<TExpected>(actual);

            #endregion

            #region IsInstanceOf

            public static void IsInstanceOf(Type expected, object? actual, string? message = null, params object?[]? args)
                => Legacy.ClassicAssert.IsInstanceOf(expected, actual, message ?? string.Empty, args);

            public static void IsInstanceOf(Type expected, object? actual)
                => Legacy.ClassicAssert.IsInstanceOf(expected, actual);

            public static void IsInstanceOf<TExpected>(object? actual, string? message = null, params object?[]? args)
                => Legacy.ClassicAssert.IsInstanceOf<TExpected>(actual, message ?? string.Empty, args);

            public static void IsInstanceOf<TExpected>(object? actual)
                => Legacy.ClassicAssert.IsInstanceOf<TExpected>(actual);

            #endregion

            #region IsNotInstanceOf

            public static void IsNotInstanceOf(Type expected, object? actual, string? message = null, params object?[]? args)
                => Legacy.ClassicAssert.IsNotInstanceOf(expected, actual, message ?? string.Empty, args);

            public static void IsNotInstanceOf(Type expected, object? actual)
                => Legacy.ClassicAssert.IsNotInstanceOf(expected, actual);

            public static void IsNotInstanceOf<TExpected>(object? actual, string? message = null, params object?[]? args)
                => Legacy.ClassicAssert.IsNotInstanceOf<TExpected>(actual, message ?? string.Empty, args);

            public static void IsNotInstanceOf<TExpected>(object? actual)
                => Legacy.ClassicAssert.IsNotInstanceOf<TExpected>(actual);

            #endregion
        }
    }
}
