// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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

using System;
using System.ComponentModel;

namespace NUnit.Framework
{
    using NUnit.Framework.Constraints;

    /// <summary>
    /// Basic Asserts on strings.
    /// </summary>
    public class StringAssert
    {
        #region Equals and ReferenceEquals

        /// <summary>
        /// The Equals method throws an InvalidOperationException. This is done 
        /// to make sure there is no mistake by calling this function.
        /// </summary>
        /// <param name="a">The left object</param>
        /// <param name="b">The right object</param>
        /// <returns>Not applicable</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static new bool Equals(object a, object b)
        {
            throw new InvalidOperationException("StringAssert.Equals should not be used for Assertions");
        }

        /// <summary>
        /// override the default ReferenceEquals to throw an InvalidOperationException. This 
        /// implementation makes sure there is no mistake in calling this function 
        /// as part of Assert. 
        /// </summary>
        /// <param name="a">The left object</param>
        /// <param name="b">The right object</param>
        public static new void ReferenceEquals(object a, object b)
        {
            throw new InvalidOperationException("StringAssert.ReferenceEquals should not be used for Assertions");
        }

        #endregion

        #region Contains

        /// <summary>
        /// Asserts that a string is found within another string.
        /// </summary>
        /// <param name="expected">The expected string</param>
        /// <param name="actual">The string to be examined</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void Contains(string expected, string actual, Func<ConstraintResult, string> getExceptionMessage)
        {
            Assert.That(actual, Does.Contain(expected), getExceptionMessage);
        }

        /// <summary>
        /// Asserts that a string is found within another string.
        /// </summary>
        /// <param name="expected">The expected string</param>
        /// <param name="actual">The string to be examined</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void Contains(string expected, string actual, Func<string> getExceptionMessage)
        {
            Assert.That(actual, Does.Contain(expected), getExceptionMessage);
        }

