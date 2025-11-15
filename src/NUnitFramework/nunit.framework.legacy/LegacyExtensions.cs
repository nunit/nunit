// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;

// ReSharper disable once CheckNamespace
namespace NUnit.Framework
{
    /// <summary>
    /// C#14 static extension methods to expose legacy Assert methods on NUnit.Framework.Assert
    /// </summary>
    public static class LegacyAssertExtensions
    {
        extension(Assert)
        {
            #region AreEqual

            /// <summary>
            /// Verifies that two doubles are equal considering a delta.
            /// </summary>
            public static void AreEqual(double expected, double actual, double delta, string? message = null, params object?[]? args)
                => Legacy.ClassicAssert.AreEqual(expected, actual, delta, message ?? string.Empty, args);

            /// <summary>
            /// Verifies that two doubles are equal considering a delta.
            /// </summary>
            public static void AreEqual(double expected, double actual, double delta)
                => Legacy.ClassicAssert.AreEqual(expected, actual, delta);

            /// <summary>
            /// Verifies that two objects are equal.
            /// </summary>
            public static void AreEqual(object? expected, object? actual, string? message = null, params object?[]? args)
                => Legacy.ClassicAssert.AreEqual(expected, actual, message ?? string.Empty, args);

            /// <summary>
            /// Verifies that two objects are equal.
            /// </summary>
            public static void AreEqual(object? expected, object? actual)
                => Legacy.ClassicAssert.AreEqual(expected, actual);

            #endregion

            #region AreNotEqual

            /// <summary>
            /// Verifies that two objects are not equal.
            /// </summary>
            public static void AreNotEqual(object? expected, object? actual, string? message = null, params object?[]? args)
                => Legacy.ClassicAssert.AreNotEqual(expected, actual, message ?? string.Empty, args);

            /// <summary>
            /// Verifies that two objects are not equal.
            /// </summary>
            public static void AreNotEqual(object? expected, object? actual)
                => Legacy.ClassicAssert.AreNotEqual(expected, actual);

            #endregion

            #region AreSame

            /// <summary>
            /// Asserts that two objects refer to the same object.
            /// </summary>
            public static void AreSame(object? expected, object? actual, string? message = null, params object?[]? args)
                => Legacy.ClassicAssert.AreSame(expected, actual, message ?? string.Empty, args);

            /// <summary>
            /// Asserts that two objects refer to the same object.
            /// </summary>
            public static void AreSame(object? expected, object? actual)
                => Legacy.ClassicAssert.AreSame(expected, actual);

            #endregion

            #region AreNotSame

            /// <summary>
            /// Asserts that two objects do not refer to the same object.
            /// </summary>
            public static void AreNotSame(object? expected, object? actual, string? message = null, params object?[]? args)
                => Legacy.ClassicAssert.AreNotSame(expected, actual, message ?? string.Empty, args);

            /// <summary>
            /// Asserts that two objects do not refer to the same object.
            /// </summary>
            public static void AreNotSame(object? expected, object? actual)
                => Legacy.ClassicAssert.AreNotSame(expected, actual);

            #endregion

            #region True/IsTrue

            /// <summary>
            /// Asserts that a condition is true.
            /// </summary>
            public static void True(bool? condition, string? message = null, params object?[]? args)
                => Legacy.ClassicAssert.True(condition, message ?? string.Empty, args);

            /// <summary>
            /// Asserts that a condition is true.
            /// </summary>
            public static void True(bool condition, string? message = null, params object?[]? args)
                => Legacy.ClassicAssert.True(condition, message ?? string.Empty, args);

            /// <summary>
            /// Asserts that a condition is true.
            /// </summary>
            public static void True(bool? condition)
                => Legacy.ClassicAssert.True(condition);

            /// <summary>
            /// Asserts that a condition is true.
            /// </summary>
            public static void True(bool condition)
                => Legacy.ClassicAssert.True(condition);

