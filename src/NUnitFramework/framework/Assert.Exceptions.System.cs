// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;

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
        public static TExpected? Throws<TExpected>(IResolveConstraint expression, Action code, string message, params object?[]? args)
            where TExpected : Exception
        {
            Exception? caughtException = null;

            // Since Action returns void, it’s always async void if it’s async at all.
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

            Assert.That(caughtException, expression, () => ConvertMessageWithArgs(message, args));

            return caughtException as TExpected;
        }

        /// <summary>
        /// Verifies that a delegate throws a particular exception when called. The returned exception may be <see
        /// langword="null"/> when inside a multiple assert block.
        /// </summary>
        /// <param name="expression">A constraint to be satisfied by the exception</param>
        /// <param name="code">A TestSnippet delegate</param>
        public static Exception? Throws(IResolveConstraint expression, Action code)
        {
            return Throws<Exception>(expression, code, string.Empty, null);
        }

        /// <summary>
        /// Verifies that a delegate throws a particular exception when called. The returned exception may be <see
        /// langword="null"/> when inside a multiple assert block.
        /// </summary>
        /// <param name="expectedExceptionType">The exception Type expected</param>
        /// <param name="code">A piece of code to execute</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static Exception? Throws(Type expectedExceptionType, Action code, string message, params object?[]? args)
        {
            return Throws<Exception>(new ExceptionTypeConstraint(expectedExceptionType), code, message, args);
        }

        /// <summary>
        /// Verifies that a delegate throws a particular exception when called. The returned exception may be <see
        /// langword="null"/> when inside a multiple assert block.
        /// </summary>
        /// <param name="expectedExceptionType">The exception Type expected</param>
        /// <param name="code">A piece of code to execute</param>
        public static Exception? Throws(Type expectedExceptionType, Action code)
        {
            return Throws<Exception>(new ExceptionTypeConstraint(expectedExceptionType), code, string.Empty, null);
        }

        #endregion

        #region Throws<TExpected>

        /// <summary>
        /// Verifies that a delegate throws a particular exception when called. The returned exception may be <see
        /// langword="null"/> when inside a multiple assert block.
        /// </summary>
        /// <typeparam name="TExpected">Type of the expected exception</typeparam>
        /// <param name="code">A piece of code to execute</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static TExpected? Throws<TExpected>(Action code, string message, params object?[]? args)
            where TExpected : Exception
        {
            return Throws<TExpected>(new ExceptionTypeConstraint<TExpected>(), code, message, args);
        }

        /// <summary>
        /// Verifies that a delegate throws a particular exception when called. The returned exception may be <see
        /// langword="null"/> when inside a multiple assert block.
        /// </summary>
        /// <typeparam name="TExpected">Type of the expected exception</typeparam>
        /// <param name="code">A piece of code to execute</param>
        public static TExpected? Throws<TExpected>(Action code)
            where TExpected : Exception
        {
            return Throws<TExpected>(code, string.Empty, null);
        }

        #endregion

        #region Catch

        /// <summary>
        /// Verifies that a delegate throws an exception when called and returns it. The returned exception may be <see
        /// langword="null"/> when inside a multiple assert block.
        /// </summary>
        /// <param name="code">A piece of code to execute</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static Exception? Catch(Action code, string message, params object?[]? args)
        {
            return Throws<Exception>(code, message, args);
        }

        /// <summary>
        /// Verifies that a delegate throws an exception when called and returns it. The returned exception may be <see
        /// langword="null"/> when inside a multiple assert block.
        /// </summary>
        /// <param name="code">A piece of code to execute</param>
        public static Exception? Catch(Action code)
        {
            return Catch<Exception>(code);
        }

        /// <summary>
        /// Verifies that a delegate throws an exception of a certain Type or one derived from it when called and
        /// returns it. The returned exception may be <see langword="null"/> when inside a multiple assert block.
        /// </summary>
        /// <param name="expectedExceptionType">The expected Exception Type</param>
        /// <param name="code">A piece of code to execute</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static Exception? Catch(Type expectedExceptionType, Action code, string message, params object?[]? args)
        {
            return Throws<Exception>(new InstanceOfTypeConstraint(expectedExceptionType), code, message, args);
        }

        /// <summary>
        /// Verifies that a delegate throws an exception of a certain Type or one derived from it when called and
        /// returns it. The returned exception may be <see langword="null"/> when inside a multiple assert block.
        /// </summary>
        /// <param name="expectedExceptionType">The expected Exception Type</param>
        /// <param name="code">A piece of code to execute</param>
        public static Exception? Catch(Type expectedExceptionType, Action code)
        {
            return Throws(new InstanceOfTypeConstraint(expectedExceptionType), code);
        }

        #endregion

        #region Catch<TExpected>

        /// <summary>
        /// Verifies that a delegate throws an exception of a certain Type or one derived from it when called and
        /// returns it. The returned exception may be <see langword="null"/> when inside a multiple assert block.
        /// </summary>
        /// <param name="code">A piece of code to execute</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static TExpected? Catch<TExpected>(Action code, string message, params object?[]? args)
            where TExpected : Exception
        {
            return Throws<TExpected>(new InstanceOfTypeConstraint<TExpected>(), code, message, args);
        }

        /// <summary>
        /// Verifies that a delegate throws an exception of a certain Type or one derived from it when called and
        /// returns it. The returned exception may be <see langword="null"/> when inside a multiple assert block.
        /// </summary>
        /// <param name="code">A piece of code to execute</param>
        public static TExpected? Catch<TExpected>(Action code)
            where TExpected : Exception
        {
            return Catch<TExpected>(code, string.Empty, null);
        }

        #endregion

        #region DoesNotThrow

        /// <summary>
        /// Verifies that a delegate does not throw an exception
        /// </summary>
        /// <param name="code">A piece of code to execute</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void DoesNotThrow(Action code, string message, params object?[]? args)
        {
            Assert.That(code, new ThrowsNothingConstraint(), () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that a delegate does not throw an exception.
        /// </summary>
        /// <param name="code">A piece of code to execute</param>
        public static void DoesNotThrow(Action code)
        {
            DoesNotThrow(code, string.Empty, null);
        }

        #endregion
    }
}
