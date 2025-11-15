// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

// ReSharper disable once CheckNamespace
namespace NUnit.Framework
{
    /// <summary>
    /// C#14 static extension methods for numeric assertions
    /// </summary>
    public static class LegacyAssertNumericExtensions
    {
        extension(Assert)
        {
            #region Zero

            public static void Zero(int actual) => Legacy.ClassicAssert.Zero(actual);
            public static void Zero(int actual, string? message, params object?[]? args) => Legacy.ClassicAssert.Zero(actual, message ?? string.Empty, args);

            [System.CLSCompliant(false)]
            public static void Zero(uint actual) => Legacy.ClassicAssert.Zero(actual);
            [System.CLSCompliant(false)]
            public static void Zero(uint actual, string? message, params object?[]? args) => Legacy.ClassicAssert.Zero(actual, message ?? string.Empty, args);

            public static void Zero(long actual) => Legacy.ClassicAssert.Zero(actual);
            public static void Zero(long actual, string? message, params object?[]? args) => Legacy.ClassicAssert.Zero(actual, message ?? string.Empty, args);

            [System.CLSCompliant(false)]
            public static void Zero(ulong actual) => Legacy.ClassicAssert.Zero(actual);
            [System.CLSCompliant(false)]
            public static void Zero(ulong actual, string? message, params object?[]? args) => Legacy.ClassicAssert.Zero(actual, message ?? string.Empty, args);

            public static void Zero(decimal actual) => Legacy.ClassicAssert.Zero(actual);
            public static void Zero(decimal actual, string? message, params object?[]? args) => Legacy.ClassicAssert.Zero(actual, message ?? string.Empty, args);

            public static void Zero(double actual) => Legacy.ClassicAssert.Zero(actual);
            public static void Zero(double actual, string? message, params object?[]? args) => Legacy.ClassicAssert.Zero(actual, message ?? string.Empty, args);

            public static void Zero(float actual) => Legacy.ClassicAssert.Zero(actual);
            public static void Zero(float actual, string? message, params object?[]? args) => Legacy.ClassicAssert.Zero(actual, message ?? string.Empty, args);

            #endregion

            #region NotZero

            public static void NotZero(int actual) => Legacy.ClassicAssert.NotZero(actual);
            public static void NotZero(int actual, string? message, params object?[]? args) => Legacy.ClassicAssert.NotZero(actual, message ?? string.Empty, args);

            [System.CLSCompliant(false)]
            public static void NotZero(uint actual) => Legacy.ClassicAssert.NotZero(actual);
            [System.CLSCompliant(false)]
            public static void NotZero(uint actual, string? message, params object?[]? args) => Legacy.ClassicAssert.NotZero(actual, message ?? string.Empty, args);

            public static void NotZero(long actual) => Legacy.ClassicAssert.NotZero(actual);
            public static void NotZero(long actual, string? message, params object?[]? args) => Legacy.ClassicAssert.NotZero(actual, message ?? string.Empty, args);

            [System.CLSCompliant(false)]
            public static void NotZero(ulong actual) => Legacy.ClassicAssert.NotZero(actual);
            [System.CLSCompliant(false)]
            public static void NotZero(ulong actual, string? message, params object?[]? args) => Legacy.ClassicAssert.NotZero(actual, message ?? string.Empty, args);

            public static void NotZero(decimal actual) => Legacy.ClassicAssert.NotZero(actual);
            public static void NotZero(decimal actual, string? message, params object?[]? args) => Legacy.ClassicAssert.NotZero(actual, message ?? string.Empty, args);

            public static void NotZero(double actual) => Legacy.ClassicAssert.NotZero(actual);
            public static void NotZero(double actual, string? message, params object?[]? args) => Legacy.ClassicAssert.NotZero(actual, message ?? string.Empty, args);

