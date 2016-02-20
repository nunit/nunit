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
    /// Asserts on Directories
    /// </summary>
    public static class DirectoryAssert
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
            throw new InvalidOperationException("DirectoryAssert.Equals should not be used for Assertions");
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
            throw new InvalidOperationException("DirectoryAssert.ReferenceEquals should not be used for Assertions");
        }

        #endregion

        #region AreEqual

        #region DirectoryInfo
        /// <summary>
        /// Verifies that two directories are equal.  Two directories are considered
        /// equal if both are null, or if both point to the same directory.
        /// If they are not equal an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="expected">A directory containing the value that is expected</param>
        /// <param name="actual">A directory containing the actual value</param>
        /// <param name="message">The message to display if the directories are not equal</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        static public void AreEqual(DirectoryInfo expected, DirectoryInfo actual, string message, params object[] args)
        {
            Assert.AreEqual(expected, actual, message, args);
        }

        /// <summary>
        /// Verifies that two directories are equal.  Two directories are considered
        /// equal if both are null, or if both point to the same directory.
        /// If they are not equal an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="expected">A directory containing the value that is expected</param>
        /// <param name="actual">A directory containing the actual value</param>
        static public void AreEqual(DirectoryInfo expected, DirectoryInfo actual)
        {
            AreEqual(expected, actual, String.Empty, null);
        }

        #endregion

        #endregion

        #region AreNotEqual

        #region DirectoryInfo
        /// <summary>
        /// Asserts that two directories are not equal. If they are equal
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="expected">A directory containing the value that is expected</param>
        /// <param name="actual">A directory containing the actual value</param>
        /// <param name="message">The message to display if directories are not equal</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        static public void AreNotEqual(DirectoryInfo expected, DirectoryInfo actual, string message, params object[] args)
        {
            Assert.AreNotEqual(expected, actual, message, args);
        }

        /// <summary>
        /// Asserts that two directories are not equal. If they are equal
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="expected">A directory containing the value that is expected</param>
        /// <param name="actual">A directory containing the actual value</param>
        static public void AreNotEqual(DirectoryInfo expected, DirectoryInfo actual)
        {
            AreNotEqual(expected, actual, string.Empty, null);
        }

        #endregion

        #endregion

        #region Exists

        #region DirectoryInfo
        /// <summary>
        /// Asserts that the directory exists. If it does not exist
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="actual">A directory containing the actual value</param>
        /// <param name="message">The message to display if directories are not equal</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        static public void Exists(DirectoryInfo actual, string message, params object[] args)
        {
            Assert.That(actual, new FileOrDirectoryExistsConstraint().IgnoreFiles, message, args);
        }

        /// <summary>
        /// Asserts that the directory exists. If it does not exist
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="actual">A directory containing the actual value</param>
        static public void Exists(DirectoryInfo actual)
        {
            Exists(actual, string.Empty, null);
        }

        #endregion

        #region String
        /// <summary>
        /// Asserts that the directory exists. If it does not exist
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="actual">The path to a directory containing the actual value</param>
        /// <param name="message">The message to display if directories are not equal</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        static public void Exists(string actual, string message, params object[] args)
        {
            Assert.That(actual, new FileOrDirectoryExistsConstraint().IgnoreFiles, message, args);
        }

        /// <summary>
        /// Asserts that the directory exists. If it does not exist
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="actual">The path to a directory containing the actual value</param>
        static public void Exists(string actual)
        {
            Exists(actual, string.Empty, null);
        }

        #endregion

        #endregion

        #region DoesNotExist

        #region DirectoryInfo
        /// <summary>
        /// Asserts that the directory does not exist. If it does exist
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="actual">A directory containing the actual value</param>
        /// <param name="message">The message to display if directories are not equal</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        static public void DoesNotExist(DirectoryInfo actual, string message, params object[] args)
        {
            Assert.That(actual, new NotConstraint(new FileOrDirectoryExistsConstraint().IgnoreFiles), message, args);
        }

        /// <summary>
        /// Asserts that the directory does not exist. If it does exist
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="actual">A directory containing the actual value</param>
        static public void DoesNotExist(DirectoryInfo actual)
        {
            DoesNotExist(actual, string.Empty, null);
        }

        #endregion

        #region String
        /// <summary>
        /// Asserts that the directory does not exist. If it does exist
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="actual">The path to a directory containing the actual value</param>
        /// <param name="message">The message to display if directories are not equal</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        static public void DoesNotExist(string actual, string message, params object[] args)
        {
            Assert.That(actual, new NotConstraint(new FileOrDirectoryExistsConstraint().IgnoreFiles), message, args);
        }

        /// <summary>
        /// Asserts that the directory does not exist. If it does exist
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="actual">The path to a directory containing the actual value</param>
        static public void DoesNotExist(string actual)
        {
            DoesNotExist(actual, string.Empty, null);
        }

        #endregion

        #endregion
    }
}
#endif