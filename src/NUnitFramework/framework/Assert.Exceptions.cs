// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System;

namespace NUnit.Framework
{
    public abstract partial class Assert
    {
        #region Throws
        /// <summary>
        /// Verifies that a delegate throws a particular exception when called. The returned exception may be <see
        /// langword="null"/> when inside a multiple assert block.
        /// </summary>
        /// <param name="expression">A constraint to be satisfied by the exception</param>
        /// <param name="code">A TestSnippet delegate</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static Exception? Throws(IResolveConstraint expression, TestDelegate code, string? message, params object?[]? args)
        {
            Exception? caughtException = null;

            // Since TestDelegate returns void, it’s always async void if it’s async at all.
            Guard.ArgumentNotAsyncVoid(code, nameof(code));

            using (new TestExecutionContext.IsolatedContext())
            {
                try
                {
                    code();
                }
                catch (Exception ex)
                {
                    caughtException = ex;
                }
            }

            Assert.That(caughtException, expression, message, args);

            return caughtException;
        }

        /// <summary>
        /// Verifies that a delegate throws a particular exception when called. The returned exception may be <see
        /// langword="null"/> when inside a multiple assert block.
        /// </summary>
        /// <param name="expression">A constraint to be satisfied by the exception</param>
        /// <param name="code">A TestSnippet delegate</param>
        public static Exception? Throws(IResolveConstraint expression, TestDelegate code)
        {
            return Throws(expression, code, string.Empty, null);
        }

        /// <summary>
        /// Verifies that a delegate throws a particular exception when called. The returned exception may be <see
        /// langword="null"/> when inside a multiple assert block.
        /// </summary>
        /// <param name="expectedExceptionType">The exception Type expected</param>
        /// <param name="code">A TestDelegate</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static Exception? Throws(Type expectedExceptionType, TestDelegate code, string? message, params object?[]? args)
        {
            return Throws(new ExceptionTypeConstraint(expectedExceptionType), code, message, args);
        }

        /// <summary>
        /// Verifies that a delegate throws a particular exception when called. The returned exception may be <see
        /// langword="null"/> when inside a multiple assert block.
        /// </summary>
        /// <param name="expectedExceptionType">The exception Type expected</param>
        /// <param name="code">A TestDelegate</param>
        public static Exception? Throws(Type expectedExceptionType, TestDelegate code)
        {
            return Throws(new ExceptionTypeConstraint(expectedExceptionType), code, string.Empty, null);
        }

        #endregion

        #region Throws<TActual>

        /// <summary>
        /// Verifies that a delegate throws a particular exception when called. The returned exception may be <see
        /// langword="null"/> when inside a multiple assert block.
        /// </summary>
        /// <typeparam name="TActual">Type of the expected exception</typeparam>
        /// <param name="code">A TestDelegate</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static TActual? Throws<TActual>(TestDelegate code, string? message, params object?[]? args) where TActual : Exception
        {
            return (TActual?)Throws(typeof(TActual), code, message, args);
        }

        /// <summary>
        /// Verifies that a delegate throws a particular exception when called. The returned exception may be <see
        /// langword="null"/> when inside a multiple assert block.
        /// </summary>
        /// <typeparam name="TActual">Type of the expected exception</typeparam>
        /// <param name="code">A TestDelegate</param>
        public static TActual? Throws<TActual>(TestDelegate code) where TActual : Exception
        {
            return Throws<TActual>(code, string.Empty, null);
        }

        #endregion

        #region Catch
        /// <summary>
        /// Verifies that a delegate throws an exception when called and returns it. The returned exception may be <see
        /// langword="null"/> when inside a multiple assert block.
        /// </summary>
        /// <param name="code">A TestDelegate</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static Exception? Catch(TestDelegate code, string? message, params object?[]? args)
        {
            return Throws(new InstanceOfTypeConstraint(typeof(Exception)), code, message, args);
        }

        /// <summary>
        /// Verifies that a delegate throws an exception when called and returns it. The returned exception may be <see
        /// langword="null"/> when inside a multiple assert block.
        /// </summary>
        /// <param name="code">A TestDelegate</param>
        public static Exception? Catch(TestDelegate code)
        {
            return Throws(new InstanceOfTypeConstraint(typeof(Exception)), code);
        }

        /// <summary>
        /// Verifies that a delegate throws an exception of a certain Type or one derived from it when called and
        /// returns it. The returned exception may be <see langword="null"/> when inside a multiple assert block.
        /// </summary>
        /// <param name="expectedExceptionType">The expected Exception Type</param>
        /// <param name="code">A TestDelegate</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static Exception? Catch(Type expectedExceptionType, TestDelegate code, string? message, params object?[]? args)
        {
            return Throws(new InstanceOfTypeConstraint(expectedExceptionType), code, message, args);
        }

        /// <summary>
        /// Verifies that a delegate throws an exception of a certain Type or one derived from it when called and
        /// returns it. The returned exception may be <see langword="null"/> when inside a multiple assert block.
        /// </summary>
        /// <param name="expectedExceptionType">The expected Exception Type</param>
        /// <param name="code">A TestDelegate</param>
        public static Exception? Catch(Type expectedExceptionType, TestDelegate code)
        {
            return Throws(new InstanceOfTypeConstraint(expectedExceptionType), code);
        }
        #endregion

        #region Catch<TActual>

        /// <summary>
        /// Verifies that a delegate throws an exception of a certain Type or one derived from it when called and
        /// returns it. The returned exception may be <see langword="null"/> when inside a multiple assert block.
        /// </summary>
        /// <param name="code">A TestDelegate</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static TActual? Catch<TActual>(TestDelegate code, string? message, params object?[]? args) where TActual : System.Exception
        {
            return (TActual?)Throws(new InstanceOfTypeConstraint(typeof(TActual)), code, message, args);
        }

        /// <summary>
        /// Verifies that a delegate throws an exception of a certain Type or one derived from it when called and
        /// returns it. The returned exception may be <see langword="null"/> when inside a multiple assert block.
        /// </summary>
        /// <param name="code">A TestDelegate</param>
        public static TActual? Catch<TActual>(TestDelegate code) where TActual : System.Exception
        {
            return (TActual?)Throws(new InstanceOfTypeConstraint(typeof(TActual)), code);
        }

        #endregion

        #region DoesNotThrow

        /// <summary>
        /// Verifies that a delegate does not throw an exception
        /// </summary>
        /// <param name="code">A TestDelegate</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void DoesNotThrow(TestDelegate code, string? message, params object?[]? args)
        {
            Assert.That(code, new ThrowsNothingConstraint(), message, args);
        }

        /// <summary>
        /// Verifies that a delegate does not throw an exception.
        /// </summary>
        /// <param name="code">A TestDelegate</param>
        public static void DoesNotThrow(TestDelegate code)
        {
            DoesNotThrow(code, string.Empty, null);
        }

        #endregion
    }
}
