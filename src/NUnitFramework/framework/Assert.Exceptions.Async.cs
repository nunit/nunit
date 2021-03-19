// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    public partial class Assert
    {
        #region ThrowsAsync

        /// <summary>
        /// Verifies that an async delegate throws a particular exception when called. The returned exception may be
        /// <see langword="null"/> when inside a multiple assert block.
        /// </summary>
        /// <param name="expression">A constraint to be satisfied by the exception</param>
        /// <param name="code">A TestSnippet delegate</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static Exception? ThrowsAsync(IResolveConstraint expression, AsyncTestDelegate code, string? message, params object?[]? args)
        {
            Exception? caughtException = null;
            try
            {
                AsyncToSyncAdapter.Await(code.Invoke);
            }
            catch (Exception e)
            {
                caughtException = e;
            }

            Assert.That(caughtException, expression, message, args);

            return caughtException;
        }

        /// <summary>
        /// Verifies that an async delegate throws a particular exception when called. The returned exception may be
        /// <see langword="null"/> when inside a multiple assert block.
        /// </summary>
        /// <param name="expression">A constraint to be satisfied by the exception</param>
        /// <param name="code">A TestSnippet delegate</param>
        public static Exception? ThrowsAsync(IResolveConstraint expression, AsyncTestDelegate code)
        {
            return ThrowsAsync(expression, code, string.Empty, null);
        }

        /// <summary>
        /// Verifies that an async delegate throws a particular exception when called. The returned exception may be
        /// <see langword="null"/> when inside a multiple assert block.
        /// </summary>
        /// <param name="expectedExceptionType">The exception Type expected</param>
        /// <param name="code">A TestDelegate</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static Exception? ThrowsAsync(Type expectedExceptionType, AsyncTestDelegate code, string? message, params object?[]? args)
        {
            return ThrowsAsync(new ExceptionTypeConstraint(expectedExceptionType), code, message, args);
        }

        /// <summary>
        /// Verifies that an async delegate throws a particular exception when called. The returned exception may be
        /// <see langword="null"/> when inside a multiple assert block.
        /// </summary>
        /// <param name="expectedExceptionType">The exception Type expected</param>
        /// <param name="code">A TestDelegate</param>
        public static Exception? ThrowsAsync(Type expectedExceptionType, AsyncTestDelegate code)
        {
            return ThrowsAsync(new ExceptionTypeConstraint(expectedExceptionType), code, string.Empty, null);
        }

        #endregion

        #region ThrowsAsync<TActual>

        /// <summary>
        /// Verifies that an async delegate throws a particular exception when called. The returned exception may be
        /// <see langword="null"/> when inside a multiple assert block.
        /// </summary>
        /// <typeparam name="TActual">Type of the expected exception</typeparam>
        /// <param name="code">A TestDelegate</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static TActual? ThrowsAsync<TActual>(AsyncTestDelegate code, string? message, params object?[]? args) where TActual : Exception
        {
            return (TActual?)ThrowsAsync(typeof (TActual), code, message, args);
        }

        /// <summary>
        /// Verifies that an async delegate throws a particular exception when called. The returned exception may be
        /// <see langword="null"/> when inside a multiple assert block.
        /// </summary>
        /// <typeparam name="TActual">Type of the expected exception</typeparam>
        /// <param name="code">A TestDelegate</param>
        public static TActual? ThrowsAsync<TActual>(AsyncTestDelegate code) where TActual : Exception
        {
            return ThrowsAsync<TActual>(code, string.Empty, null);
        }

        #endregion

        #region CatchAsync

        /// <summary>
        /// Verifies that an async delegate throws an exception when called and returns it. The returned exception may
        /// be <see langword="null"/> when inside a multiple assert block.
        /// </summary>
        /// <param name="code">A TestDelegate</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static Exception? CatchAsync(AsyncTestDelegate code, string? message, params object?[]? args)
        {
            return ThrowsAsync(new InstanceOfTypeConstraint(typeof(Exception)), code, message, args);
        }

        /// <summary>
        /// Verifies that an async delegate throws an exception when called and returns it. The returned exception may
        /// be <see langword="null"/> when inside a multiple assert block.
        /// </summary>
        /// <param name="code">A TestDelegate</param>
        public static Exception? CatchAsync(AsyncTestDelegate code)
        {
            return ThrowsAsync(new InstanceOfTypeConstraint(typeof(Exception)), code);
        }

        /// <summary>
        /// Verifies that an async delegate throws an exception of a certain Type or one derived from it when called and
        /// returns it. The returned exception may be <see langword="null"/> when inside a multiple assert block.
        /// </summary>
        /// <param name="expectedExceptionType">The expected Exception Type</param>
        /// <param name="code">A TestDelegate</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static Exception? CatchAsync(Type expectedExceptionType, AsyncTestDelegate code, string? message, params object?[]? args)
        {
            return ThrowsAsync(new InstanceOfTypeConstraint(expectedExceptionType), code, message, args);
        }

        /// <summary>
        /// Verifies that an async delegate throws an exception of a certain Type or one derived from it when called and
        /// returns it. The returned exception may be <see langword="null"/> when inside a multiple assert block.
        /// </summary>
        /// <param name="expectedExceptionType">The expected Exception Type</param>
        /// <param name="code">A TestDelegate</param>
        public static Exception? CatchAsync(Type expectedExceptionType, AsyncTestDelegate code)
        {
            return ThrowsAsync(new InstanceOfTypeConstraint(expectedExceptionType), code);
        }

        #endregion

        #region CatchAsync<TActual>

        /// <summary>
        /// Verifies that an async delegate throws an exception of a certain Type or one derived from it when called and
        /// returns it. The returned exception may be <see langword="null"/> when inside a multiple assert block.
        /// </summary>
        /// <param name="code">A TestDelegate</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static TActual? CatchAsync<TActual>(AsyncTestDelegate code, string? message, params object?[]? args) where TActual : Exception
        {
            return (TActual?)ThrowsAsync(new InstanceOfTypeConstraint(typeof (TActual)), code, message, args);
        }

        /// <summary>
        /// Verifies that an async delegate throws an exception of a certain Type or one derived from it when called and
        /// returns it. The returned exception may be <see langword="null"/> when inside a multiple assert block.
        /// </summary>
        /// <param name="code">A TestDelegate</param>
        public static TActual? CatchAsync<TActual>(AsyncTestDelegate code) where TActual : Exception
        {
            return (TActual?)ThrowsAsync(new InstanceOfTypeConstraint(typeof (TActual)), code);
        }

        #endregion

        #region DoesNotThrowAsync

        /// <summary>
        /// Verifies that an async delegate does not throw an exception
        /// </summary>
        /// <param name="code">A TestDelegate</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void DoesNotThrowAsync(AsyncTestDelegate code, string? message, params object?[]? args)
        {
            Assert.That(code, new ThrowsNothingConstraint(), message, args);
        }
        /// <summary>
        /// Verifies that an async delegate does not throw an exception.
        /// </summary>
        /// <param name="code">A TestDelegate</param>
        public static void DoesNotThrowAsync(AsyncTestDelegate code)
        {
            DoesNotThrowAsync(code, string.Empty, null);
        }

        #endregion
    }
}
