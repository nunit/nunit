// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

// ReSharper disable once CheckNamespace
namespace NUnit.Framework
{
    /// <summary>
    /// C#14 static extension methods for type assertions
    /// </summary>
    public static class ClassicAssertTypeExtensions
    {
        extension(Assert)
        {
            #region IsAssignableFrom

                /// <inheritdoc cref="Legacy.ClassicAssert.IsAssignableFrom(Type, object)"/>
            public static void IsAssignableFrom(Type expected, object? actual)
                => Legacy.ClassicAssert.IsAssignableFrom(expected, actual);
                /// <inheritdoc cref="Legacy.ClassicAssert.IsAssignableFrom(Type, object, string, object[])"/>
                public static void IsAssignableFrom(Type expected, object? actual, string message, params object?[]? args)
                    => Legacy.ClassicAssert.IsAssignableFrom(expected, actual, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.IsAssignableFrom{TExpected}(object)"/>
            public static void IsAssignableFrom<TExpected>(object? actual)
                => Legacy.ClassicAssert.IsAssignableFrom<TExpected>(actual);
                /// <inheritdoc cref="Legacy.ClassicAssert.IsAssignableFrom{TExpected}(object, string, object[])"/>
                public static void IsAssignableFrom<TExpected>(object? actual, string message, params object?[]? args)
                    => Legacy.ClassicAssert.IsAssignableFrom<TExpected>(actual, message, args);

            #endregion

            #region IsNotAssignableFrom

                /// <inheritdoc cref="Legacy.ClassicAssert.IsNotAssignableFrom(Type, object)"/>
            public static void IsNotAssignableFrom(Type expected, object? actual)
                => Legacy.ClassicAssert.IsNotAssignableFrom(expected, actual);
                /// <inheritdoc cref="Legacy.ClassicAssert.IsNotAssignableFrom(Type, object, string, object[])"/>
                public static void IsNotAssignableFrom(Type expected, object? actual, string message, params object?[]? args)
                    => Legacy.ClassicAssert.IsNotAssignableFrom(expected, actual, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.IsNotAssignableFrom{TExpected}(object)"/>
            public static void IsNotAssignableFrom<TExpected>(object? actual)
                => Legacy.ClassicAssert.IsNotAssignableFrom<TExpected>(actual);
                /// <inheritdoc cref="Legacy.ClassicAssert.IsNotAssignableFrom{TExpected}(object, string, object[])"/>
                public static void IsNotAssignableFrom<TExpected>(object? actual, string message, params object?[]? args)
                    => Legacy.ClassicAssert.IsNotAssignableFrom<TExpected>(actual, message, args);

            #endregion

            #region IsInstanceOf

                /// <inheritdoc cref="Legacy.ClassicAssert.IsInstanceOf(Type, object)"/>
            public static void IsInstanceOf(Type expected, object? actual)
                => Legacy.ClassicAssert.IsInstanceOf(expected, actual);
                /// <inheritdoc cref="Legacy.ClassicAssert.IsInstanceOf(Type, object, string, object[])"/>
                public static void IsInstanceOf(Type expected, object? actual, string message, params object?[]? args)
                    => Legacy.ClassicAssert.IsInstanceOf(expected, actual, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.IsInstanceOf{TExpected}(object)"/>
            public static void IsInstanceOf<TExpected>(object? actual)
                => Legacy.ClassicAssert.IsInstanceOf<TExpected>(actual);
                /// <inheritdoc cref="Legacy.ClassicAssert.IsInstanceOf{TExpected}(object, string, object[])"/>
                public static void IsInstanceOf<TExpected>(object? actual, string message, params object?[]? args)
                    => Legacy.ClassicAssert.IsInstanceOf<TExpected>(actual, message, args);

            #endregion

            #region IsNotInstanceOf

                /// <inheritdoc cref="Legacy.ClassicAssert.IsNotInstanceOf(Type, object)"/>
            public static void IsNotInstanceOf(Type expected, object? actual)
                => Legacy.ClassicAssert.IsNotInstanceOf(expected, actual);
                /// <inheritdoc cref="Legacy.ClassicAssert.IsNotInstanceOf(Type, object, string, object[])"/>
                public static void IsNotInstanceOf(Type expected, object? actual, string message, params object?[]? args)
                    => Legacy.ClassicAssert.IsNotInstanceOf(expected, actual, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.IsNotInstanceOf{TExpected}(object)"/>
            public static void IsNotInstanceOf<TExpected>(object? actual)
                => Legacy.ClassicAssert.IsNotInstanceOf<TExpected>(actual);
                /// <inheritdoc cref="Legacy.ClassicAssert.IsNotInstanceOf{TExpected}(object, string, object[])"/>
                public static void IsNotInstanceOf<TExpected>(object? actual, string message, params object?[]? args)
                    => Legacy.ClassicAssert.IsNotInstanceOf<TExpected>(actual, message, args);

            #endregion
        }
    }
}