            /// <summary>
            /// Asserts that a condition is true.
            /// </summary>
            public static void IsTrue(bool? condition, string? message = null, params object?[]? args)
                => Legacy.ClassicAssert.IsTrue(condition, message ?? string.Empty, args);

            /// <summary>
            /// Asserts that a condition is true.
            /// </summary>
            public static void IsTrue(bool condition, string? message = null, params object?[]? args)
                => Legacy.ClassicAssert.IsTrue(condition, message ?? string.Empty, args);

            /// <summary>
            /// Asserts that a condition is true.
            /// </summary>
            public static void IsTrue(bool? condition)
                => Legacy.ClassicAssert.IsTrue(condition);

            /// <summary>
            /// Asserts that a condition is true.
            /// </summary>
            public static void IsTrue(bool condition)
                => Legacy.ClassicAssert.IsTrue(condition);

            #endregion

            #region False/IsFalse

            /// <summary>
            /// Asserts that a condition is false.
            /// </summary>
            public static void False(bool? condition, string? message = null, params object?[]? args)
                => Legacy.ClassicAssert.False(condition, message ?? string.Empty, args);

            /// <summary>
            /// Asserts that a condition is false.
            /// </summary>
            public static void False(bool condition, string? message = null, params object?[]? args)
                => Legacy.ClassicAssert.False(condition, message ?? string.Empty, args);

            /// <summary>
            /// Asserts that a condition is false.
            /// </summary>
            public static void False(bool? condition)
                => Legacy.ClassicAssert.False(condition);

            /// <summary>
            /// Asserts that a condition is false.
            /// </summary>
            public static void False(bool condition)
                => Legacy.ClassicAssert.False(condition);

            /// <summary>
            /// Asserts that a condition is false.
            /// </summary>
            public static void IsFalse(bool? condition, string? message = null, params object?[]? args)
                => Legacy.ClassicAssert.IsFalse(condition, message ?? string.Empty, args);

            /// <summary>
            /// Asserts that a condition is false.
            /// </summary>
            public static void IsFalse(bool condition, string? message = null, params object?[]? args)
                => Legacy.ClassicAssert.IsFalse(condition, message ?? string.Empty, args);

            /// <summary>
            /// Asserts that a condition is false.
            /// </summary>
            public static void IsFalse(bool? condition)
                => Legacy.ClassicAssert.IsFalse(condition);

            /// <summary>
            /// Asserts that a condition is false.
            /// </summary>
            public static void IsFalse(bool condition)
                => Legacy.ClassicAssert.IsFalse(condition);

            #endregion

            #region NotNull/IsNotNull

            /// <summary>
            /// Verifies that the object is not null.
            /// </summary>
            public static void NotNull(object? anObject, string? message = null, params object?[]? args)
                => Legacy.ClassicAssert.NotNull(anObject, message ?? string.Empty, args);

            /// <summary>
            /// Verifies that the object is not null.
            /// </summary>
            public static void NotNull(object? anObject)
                => Legacy.ClassicAssert.NotNull(anObject);

            /// <summary>
            /// Verifies that the object is not null.
            /// </summary>
            public static void IsNotNull(object? anObject, string? message = null, params object?[]? args)
                => Legacy.ClassicAssert.IsNotNull(anObject, message ?? string.Empty, args);

            /// <summary>
            /// Verifies that the object is not null.
            /// </summary>
            public static void IsNotNull(object? anObject)
                => Legacy.ClassicAssert.IsNotNull(anObject);

            #endregion

            #region Null/IsNull

            /// <summary>
            /// Verifies that the object is null.
            /// </summary>
            public static void Null(object? anObject, string? message = null, params object?[]? args)
                => Legacy.ClassicAssert.Null(anObject, message ?? string.Empty, args);

            /// <summary>
            /// Verifies that the object is null.
            /// </summary>
            public static void Null(object? anObject)
                => Legacy.ClassicAssert.Null(anObject);

