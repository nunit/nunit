// ***********************************************************************
// Copyright (c) 2006 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

#if !SILVERLIGHT && !PORTABLE
using System;
using System.IO;
using System.ComponentModel;
using NUnit.Framework.Constraints;

namespace NUnit.Framework
{
    /// <summary>
    /// Asserts on Files
    /// </summary>
    public static class FileAssert
    {
        #region Equals and ReferenceEquals

        /// <summary>
        /// The Equals method throws an InvalidOperationException. This is done 
        /// to make sure there is no mistake by calling this function.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static new bool Equals(object a, object b)
        {
            throw new InvalidOperationException("FileAssert.Equals should not be used for Assertions");
        }

        /// <summary>
        /// override the default ReferenceEquals to throw an InvalidOperationException. This 
        /// implementation makes sure there is no mistake in calling this function 
        /// as part of Assert. 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static new void ReferenceEquals(object a, object b)
        {
            throw new InvalidOperationException("FileAssert.ReferenceEquals should not be used for Assertions");
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
        static public void AreEqual(Stream expected, Stream actual, string message, params object[] args)
        {
            Assert.That(actual, Is.EqualTo(expected), message, args);
        }

        /// <summary>
        /// Verifies that two Streams are equal.  Two Streams are considered
        /// equal if both are null, or if both have the same value byte for byte.
        /// If they are not equal an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="expected">The expected Stream</param>
        /// <param name="actual">The actual Stream</param>
        static public void AreEqual(Stream expected, Stream actual)
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
        static public void AreEqual(FileInfo expected, FileInfo actual, string message, params object[] args)
        {
            using (FileStream exStream = expected.OpenRead())
            using (FileStream acStream = actual.OpenRead())
            {
                AreEqual(exStream, acStream, message, args);
            }
        }

        /// <summary>
        /// Verifies that two files are equal.  Two files are considered
        /// equal if both are null, or if both have the same value byte for byte.
        /// If they are not equal an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="expected">A file containing the value that is expected</param>
        /// <param name="actual">A file containing the actual value</param>
        static public void AreEqual(FileInfo expected, FileInfo actual)
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
        static public void AreEqual(string expected, string actual, string message, params object[] args)
        {
            using (FileStream exStream = File.OpenRead(expected))
            using (FileStream acStream = File.OpenRead(actual))
            {
                AreEqual(exStream, acStream, message, args);
            }
        }

        /// <summary>
        /// Verifies that two files are equal.  Two files are considered
        /// equal if both are null, or if both have the same value byte for byte.
        /// If they are not equal an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="expected">The path to a file containing the value that is expected</param>
        /// <param name="actual">The path to a file containing the actual value</param>
        static public void AreEqual(string expected, string actual)
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
        static public void AreNotEqual(Stream expected, Stream actual, string message, params object[] args)
        {
            Assert.That(actual, Is.Not.EqualTo(expected), message, args);
        }

        /// <summary>
        /// Asserts that two Streams are not equal. If they are equal
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="expected">The expected Stream</param>
        /// <param name="actual">The actual Stream</param>
        static public void AreNotEqual(Stream expected, Stream actual)
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
        static public void AreNotEqual(FileInfo expected, FileInfo actual, string message, params object[] args)
        {
            using (FileStream exStream = expected.OpenRead())
            using (FileStream acStream = actual.OpenRead())
            {
                AreNotEqual(exStream, acStream, message, args);
            }
        }

        /// <summary>
        /// Asserts that two files are not equal. If they are equal
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="expected">A file containing the value that is expected</param>
        /// <param name="actual">A file containing the actual value</param>
        static public void AreNotEqual(FileInfo expected, FileInfo actual)
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
        static public void AreNotEqual(string expected, string actual, string message, params object[] args)
        {
            using (FileStream exStream = File.OpenRead(expected))
            using (FileStream acStream = File.OpenRead(actual))
            {
                AreNotEqual(exStream, acStream, message, args);
            }
        }

        /// <summary>
        /// Asserts that two files are not equal. If they are equal
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="expected">The path to a file containing the value that is expected</param>
        /// <param name="actual">The path to a file containing the actual value</param>
        static public void AreNotEqual(string expected, string actual)
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
        static public void Exists(FileInfo actual, string message, params object[] args)
        {
            Assert.That(actual, new FileOrDirectoryExistsConstraint().IgnoreDirectories, message, args);
        }

        /// <summary>
        /// Asserts that the file exists. If it does not exist
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="actual">A file containing the actual value</param>
        static public void Exists(FileInfo actual)
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
        static public void Exists(string actual, string message, params object[] args)
        {
            Assert.That(actual, new FileOrDirectoryExistsConstraint().IgnoreDirectories, message, args);
        }

        /// <summary>
        /// Asserts that the file exists. If it does not exist
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="actual">The path to a file containing the actual value</param>
        static public void Exists(string actual)
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
        static public void DoesNotExist(FileInfo actual, string message, params object[] args)
        {
            Assert.That(actual, new NotConstraint(new FileOrDirectoryExistsConstraint().IgnoreDirectories), message, args);
        }

        /// <summary>
        /// Asserts that the file does not exist. If it does exist
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="actual">A file containing the actual value</param>
        static public void DoesNotExist(FileInfo actual)
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
        static public void DoesNotExist(string actual, string message, params object[] args)
        {
            Assert.That(actual, new NotConstraint(new FileOrDirectoryExistsConstraint().IgnoreDirectories), message, args);
        }

        /// <summary>
        /// Asserts that the file does not exist. If it does exist
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="actual">The path to a file containing the actual value</param>
        static public void DoesNotExist(string actual)
        {
            DoesNotExist(actual, string.Empty, null);
        }

        #endregion

        #endregion
    }
}
#endif
