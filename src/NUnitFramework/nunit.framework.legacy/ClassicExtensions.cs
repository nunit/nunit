// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections;

// ReSharper disable once CheckNamespace
namespace NUnit.Framework
{
    /// <summary>
    /// C#14 static extension methods to expose classic Assert methods on NUnit.Framework.Assert
    /// </summary>
    public static class ClassicAssertExtensions
    {
        extension(Assert)
        {
            #region AreEqual

                /// <inheritdoc cref="Legacy.ClassicAssert.AreEqual(double, double, double, string, object[])"/>
                public static void AreEqual(double expected, double actual, double delta, string message, params object?[]? args)
                    => Legacy.ClassicAssert.AreEqual(expected, actual, delta, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.AreEqual(double, double, double)"/>
            public static void AreEqual(double expected, double actual, double delta)
                => Legacy.ClassicAssert.AreEqual(expected, actual, delta);

                /// <inheritdoc cref="Legacy.ClassicAssert.AreEqual(object, object, string, object[])"/>
                public static void AreEqual(object? expected, object? actual, string message, params object?[]? args)
                    => Legacy.ClassicAssert.AreEqual(expected, actual, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.AreEqual(object, object)"/>
            public static void AreEqual(object? expected, object? actual)
                => Legacy.ClassicAssert.AreEqual(expected, actual);

            #endregion

            #region AreNotEqual

                /// <inheritdoc cref="Legacy.ClassicAssert.AreNotEqual(object, object, string, object[])"/>
                public static void AreNotEqual(object? expected, object? actual, string message, params object?[]? args)
                    => Legacy.ClassicAssert.AreNotEqual(expected, actual, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.AreNotEqual(object, object)"/>
            public static void AreNotEqual(object? expected, object? actual)
                => Legacy.ClassicAssert.AreNotEqual(expected, actual);

            #endregion

            #region AreSame

                /// <inheritdoc cref="Legacy.ClassicAssert.AreSame(object, object, string, object[])"/>
                public static void AreSame(object? expected, object? actual, string message, params object?[]? args)
                    => Legacy.ClassicAssert.AreSame(expected, actual, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.AreSame(object, object)"/>
            public static void AreSame(object? expected, object? actual)
                => Legacy.ClassicAssert.AreSame(expected, actual);

            #endregion

            #region AreNotSame

                /// <inheritdoc cref="Legacy.ClassicAssert.AreNotSame(object, object, string, object[])"/>
                public static void AreNotSame(object? expected, object? actual, string message, params object?[]? args)
                    => Legacy.ClassicAssert.AreNotSame(expected, actual, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.AreNotSame(object, object)"/>
            public static void AreNotSame(object? expected, object? actual)
                => Legacy.ClassicAssert.AreNotSame(expected, actual);

            #endregion

            #region True/IsTrue

                /// <inheritdoc cref="Legacy.ClassicAssert.True(bool?, string, object[])"/>
                public static void True(bool? condition, string message, params object?[]? args)
                    => Legacy.ClassicAssert.True(condition, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.True(bool, string, object[])"/>
                public static void True(bool condition, string message, params object?[]? args)
                    => Legacy.ClassicAssert.True(condition, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.True(bool?)"/>
            public static void True(bool? condition)
                => Legacy.ClassicAssert.True(condition);

                /// <inheritdoc cref="Legacy.ClassicAssert.True(bool)"/>
            public static void True(bool condition)
                => Legacy.ClassicAssert.True(condition);

                /// <inheritdoc cref="Legacy.ClassicAssert.IsTrue(bool?, string, object[])"/>
                public static void IsTrue(bool? condition, string message, params object?[]? args)
                    => Legacy.ClassicAssert.IsTrue(condition, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.IsTrue(bool, string, object[])"/>
                public static void IsTrue(bool condition, string message, params object?[]? args)
                    => Legacy.ClassicAssert.IsTrue(condition, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.IsTrue(bool?)"/>
            public static void IsTrue(bool? condition)
                => Legacy.ClassicAssert.IsTrue(condition);

                /// <inheritdoc cref="Legacy.ClassicAssert.IsTrue(bool)"/>
            public static void IsTrue(bool condition)
                => Legacy.ClassicAssert.IsTrue(condition);

            #endregion

            #region False/IsFalse

                /// <inheritdoc cref="Legacy.ClassicAssert.False(bool?, string, object[])"/>
                public static void False(bool? condition, string message, params object?[]? args)
                    => Legacy.ClassicAssert.False(condition, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.False(bool, string, object[])"/>
                public static void False(bool condition, string message, params object?[]? args)
                    => Legacy.ClassicAssert.False(condition, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.False(bool?)"/>
            public static void False(bool? condition)
                => Legacy.ClassicAssert.False(condition);

                /// <inheritdoc cref="Legacy.ClassicAssert.False(bool)"/>
            public static void False(bool condition)
                => Legacy.ClassicAssert.False(condition);

                /// <inheritdoc cref="Legacy.ClassicAssert.IsFalse(bool?, string, object[])"/>
                public static void IsFalse(bool? condition, string message, params object?[]? args)
                    => Legacy.ClassicAssert.IsFalse(condition, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.IsFalse(bool, string, object[])"/>
                public static void IsFalse(bool condition, string message, params object?[]? args)
                    => Legacy.ClassicAssert.IsFalse(condition, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.IsFalse(bool?)"/>
            public static void IsFalse(bool? condition)
                => Legacy.ClassicAssert.IsFalse(condition);

                /// <inheritdoc cref="Legacy.ClassicAssert.IsFalse(bool)"/>
            public static void IsFalse(bool condition)
                => Legacy.ClassicAssert.IsFalse(condition);

            #endregion

            #region NotNull/IsNotNull

                /// <inheritdoc cref="Legacy.ClassicAssert.NotNull(object, string, object[])"/>
                public static void NotNull(object? anObject, string message, params object?[]? args)
                    => Legacy.ClassicAssert.NotNull(anObject, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.NotNull(object)"/>
            public static void NotNull(object? anObject)
                => Legacy.ClassicAssert.NotNull(anObject);

                /// <inheritdoc cref="Legacy.ClassicAssert.IsNotNull(object, string, object[])"/>
                public static void IsNotNull(object? anObject, string message, params object?[]? args)
                    => Legacy.ClassicAssert.IsNotNull(anObject, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.IsNotNull(object)"/>
            public static void IsNotNull(object? anObject)
                => Legacy.ClassicAssert.IsNotNull(anObject);

            #endregion

            #region Null/IsNull

                /// <inheritdoc cref="Legacy.ClassicAssert.Null(object, string, object[])"/>
                public static void Null(object? anObject, string message, params object?[]? args)
                    => Legacy.ClassicAssert.Null(anObject, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.Null(object)"/>
            public static void Null(object? anObject)
                => Legacy.ClassicAssert.Null(anObject);

                /// <inheritdoc cref="Legacy.ClassicAssert.IsNull(object, string, object[])"/>
                public static void IsNull(object? anObject, string message, params object?[]? args)
                    => Legacy.ClassicAssert.IsNull(anObject, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.IsNull(object)"/>
            public static void IsNull(object? anObject)
                => Legacy.ClassicAssert.IsNull(anObject);

            #endregion

            #region IsNaN

                /// <inheritdoc cref="Legacy.ClassicAssert.IsNaN(double, string, object[])"/>
                public static void IsNaN(double aDouble, string message, params object?[]? args)
                    => Legacy.ClassicAssert.IsNaN(aDouble, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.IsNaN(double)"/>
            public static void IsNaN(double aDouble)
                => Legacy.ClassicAssert.IsNaN(aDouble);

                /// <inheritdoc cref="Legacy.ClassicAssert.IsNaN(double?, string, object[])"/>
                public static void IsNaN(double? aDouble, string message, params object?[]? args)
                    => Legacy.ClassicAssert.IsNaN(aDouble, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.IsNaN(double?)"/>
            public static void IsNaN(double? aDouble)
                => Legacy.ClassicAssert.IsNaN(aDouble);

            #endregion

            #region IsEmpty

                /// <inheritdoc cref="Legacy.ClassicAssert.IsEmpty(string, string, object[])"/>
                public static void IsEmpty(string? aString, string message, params object?[]? args)
                    => Legacy.ClassicAssert.IsEmpty(aString, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.IsEmpty(string)"/>
            public static void IsEmpty(string? aString)
                => Legacy.ClassicAssert.IsEmpty(aString);

                /// <inheritdoc cref="Legacy.ClassicAssert.IsEmpty(IEnumerable, string, object[])"/>
                public static void IsEmpty(IEnumerable collection, string message, params object?[]? args)
                    => Legacy.ClassicAssert.IsEmpty(collection, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.IsEmpty(IEnumerable)"/>
            public static void IsEmpty(IEnumerable collection)
                => Legacy.ClassicAssert.IsEmpty(collection);

            #endregion

            #region IsNotEmpty

                /// <inheritdoc cref="Legacy.ClassicAssert.IsNotEmpty(string, string, object[])"/>
                public static void IsNotEmpty(string? aString, string message, params object?[]? args)
                    => Legacy.ClassicAssert.IsNotEmpty(aString, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.IsNotEmpty(string)"/>
            public static void IsNotEmpty(string? aString)
                => Legacy.ClassicAssert.IsNotEmpty(aString);

                /// <inheritdoc cref="Legacy.ClassicAssert.IsNotEmpty(IEnumerable, string, object[])"/>
                public static void IsNotEmpty(IEnumerable collection, string message, params object?[]? args)
                    => Legacy.ClassicAssert.IsNotEmpty(collection, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.IsNotEmpty(IEnumerable)"/>
            public static void IsNotEmpty(IEnumerable collection)
                => Legacy.ClassicAssert.IsNotEmpty(collection);

            #endregion

            #region Contains

                /// <inheritdoc cref="Legacy.ClassicAssert.Contains(object, ICollection, string, object[])"/>
                public static void Contains(object? expected, ICollection? actual, string message, params object?[]? args)
                    => Legacy.ClassicAssert.Contains(expected, actual, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.Contains(object, ICollection)"/>
            public static void Contains(object? expected, ICollection? actual)
                => Legacy.ClassicAssert.Contains(expected, actual);

            #endregion

            // Zero, NotZero, Positive, Negative and comparison methods will be added in separate files
            // to keep this file manageable. They follow the same pattern.
        }
    }
}
