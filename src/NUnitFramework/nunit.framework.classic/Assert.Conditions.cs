// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Classic
{
    public partial class Assert
    {
        #region True

        /// <summary>
        /// Asserts that a condition is true. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void True(bool? condition, string? message, params object?[]? args)
        {
            Framework.Assert.That(condition, Is.True, () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Asserts that a condition is true. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void True(bool condition, string? message, params object?[]? args)
        {
            Framework.Assert.That(condition, Is.True, () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Asserts that a condition is true. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        public static void True(bool? condition)
        {
            Framework.Assert.That(condition, Is.True);
        }

        /// <summary>
        /// Asserts that a condition is true. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        public static void True(bool condition)
        {
            Framework.Assert.That(condition, Is.True);
        }

        /// <summary>
        /// Asserts that a condition is true. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsTrue(bool? condition, string? message, params object?[]? args)
        {
            Framework.Assert.That(condition, Is.True, () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Asserts that a condition is true. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsTrue(bool condition, string? message, params object?[]? args)
        {
            Framework.Assert.That(condition, Is.True, () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Asserts that a condition is true. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        public static void IsTrue(bool? condition)
        {
            Framework.Assert.That(condition, Is.True);
        }

        /// <summary>
        /// Asserts that a condition is true. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        public static void IsTrue(bool condition)
        {
            Framework.Assert.That(condition, Is.True);
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
        public static void False(bool? condition, string? message, params object?[]? args)
        {
            Framework.Assert.That(condition, Is.False, () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Asserts that a condition is false. Returns without throwing an exception when inside a multiple assert
        /// block.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void False(bool condition, string? message, params object?[]? args)
        {
            Framework.Assert.That(condition, Is.False, () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Asserts that a condition is false. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        public static void False(bool? condition)
        {
            Framework.Assert.That(condition, Is.False);
        }

        /// <summary>
        /// Asserts that a condition is false. Returns without throwing an exception when inside a multiple assert
        /// block.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        public static void False(bool condition)
        {
            Framework.Assert.That(condition, Is.False);
        }

        /// <summary>
        /// Asserts that a condition is false. Returns without throwing an exception when inside a multiple assert
        /// block.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsFalse(bool? condition, string? message, params object?[]? args)
        {
            Framework.Assert.That(condition, Is.False, () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Asserts that a condition is false. Returns without throwing an exception when inside a multiple assert
        /// block.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsFalse(bool condition, string? message, params object?[]? args)
        {
            Framework.Assert.That(condition, Is.False, () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Asserts that a condition is false. Returns without throwing an exception when inside a multiple assert
        /// block.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        public static void IsFalse(bool? condition)
        {
            Framework.Assert.That(condition, Is.False);
        }

        /// <summary>
        /// Asserts that a condition is false. Returns without throwing an exception when inside a multiple assert
        /// block.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        public static void IsFalse(bool condition)
        {
            Framework.Assert.That(condition, Is.False);
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
        public static void NotNull(object? anObject, string? message, params object?[]? args)
        {
            Framework.Assert.That(anObject, Is.Not.Null, () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that the object that is passed in is not equal to <see langword="null"/>. Returns without throwing an
        /// exception when inside a multiple assert block.
        /// </summary>
        /// <param name="anObject">The object that is to be tested</param>
        public static void NotNull(object? anObject)
        {
            Framework.Assert.That(anObject, Is.Not.Null);
        }

        /// <summary>
        /// Verifies that the object that is passed in is not equal to <see langword="null"/>. Returns without throwing an
        /// exception when inside a multiple assert block.
        /// </summary>
        /// <param name="anObject">The object that is to be tested</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsNotNull(object? anObject, string? message, params object?[]? args)
        {
            Framework.Assert.That(anObject, Is.Not.Null, () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that the object that is passed in is not equal to <see langword="null"/>. Returns without throwing an
        /// exception when inside a multiple assert block.
        /// </summary>
        /// <param name="anObject">The object that is to be tested</param>
        public static void IsNotNull(object? anObject)
        {
            Framework.Assert.That(anObject, Is.Not.Null);
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
        public static void Null(object? anObject, string? message, params object?[]? args)
        {
            Framework.Assert.That(anObject, Is.Null, () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that the object that is passed in is equal to <see langword="null"/>. Returns without throwing an
        /// exception when inside a multiple assert block.
        /// </summary>
        /// <param name="anObject">The object that is to be tested</param>
        public static void Null(object? anObject)
        {
            Framework.Assert.That(anObject, Is.Null);
        }

        /// <summary>
        /// Verifies that the object that is passed in is equal to <see langword="null"/>. Returns without throwing an
        /// exception when inside a multiple assert block.
        /// </summary>
        /// <param name="anObject">The object that is to be tested</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsNull(object? anObject, string? message, params object?[]? args)
        {
            Framework.Assert.That(anObject, Is.Null, () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that the object that is passed in is equal to <see langword="null"/>. Returns without throwing an
        /// exception when inside a multiple assert block.
        /// </summary>
        /// <param name="anObject">The object that is to be tested</param>
        public static void IsNull(object? anObject)
        {
            Framework.Assert.That(anObject, Is.Null);
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
        public static void IsNaN(double aDouble, string? message, params object?[]? args)
        {
            Framework.Assert.That(aDouble, Is.NaN, () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that the double that is passed in is an <c>NaN</c> value. Returns without throwing an
        /// exception when inside a multiple assert block.
        /// </summary>
        /// <param name="aDouble">The value that is to be tested</param>
        public static void IsNaN(double aDouble)
        {
            Framework.Assert.That(aDouble, Is.NaN);
        }

        /// <summary>
        /// Verifies that the double that is passed in is an <c>NaN</c> value. Returns without throwing an
        /// exception when inside a multiple assert block.
        /// </summary>
        /// <param name="aDouble">The value that is to be tested</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsNaN(double? aDouble, string? message, params object?[]? args)
        {
            Framework.Assert.That(aDouble, Is.NaN, () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that the double that is passed in is an <c>NaN</c> value. Returns without throwing an
        /// exception when inside a multiple assert block.
        /// </summary>
        /// <param name="aDouble">The value that is to be tested</param>
        public static void IsNaN(double? aDouble)
        {
            Framework.Assert.That(aDouble, Is.NaN);
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
        public static void IsEmpty(string? aString, string? message, params object?[]? args)
        {
            Framework.Assert.That(aString, new EmptyStringConstraint(), () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Assert that a string is empty. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="aString">The string to be tested</param>
        public static void IsEmpty(string? aString)
        {
            Framework.Assert.That(aString, new EmptyStringConstraint());
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
        public static void IsEmpty(IEnumerable collection, string? message, params object?[]? args)
        {
            Framework.Assert.That(collection, new EmptyCollectionConstraint(), () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Assert that an array, list or other collection is empty. Returns without throwing an exception when inside a
        /// multiple assert block.
        /// </summary>
        /// <param name="collection">An array, list or other collection implementing ICollection</param>
        public static void IsEmpty(IEnumerable collection)
        {
            Framework.Assert.That(collection, new EmptyCollectionConstraint());
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
        public static void IsNotEmpty(string? aString, string? message, params object?[]? args)
        {
            Framework.Assert.That(aString, Is.Not.Empty, () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Assert that a string is not empty. Returns without throwing an exception when inside a multiple assert
        /// block.
        /// </summary>
        /// <param name="aString">The string to be tested</param>
        public static void IsNotEmpty(string? aString)
        {
            Framework.Assert.That(aString, Is.Not.Empty);
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
        public static void IsNotEmpty(IEnumerable collection, string? message, params object?[]? args)
        {
            Framework.Assert.That(collection, Is.Not.Empty, () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Assert that an array, list or other collection is not empty. Returns without throwing an exception when
        /// inside a multiple assert block.
        /// </summary>
        /// <param name="collection">An array, list or other collection implementing ICollection</param>
        public static void IsNotEmpty(IEnumerable collection)
        {
            Framework.Assert.That(collection, Is.Not.Empty);
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
            Framework.Assert.That(actual, Is.Zero);
        }

        /// <summary>
        /// Asserts that an int is zero. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Zero(int actual, string? message, params object?[]? args)
        {
            Framework.Assert.That(actual, Is.Zero, () => ConvertMessageWithArgs(message, args));
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
            Framework.Assert.That(actual, Is.Zero);
        }

        /// <summary>
        /// Asserts that an unsigned int is zero. Returns without throwing an exception when inside a multiple assert
        /// block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        [CLSCompliant(false)]
        public static void Zero(uint actual, string? message, params object?[]? args)
        {
            Framework.Assert.That(actual, Is.Zero, () => ConvertMessageWithArgs(message, args));
        }

        #endregion

        #region Longs

        /// <summary>
        /// Asserts that a Long is zero. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void Zero(long actual)
        {
            Framework.Assert.That(actual, Is.Zero);
        }

        /// <summary>
        /// Asserts that a Long is zero. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Zero(long actual, string? message, params object?[]? args)
        {
            Framework.Assert.That(actual, Is.Zero, () => ConvertMessageWithArgs(message, args));
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
            Framework.Assert.That(actual, Is.Zero);
        }

        /// <summary>
        /// Asserts that an unsigned Long is zero. Returns without throwing an exception when inside a multiple assert
        /// block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        [CLSCompliant(false)]
        public static void Zero(ulong actual, string? message, params object?[]? args)
        {
            Framework.Assert.That(actual, Is.Zero, () => ConvertMessageWithArgs(message, args));
        }

        #endregion

        #region Decimals

        /// <summary>
        /// Asserts that a decimal is zero. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void Zero(decimal actual)
        {
            Framework.Assert.That(actual, Is.Zero);
        }

        /// <summary>
        /// Asserts that a decimal is zero. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Zero(decimal actual, string? message, params object?[]? args)
        {
            Framework.Assert.That(actual, Is.Zero, () => ConvertMessageWithArgs(message, args));
        }

        #endregion

        #region Doubles

        /// <summary>
        /// Asserts that a double is zero. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void Zero(double actual)
        {
            Framework.Assert.That(actual, Is.Zero);
        }

        /// <summary>
        /// Asserts that a double is zero. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Zero(double actual, string? message, params object?[]? args)
        {
            Framework.Assert.That(actual, Is.Zero, () => ConvertMessageWithArgs(message, args));
        }

        #endregion

        #region Floats

        /// <summary>
        /// Asserts that a float is zero. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void Zero(float actual)
        {
            Framework.Assert.That(actual, Is.Zero);
        }

        /// <summary>
        /// Asserts that a float is zero. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Zero(float actual, string? message, params object?[]? args)
        {
            Framework.Assert.That(actual, Is.Zero, () => ConvertMessageWithArgs(message, args));
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
            Framework.Assert.That(actual, Is.Not.Zero);
        }

        /// <summary>
        /// Asserts that an int is not zero. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void NotZero(int actual, string? message, params object?[]? args)
        {
            Framework.Assert.That(actual, Is.Not.Zero, () => ConvertMessageWithArgs(message, args));
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
            Framework.Assert.That(actual, Is.Not.Zero);
        }

        /// <summary>
        /// Asserts that an unsigned int is not zero. Returns without throwing an exception when inside a multiple
        /// assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        [CLSCompliant(false)]
        public static void NotZero(uint actual, string? message, params object?[]? args)
        {
            Framework.Assert.That(actual, Is.Not.Zero, () => ConvertMessageWithArgs(message, args));
        }

        #endregion

        #region Longs

        /// <summary>
        /// Asserts that a Long is not zero. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void NotZero(long actual)
        {
            Framework.Assert.That(actual, Is.Not.Zero);
        }

        /// <summary>
        /// Asserts that a Long is not zero. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void NotZero(long actual, string? message, params object?[]? args)
        {
            Framework.Assert.That(actual, Is.Not.Zero, () => ConvertMessageWithArgs(message, args));
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
            Framework.Assert.That(actual, Is.Not.Zero);
        }

        /// <summary>
        /// Asserts that an unsigned Long is not zero. Returns without throwing an exception when inside a multiple
        /// assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        [CLSCompliant(false)]
        public static void NotZero(ulong actual, string? message, params object?[]? args)
        {
            Framework.Assert.That(actual, Is.Not.Zero, () => ConvertMessageWithArgs(message, args));
        }

        #endregion

        #region Decimals

        /// <summary>
        /// Asserts that a decimal is zero. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void NotZero(decimal actual)
        {
            Framework.Assert.That(actual, Is.Not.Zero);
        }

        /// <summary>
        /// Asserts that a decimal is zero. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void NotZero(decimal actual, string? message, params object?[]? args)
        {
            Framework.Assert.That(actual, Is.Not.Zero, () => ConvertMessageWithArgs(message, args));
        }

        #endregion

        #region Doubles

        /// <summary>
        /// Asserts that a double is zero. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void NotZero(double actual)
        {
            Framework.Assert.That(actual, Is.Not.Zero);
        }

        /// <summary>
        /// Asserts that a double is zero. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void NotZero(double actual, string? message, params object?[]? args)
        {
            Framework.Assert.That(actual, Is.Not.Zero, () => ConvertMessageWithArgs(message, args));
        }

        #endregion

        #region Floats

        /// <summary>
        /// Asserts that a float is zero. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void NotZero(float actual)
        {
            Framework.Assert.That(actual, Is.Not.Zero);
        }

        /// <summary>
        /// Asserts that a float is zero. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void NotZero(float actual, string? message, params object?[]? args)
        {
            Framework.Assert.That(actual, Is.Not.Zero, () => ConvertMessageWithArgs(message, args));
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
            Framework.Assert.That(actual, Is.Positive);
        }

        /// <summary>
        /// Asserts that an int is positive. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Positive(int actual, string? message, params object?[]? args)
        {
            Framework.Assert.That(actual, Is.Positive, () => ConvertMessageWithArgs(message, args));
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
            Framework.Assert.That(actual, Is.Positive);
        }

        /// <summary>
        /// Asserts that an unsigned int is positive. Returns without throwing an exception when inside a multiple
        /// assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        [CLSCompliant(false)]
        public static void Positive(uint actual, string? message, params object?[]? args)
        {
            Framework.Assert.That(actual, Is.Positive, () => ConvertMessageWithArgs(message, args));
        }

        #endregion

        #region Longs

        /// <summary>
        /// Asserts that a Long is positive. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void Positive(long actual)
        {
            Framework.Assert.That(actual, Is.Positive);
        }

        /// <summary>
        /// Asserts that a Long is positive. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Positive(long actual, string? message, params object?[]? args)
        {
            Framework.Assert.That(actual, Is.Positive, () => ConvertMessageWithArgs(message, args));
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
            Framework.Assert.That(actual, Is.Positive);
        }

        /// <summary>
        /// Asserts that an unsigned Long is positive. Returns without throwing an exception when inside a multiple
        /// assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        [CLSCompliant(false)]
        public static void Positive(ulong actual, string? message, params object?[]? args)
        {
            Framework.Assert.That(actual, Is.Positive, () => ConvertMessageWithArgs(message, args));
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
            Framework.Assert.That(actual, Is.Positive);
        }

        /// <summary>
        /// Asserts that a decimal is positive. Returns without throwing an exception when inside a multiple assert
        /// block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Positive(decimal actual, string? message, params object?[]? args)
        {
            Framework.Assert.That(actual, Is.Positive, () => ConvertMessageWithArgs(message, args));
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
            Framework.Assert.That(actual, Is.Positive);
        }

        /// <summary>
        /// Asserts that a double is positive. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Positive(double actual, string? message, params object?[]? args)
        {
            Framework.Assert.That(actual, Is.Positive, () => ConvertMessageWithArgs(message, args));
        }

        #endregion

        #region Floats

        /// <summary>
        /// Asserts that a float is positive. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void Positive(float actual)
        {
            Framework.Assert.That(actual, Is.Positive);
        }

        /// <summary>
        /// Asserts that a float is positive. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Positive(float actual, string? message, params object?[]? args)
        {
            Framework.Assert.That(actual, Is.Positive, () => ConvertMessageWithArgs(message, args));
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
            Framework.Assert.That(actual, Is.Negative);
        }

        /// <summary>
        /// Asserts that an int is negative. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Negative(int actual, string? message, params object?[]? args)
        {
            Framework.Assert.That(actual, Is.Negative, () => ConvertMessageWithArgs(message, args));
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
            Framework.Assert.That(actual, Is.Negative);
        }

        /// <summary>
        /// Asserts that an unsigned int is negative. Returns without throwing an exception when inside a multiple
        /// assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        [CLSCompliant(false)]
        public static void Negative(uint actual, string? message, params object?[]? args)
        {
            Framework.Assert.That(actual, Is.Negative, () => ConvertMessageWithArgs(message, args));
        }

        #endregion

        #region Longs

        /// <summary>
        /// Asserts that a Long is negative. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void Negative(long actual)
        {
            Framework.Assert.That(actual, Is.Negative);
        }

        /// <summary>
        /// Asserts that a Long is negative. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Negative(long actual, string? message, params object?[]? args)
        {
            Framework.Assert.That(actual, Is.Negative, () => ConvertMessageWithArgs(message, args));
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
            Framework.Assert.That(actual, Is.Negative);
        }

        /// <summary>
        /// Asserts that an unsigned Long is negative. Returns without throwing an exception when inside a multiple
        /// assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        [CLSCompliant(false)]
        public static void Negative(ulong actual, string? message, params object?[]? args)
        {
            Framework.Assert.That(actual, Is.Negative, () => ConvertMessageWithArgs(message, args));
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
            Framework.Assert.That(actual, Is.Negative);
        }

        /// <summary>
        /// Asserts that a decimal is negative. Returns without throwing an exception when inside a multiple assert
        /// block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Negative(decimal actual, string? message, params object?[]? args)
        {
            Framework.Assert.That(actual, Is.Negative, () => ConvertMessageWithArgs(message, args));
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
            Framework.Assert.That(actual, Is.Negative);
        }

        /// <summary>
        /// Asserts that a double is negative. Returns without throwing an exception when inside a multiple assert
        /// block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Negative(double actual, string? message, params object?[]? args)
        {
            Framework.Assert.That(actual, Is.Negative, () => ConvertMessageWithArgs(message, args));
        }

        #endregion

        #region Floats

        /// <summary>
        /// Asserts that a float is negative. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void Negative(float actual)
        {
            Framework.Assert.That(actual, Is.Negative);
        }

        /// <summary>
        /// Asserts that a float is negative. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Negative(float actual, string? message, params object?[]? args)
        {
            Framework.Assert.That(actual, Is.Negative, () => ConvertMessageWithArgs(message, args));
        }

        #endregion

        #endregion
    }
}
