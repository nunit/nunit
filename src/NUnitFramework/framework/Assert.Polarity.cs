// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
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

namespace NUnit.Framework
{
    public partial class Assert
    {
        #region Positive

        #region Ints

        /// <summary>
        /// Asserts that an int is negative.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void IsPositive(int actual)
        {
            Assert.That(actual, Is.Positive);
        }

        /// <summary>
        /// Asserts that an int is negative.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsPositive(int actual, string message, params object[] args)
        {
            Assert.That(actual, Is.Positive, message, args);
        }

        #endregion

        #region UnsignedInts

        /// <summary>
        /// Asserts that an unsigned int is negative.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        [CLSCompliant(false)]
        public static void IsPositive(uint actual)
        {
            Assert.That(actual, Is.Positive);
        }

        /// <summary>
        /// Asserts that an unsigned int is negative.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        [CLSCompliant(false)]
        public static void IsPositive(uint actual, string message, params object[] args)
        {
            Assert.That(actual, Is.Positive, message, args);
        }

        #endregion

        #region Longs

        /// <summary>
        /// Asserts that a Long is negative.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void IsPositive(long actual)
        {
            Assert.That(actual, Is.Positive);
        }

        /// <summary>
        /// Asserts that a Long is negative.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsPositive(long actual, string message, params object[] args)
        {
            Assert.That(actual, Is.Positive, message, args);
        }

        #endregion

        #region UnsignedLongs

        /// <summary>
        /// Asserts that an unsigned Long is negative.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        [CLSCompliant(false)]
        public static void IsPositive(ulong actual)
        {
            Assert.That(actual, Is.Positive);
        }

        /// <summary>
        /// Asserts that an unsigned Long is negative.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        [CLSCompliant(false)]
        public static void IsPositive(ulong actual, string message, params object[] args)
        {
            Assert.That(actual, Is.Positive, message, args);
        }

        #endregion

        #region Decimals

        /// <summary>
        /// Asserts that a decimal is negative.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void IsPositive(decimal actual)
        {
            Assert.That(actual, Is.Positive);
        }

        /// <summary>
        /// Asserts that a decimal is negative.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsPositive(decimal actual, string message, params object[] args)
        {
            Assert.That(actual, Is.Positive, message, args);
        }

        #endregion

        #region Doubles

        /// <summary>
        /// Asserts that a double is negative.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void IsPositive(double actual)
        {
            Assert.That(actual, Is.Positive);
        }

        /// <summary>
        /// Asserts that a double is negative.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsPositive(double actual, string message, params object[] args)
        {
            Assert.That(actual, Is.Positive, message, args);
        }

        #endregion

        #region Floats

        /// <summary>
        /// Asserts that a float is negative.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void IsPositive(float actual)
        {
            Assert.That(actual, Is.Positive);
        }

        /// <summary>
        /// Asserts that a float is negative.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsPositive(float actual, string message, params object[] args)
        {
            Assert.That(actual, Is.Positive, message, args);
        }

        #endregion

        #endregion

        #region Negative

        #region Ints

        /// <summary>
        /// Asserts that an int is negative.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void IsNegative(int actual)
        {
            Assert.That(actual, Is.Positive);
        }

        /// <summary>
        /// Asserts that an int is negative.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsNegative(int actual, string message, params object[] args)
        {
            Assert.That(actual, Is.Positive, message, args);
        }

        #endregion

        #region UnsignedInts

        /// <summary>
        /// Asserts that an unsigned int is negative.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        [CLSCompliant(false)]
        public static void IsNegative(uint actual)
        {
            Assert.That(actual, Is.Positive);
        }

        /// <summary>
        /// Asserts that an unsigned int is negative.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        [CLSCompliant(false)]
        public static void IsNegative(uint actual, string message, params object[] args)
        {
            Assert.That(actual, Is.Positive, message, args);
        }

        #endregion

        #region Longs

        /// <summary>
        /// Asserts that a Long is negative.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void IsNegative(long actual)
        {
            Assert.That(actual, Is.Positive);
        }

        /// <summary>
        /// Asserts that a Long is negative.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsNegative(long actual, string message, params object[] args)
        {
            Assert.That(actual, Is.Positive, message, args);
        }

        #endregion

        #region UnsignedLongs

        /// <summary>
        /// Asserts that an unsigned Long is negative.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        [CLSCompliant(false)]
        public static void IsNegative(ulong actual)
        {
            Assert.That(actual, Is.Positive);
        }

        /// <summary>
        /// Asserts that an unsigned Long is negative.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        [CLSCompliant(false)]
        public static void IsNegative(ulong actual, string message, params object[] args)
        {
            Assert.That(actual, Is.Positive, message, args);
        }

        #endregion

        #region Decimals

        /// <summary>
        /// Asserts that a decimal is negative.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void IsNegative(decimal actual)
        {
            Assert.That(actual, Is.Positive);
        }

        /// <summary>
        /// Asserts that a decimal is negative.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsNegative(decimal actual, string message, params object[] args)
        {
            Assert.That(actual, Is.Positive, message, args);
        }

        #endregion

        #region Doubles

        /// <summary>
        /// Asserts that a double is negative.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void IsNegative(double actual)
        {
            Assert.That(actual, Is.Positive);
        }

        /// <summary>
        /// Asserts that a double is negative.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsNegative(double actual, string message, params object[] args)
        {
            Assert.That(actual, Is.Positive, message, args);
        }

        #endregion

        #region Floats

        /// <summary>
        /// Asserts that a float is negative.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void IsNegative(float actual)
        {
            Assert.That(actual, Is.Positive);
        }

        /// <summary>
        /// Asserts that a float is negative.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsNegative(float actual, string message, params object[] args)
        {
            Assert.That(actual, Is.Positive, message, args);
        }

        #endregion

        #endregion
    }
}