        /// <summary>
        /// Asserts that a string is found within another string.
        /// </summary>
        /// <param name="expected">The expected string</param>
        /// <param name="actual">The string to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Arguments used in formatting the message</param>
        public static void Contains(string expected, string actual, string message, params object[] args)
        {
            Assert.That(actual, Does.Contain(expected), message, args);
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
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void DoesNotContain(string expected, string actual, Func<ConstraintResult, string> getExceptionMessage)
        {
            Assert.That(actual, Does.Not.Contain(expected), getExceptionMessage);
        }

        /// <summary>
        /// Asserts that a string is not found within another string.
        /// </summary>
        /// <param name="expected">The expected string</param>
        /// <param name="actual">The string to be examined</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void DoesNotContain(string expected, string actual, Func<string> getExceptionMessage)
        {
            Assert.That(actual, Does.Not.Contain(expected), getExceptionMessage);
        }

        /// <summary>
        /// Asserts that a string is not found within another string.
        /// </summary>
        /// <param name="expected">The expected string</param>
        /// <param name="actual">The string to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Arguments used in formatting the message</param>
        public static void DoesNotContain(string expected, string actual, string message, params object[] args)
        {
            Assert.That(actual, Does.Not.Contain(expected), message, args);
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
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void StartsWith(string expected, string actual, Func<ConstraintResult, string> getExceptionMessage)
        {
            Assert.That(actual, Does.StartWith(expected), getExceptionMessage);
        }

        /// <summary>
        /// Asserts that a string starts with another string.
        /// </summary>
        /// <param name="expected">The expected string</param>
        /// <param name="actual">The string to be examined</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void StartsWith(string expected, string actual, Func<string> getExceptionMessage)
        {
            Assert.That(actual, Does.StartWith(expected), getExceptionMessage);
        }

        /// <summary>
        /// Asserts that a string starts with another string.
        /// </summary>
        /// <param name="expected">The expected string</param>
        /// <param name="actual">The string to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Arguments used in formatting the message</param>
        public static void StartsWith(string expected, string actual, string message, params object[] args)
        {
            Assert.That(actual, Does.StartWith(expected), message, args);
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
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void DoesNotStartWith(string expected, string actual, Func<ConstraintResult, string> getExceptionMessage)
        {
            Assert.That(actual, Does.Not.StartWith(expected), getExceptionMessage);
        }

        /// <summary>
        /// Asserts that a string does not start with another string.
        /// </summary>
        /// <param name="expected">The expected string</param>
        /// <param name="actual">The string to be examined</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void DoesNotStartWith(string expected, string actual, Func<string> getExceptionMessage)
        {
            Assert.That(actual, Does.Not.StartWith(expected), getExceptionMessage);
        }

        /// <summary>
        /// Asserts that a string does not start with another string.
        /// </summary>
        /// <param name="expected">The expected string</param>
        /// <param name="actual">The string to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Arguments used in formatting the message</param>
        public static void DoesNotStartWith(string expected, string actual, string message, params object[] args)
        {
            Assert.That(actual, Does.Not.StartWith(expected), message, args);
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
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void EndsWith(string expected, string actual, Func<ConstraintResult, string> getExceptionMessage)
        {
            Assert.That(actual, Does.EndWith(expected), getExceptionMessage);
        }

        /// <summary>
        /// Asserts that a string ends with another string.
        /// </summary>
        /// <param name="expected">The expected string</param>
        /// <param name="actual">The string to be examined</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void EndsWith(string expected, string actual, Func<string> getExceptionMessage)
        {
            Assert.That(actual, Does.EndWith(expected), getExceptionMessage);
        }

        /// <summary>
        /// Asserts that a string ends with another string.
        /// </summary>
        /// <param name="expected">The expected string</param>
        /// <param name="actual">The string to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Arguments used in formatting the message</param>
        public static void EndsWith(string expected, string actual, string message, params object[] args)
        {
            Assert.That(actual, Does.EndWith(expected), message, args);
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
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void DoesNotEndWith(string expected, string actual, Func<ConstraintResult, string> getExceptionMessage)
        {
            Assert.That(actual, Does.Not.EndWith(expected), getExceptionMessage);
        }

        /// <summary>
        /// Asserts that a string does not end with another string.
        /// </summary>
        /// <param name="expected">The expected string</param>
        /// <param name="actual">The string to be examined</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void DoesNotEndWith(string expected, string actual, Func<string> getExceptionMessage)
        {
            Assert.That(actual, Does.Not.EndWith(expected), getExceptionMessage);
        }

        /// <summary>
        /// Asserts that a string does not end with another string.
        /// </summary>
        /// <param name="expected">The expected string</param>
        /// <param name="actual">The string to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Arguments used in formatting the message</param>
        public static void DoesNotEndWith(string expected, string actual, string message, params object[] args)
        {
            Assert.That(actual, Does.Not.EndWith(expected), message, args);
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
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void AreEqualIgnoringCase(string expected, string actual, Func<ConstraintResult, string> getExceptionMessage)
        {
            Assert.That(actual, Is.EqualTo(expected).IgnoreCase, getExceptionMessage);
        }

        /// <summary>
        /// Asserts that two strings are equal, without regard to case.
        /// </summary>
        /// <param name="expected">The expected string</param>
        /// <param name="actual">The actual string</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void AreEqualIgnoringCase(string expected, string actual, Func<string> getExceptionMessage)
        {
            Assert.That(actual, Is.EqualTo(expected).IgnoreCase, getExceptionMessage);
        }

        /// <summary>
        /// Asserts that two strings are equal, without regard to case.
        /// </summary>
        /// <param name="expected">The expected string</param>
        /// <param name="actual">The actual string</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Arguments used in formatting the message</param>
        public static void AreEqualIgnoringCase(string expected, string actual, string message, params object[] args)
        {
            Assert.That(actual, Is.EqualTo(expected).IgnoreCase, message, args);
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
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void AreNotEqualIgnoringCase(string expected, string actual, Func<ConstraintResult, string> getExceptionMessage)
        {
            Assert.That(actual, Is.Not.EqualTo(expected).IgnoreCase, getExceptionMessage);
        }

        /// <summary>
        /// Asserts that two strings are not equal, without regard to case.
        /// </summary>
        /// <param name="expected">The expected string</param>
        /// <param name="actual">The actual string</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void AreNotEqualIgnoringCase(string expected, string actual, Func<string> getExceptionMessage)
        {
            Assert.That(actual, Is.Not.EqualTo(expected).IgnoreCase, getExceptionMessage);
        }

        /// <summary>
        /// Asserts that two strings are not equal, without regard to case.
        /// </summary>
        /// <param name="expected">The expected string</param>
        /// <param name="actual">The actual string</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Arguments used in formatting the message</param>
        public static void AreNotEqualIgnoringCase(string expected, string actual, string message, params object[] args)
        {
            Assert.That(actual, Is.Not.EqualTo(expected).IgnoreCase, message, args);
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
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void IsMatch(string pattern, string actual, Func<ConstraintResult, string> getExceptionMessage)
        {
            Assert.That(actual, Does.Match(pattern), getExceptionMessage);
        }

        /// <summary>
        /// Asserts that a string matches an expected regular expression pattern.
        /// </summary>
        /// <param name="pattern">The regex pattern to be matched</param>
        /// <param name="actual">The actual string</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void IsMatch(string pattern, string actual, Func<string> getExceptionMessage)
        {
            Assert.That(actual, Does.Match(pattern), getExceptionMessage);
        }

        /// <summary>
        /// Asserts that a string matches an expected regular expression pattern.
        /// </summary>
        /// <param name="pattern">The regex pattern to be matched</param>
        /// <param name="actual">The actual string</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Arguments used in formatting the message</param>
        public static void IsMatch(string pattern, string actual, string message, params object[] args)
        {
            Assert.That(actual, Does.Match(pattern), message, args);
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
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void DoesNotMatch(string pattern, string actual, Func<ConstraintResult, string> getExceptionMessage)
        {
            Assert.That(actual, Does.Not.Match(pattern), getExceptionMessage);
        } 

        /// <summary>
        /// Asserts that a string does not match an expected regular expression pattern.
        /// </summary>
        /// <param name="pattern">The regex pattern to be used</param>
        /// <param name="actual">The actual string</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void DoesNotMatch(string pattern, string actual, Func<string> getExceptionMessage)
        {
            Assert.That(actual, Does.Not.Match(pattern), getExceptionMessage);
        } 

        /// <summary>
        /// Asserts that a string does not match an expected regular expression pattern.
        /// </summary>
        /// <param name="pattern">The regex pattern to be used</param>
        /// <param name="actual">The actual string</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Arguments used in formatting the message</param>
        public static void DoesNotMatch(string pattern, string actual, string message, params object[] args)
        {
            Assert.That(actual, Does.Not.Match(pattern), message, args);
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
    }
}
