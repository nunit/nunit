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
        #region Zero

        #region Ints

        /// <summary>
        /// Asserts that an int is zero.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void Zero(int actual)
        {
            Assert.That(actual, Is.Zero);
        }

        /// <summary>
        /// Asserts that an int is zero.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Zero(int actual, string message, params object[] args)
        {
            Assert.That(actual, Is.Zero, message, args);
        }

        #endregion

        #region UnsignedInts

        /// <summary>
        /// Asserts that an unsigned int is zero.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        [CLSCompliant(false)]
        public static void Zero(uint actual)
        {
            Assert.That(actual, Is.Zero);
        }

        /// <summary>
        /// Asserts that an unsigned int is zero.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        [CLSCompliant(false)]
        public static void Zero(uint actual, string message, params object[] args)
        {
            Assert.That(actual, Is.Zero, message, args);
        }

        #endregion

        #region Longs

        /// <summary>
        /// Asserts that a Long is zero.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void Zero(long actual)
        {
            Assert.That(actual, Is.Zero);
        }

        /// <summary>
        /// Asserts that a Long is zero.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Zero(long actual, string message, params object[] args)
        {
            Assert.That(actual, Is.Zero, message, args);
        }

        #endregion

        #region UnsignedLongs
        
        /// <summary>
        /// Asserts that an unsigned Long is zero.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        [CLSCompliant(false)]
        public static void Zero(ulong actual)
        {
            Assert.That(actual, Is.Zero);
        }

        /// <summary>
        /// Asserts that an unsigned Long is zero.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        [CLSCompliant(false)]
        public static void Zero(ulong actual, string message, params object[] args)
        {
            Assert.That(actual, Is.Zero, message, args);
        }

        #endregion

        #region Decimals

        /// <summary>
        /// Asserts that a decimal is zero.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void Zero(decimal actual)
        {
            Assert.That(actual, Is.Zero);
        }

        /// <summary>
        /// Asserts that a decimal is zero.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Zero(decimal actual, string message, params object[] args)
        {
            Assert.That(actual, Is.Zero, message, args);
        }

        #endregion

        #region Doubles

        /// <summary>
        /// Asserts that a double is zero.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void Zero(double actual)
        {
            Assert.That(actual, Is.Zero);
        }

        /// <summary>
        /// Asserts that a double is zero.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Zero(double actual, string message, params object[] args)
        {
            Assert.That(actual, Is.Zero, message, args);
        }
        
        #endregion

        #region Floats
        
        /// <summary>
        /// Asserts that a float is zero.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void Zero(float actual)
        {
            Assert.That(actual, Is.Zero);
        }

        /// <summary>
        /// Asserts that a float is zero.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Zero(float actual, string message, params object[] args)
        {
            Assert.That(actual, Is.Zero, message, args);
        }

        #endregion

        #endregion

        #region NotZero

        #region Ints

        /// <summary>
        /// Asserts that an int is not zero.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void NotZero(int actual)
        {
            Assert.That(actual, Is.Not.Zero);
        }

        /// <summary>
        /// Asserts that an int is not zero.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void NotZero(int actual, string message, params object[] args)
        {
            Assert.That(actual, Is.Not.Zero, message, args);
        }

        #endregion

        #region UnsignedInts

        /// <summary>
        /// Asserts that an unsigned int is not zero.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        [CLSCompliant(false)]
        public static void NotZero(uint actual)
        {
            Assert.That(actual, Is.Not.Zero);
        }

        /// <summary>
        /// Asserts that an unsigned int is not zero.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        [CLSCompliant(false)]
        public static void NotZero(uint actual, string message, params object[] args)
        {
            Assert.That(actual, Is.Not.Zero, message, args);
        }

        #endregion

        #region Longs

        /// <summary>
        /// Asserts that a Long is not zero.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void NotZero(long actual)
        {
            Assert.That(actual, Is.Not.Zero);
        }

        /// <summary>
        /// Asserts that a Long is not zero.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void NotZero(long actual, string message, params object[] args)
        {
            Assert.That(actual, Is.Not.Zero, message, args);
        }

        #endregion

        #region UnsignedLongs

        /// <summary>
        /// Asserts that an unsigned Long is not zero.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        [CLSCompliant(false)]
        public static void NotZero(ulong actual)
        {
            Assert.That(actual, Is.Not.Zero);
        }

        /// <summary>
        /// Asserts that an unsigned Long is not zero.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        [CLSCompliant(false)]
        public static void NotZero(ulong actual, string message, params object[] args)
        {
            Assert.That(actual, Is.Not.Zero, message, args);
        }

        #endregion

        #region Decimals

        /// <summary>
        /// Asserts that a decimal is zero.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void NotZero(decimal actual)
        {
            Assert.That(actual, Is.Not.Zero);
        }

        /// <summary>
        /// Asserts that a decimal is zero.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void NotZero(decimal actual, string message, params object[] args)
        {
            Assert.That(actual, Is.Not.Zero, message, args);
        }

        #endregion

        #region Doubles

        /// <summary>
        /// Asserts that a double is zero.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void NotZero(double actual)
        {
            Assert.That(actual, Is.Not.Zero);
        }

        /// <summary>
        /// Asserts that a double is zero.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void NotZero(double actual, string message, params object[] args)
        {
            Assert.That(actual, Is.Not.Zero, message, args);
        }

        #endregion

        #region Floats

        /// <summary>
        /// Asserts that a float is zero.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        public static void NotZero(float actual)
        {
            Assert.That(actual, Is.Not.Zero);
        }

        /// <summary>
        /// Asserts that a float is zero.
        /// </summary>
        /// <param name="actual">The number to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void NotZero(float actual, string message, params object[] args)
        {
            Assert.That(actual, Is.Not.Zero, message, args);
        }

        #endregion

        #endregion
    }
}