            /// <summary>
            /// Verifies that the object is null.
            /// </summary>
            public static void IsNull(object? anObject, string? message = null, params object?[]? args)
                => Legacy.ClassicAssert.IsNull(anObject, message ?? string.Empty, args);

            /// <summary>
            /// Verifies that the object is null.
            /// </summary>
            public static void IsNull(object? anObject)
                => Legacy.ClassicAssert.IsNull(anObject);

            #endregion

            #region IsNaN

            /// <summary>
            /// Verifies that the double is NaN.
            /// </summary>
            public static void IsNaN(double aDouble, string? message = null, params object?[]? args)
                => Legacy.ClassicAssert.IsNaN(aDouble, message ?? string.Empty, args);

            /// <summary>
            /// Verifies that the double is NaN.
            /// </summary>
            public static void IsNaN(double aDouble)
                => Legacy.ClassicAssert.IsNaN(aDouble);

            /// <summary>
            /// Verifies that the nullable double is NaN.
            /// </summary>
            public static void IsNaN(double? aDouble, string? message = null, params object?[]? args)
                => Legacy.ClassicAssert.IsNaN(aDouble, message ?? string.Empty, args);

            /// <summary>
            /// Verifies that the nullable double is NaN.
            /// </summary>
            public static void IsNaN(double? aDouble)
                => Legacy.ClassicAssert.IsNaN(aDouble);

            #endregion

            #region IsEmpty

            /// <summary>
            /// Assert that a string is empty.
            /// </summary>
            public static void IsEmpty(string? aString, string? message = null, params object?[]? args)
                => Legacy.ClassicAssert.IsEmpty(aString, message ?? string.Empty, args);

            /// <summary>
            /// Assert that a string is empty.
            /// </summary>
            public static void IsEmpty(string? aString)
                => Legacy.ClassicAssert.IsEmpty(aString);

            /// <summary>
            /// Assert that a collection is empty.
            /// </summary>
            public static void IsEmpty(IEnumerable collection, string? message = null, params object?[]? args)
                => Legacy.ClassicAssert.IsEmpty(collection, message ?? string.Empty, args);

            /// <summary>
            /// Assert that a collection is empty.
            /// </summary>
            public static void IsEmpty(IEnumerable collection)
                => Legacy.ClassicAssert.IsEmpty(collection);

            #endregion

            #region IsNotEmpty

            /// <summary>
            /// Assert that a string is not empty.
            /// </summary>
            public static void IsNotEmpty(string? aString, string? message = null, params object?[]? args)
                => Legacy.ClassicAssert.IsNotEmpty(aString, message ?? string.Empty, args);

            /// <summary>
            /// Assert that a string is not empty.
            /// </summary>
            public static void IsNotEmpty(string? aString)
                => Legacy.ClassicAssert.IsNotEmpty(aString);

            /// <summary>
            /// Assert that a collection is not empty.
            /// </summary>
            public static void IsNotEmpty(IEnumerable collection, string? message = null, params object?[]? args)
                => Legacy.ClassicAssert.IsNotEmpty(collection, message ?? string.Empty, args);

            /// <summary>
            /// Assert that a collection is not empty.
            /// </summary>
            public static void IsNotEmpty(IEnumerable collection)
                => Legacy.ClassicAssert.IsNotEmpty(collection);

            #endregion

            #region Contains

            /// <summary>
            /// Asserts that an object is contained in a collection.
            /// </summary>
            public static void Contains(object? expected, ICollection? actual, string? message = null, params object?[]? args)
                => Legacy.ClassicAssert.Contains(expected, actual, message ?? string.Empty, args);

            /// <summary>
            /// Asserts that an object is contained in a collection.
            /// </summary>
            public static void Contains(object? expected, ICollection? actual)
                => Legacy.ClassicAssert.Contains(expected, actual);

            #endregion

            // Zero, NotZero, Positive, Negative and comparison methods will be added in separate files
            // to keep this file manageable. They follow the same pattern.
        }
    }
}
