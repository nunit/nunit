// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.ComponentModel;

namespace NUnit.Framework.Legacy
{
    /// <summary>
    /// Basic Asserts on strings.
    /// </summary>
    // Abstract because we support syntax extension by inheriting and declaring new static members.
    public abstract class StringAssert : AssertBase
    {
        #region Equals and ReferenceEquals

        /// <summary>
        /// DO NOT USE! Use StringAssert.AreEqualIgnoringCase(...) or Assert.AreEqual(...) instead.
        /// The Equals method throws an InvalidOperationException. This is done
        /// to make sure there is no mistake by calling this function.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static new bool Equals(object? a, object? b)
        {
            throw new InvalidOperationException("StringAssert.Equals should not be used. Use StringAssert.AreEqualIgnoringCase or Assert.AreEqual instead.");
        }

        /// <summary>
        /// DO NOT USE!
        /// The ReferenceEquals method throws an InvalidOperationException. This is done
        /// to make sure there is no mistake by calling this function.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static new void ReferenceEquals(object? a, object? b)
        {
            throw new InvalidOperationException("StringAssert.ReferenceEquals should not be used.");
        }

        #endregion

        #region Contains

        /// <summary>
        /// Asserts that a string is found within another string.
        /// </summary>
        /// <param name="expected">The expected string</param>
        /// <param name="actual">The string to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Arguments used in formatting the message</param>
        public static void Contains(string expected, string actual, string message, params object?[]? args)
        {
            Assert.That(actual, Does.Contain(expected), () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Asserts that a string is found within another string.
        /// </summary>
        /// <param name="expected">The expected string</param>
        /// <param name="actual">The string to be examined</param>
        public static void Contains(string expected, string actual)
        {
            Contains(expected, actual, string.Empty, null);
        }

        #endregion

        #region DoesNotContain

        /// <summary>
        /// Asserts that a string is not found within another string.
        /// </summary>
        /// <param name="expected">The expected string</param>
        /// <param name="actual">The string to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Arguments used in formatting the message</param>
        public static void DoesNotContain(string expected, string actual, string message, params object?[]? args)
        {
            Assert.That(actual, Does.Not.Contain(expected), () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Asserts that a string is found within another string.
        /// </summary>
        /// <param name="expected">The expected string</param>
        /// <param name="actual">The string to be examined</param>
        public static void DoesNotContain(string expected, string actual)
        {
            DoesNotContain(expected, actual, string.Empty, null);
        }

        #endregion

        #region StartsWith

        /// <summary>
        /// Asserts that a string starts with another string.
        /// </summary>
        /// <param name="expected">The expected string</param>
        /// <param name="actual">The string to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Arguments used in formatting the message</param>
        public static void StartsWith(string expected, string actual, string message, params object?[]? args)
        {
            Assert.That(actual, Does.StartWith(expected), () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Asserts that a string starts with another string.
        /// </summary>
        /// <param name="expected">The expected string</param>
        /// <param name="actual">The string to be examined</param>
        public static void StartsWith(string expected, string actual)
        {
            StartsWith(expected, actual, string.Empty, null);
        }

        #endregion

        #region DoesNotStartWith

        /// <summary>
        /// Asserts that a string does not start with another string.
        /// </summary>
        /// <param name="expected">The expected string</param>
        /// <param name="actual">The string to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Arguments used in formatting the message</param>
        public static void DoesNotStartWith(string expected, string actual, string message, params object?[]? args)
        {
            Assert.That(actual, Does.Not.StartWith(expected), () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Asserts that a string does not start with another string.
        /// </summary>
        /// <param name="expected">The expected string</param>
        /// <param name="actual">The string to be examined</param>
        public static void DoesNotStartWith(string expected, string actual)
        {
            DoesNotStartWith(expected, actual, string.Empty, null);
        }

        #endregion

        #region EndsWith

        /// <summary>
        /// Asserts that a string ends with another string.
        /// </summary>
        /// <param name="expected">The expected string</param>
        /// <param name="actual">The string to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Arguments used in formatting the message</param>
        public static void EndsWith(string expected, string actual, string message, params object?[]? args)
        {
            Assert.That(actual, Does.EndWith(expected), () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Asserts that a string ends with another string.
        /// </summary>
        /// <param name="expected">The expected string</param>
        /// <param name="actual">The string to be examined</param>
        public static void EndsWith(string expected, string actual)
        {
            EndsWith(expected, actual, string.Empty, null);
        }

        #endregion

        #region DoesNotEndWith

        /// <summary>
        /// Asserts that a string does not end with another string.
        /// </summary>
        /// <param name="expected">The expected string</param>
        /// <param name="actual">The string to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Arguments used in formatting the message</param>
        public static void DoesNotEndWith(string expected, string actual, string message, params object?[]? args)
        {
            Assert.That(actual, Does.Not.EndWith(expected), () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Asserts that a string does not end with another string.
        /// </summary>
        /// <param name="expected">The expected string</param>
        /// <param name="actual">The string to be examined</param>
        public static void DoesNotEndWith(string expected, string actual)
        {
            DoesNotEndWith(expected, actual, string.Empty, null);
        }

        #endregion

        #region AreEqualIgnoringCase
        /// <summary>
        /// Asserts that two strings are equal, without regard to case.
        /// </summary>
        /// <param name="expected">The expected string</param>
        /// <param name="actual">The actual string</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Arguments used in formatting the message</param>
        public static void AreEqualIgnoringCase(string expected, string actual, string message, params object?[]? args)
        {
            Assert.That(actual, Is.EqualTo(expected).IgnoreCase, () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Asserts that two strings are equal, without regard to case.
        /// </summary>
        /// <param name="expected">The expected string</param>
        /// <param name="actual">The actual string</param>
        public static void AreEqualIgnoringCase(string expected, string actual)
        {
            AreEqualIgnoringCase(expected, actual, string.Empty, null);
        }
        #endregion

        #region AreNotEqualIgnoringCase
        /// <summary>
        /// Asserts that two strings are not equal, without regard to case.
        /// </summary>
        /// <param name="expected">The expected string</param>
        /// <param name="actual">The actual string</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Arguments used in formatting the message</param>
        public static void AreNotEqualIgnoringCase(string expected, string actual, string message, params object?[]? args)
        {
            Assert.That(actual, Is.Not.EqualTo(expected).IgnoreCase, () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Asserts that two strings are not equal, without regard to case.
        /// </summary>
        /// <param name="expected">The expected string</param>
        /// <param name="actual">The actual string</param>
        public static void AreNotEqualIgnoringCase(string expected, string actual)
        {
            AreNotEqualIgnoringCase(expected, actual, string.Empty, null);
        }
        #endregion

        #region IsMatch
        /// <summary>
        /// Asserts that a string matches an expected regular expression pattern.
        /// </summary>
        /// <param name="pattern">The regex pattern to be matched</param>
        /// <param name="actual">The actual string</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Arguments used in formatting the message</param>
        public static void IsMatch(string pattern, string actual, string message, params object?[]? args)
        {
            Assert.That(actual, Does.Match(pattern), () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Asserts that a string matches an expected regular expression pattern.
        /// </summary>
        /// <param name="pattern">The regex pattern to be matched</param>
        /// <param name="actual">The actual string</param>
        public static void IsMatch(string pattern, string actual)
        {
            IsMatch(pattern, actual, string.Empty, null);
        }
        #endregion

        #region DoesNotMatch
        /// <summary>
        /// Asserts that a string does not match an expected regular expression pattern.
        /// </summary>
        /// <param name="pattern">The regex pattern to be used</param>
        /// <param name="actual">The actual string</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Arguments used in formatting the message</param>
        public static void DoesNotMatch(string pattern, string actual, string message, params object?[]? args)
        {
            Assert.That(actual, Does.Not.Match(pattern), () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Asserts that a string does not match an expected regular expression pattern.
        /// </summary>
        /// <param name="pattern">The regex pattern to be used</param>
        /// <param name="actual">The actual string</param>
        public static void DoesNotMatch(string pattern, string actual)
        {
            DoesNotMatch(pattern, actual, string.Empty, null);
        }
        #endregion

        #region IsNullOrEmpty

        /// <summary>
        /// Asserts that a string is either null or empty.
        /// </summary>
        /// <param name="actual">The string to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Arguments used in formatting the message</param>
        public static void IsNullOrEmpty(string? actual, string message, params object?[]? args)
        {
            Assert.That(actual, Is.Null.Or.Empty, () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Asserts that a string is either null or empty.
        /// </summary>
        /// <param name="actual">The string to be examined</param>
        public static void IsNullOrEmpty(string? actual) => IsNullOrEmpty(actual, string.Empty, null);

        #endregion

        #region IsNotNullNorEmpty

        /// <summary>
        /// Asserts that a string is neither null nor empty.
        /// </summary>
        /// <param name="actual">The string to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Arguments used in formatting the message</param>
        public static void IsNotNullNorEmpty(string? actual, string message, params object?[]? args)
        {
            Assert.That(actual, Is.Not.Null.And.Not.Empty, () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Asserts that a string is neither null nor empty.
        /// </summary>
        /// <param name="actual">The string to be examined</param>
        public static void IsNotNullNorEmpty(string? actual) => IsNotNullNorEmpty(actual, string.Empty, null);

        #endregion

        #region IsNullOrWhiteSpace

        /// <summary>
        /// Asserts that a string is either null, empty or consists only of white-space characters.
        /// </summary>
        /// <param name="actual">The string to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Arguments used in formatting the message</param>
        public static void IsNullOrWhiteSpace(string? actual, string message, params object?[]? args)
        {
            Assert.That(actual, Is.WhiteSpace, () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Asserts that a string is either null, empty or consists only of white-space characters.
        /// </summary>
        /// <param name="actual">The string to be examined</param>
        public static void IsNullOrWhiteSpace(string? actual) => IsNullOrWhiteSpace(actual, string.Empty, null);

        #endregion

        #region IsNotNullNorWhiteSpace

        /// <summary>
        /// Asserts that a string is not null, not empty and does not consist only of white-space characters.
        /// </summary>
        /// <param name="actual">The string to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Arguments used in formatting the message</param>
        public static void IsNotNullNorWhiteSpace(string? actual, string message, params object?[]? args)
        {
            Assert.That(actual, Is.Not.WhiteSpace, () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Asserts that a string is not null, not empty and does not consist only of white-space characters.
        /// </summary>
        /// <param name="actual">The string to be examined</param>
        public static void IsNotNullNorWhiteSpace(string? actual) => IsNotNullNorWhiteSpace(actual, string.Empty, null);

        #endregion
    }
}
