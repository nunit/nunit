// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

// ReSharper disable once CheckNamespace
namespace NUnit.Framework
{
    /// <summary>
    /// C#14 static extension methods for numeric assertions
    /// </summary>
    public static class ClassicAssertNumericExtensions
    {
        extension(Assert)
        {
            #region Zero

                /// <inheritdoc cref="Legacy.ClassicAssert.Zero(int)"/>
            public static void Zero(int actual) => Legacy.ClassicAssert.Zero(actual);
                /// <inheritdoc cref="Legacy.ClassicAssert.Zero(int, string, object[])"/>
                public static void Zero(int actual, string message, params object?[]? args) => Legacy.ClassicAssert.Zero(actual, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.Zero(uint)"/>
            [System.CLSCompliant(false)]
            public static void Zero(uint actual) => Legacy.ClassicAssert.Zero(actual);
                /// <inheritdoc cref="Legacy.ClassicAssert.Zero(uint, string, object[])"/>
            [System.CLSCompliant(false)]
                public static void Zero(uint actual, string message, params object?[]? args) => Legacy.ClassicAssert.Zero(actual, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.Zero(long)"/>
            public static void Zero(long actual) => Legacy.ClassicAssert.Zero(actual);
                /// <inheritdoc cref="Legacy.ClassicAssert.Zero(long, string, object[])"/>
                public static void Zero(long actual, string message, params object?[]? args) => Legacy.ClassicAssert.Zero(actual, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.Zero(ulong)"/>
            [System.CLSCompliant(false)]
            public static void Zero(ulong actual) => Legacy.ClassicAssert.Zero(actual);
                /// <inheritdoc cref="Legacy.ClassicAssert.Zero(ulong, string, object[])"/>
            [System.CLSCompliant(false)]
                public static void Zero(ulong actual, string message, params object?[]? args) => Legacy.ClassicAssert.Zero(actual, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.Zero(decimal)"/>
            public static void Zero(decimal actual) => Legacy.ClassicAssert.Zero(actual);
                /// <inheritdoc cref="Legacy.ClassicAssert.Zero(decimal, string, object[])"/>
                public static void Zero(decimal actual, string message, params object?[]? args) => Legacy.ClassicAssert.Zero(actual, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.Zero(double)"/>
            public static void Zero(double actual) => Legacy.ClassicAssert.Zero(actual);
                /// <inheritdoc cref="Legacy.ClassicAssert.Zero(double, string, object[])"/>
                public static void Zero(double actual, string message, params object?[]? args) => Legacy.ClassicAssert.Zero(actual, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.Zero(float)"/>
            public static void Zero(float actual) => Legacy.ClassicAssert.Zero(actual);
                /// <inheritdoc cref="Legacy.ClassicAssert.Zero(float, string, object[])"/>
                public static void Zero(float actual, string message, params object?[]? args) => Legacy.ClassicAssert.Zero(actual, message, args);

            #endregion

            #region NotZero

                /// <inheritdoc cref="Legacy.ClassicAssert.NotZero(int)"/>
            public static void NotZero(int actual) => Legacy.ClassicAssert.NotZero(actual);
                /// <inheritdoc cref="Legacy.ClassicAssert.NotZero(int, string, object[])"/>
                public static void NotZero(int actual, string message, params object?[]? args) => Legacy.ClassicAssert.NotZero(actual, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.NotZero(uint)"/>
            [System.CLSCompliant(false)]
            public static void NotZero(uint actual) => Legacy.ClassicAssert.NotZero(actual);
                /// <inheritdoc cref="Legacy.ClassicAssert.NotZero(uint, string, object[])"/>
            [System.CLSCompliant(false)]
                public static void NotZero(uint actual, string message, params object?[]? args) => Legacy.ClassicAssert.NotZero(actual, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.NotZero(long)"/>
            public static void NotZero(long actual) => Legacy.ClassicAssert.NotZero(actual);
                /// <inheritdoc cref="Legacy.ClassicAssert.NotZero(long, string, object[])"/>
                public static void NotZero(long actual, string message, params object?[]? args) => Legacy.ClassicAssert.NotZero(actual, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.NotZero(ulong)"/>
            [System.CLSCompliant(false)]
            public static void NotZero(ulong actual) => Legacy.ClassicAssert.NotZero(actual);
                /// <inheritdoc cref="Legacy.ClassicAssert.NotZero(ulong, string, object[])"/>
            [System.CLSCompliant(false)]
                public static void NotZero(ulong actual, string message, params object?[]? args) => Legacy.ClassicAssert.NotZero(actual, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.NotZero(decimal)"/>
            public static void NotZero(decimal actual) => Legacy.ClassicAssert.NotZero(actual);
                /// <inheritdoc cref="Legacy.ClassicAssert.NotZero(decimal, string, object[])"/>
                public static void NotZero(decimal actual, string message, params object?[]? args) => Legacy.ClassicAssert.NotZero(actual, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.NotZero(double)"/>
            public static void NotZero(double actual) => Legacy.ClassicAssert.NotZero(actual);
                /// <inheritdoc cref="Legacy.ClassicAssert.NotZero(double, string, object[])"/>
                public static void NotZero(double actual, string message, params object?[]? args) => Legacy.ClassicAssert.NotZero(actual, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.NotZero(float)"/>
            public static void NotZero(float actual) => Legacy.ClassicAssert.NotZero(actual);
                /// <inheritdoc cref="Legacy.ClassicAssert.NotZero(float, string, object[])"/>
                public static void NotZero(float actual, string message, params object?[]? args) => Legacy.ClassicAssert.NotZero(actual, message, args);

            #endregion

            #region Positive

                /// <inheritdoc cref="Legacy.ClassicAssert.Positive(int)"/>
            public static void Positive(int actual) => Legacy.ClassicAssert.Positive(actual);
                /// <inheritdoc cref="Legacy.ClassicAssert.Positive(int, string, object[])"/>
                public static void Positive(int actual, string message, params object?[]? args) => Legacy.ClassicAssert.Positive(actual, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.Positive(uint)"/>
            [System.CLSCompliant(false)]
            public static void Positive(uint actual) => Legacy.ClassicAssert.Positive(actual);
                /// <inheritdoc cref="Legacy.ClassicAssert.Positive(uint, string, object[])"/>
            [System.CLSCompliant(false)]
                public static void Positive(uint actual, string message, params object?[]? args) => Legacy.ClassicAssert.Positive(actual, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.Positive(long)"/>
            public static void Positive(long actual) => Legacy.ClassicAssert.Positive(actual);
                /// <inheritdoc cref="Legacy.ClassicAssert.Positive(long, string, object[])"/>
                public static void Positive(long actual, string message, params object?[]? args) => Legacy.ClassicAssert.Positive(actual, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.Positive(ulong)"/>
            [System.CLSCompliant(false)]
            public static void Positive(ulong actual) => Legacy.ClassicAssert.Positive(actual);
                /// <inheritdoc cref="Legacy.ClassicAssert.Positive(ulong, string, object[])"/>
            [System.CLSCompliant(false)]
                public static void Positive(ulong actual, string message, params object?[]? args) => Legacy.ClassicAssert.Positive(actual, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.Positive(decimal)"/>
            public static void Positive(decimal actual) => Legacy.ClassicAssert.Positive(actual);
                /// <inheritdoc cref="Legacy.ClassicAssert.Positive(decimal, string, object[])"/>
                public static void Positive(decimal actual, string message, params object?[]? args) => Legacy.ClassicAssert.Positive(actual, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.Positive(double)"/>
            public static void Positive(double actual) => Legacy.ClassicAssert.Positive(actual);
                /// <inheritdoc cref="Legacy.ClassicAssert.Positive(double, string, object[])"/>
                public static void Positive(double actual, string message, params object?[]? args) => Legacy.ClassicAssert.Positive(actual, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.Positive(float)"/>
            public static void Positive(float actual) => Legacy.ClassicAssert.Positive(actual);
                /// <inheritdoc cref="Legacy.ClassicAssert.Positive(float, string, object[])"/>
                public static void Positive(float actual, string message, params object?[]? args) => Legacy.ClassicAssert.Positive(actual, message, args);

            #endregion

            #region Negative

                /// <inheritdoc cref="Legacy.ClassicAssert.Negative(int)"/>
            public static void Negative(int actual) => Legacy.ClassicAssert.Negative(actual);
                /// <inheritdoc cref="Legacy.ClassicAssert.Negative(int, string, object[])"/>
                public static void Negative(int actual, string message, params object?[]? args) => Legacy.ClassicAssert.Negative(actual, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.Negative(uint)"/>
            [System.CLSCompliant(false)]
            public static void Negative(uint actual) => Legacy.ClassicAssert.Negative(actual);
                /// <inheritdoc cref="Legacy.ClassicAssert.Negative(uint, string, object[])"/>
            [System.CLSCompliant(false)]
                public static void Negative(uint actual, string message, params object?[]? args) => Legacy.ClassicAssert.Negative(actual, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.Negative(long)"/>
            public static void Negative(long actual) => Legacy.ClassicAssert.Negative(actual);
                /// <inheritdoc cref="Legacy.ClassicAssert.Negative(long, string, object[])"/>
                public static void Negative(long actual, string message, params object?[]? args) => Legacy.ClassicAssert.Negative(actual, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.Negative(ulong)"/>
            [System.CLSCompliant(false)]
            public static void Negative(ulong actual) => Legacy.ClassicAssert.Negative(actual);
                /// <inheritdoc cref="Legacy.ClassicAssert.Negative(ulong, string, object[])"/>
            [System.CLSCompliant(false)]
                public static void Negative(ulong actual, string message, params object?[]? args) => Legacy.ClassicAssert.Negative(actual, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.Negative(decimal)"/>
            public static void Negative(decimal actual) => Legacy.ClassicAssert.Negative(actual);
                /// <inheritdoc cref="Legacy.ClassicAssert.Negative(decimal, string, object[])"/>
                public static void Negative(decimal actual, string message, params object?[]? args) => Legacy.ClassicAssert.Negative(actual, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.Negative(double)"/>
            public static void Negative(double actual) => Legacy.ClassicAssert.Negative(actual);
                /// <inheritdoc cref="Legacy.ClassicAssert.Negative(double, string, object[])"/>
                public static void Negative(double actual, string message, params object?[]? args) => Legacy.ClassicAssert.Negative(actual, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.Negative(float)"/>
            public static void Negative(float actual) => Legacy.ClassicAssert.Negative(actual);
                /// <inheritdoc cref="Legacy.ClassicAssert.Negative(float, string, object[])"/>
                public static void Negative(float actual, string message, params object?[]? args) => Legacy.ClassicAssert.Negative(actual, message, args);

            #endregion
        }
    }
}
