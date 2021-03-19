// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

namespace NUnit.Framework
{
    public abstract partial class Assert
    {
        #region AreEqual

        #region Doubles

        /// <summary>
        /// Verifies that two doubles are equal considering a delta. If the expected value is infinity then the delta
        /// value is ignored. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="expected">The expected value</param>
        /// <param name="actual">The actual value</param>
        /// <param name="delta">The maximum acceptable difference between the the expected and the actual</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void AreEqual(double expected, double actual, double delta, string? message, params object?[]? args)
        {
            AssertDoublesAreEqual(expected, actual, delta, message, args);
        }

        /// <summary>
        /// Verifies that two doubles are equal considering a delta. If the expected value is infinity then the delta
        /// value is ignored. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="expected">The expected value</param>
        /// <param name="actual">The actual value</param>
        /// <param name="delta">The maximum acceptable difference between the the expected and the actual</param>
        public static void AreEqual(double expected, double actual, double delta)
        {
            AssertDoublesAreEqual(expected, actual, delta, null, null);
        }

        #endregion

        #region Objects

        /// <summary>
        /// Verifies that two objects are equal. Two objects are considered equal if both are null, or if both have the
        /// same value. NUnit has special semantics for some object types. Returns without throwing an exception when
        /// inside a multiple assert block.
        /// </summary>
        /// <param name="expected">The value that is expected</param>
        /// <param name="actual">The actual value</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void AreEqual(object? expected, object? actual, string? message, params object?[]? args)
        {
            Assert.That(actual, Is.EqualTo(expected), message, args);
        }

        /// <summary>
        /// Verifies that two objects are equal. Two objects are considered equal if both are null, or if both have the
        /// same value. NUnit has special semantics for some object types. Returns without throwing an exception when
        /// inside a multiple assert block.
        /// </summary>
        /// <param name="expected">The value that is expected</param>
        /// <param name="actual">The actual value</param>
        public static void AreEqual(object? expected, object? actual)
        {
            Assert.That(actual, Is.EqualTo(expected), null, null);
        }

        #endregion

        #endregion

        #region AreNotEqual

        #region Objects

        /// <summary>
        /// Verifies that two objects are not equal. Two objects are considered equal if both are null, or if both have
        /// the same value. NUnit has special semantics for some object types. Returns without throwing an exception
        /// when inside a multiple assert block.
        /// </summary>
        /// <param name="expected">The value that is expected</param>
        /// <param name="actual">The actual value</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void AreNotEqual(object? expected, object? actual, string? message, params object?[]? args)
        {
            Assert.That(actual, Is.Not.EqualTo(expected), message, args);
        }

        /// <summary>
        /// Verifies that two objects are not equal. Two objects are considered equal if both are null, or if both have
        /// the same value. NUnit has special semantics for some object types. Returns without throwing an exception
        /// when inside a multiple assert block.
        /// </summary>
        /// <param name="expected">The value that is expected</param>
        /// <param name="actual">The actual value</param>
        public static void AreNotEqual(object? expected, object? actual)
        {
            Assert.That(actual, Is.Not.EqualTo(expected), null, null);
        }

        #endregion

        #endregion

        #region AreSame

        /// <summary>
        /// Asserts that two objects refer to the same object. Returns without throwing an exception when inside a
        /// multiple assert block.
        /// </summary>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The actual object</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void AreSame(object? expected, object? actual, string? message, params object?[]? args)
        {
            Assert.That(actual, Is.SameAs(expected), message, args);
        }

        /// <summary>
        /// Asserts that two objects refer to the same object. Returns without throwing an exception when inside a
        /// multiple assert block.
        /// </summary>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The actual object</param>
        public static void AreSame(object? expected, object? actual)
        {
            Assert.That(actual, Is.SameAs(expected), null, null);
        }

        #endregion

        #region AreNotSame

        /// <summary>
        /// Asserts that two objects do not refer to the same object. Returns without throwing an exception when inside
        /// a multiple assert block.
        /// </summary>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The actual object</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void AreNotSame(object? expected, object? actual, string? message, params object?[]? args)
        {
            Assert.That(actual, Is.Not.SameAs(expected), message, args);
        }

        /// <summary>
        /// Asserts that two objects do not refer to the same object. Returns without throwing an exception when inside
        /// a multiple assert block.
        /// </summary>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The actual object</param>
        public static void AreNotSame(object? expected, object? actual)
        {
            Assert.That(actual, Is.Not.SameAs(expected), null, null);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Helper for Assert.AreEqual(double expected, double actual, ...)
        /// allowing code generation to work consistently.
        /// </summary>
        /// <param name="expected">The expected value</param>
        /// <param name="actual">The actual value</param>
        /// <param name="delta">The maximum acceptable difference between the
        /// the expected and the actual</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        protected static void AssertDoublesAreEqual(double expected, double actual, double delta, string? message, object?[]? args)
        {
            if (double.IsNaN(expected) || double.IsInfinity(expected))
                Assert.That(actual, Is.EqualTo(expected), message, args);
            else
                Assert.That(actual, Is.EqualTo(expected).Within(delta), message, args);
        }

        #endregion
    }
}
