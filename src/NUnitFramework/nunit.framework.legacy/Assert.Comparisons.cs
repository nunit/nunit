// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Legacy
{
    public partial class ClassicAssert
    {
        #region Greater

        #region Ints

        /// <summary>
        /// Verifies that the first int is greater than the second
        /// int. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Greater(int arg1, int arg2, string message, params object[] args)
        {
            Framework.Assert.That(arg1, Is.GreaterThan(arg2), () => AssertBase.ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that the first int is greater than the second
        /// int. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        public static void Greater(int arg1, int arg2)
        {
            Framework.Assert.That(arg1, Is.GreaterThan(arg2));
        }

        #endregion

        #region Unsigned Ints

        /// <summary>
        /// Verifies that the first value is greater than the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        [CLSCompliant(false)]
        public static void Greater(uint arg1, uint arg2, string message, params object[] args)
        {
            Framework.Assert.That(arg1, Is.GreaterThan(arg2), () => AssertBase.ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that the first value is greater than the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        [CLSCompliant(false)]
        public static void Greater(uint arg1, uint arg2)
        {
            Framework.Assert.That(arg1, Is.GreaterThan(arg2));
        }

        #endregion

        #region Longs

        /// <summary>
        /// Verifies that the first value is greater than the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Greater(long arg1, long arg2, string message, params object[] args)
        {
            Framework.Assert.That(arg1, Is.GreaterThan(arg2), () => AssertBase.ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that the first value is greater than the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        public static void Greater(long arg1, long arg2)
        {
            Framework.Assert.That(arg1, Is.GreaterThan(arg2));
        }

        #endregion

        #region Unsigned Longs

        /// <summary>
        /// Verifies that the first value is greater than the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        [CLSCompliant(false)]
        public static void Greater(ulong arg1, ulong arg2, string message, params object[] args)
        {
            Framework.Assert.That(arg1, Is.GreaterThan(arg2), () => AssertBase.ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that the first value is greater than the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        [CLSCompliant(false)]
        public static void Greater(ulong arg1, ulong arg2)
        {
            Framework.Assert.That(arg1, Is.GreaterThan(arg2));
        }

        #endregion

        #region Decimals

        /// <summary>
        /// Verifies that the first value is greater than the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Greater(decimal arg1, decimal arg2, string message, params object[] args)
        {
            Framework.Assert.That(arg1, Is.GreaterThan(arg2), () => AssertBase.ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that the first value is greater than the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        public static void Greater(decimal arg1, decimal arg2)
        {
            Framework.Assert.That(arg1, Is.GreaterThan(arg2));
        }

        #endregion

        #region Doubles

        /// <summary>
        /// Verifies that the first value is greater than the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Greater(double arg1, double arg2, string message, params object[] args)
        {
            Framework.Assert.That(arg1, Is.GreaterThan(arg2), () => AssertBase.ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that the first value is greater than the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        public static void Greater(double arg1, double arg2)
        {
            Framework.Assert.That(arg1, Is.GreaterThan(arg2));
        }

        #endregion

        #region Floats

        /// <summary>
        /// Verifies that the first value is greater than the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Greater(float arg1, float arg2, string message, params object[] args)
        {
            Framework.Assert.That(arg1, Is.GreaterThan(arg2), () => AssertBase.ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that the first value is greater than the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        public static void Greater(float arg1, float arg2)
        {
            Framework.Assert.That(arg1, Is.GreaterThan(arg2));
        }

        #endregion

        #region IComparables

        /// <summary>
        /// Verifies that the first value is greater than the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Greater(IComparable arg1, IComparable arg2, string message, params object[] args)
        {
            Framework.Assert.That(arg1, Is.GreaterThan(arg2), () => AssertBase.ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that the first value is greater than the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        public static void Greater(IComparable arg1, IComparable arg2)
        {
            Framework.Assert.That(arg1, Is.GreaterThan(arg2));
        }

        #endregion

        #endregion

        #region Less

        #region Ints

        /// <summary>
        /// Verifies that the first value is less than the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Less(int arg1, int arg2, string message, params object[] args)
        {
            Framework.Assert.That(arg1, Is.LessThan(arg2), () => AssertBase.ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that the first value is less than the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        public static void Less(int arg1, int arg2)
        {
            Framework.Assert.That(arg1, Is.LessThan(arg2));
        }

        #endregion

        #region Unsigned Ints

        /// <summary>
        /// Verifies that the first value is less than the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        [CLSCompliant(false)]
        public static void Less(uint arg1, uint arg2, string message, params object[] args)
        {
            Framework.Assert.That(arg1, Is.LessThan(arg2), () => AssertBase.ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that the first value is less than the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        [CLSCompliant(false)]
        public static void Less(uint arg1, uint arg2)
        {
            Framework.Assert.That(arg1, Is.LessThan(arg2));
        }

        #endregion

        #region Longs

        /// <summary>
        /// Verifies that the first value is less than the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Less(long arg1, long arg2, string message, params object[] args)
        {
            Framework.Assert.That(arg1, Is.LessThan(arg2), () => AssertBase.ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that the first value is less than the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        public static void Less(long arg1, long arg2)
        {
            Framework.Assert.That(arg1, Is.LessThan(arg2));
        }

        #endregion

        #region Unsigned Longs

        /// <summary>
        /// Verifies that the first value is less than the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        [CLSCompliant(false)]
        public static void Less(ulong arg1, ulong arg2, string message, params object[] args)
        {
            Framework.Assert.That(arg1, Is.LessThan(arg2), () => AssertBase.ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that the first value is less than the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        [CLSCompliant(false)]
        public static void Less(ulong arg1, ulong arg2)
        {
            Framework.Assert.That(arg1, Is.LessThan(arg2));
        }

        #endregion

        #region Decimals

        /// <summary>
        /// Verifies that the first value is less than the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Less(decimal arg1, decimal arg2, string message, params object[] args)
        {
            Framework.Assert.That(arg1, Is.LessThan(arg2), () => AssertBase.ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that the first value is less than the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        public static void Less(decimal arg1, decimal arg2)
        {
            Framework.Assert.That(arg1, Is.LessThan(arg2));
        }

        #endregion

        #region Doubles

        /// <summary>
        /// Verifies that the first value is less than the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Less(double arg1, double arg2, string message, params object[] args)
        {
            Framework.Assert.That(arg1, Is.LessThan(arg2), () => AssertBase.ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that the first value is less than the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        public static void Less(double arg1, double arg2)
        {
            Framework.Assert.That(arg1, Is.LessThan(arg2));
        }

        #endregion

        #region Floats

        /// <summary>
        /// Verifies that the first value is less than the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Less(float arg1, float arg2, string message, params object[] args)
        {
            Framework.Assert.That(arg1, Is.LessThan(arg2), () => AssertBase.ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that the first value is less than the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        public static void Less(float arg1, float arg2)
        {
            Framework.Assert.That(arg1, Is.LessThan(arg2));
        }

        #endregion

        #region IComparables

        /// <summary>
        /// Verifies that the first value is less than the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Less(IComparable arg1, IComparable arg2, string message, params object[] args)
        {
            Framework.Assert.That(arg1, Is.LessThan(arg2), () => AssertBase.ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that the first value is less than the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        public static void Less(IComparable arg1, IComparable arg2)
        {
            Framework.Assert.That(arg1, Is.LessThan(arg2));
        }

        #endregion

        #endregion

        #region GreaterOrEqual

        #region Ints

        /// <summary>
        /// Verifies that the first value is greater than or equal to the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void GreaterOrEqual(int arg1, int arg2, string message, params object[] args)
        {
            Framework.Assert.That(arg1, Is.GreaterThanOrEqualTo(arg2), () => AssertBase.ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that the first value is greater than or equal to the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        public static void GreaterOrEqual(int arg1, int arg2)
        {
            Framework.Assert.That(arg1, Is.GreaterThanOrEqualTo(arg2));
        }

        #endregion

        #region Unsigned Ints

        /// <summary>
        /// Verifies that the first value is greater than or equal to the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        [CLSCompliant(false)]
        public static void GreaterOrEqual(uint arg1, uint arg2, string message, params object[] args)
        {
            Framework.Assert.That(arg1, Is.GreaterThanOrEqualTo(arg2), () => AssertBase.ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that the first value is greater than or equal to the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        [CLSCompliant(false)]
        public static void GreaterOrEqual(uint arg1, uint arg2)
        {
            Framework.Assert.That(arg1, Is.GreaterThanOrEqualTo(arg2));
        }

        #endregion

        #region Longs

        /// <summary>
        /// Verifies that the first value is greater than or equal to the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void GreaterOrEqual(long arg1, long arg2, string message, params object[] args)
        {
            Framework.Assert.That(arg1, Is.GreaterThanOrEqualTo(arg2), () => AssertBase.ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that the first value is greater than or equal to the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        public static void GreaterOrEqual(long arg1, long arg2)
        {
            Framework.Assert.That(arg1, Is.GreaterThanOrEqualTo(arg2));
        }

        #endregion

        #region Unsigned Longs

        /// <summary>
        /// Verifies that the first value is greater than or equal to the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        [CLSCompliant(false)]
        public static void GreaterOrEqual(ulong arg1, ulong arg2, string message, params object[] args)
        {
            Framework.Assert.That(arg1, Is.GreaterThanOrEqualTo(arg2), () => AssertBase.ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that the first value is greater than or equal to the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        [CLSCompliant(false)]
        public static void GreaterOrEqual(ulong arg1, ulong arg2)
        {
            Framework.Assert.That(arg1, Is.GreaterThanOrEqualTo(arg2));
        }

        #endregion

        #region Decimals

        /// <summary>
        /// Verifies that the first value is greater than or equal to the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void GreaterOrEqual(decimal arg1, decimal arg2, string message, params object[] args)
        {
            Framework.Assert.That(arg1, Is.GreaterThanOrEqualTo(arg2), () => AssertBase.ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that the first value is greater than or equal to the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        public static void GreaterOrEqual(decimal arg1, decimal arg2)
        {
            Framework.Assert.That(arg1, Is.GreaterThanOrEqualTo(arg2));
        }

        #endregion

        #region Doubles

        /// <summary>
        /// Verifies that the first value is greater than or equal to the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void GreaterOrEqual(double arg1, double arg2, string message, params object[] args)
        {
            Framework.Assert.That(arg1, Is.GreaterThanOrEqualTo(arg2), () => AssertBase.ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that the first value is greater than or equal to the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        public static void GreaterOrEqual(double arg1, double arg2)
        {
            Framework.Assert.That(arg1, Is.GreaterThanOrEqualTo(arg2));
        }

        #endregion

        #region Floats

        /// <summary>
        /// Verifies that the first value is greater than or equal to the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void GreaterOrEqual(float arg1, float arg2, string message, params object[] args)
        {
            Framework.Assert.That(arg1, Is.GreaterThanOrEqualTo(arg2), () => AssertBase.ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that the first value is greater than or equal to the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        public static void GreaterOrEqual(float arg1, float arg2)
        {
            Framework.Assert.That(arg1, Is.GreaterThanOrEqualTo(arg2));
        }

        #endregion

        #region IComparables

        /// <summary>
        /// Verifies that the first value is greater than or equal to the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void GreaterOrEqual(IComparable arg1, IComparable arg2, string message, params object[] args)
        {
            Framework.Assert.That(arg1, Is.GreaterThanOrEqualTo(arg2), () => AssertBase.ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that the first value is greater than or equal to the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        public static void GreaterOrEqual(IComparable arg1, IComparable arg2)
        {
            Framework.Assert.That(arg1, Is.GreaterThanOrEqualTo(arg2));
        }

        #endregion

        #endregion

        #region LessOrEqual

        #region Ints

        /// <summary>
        /// Verifies that the first value is less than or equal to the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void LessOrEqual(int arg1, int arg2, string message, params object[] args)
        {
            Framework.Assert.That(arg1, Is.LessThanOrEqualTo(arg2), () => AssertBase.ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that the first value is less than or equal to the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        public static void LessOrEqual(int arg1, int arg2)
        {
            Framework.Assert.That(arg1, Is.LessThanOrEqualTo(arg2));
        }

        #endregion

        #region Unsigned Ints

        /// <summary>
        /// Verifies that the first value is less than or equal to the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        [CLSCompliant(false)]
        public static void LessOrEqual(uint arg1, uint arg2, string message, params object[] args)
        {
            Framework.Assert.That(arg1, Is.LessThanOrEqualTo(arg2), () => AssertBase.ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that the first value is less than or equal to the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        [CLSCompliant(false)]
        public static void LessOrEqual(uint arg1, uint arg2)
        {
            Framework.Assert.That(arg1, Is.LessThanOrEqualTo(arg2));
        }

        #endregion

        #region Longs

        /// <summary>
        /// Verifies that the first value is less than or equal to the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void LessOrEqual(long arg1, long arg2, string message, params object[] args)
        {
            Framework.Assert.That(arg1, Is.LessThanOrEqualTo(arg2), () => AssertBase.ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that the first value is less than or equal to the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        public static void LessOrEqual(long arg1, long arg2)
        {
            Framework.Assert.That(arg1, Is.LessThanOrEqualTo(arg2));
        }

        #endregion

        #region Unsigned Longs

        /// <summary>
        /// Verifies that the first value is less than or equal to the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        [CLSCompliant(false)]
        public static void LessOrEqual(ulong arg1, ulong arg2, string message, params object[] args)
        {
            Framework.Assert.That(arg1, Is.LessThanOrEqualTo(arg2), () => AssertBase.ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that the first value is less than or equal to the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        [CLSCompliant(false)]
        public static void LessOrEqual(ulong arg1, ulong arg2)
        {
            Framework.Assert.That(arg1, Is.LessThanOrEqualTo(arg2));
        }

        #endregion

        #region Decimals

        /// <summary>
        /// Verifies that the first value is less than or equal to the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void LessOrEqual(decimal arg1, decimal arg2, string message, params object[] args)
        {
            Framework.Assert.That(arg1, Is.LessThanOrEqualTo(arg2), () => AssertBase.ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that the first value is less than or equal to the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        public static void LessOrEqual(decimal arg1, decimal arg2)
        {
            Framework.Assert.That(arg1, Is.LessThanOrEqualTo(arg2));
        }

        #endregion

        #region Doubles

        /// <summary>
        /// Verifies that the first value is less than or equal to the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void LessOrEqual(double arg1, double arg2, string message, params object[] args)
        {
            Framework.Assert.That(arg1, Is.LessThanOrEqualTo(arg2), () => AssertBase.ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that the first value is less than or equal to the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        public static void LessOrEqual(double arg1, double arg2)
        {
            Framework.Assert.That(arg1, Is.LessThanOrEqualTo(arg2));
        }

        #endregion

        #region Floats

        /// <summary>
        /// Verifies that the first value is less than or equal to the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void LessOrEqual(float arg1, float arg2, string message, params object[] args)
        {
            Framework.Assert.That(arg1, Is.LessThanOrEqualTo(arg2), () => AssertBase.ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that the first value is less than or equal to the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        public static void LessOrEqual(float arg1, float arg2)
        {
            Framework.Assert.That(arg1, Is.LessThanOrEqualTo(arg2));
        }

        #endregion

        #region IComparables

        /// <summary>
        /// Verifies that the first value is less than or equal to the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void LessOrEqual(IComparable arg1, IComparable arg2, string message, params object[] args)
        {
            Framework.Assert.That(arg1, Is.LessThanOrEqualTo(arg2), () => AssertBase.ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that the first value is less than or equal to the second
        /// value. If it is not, then an
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        public static void LessOrEqual(IComparable arg1, IComparable arg2)
        {
            Framework.Assert.That(arg1, Is.LessThanOrEqualTo(arg2));
        }

        #endregion

        #endregion
    }
}
