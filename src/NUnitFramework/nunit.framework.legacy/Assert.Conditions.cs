// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Legacy
{
    public partial class ClassicAssert
    {
        #region True

        /// <summary>
        /// Asserts that a condition is true. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void True(bool? condition, string message, params object?[]? args)
        {
            That(condition, Is.True, () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Asserts that a condition is true. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void True(bool condition, string message, params object?[]? args)
        {
            That(condition, Is.True, () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Asserts that a condition is true. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        public static void True(bool? condition)
        {
            That(condition, Is.True);
        }

        /// <summary>
        /// Asserts that a condition is true. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        public static void True(bool condition)
        {
            That(condition, Is.True);
        }

        /// <summary>
        /// Asserts that a condition is true. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsTrue(bool? condition, string message, params object?[]? args)
        {
            That(condition, Is.True, () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Asserts that a condition is true. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsTrue(bool condition, string message, params object?[]? args)
        {
            That(condition, Is.True, () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Asserts that a condition is true. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        public static void IsTrue(bool? condition)
        {
            That(condition, Is.True);
        }

        /// <summary>
        /// Asserts that a condition is true. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        public static void IsTrue(bool condition)
        {
            That(condition, Is.True);
        }

        #endregion

        #region False

        /// <summary>
        /// Asserts that a condition is false. Returns without throwing an exception when inside a multiple assert
        /// block.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void False(bool? condition, string message, params object?[]? args)
        {
            That(condition, Is.False, () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Asserts that a condition is false. Returns without throwing an exception when inside a multiple assert
        /// block.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void False(bool condition, string message, params object?[]? args)
        {
            That(condition, Is.False, () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Asserts that a condition is false. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        public static void False(bool? condition)
        {
            That(condition, Is.False);
        }

        /// <summary>
        /// Asserts that a condition is false. Returns without throwing an exception when inside a multiple assert
        /// block.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        public static void False(bool condition)
        {
            That(condition, Is.False);
        }

        /// <summary>
        /// Asserts that a condition is false. Returns without throwing an exception when inside a multiple assert
        /// block.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsFalse(bool? condition, string message, params object?[]? args)
        {
            That(condition, Is.False, () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Asserts that a condition is false. Returns without throwing an exception when inside a multiple assert
        /// block.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsFalse(bool condition, string message, params object?[]? args)
        {
            That(condition, Is.False, () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Asserts that a condition is false. Returns without throwing an exception when inside a multiple assert
        /// block.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        public static void IsFalse(bool? condition)
        {
            That(condition, Is.False);
        }

        /// <summary>
        /// Asserts that a condition is false. Returns without throwing an exception when inside a multiple assert
        /// block.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        public static void IsFalse(bool condition)
        {
            That(condition, Is.False);
        }

        #endregion

        #region NotNull

        /// <summary>
        /// Verifies that the object that is passed in is not equal to <see langword="null"/>. Returns without throwing an
        /// exception when inside a multiple assert block.
        /// </summary>
        /// <param name="anObject">The object that is to be tested</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void NotNull(object? anObject, string message, params object?[]? args)
        {
            That(anObject, Is.Not.Null, () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that the object that is passed in is not equal to <see langword="null"/>. Returns without throwing an
        /// exception when inside a multiple assert block.
        /// </summary>
        /// <param name="anObject">The object that is to be tested</param>
        public static void NotNull(object? anObject)
        {
            That(anObject, Is.Not.Null);
        }

        /// <summary>
        /// Verifies that the object that is passed in is not equal to <see langword="null"/>. Returns without throwing an
        /// exception when inside a multiple assert block.
        /// </summary>
        /// <param name="anObject">The object that is to be tested</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsNotNull(object? anObject, string message, params object?[]? args)
        {
            That(anObject, Is.Not.Null, () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that the object that is passed in is not equal to <see langword="null"/>. Returns without throwing an
        /// exception when inside a multiple assert block.
        /// </summary>
        /// <param name="anObject">The object that is to be tested</param>
        public static void IsNotNull(object? anObject)
        {
            That(anObject, Is.Not.Null);
        }

        #endregion

        #region Null

        /// <summary>
        /// Verifies that the object that is passed in is equal to <see langword="null"/>. Returns without throwing an
        /// exception when inside a multiple assert block.
        /// </summary>
        /// <param name="anObject">The object that is to be tested</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Null(object? anObject, string message, params object?[]? args)
        {
            That(anObject, Is.Null, () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that the object that is passed in is equal to <see langword="null"/>. Returns without throwing an
        /// exception when inside a multiple assert block.
        /// </summary>
        /// <param name="anObject">The object that is to be tested</param>
        public static void Null(object? anObject)
        {
            That(anObject, Is.Null);
        }

        /// <summary>
        /// Verifies that the object that is passed in is equal to <see langword="null"/>. Returns without throwing an
        /// exception when inside a multiple assert block.
        /// </summary>
        /// <param name="anObject">The object that is to be tested</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsNull(object? anObject, string message, params object?[]? args)
        {
            That(anObject, Is.Null, () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that the object that is passed in is equal to <see langword="null"/>. Returns without throwing an
        /// exception when inside a multiple assert block.
        /// </summary>
        /// <param name="anObject">The object that is to be tested</param>
        public static void IsNull(object? anObject)
        {
            That(anObject, Is.Null);
        }

        #endregion

        #region IsNaN

        /// <summary>
        /// Verifies that the double that is passed in is an <c>NaN</c>. Returns without throwing an
        /// exception when inside a multiple assert block.
        /// </summary>
        /// <param name="aDouble">The value that is to be tested</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsNaN(double aDouble, string message, params object?[]? args)
        {
            That(aDouble, Is.NaN, () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that the double that is passed in is an <c>NaN</c> value. Returns without throwing an
        /// exception when inside a multiple assert block.
        /// </summary>
        /// <param name="aDouble">The value that is to be tested</param>
        public static void IsNaN(double aDouble)
        {
            That(aDouble, Is.NaN);
        }

        /// <summary>
        /// Verifies that the double that is passed in is an <c>NaN</c> value. Returns without throwing an
        /// exception when inside a multiple assert block.
        /// </summary>
        /// <param name="aDouble">The value that is to be tested</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsNaN(double? aDouble, string message, params object?[]? args)
        {
            That(aDouble, Is.NaN, () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that the double that is passed in is an <c>NaN</c> value. Returns without throwing an
        /// exception when inside a multiple assert block.
        /// </summary>
        /// <param name="aDouble">The value that is to be tested</param>
        public static void IsNaN(double? aDouble)
        {
            That(aDouble, Is.NaN);
        }

        #endregion

        #region IsEmpty

        #region String

        /// <summary>
        /// Assert that a string is empty. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="aString">The string to be tested</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsEmpty(string? aString, string message, params object?[]? args)
        {
            That(aString, new EmptyStringConstraint(), () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Assert that a string is empty. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="aString">The string to be tested</param>
        public static void IsEmpty(string? aString)
        {
            That(aString, new EmptyStringConstraint());
        }

        #endregion

        #region Collection

        /// <summary>
        /// Assert that an array, list or other collection is empty. Returns without throwing an exception when inside a
        /// multiple assert block.
        /// </summary>
        /// <param name="collection">An array, list or other collection implementing ICollection</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsEmpty(IEnumerable collection, string message, params object?[]? args)
        {
            That(collection, new EmptyCollectionConstraint(), () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Assert that an array, list or other collection is empty. Returns without throwing an exception when inside a
        /// multiple assert block.
        /// </summary>
        /// <param name="collection">An array, list or other collection implementing ICollection</param>
        public static void IsEmpty(IEnumerable collection)
        {
            That(collection, new EmptyCollectionConstraint());
        }

        #endregion

        #endregion

        #region IsNotEmpty

        #region String

        /// <summary>
        /// Assert that a string is not empty. Returns without throwing an exception when inside a multiple assert
        /// block.
        /// </summary>
        /// <param name="aString">The string to be tested</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsNotEmpty(string? aString, string message, params object?[]? args)
        {
            That(aString, Is.Not.Empty, () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Assert that a string is not empty. Returns without throwing an exception when inside a multiple assert
        /// block.
        /// </summary>
        /// <param name="aString">The string to be tested</param>
        public static void IsNotEmpty(string? aString)
        {
            That(aString, Is.Not.Empty);
        }

        #endregion

        #region Collection

        /// <summary>
        /// Assert that an array, list or other collection is not empty. Returns without throwing an exception when
        /// inside a multiple assert block.
        /// </summary>
        /// <param name="collection">An array, list or other collection implementing ICollection</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsNotEmpty(IEnumerable collection, string message, params object?[]? args)
        {
            That(collection, Is.Not.Empty, () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Assert that an array, list or other collection is not empty. Returns without throwing an exception when
        /// inside a multiple assert block.
        /// </summary>
        /// <param name="collection">An array, list or other collection implementing ICollection</param>
        public static void IsNotEmpty(IEnumerable collection)
        {
            That(collection, Is.Not.Empty);
        }

        #endregion

        #endregion

        #region Zero

        #region Ints

        /// <summary>
        /// Asserts that an int is zero. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void Zero(int actual)
        {
            That(actual, Is.Zero);
        }

        /// <summary>
        /// Asserts that an int is zero. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Zero(int actual, string message, params object?[]? args)
        {
            That(actual, Is.Zero, () => ConvertMessageWithArgs(message, args));
        }

        #endregion

        #region UnsignedInts

        /// <summary>
        /// Asserts that an unsigned int is zero. Returns without throwing an exception when inside a multiple assert
        /// block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        [CLSCompliant(false)]
        public static void Zero(uint actual)
        {
            That(actual, Is.Zero);
        }

        /// <summary>
        /// Asserts that an unsigned int is zero. Returns without throwing an exception when inside a multiple assert
        /// block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        [CLSCompliant(false)]
        public static void Zero(uint actual, string message, params object?[]? args)
        {
            That(actual, Is.Zero, () => ConvertMessageWithArgs(message, args));
        }

        #endregion

        #region Longs

        /// <summary>
        /// Asserts that a Long is zero. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void Zero(long actual)
        {
            That(actual, Is.Zero);
        }

        /// <summary>
        /// Asserts that a Long is zero. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Zero(long actual, string message, params object?[]? args)
        {
            That(actual, Is.Zero, () => ConvertMessageWithArgs(message, args));
        }

        #endregion

        #region UnsignedLongs

        /// <summary>
        /// Asserts that an unsigned Long is zero. Returns without throwing an exception when inside a multiple assert
        /// block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        [CLSCompliant(false)]
        public static void Zero(ulong actual)
        {
            That(actual, Is.Zero);
        }

        /// <summary>
        /// Asserts that an unsigned Long is zero. Returns without throwing an exception when inside a multiple assert
        /// block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        [CLSCompliant(false)]
        public static void Zero(ulong actual, string message, params object?[]? args)
        {
            That(actual, Is.Zero, () => ConvertMessageWithArgs(message, args));
        }

        #endregion

        #region Decimals

        /// <summary>
        /// Asserts that a decimal is zero. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void Zero(decimal actual)
        {
            That(actual, Is.Zero);
        }

        /// <summary>
        /// Asserts that a decimal is zero. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Zero(decimal actual, string message, params object?[]? args)
        {
            That(actual, Is.Zero, () => ConvertMessageWithArgs(message, args));
        }

        #endregion

        #region Doubles

        /// <summary>
        /// Asserts that a double is zero. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void Zero(double actual)
        {
            That(actual, Is.Zero);
        }

        /// <summary>
        /// Asserts that a double is zero. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Zero(double actual, string message, params object?[]? args)
        {
            That(actual, Is.Zero, () => ConvertMessageWithArgs(message, args));
        }

        #endregion

        #region Floats

        /// <summary>
        /// Asserts that a float is zero. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void Zero(float actual)
        {
            That(actual, Is.Zero);
        }

        /// <summary>
        /// Asserts that a float is zero. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Zero(float actual, string message, params object?[]? args)
        {
            That(actual, Is.Zero, () => ConvertMessageWithArgs(message, args));
        }

        #endregion

        #endregion

        #region NotZero

        #region Ints

        /// <summary>
        /// Asserts that an int is not zero. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void NotZero(int actual)
        {
            That(actual, Is.Not.Zero);
        }

        /// <summary>
        /// Asserts that an int is not zero. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void NotZero(int actual, string message, params object?[]? args)
        {
            That(actual, Is.Not.Zero, () => ConvertMessageWithArgs(message, args));
        }

        #endregion

        #region UnsignedInts

        /// <summary>
        /// Asserts that an unsigned int is not zero. Returns without throwing an exception when inside a multiple
        /// assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        [CLSCompliant(false)]
        public static void NotZero(uint actual)
        {
            That(actual, Is.Not.Zero);
        }

        /// <summary>
        /// Asserts that an unsigned int is not zero. Returns without throwing an exception when inside a multiple
        /// assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        [CLSCompliant(false)]
        public static void NotZero(uint actual, string message, params object?[]? args)
        {
            That(actual, Is.Not.Zero, () => ConvertMessageWithArgs(message, args));
        }

        #endregion

        #region Longs

        /// <summary>
        /// Asserts that a Long is not zero. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void NotZero(long actual)
        {
            That(actual, Is.Not.Zero);
        }

        /// <summary>
        /// Asserts that a Long is not zero. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void NotZero(long actual, string message, params object?[]? args)
        {
            That(actual, Is.Not.Zero, () => ConvertMessageWithArgs(message, args));
        }

        #endregion

        #region UnsignedLongs

        /// <summary>
        /// Asserts that an unsigned Long is not zero. Returns without throwing an exception when inside a multiple
        /// assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        [CLSCompliant(false)]
        public static void NotZero(ulong actual)
        {
            That(actual, Is.Not.Zero);
        }

        /// <summary>
        /// Asserts that an unsigned Long is not zero. Returns without throwing an exception when inside a multiple
        /// assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        [CLSCompliant(false)]
        public static void NotZero(ulong actual, string message, params object?[]? args)
        {
            That(actual, Is.Not.Zero, () => ConvertMessageWithArgs(message, args));
        }

        #endregion

        #region Decimals

        /// <summary>
        /// Asserts that a decimal is zero. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void NotZero(decimal actual)
        {
            That(actual, Is.Not.Zero);
        }

        /// <summary>
        /// Asserts that a decimal is zero. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void NotZero(decimal actual, string message, params object?[]? args)
        {
            That(actual, Is.Not.Zero, () => ConvertMessageWithArgs(message, args));
        }

        #endregion

        #region Doubles

        /// <summary>
        /// Asserts that a double is zero. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void NotZero(double actual)
        {
            That(actual, Is.Not.Zero);
        }

        /// <summary>
        /// Asserts that a double is zero. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void NotZero(double actual, string message, params object?[]? args)
        {
            That(actual, Is.Not.Zero, () => ConvertMessageWithArgs(message, args));
        }

        #endregion

        #region Floats

        /// <summary>
        /// Asserts that a float is zero. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void NotZero(float actual)
        {
            That(actual, Is.Not.Zero);
        }

        /// <summary>
        /// Asserts that a float is zero. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void NotZero(float actual, string message, params object?[]? args)
        {
            That(actual, Is.Not.Zero, () => ConvertMessageWithArgs(message, args));
        }

        #endregion

        #endregion

        #region Positive

        #region Ints

        /// <summary>
        /// Asserts that an int is positive. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void Positive(int actual)
        {
            That(actual, Is.Positive);
        }

        /// <summary>
        /// Asserts that an int is positive. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Positive(int actual, string message, params object?[]? args)
        {
            That(actual, Is.Positive, () => ConvertMessageWithArgs(message, args));
        }

        #endregion

        #region UnsignedInts

        /// <summary>
        /// Asserts that an unsigned int is positive. Returns without throwing an exception when inside a multiple
        /// assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        [CLSCompliant(false)]
        public static void Positive(uint actual)
        {
            That(actual, Is.Positive);
        }

        /// <summary>
        /// Asserts that an unsigned int is positive. Returns without throwing an exception when inside a multiple
        /// assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        [CLSCompliant(false)]
        public static void Positive(uint actual, string message, params object?[]? args)
        {
            That(actual, Is.Positive, () => ConvertMessageWithArgs(message, args));
        }

        #endregion

        #region Longs

        /// <summary>
        /// Asserts that a Long is positive. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void Positive(long actual)
        {
            That(actual, Is.Positive);
        }

        /// <summary>
        /// Asserts that a Long is positive. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Positive(long actual, string message, params object?[]? args)
        {
            That(actual, Is.Positive, () => ConvertMessageWithArgs(message, args));
        }

        #endregion

        #region UnsignedLongs

        /// <summary>
        /// Asserts that an unsigned Long is positive. Returns without throwing an exception when inside a multiple
        /// assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        [CLSCompliant(false)]
        public static void Positive(ulong actual)
        {
            That(actual, Is.Positive);
        }

        /// <summary>
        /// Asserts that an unsigned Long is positive. Returns without throwing an exception when inside a multiple
        /// assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        [CLSCompliant(false)]
        public static void Positive(ulong actual, string message, params object?[]? args)
        {
            That(actual, Is.Positive, () => ConvertMessageWithArgs(message, args));
        }

        #endregion

        #region Decimals

        /// <summary>
        /// Asserts that a decimal is positive. Returns without throwing an exception when inside a multiple assert
        /// block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void Positive(decimal actual)
        {
            That(actual, Is.Positive);
        }

        /// <summary>
        /// Asserts that a decimal is positive. Returns without throwing an exception when inside a multiple assert
        /// block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Positive(decimal actual, string message, params object?[]? args)
        {
            That(actual, Is.Positive, () => ConvertMessageWithArgs(message, args));
        }

        #endregion

        #region Doubles

        /// <summary>
        /// Asserts that a double is positive. Returns without throwing an exception when inside a multiple assert
        /// block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void Positive(double actual)
        {
            That(actual, Is.Positive);
        }

        /// <summary>
        /// Asserts that a double is positive. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Positive(double actual, string message, params object?[]? args)
        {
            That(actual, Is.Positive, () => ConvertMessageWithArgs(message, args));
        }

        #endregion

        #region Floats

        /// <summary>
        /// Asserts that a float is positive. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void Positive(float actual)
        {
            That(actual, Is.Positive);
        }

        /// <summary>
        /// Asserts that a float is positive. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Positive(float actual, string message, params object?[]? args)
        {
            That(actual, Is.Positive, () => ConvertMessageWithArgs(message, args));
        }

        #endregion

        #endregion

        #region Negative

        #region Ints

        /// <summary>
        /// Asserts that an int is negative. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void Negative(int actual)
        {
            That(actual, Is.Negative);
        }

        /// <summary>
        /// Asserts that an int is negative. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Negative(int actual, string message, params object?[]? args)
        {
            That(actual, Is.Negative, () => ConvertMessageWithArgs(message, args));
        }

        #endregion

        #region UnsignedInts

        /// <summary>
        /// Asserts that an unsigned int is negative. Returns without throwing an exception when inside a multiple
        /// assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        [CLSCompliant(false)]
        public static void Negative(uint actual)
        {
            That(actual, Is.Negative);
        }

        /// <summary>
        /// Asserts that an unsigned int is negative. Returns without throwing an exception when inside a multiple
        /// assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        [CLSCompliant(false)]
        public static void Negative(uint actual, string message, params object?[]? args)
        {
            That(actual, Is.Negative, () => ConvertMessageWithArgs(message, args));
        }

        #endregion

        #region Longs

        /// <summary>
        /// Asserts that a Long is negative. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void Negative(long actual)
        {
            That(actual, Is.Negative);
        }

        /// <summary>
        /// Asserts that a Long is negative. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Negative(long actual, string message, params object?[]? args)
        {
            That(actual, Is.Negative, () => ConvertMessageWithArgs(message, args));
        }

        #endregion

        #region UnsignedLongs

        /// <summary>
        /// Asserts that an unsigned Long is negative. Returns without throwing an exception when inside a multiple
        /// assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        [CLSCompliant(false)]
        public static void Negative(ulong actual)
        {
            That(actual, Is.Negative);
        }

        /// <summary>
        /// Asserts that an unsigned Long is negative. Returns without throwing an exception when inside a multiple
        /// assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        [CLSCompliant(false)]
        public static void Negative(ulong actual, string message, params object?[]? args)
        {
            That(actual, Is.Negative, () => ConvertMessageWithArgs(message, args));
        }

        #endregion

        #region Decimals

        /// <summary>
        /// Asserts that a decimal is negative. Returns without throwing an exception when inside a multiple assert
        /// block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void Negative(decimal actual)
        {
            That(actual, Is.Negative);
        }

        /// <summary>
        /// Asserts that a decimal is negative. Returns without throwing an exception when inside a multiple assert
        /// block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Negative(decimal actual, string message, params object?[]? args)
        {
            That(actual, Is.Negative, () => ConvertMessageWithArgs(message, args));
        }

        #endregion

        #region Doubles

        /// <summary>
        /// Asserts that a double is negative. Returns without throwing an exception when inside a multiple assert
        /// block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void Negative(double actual)
        {
            That(actual, Is.Negative);
        }

        /// <summary>
        /// Asserts that a double is negative. Returns without throwing an exception when inside a multiple assert
        /// block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Negative(double actual, string message, params object?[]? args)
        {
            That(actual, Is.Negative, () => ConvertMessageWithArgs(message, args));
        }

        #endregion

        #region Floats

        /// <summary>
        /// Asserts that a float is negative. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void Negative(float actual)
        {
            That(actual, Is.Negative);
        }

        /// <summary>
        /// Asserts that a float is negative. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Negative(float actual, string message, params object?[]? args)
        {
            That(actual, Is.Negative, () => ConvertMessageWithArgs(message, args));
        }

        #endregion

        #endregion
    }
}