            public static void NotZero(float actual) => Legacy.ClassicAssert.NotZero(actual);
            public static void NotZero(float actual, string? message, params object?[]? args) => Legacy.ClassicAssert.NotZero(actual, message ?? string.Empty, args);

            #endregion

            #region Positive

            public static void Positive(int actual) => Legacy.ClassicAssert.Positive(actual);
            public static void Positive(int actual, string? message, params object?[]? args) => Legacy.ClassicAssert.Positive(actual, message ?? string.Empty, args);

            [System.CLSCompliant(false)]
            public static void Positive(uint actual) => Legacy.ClassicAssert.Positive(actual);
            [System.CLSCompliant(false)]
            public static void Positive(uint actual, string? message, params object?[]? args) => Legacy.ClassicAssert.Positive(actual, message ?? string.Empty, args);

            public static void Positive(long actual) => Legacy.ClassicAssert.Positive(actual);
            public static void Positive(long actual, string? message, params object?[]? args) => Legacy.ClassicAssert.Positive(actual, message ?? string.Empty, args);

            [System.CLSCompliant(false)]
            public static void Positive(ulong actual) => Legacy.ClassicAssert.Positive(actual);
            [System.CLSCompliant(false)]
            public static void Positive(ulong actual, string? message, params object?[]? args) => Legacy.ClassicAssert.Positive(actual, message ?? string.Empty, args);

            public static void Positive(decimal actual) => Legacy.ClassicAssert.Positive(actual);
            public static void Positive(decimal actual, string? message, params object?[]? args) => Legacy.ClassicAssert.Positive(actual, message ?? string.Empty, args);

            public static void Positive(double actual) => Legacy.ClassicAssert.Positive(actual);
            public static void Positive(double actual, string? message, params object?[]? args) => Legacy.ClassicAssert.Positive(actual, message ?? string.Empty, args);

            public static void Positive(float actual) => Legacy.ClassicAssert.Positive(actual);
            public static void Positive(float actual, string? message, params object?[]? args) => Legacy.ClassicAssert.Positive(actual, message ?? string.Empty, args);

            #endregion

            #region Negative

            public static void Negative(int actual) => Legacy.ClassicAssert.Negative(actual);
            public static void Negative(int actual, string? message, params object?[]? args) => Legacy.ClassicAssert.Negative(actual, message ?? string.Empty, args);

            [System.CLSCompliant(false)]
            public static void Negative(uint actual) => Legacy.ClassicAssert.Negative(actual);
            [System.CLSCompliant(false)]
            public static void Negative(uint actual, string? message, params object?[]? args) => Legacy.ClassicAssert.Negative(actual, message ?? string.Empty, args);

            public static void Negative(long actual) => Legacy.ClassicAssert.Negative(actual);
            public static void Negative(long actual, string? message, params object?[]? args) => Legacy.ClassicAssert.Negative(actual, message ?? string.Empty, args);

            [System.CLSCompliant(false)]
            public static void Negative(ulong actual) => Legacy.ClassicAssert.Negative(actual);
            [System.CLSCompliant(false)]
            public static void Negative(ulong actual, string? message, params object?[]? args) => Legacy.ClassicAssert.Negative(actual, message ?? string.Empty, args);

            public static void Negative(decimal actual) => Legacy.ClassicAssert.Negative(actual);
            public static void Negative(decimal actual, string? message, params object?[]? args) => Legacy.ClassicAssert.Negative(actual, message ?? string.Empty, args);

            public static void Negative(double actual) => Legacy.ClassicAssert.Negative(actual);
            public static void Negative(double actual, string? message, params object?[]? args) => Legacy.ClassicAssert.Negative(actual, message ?? string.Empty, args);

            public static void Negative(float actual) => Legacy.ClassicAssert.Negative(actual);
            public static void Negative(float actual, string? message, params object?[]? args) => Legacy.ClassicAssert.Negative(actual, message ?? string.Empty, args);

            #endregion
        }
    }
}
