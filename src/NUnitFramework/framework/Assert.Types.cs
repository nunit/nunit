// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System;

namespace NUnit.Framework
{
    public abstract partial class Assert
    {
        #region IsAssignableFrom

        /// <summary>
        /// Asserts that an object may be assigned a value of a given Type. Returns without throwing an exception when
        /// inside a multiple assert block.
        /// </summary>
        /// <param name="expected">The expected Type.</param>
        /// <param name="actual">The object under examination</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsAssignableFrom(Type expected, object? actual, string? message, params object?[]? args)
        {
            Assert.That(actual, Is.AssignableFrom(expected) ,message, args);
        }

        /// <summary>
        /// Asserts that an object may be assigned a value of a given Type. Returns without throwing an exception when
        /// inside a multiple assert block.
        /// </summary>
        /// <param name="expected">The expected Type.</param>
        /// <param name="actual">The object under examination</param>
        public static void IsAssignableFrom(Type expected, object? actual)
        {
            Assert.That(actual, Is.AssignableFrom(expected) ,null, null);
        }

        #endregion

        #region IsAssignableFrom<TExpected>

        /// <summary>
        /// Asserts that an object may be assigned a value of a given Type. Returns without throwing an exception when
        /// inside a multiple assert block.
        /// </summary>
        /// <typeparam name="TExpected">The expected Type.</typeparam>
        /// <param name="actual">The object under examination</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsAssignableFrom<TExpected>(object? actual, string? message, params object?[]? args)
        {
            Assert.That(actual, Is.AssignableFrom(typeof(TExpected)) ,message, args);
        }

        /// <summary>
        /// Asserts that an object may be assigned a value of a given Type. Returns without throwing an exception when
        /// inside a multiple assert block.
        /// </summary>
        /// <typeparam name="TExpected">The expected Type.</typeparam>
        /// <param name="actual">The object under examination</param>
        public static void IsAssignableFrom<TExpected>(object? actual)
        {
            Assert.That(actual, Is.AssignableFrom(typeof(TExpected)) ,null, null);
        }

        #endregion

        #region IsNotAssignableFrom

        /// <summary>
        /// Asserts that an object may not be assigned a value of a given Type. Returns without throwing an exception
        /// when inside a multiple assert block.
        /// </summary>
        /// <param name="expected">The expected Type.</param>
        /// <param name="actual">The object under examination</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsNotAssignableFrom(Type expected, object? actual, string? message, params object?[]? args)
        {
            Assert.That(actual, Is.Not.AssignableFrom(expected) ,message, args);
        }

        /// <summary>
        /// Asserts that an object may not be assigned a value of a given Type. Returns without throwing an exception
        /// when inside a multiple assert block.
        /// </summary>
        /// <param name="expected">The expected Type.</param>
        /// <param name="actual">The object under examination</param>
        public static void IsNotAssignableFrom(Type expected, object? actual)
        {
            Assert.That(actual, Is.Not.AssignableFrom(expected) ,null, null);
        }

        #endregion

        #region IsNotAssignableFrom<TExpected>

        /// <summary>
        /// Asserts that an object may not be assigned a value of a given Type. Returns without throwing an exception
        /// when inside a multiple assert block.
        /// </summary>
        /// <typeparam name="TExpected">The expected Type.</typeparam>
        /// <param name="actual">The object under examination</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsNotAssignableFrom<TExpected>(object? actual, string? message, params object?[]? args)
        {
            Assert.That(actual, Is.Not.AssignableFrom(typeof(TExpected)) ,message, args);
        }

        /// <summary>
        /// Asserts that an object may not be assigned a value of a given Type. Returns without throwing an exception
        /// when inside a multiple assert block.
        /// </summary>
        /// <typeparam name="TExpected">The expected Type.</typeparam>
        /// <param name="actual">The object under examination</param>
        public static void IsNotAssignableFrom<TExpected>(object? actual)
        {
            Assert.That(actual, Is.Not.AssignableFrom(typeof(TExpected)) ,null, null);
        }

        #endregion

        #region IsInstanceOf

        /// <summary>
        /// Asserts that an object is an instance of a given type. Returns without throwing an exception when inside a
        /// multiple assert block.
        /// </summary>
        /// <param name="expected">The expected Type</param>
        /// <param name="actual">The object being examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsInstanceOf(Type expected, object? actual, string? message, params object?[]? args)
        {
            Assert.That(actual, Is.InstanceOf(expected) ,message, args);
        }

        /// <summary>
        /// Asserts that an object is an instance of a given type. Returns without throwing an exception when inside a
        /// multiple assert block.
        /// </summary>
        /// <param name="expected">The expected Type</param>
        /// <param name="actual">The object being examined</param>
        public static void IsInstanceOf(Type expected, object? actual)
        {
            Assert.That(actual, Is.InstanceOf(expected) ,null, null);
        }

        #endregion

        #region IsInstanceOf<TExpected>

        /// <summary>
        /// Asserts that an object is an instance of a given type. Returns without throwing an exception when inside a
        /// multiple assert block.
        /// </summary>
        /// <typeparam name="TExpected">The expected Type</typeparam>
        /// <param name="actual">The object being examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsInstanceOf<TExpected>(object? actual, string? message, params object?[]? args)
        {
            Assert.That(actual, Is.InstanceOf(typeof(TExpected)) ,message, args);
        }

        /// <summary>
        /// Asserts that an object is an instance of a given type. Returns without throwing an exception when inside a
        /// multiple assert block.
        /// </summary>
        /// <typeparam name="TExpected">The expected Type</typeparam>
        /// <param name="actual">The object being examined</param>
        public static void IsInstanceOf<TExpected>(object? actual)
        {
            Assert.That(actual, Is.InstanceOf(typeof(TExpected)) ,null, null);
        }

        #endregion

        #region IsNotInstanceOf

        /// <summary>
        /// Asserts that an object is not an instance of a given type. Returns without throwing an exception when inside
        /// a multiple assert block.
        /// </summary>
        /// <param name="expected">The expected Type</param>
        /// <param name="actual">The object being examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsNotInstanceOf(Type expected, object? actual, string? message, params object?[]? args)
        {
            Assert.That(actual, Is.Not.InstanceOf(expected) ,message, args);
        }

        /// <summary>
        /// Asserts that an object is not an instance of a given type. Returns without throwing an exception when inside
        /// a multiple assert block.
        /// </summary>
        /// <param name="expected">The expected Type</param>
        /// <param name="actual">The object being examined</param>
        public static void IsNotInstanceOf(Type expected, object? actual)
        {
            Assert.That(actual, Is.Not.InstanceOf(expected) ,null, null);
        }

        #endregion

        #region IsNotInstanceOf<TExpected>

        /// <summary>
        /// Asserts that an object is not an instance of a given type. Returns without throwing an exception when inside
        /// a multiple assert block.
        /// </summary>
        /// <typeparam name="TExpected">The expected Type</typeparam>
        /// <param name="actual">The object being examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void IsNotInstanceOf<TExpected>(object? actual, string? message, params object?[]? args)
        {
            Assert.That(actual, Is.Not.InstanceOf(typeof(TExpected)) ,message, args);
        }

        /// <summary>
        /// Asserts that an object is not an instance of a given type. Returns without throwing an exception when inside
        /// a multiple assert block.
        /// </summary>
        /// <typeparam name="TExpected">The expected Type</typeparam>
        /// <param name="actual">The object being examined</param>
        public static void IsNotInstanceOf<TExpected>(object? actual)
        {
            Assert.That(actual, Is.Not.InstanceOf(typeof(TExpected)) ,null, null);
        }

        #endregion
    }
}
