// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.ComponentModel;
using System.IO;
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Legacy
{
    /// <summary>
    /// Asserts on Files
    /// </summary>
    public abstract class FileAssert : AssertBase
    {
        #region Equals and ReferenceEquals

        /// <summary>
        /// DO NOT USE! Use FileAssert.AreEqual(...) instead.
        /// The Equals method throws an InvalidOperationException. This is done
        /// to make sure there is no mistake by calling this function.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static new bool Equals(object a, object b)
        {
            throw new InvalidOperationException("FileAssert.Equals should not be used. Use FileAssert.AreEqual instead.");
        }

        /// <summary>
        /// DO NOT USE!
        /// The ReferenceEquals method throws an InvalidOperationException. This is done
        /// to make sure there is no mistake by calling this function.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static new void ReferenceEquals(object a, object b)
        {
            throw new InvalidOperationException("FileAssert.ReferenceEquals should not be used.");
        }

        #endregion

        #region AreEqual

        #region Streams

        /// <summary>
        /// Verifies that two Streams are equal.  Two Streams are considered
        /// equal if both are null, or if both have the same value byte for byte.
        /// If they are not equal an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="expected">The expected Stream</param>
        /// <param name="actual">The actual Stream</param>
        /// <param name="message">The message to display if Streams are not equal</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void AreEqual(Stream? expected, Stream? actual, string message, params object?[]? args)
        {
            Framework.Assert.That(actual, Is.EqualTo(expected), () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that two Streams are equal.  Two Streams are considered
        /// equal if both are null, or if both have the same value byte for byte.
        /// If they are not equal an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="expected">The expected Stream</param>
        /// <param name="actual">The actual Stream</param>
        public static void AreEqual(Stream? expected, Stream? actual)
        {
            AreEqual(expected, actual, string.Empty, null);
        }

        #endregion

        #region FileInfo
        /// <summary>
        /// Verifies that two files are equal.  Two files are considered
        /// equal if both are null, or if both have the same value byte for byte.
        /// If they are not equal an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="expected">A file containing the value that is expected</param>
        /// <param name="actual">A file containing the actual value</param>
        /// <param name="message">The message to display if Streams are not equal</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void AreEqual(FileInfo expected, FileInfo actual, string message, params object?[]? args)
        {
            using FileStream exStream = expected.OpenRead();
            using FileStream acStream = actual.OpenRead();
            AreEqual(exStream, acStream, message, args);
        }

        /// <summary>
        /// Verifies that two files are equal.  Two files are considered
        /// equal if both are null, or if both have the same value byte for byte.
        /// If they are not equal an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="expected">A file containing the value that is expected</param>
        /// <param name="actual">A file containing the actual value</param>
        public static void AreEqual(FileInfo expected, FileInfo actual)
        {
            AreEqual(expected, actual, string.Empty, null);
        }

        #endregion

        #region String
        /// <summary>
        /// Verifies that two files are equal.  Two files are considered
        /// equal if both are null, or if both have the same value byte for byte.
        /// If they are not equal an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="expected">The path to a file containing the value that is expected</param>
        /// <param name="actual">The path to a file containing the actual value</param>
        /// <param name="message">The message to display if Streams are not equal</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void AreEqual(string expected, string actual, string message, params object?[]? args)
        {
            using FileStream exStream = File.OpenRead(expected);
            using FileStream acStream = File.OpenRead(actual);
            AreEqual(exStream, acStream, message, args);
        }

        /// <summary>
        /// Verifies that two files are equal.  Two files are considered
        /// equal if both are null, or if both have the same value byte for byte.
        /// If they are not equal an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="expected">The path to a file containing the value that is expected</param>
        /// <param name="actual">The path to a file containing the actual value</param>
        public static void AreEqual(string expected, string actual)
        {
            AreEqual(expected, actual, string.Empty, null);
        }

        #endregion

        #endregion

        #region AreNotEqual

        #region Streams
        /// <summary>
        /// Asserts that two Streams are not equal. If they are equal
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="expected">The expected Stream</param>
        /// <param name="actual">The actual Stream</param>
        /// <param name="message">The message to be displayed when the two Stream are the same.</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void AreNotEqual(Stream? expected, Stream? actual, string message, params object?[]? args)
        {
            Framework.Assert.That(actual, Is.Not.EqualTo(expected), () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Asserts that two Streams are not equal. If they are equal
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="expected">The expected Stream</param>
        /// <param name="actual">The actual Stream</param>
        public static void AreNotEqual(Stream? expected, Stream? actual)
        {
            AreNotEqual(expected, actual, string.Empty, null);
        }
        #endregion

        #region FileInfo
        /// <summary>
        /// Asserts that two files are not equal. If they are equal
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="expected">A file containing the value that is expected</param>
        /// <param name="actual">A file containing the actual value</param>
        /// <param name="message">The message to display if Streams are not equal</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void AreNotEqual(FileInfo expected, FileInfo actual, string message, params object?[]? args)
        {
            using FileStream exStream = expected.OpenRead();
            using FileStream acStream = actual.OpenRead();
            AreNotEqual(exStream, acStream, message, args);
        }

        /// <summary>
        /// Asserts that two files are not equal. If they are equal
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="expected">A file containing the value that is expected</param>
        /// <param name="actual">A file containing the actual value</param>
        public static void AreNotEqual(FileInfo expected, FileInfo actual)
        {
            AreNotEqual(expected, actual, string.Empty, null);
        }

        #endregion

        #region String
        /// <summary>
        /// Asserts that two files are not equal. If they are equal
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="expected">The path to a file containing the value that is expected</param>
        /// <param name="actual">The path to a file containing the actual value</param>
        /// <param name="message">The message to display if Streams are not equal</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void AreNotEqual(string expected, string actual, string message, params object?[]? args)
        {
            using FileStream exStream = File.OpenRead(expected);
            using FileStream acStream = File.OpenRead(actual);
            AreNotEqual(exStream, acStream, message, args);
        }

        /// <summary>
        /// Asserts that two files are not equal. If they are equal
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="expected">The path to a file containing the value that is expected</param>
        /// <param name="actual">The path to a file containing the actual value</param>
        public static void AreNotEqual(string expected, string actual)
        {
            AreNotEqual(expected, actual, string.Empty, null);
        }

        #endregion

        #endregion

        #region Exists

        #region FileInfo
        /// <summary>
        /// Asserts that the file exists. If it does not exist
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="actual">A file containing the actual value</param>
        /// <param name="message">The message to display if Streams are not equal</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void Exists(FileInfo actual, string message, params object?[]? args)
        {
            Framework.Assert.That(actual, new FileOrDirectoryExistsConstraint().IgnoreDirectories, () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Asserts that the file exists. If it does not exist
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="actual">A file containing the actual value</param>
        public static void Exists(FileInfo actual)
        {
            Exists(actual, string.Empty, null);
        }

        #endregion

        #region String
        /// <summary>
        /// Asserts that the file exists. If it does not exist
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="actual">The path to a file containing the actual value</param>
        /// <param name="message">The message to display if Streams are not equal</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void Exists(string actual, string message, params object?[]? args)
        {
            Framework.Assert.That(actual, new FileOrDirectoryExistsConstraint().IgnoreDirectories, () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Asserts that the file exists. If it does not exist
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="actual">The path to a file containing the actual value</param>
        public static void Exists(string actual)
        {
            Exists(actual, string.Empty, null);
        }

        #endregion

        #endregion

        #region DoesNotExist

        #region FileInfo
        /// <summary>
        /// Asserts that the file does not exist. If it does exist
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="actual">A file containing the actual value</param>
        /// <param name="message">The message to display if Streams are not equal</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void DoesNotExist(FileInfo actual, string message, params object?[]? args)
        {
            Framework.Assert.That(actual, new NotConstraint(new FileOrDirectoryExistsConstraint().IgnoreDirectories), () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Asserts that the file does not exist. If it does exist
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="actual">A file containing the actual value</param>
        public static void DoesNotExist(FileInfo actual)
        {
            DoesNotExist(actual, string.Empty, null);
        }

        #endregion

        #region String
        /// <summary>
        /// Asserts that the file does not exist. If it does exist
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="actual">The path to a file containing the actual value</param>
        /// <param name="message">The message to display if Streams are not equal</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void DoesNotExist(string actual, string message, params object?[]? args)
        {
            Framework.Assert.That(actual, new NotConstraint(new FileOrDirectoryExistsConstraint().IgnoreDirectories), () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Asserts that the file does not exist. If it does exist
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="actual">The path to a file containing the actual value</param>
        public static void DoesNotExist(string actual)
        {
            DoesNotExist(actual, string.Empty, null);
        }

        #endregion

        #endregion
    }
}
