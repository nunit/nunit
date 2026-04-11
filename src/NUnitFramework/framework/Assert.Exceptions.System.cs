// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Runtime.CompilerServices;
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
        [OverloadResolutionPriority(1)]
        public static Exception? Throws(IResolveConstraint expression, Action code, string message, params object?[]? args)
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

            return caughtException;
        }

        /// <summary>
        /// Verifies that a delegate throws a particular exception when called. The returned exception may be <see
        /// langword="null"/> when inside a multiple assert block.
        /// </summary>
        /// <param name="expression">A constraint to be satisfied by the exception</param>
        /// <param name="code">A TestSnippet delegate</param>
        [OverloadResolutionPriority(1)]
        public static Exception? Throws(IResolveConstraint expression, Action code)
        {
            return Throws(expression, code, string.Empty, null);
        }

        /// <summary>
        /// Verifies that a delegate throws a particular exception when called. The returned exception may be <see
        /// langword="null"/> when inside a multiple assert block.
        /// </summary>
        /// <param name="expectedExceptionType">The exception Type expected</param>
        /// <param name="code">A piece of code to execute</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        [OverloadResolutionPriority(1)]
        public static Exception? Throws(Type expectedExceptionType, Action code, string message, params object?[]? args)
        {
            return Throws(new ExceptionTypeConstraint(expectedExceptionType), code, message, args);
        }

        /// <summary>
        /// Verifies that a delegate throws a particular exception when called. The returned exception may be <see
        /// langword="null"/> when inside a multiple assert block.
        /// </summary>
        /// <param name="expectedExceptionType">The exception Type expected</param>
        /// <param name="code">A piece of code to execute</param>
        [OverloadResolutionPriority(1)]
        public static Exception? Throws(Type expectedExceptionType, Action code)
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
        /// <param name="code">A piece of code to execute</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        [OverloadResolutionPriority(1)]
        public static TActual? Throws<TActual>(Action code, string message, params object?[]? args)
            where TActual : Exception
        {
            return (TActual?)Throws(typeof(TActual), code, message, args);
        }

        /// <summary>
        /// Verifies that a delegate throws a particular exception when called. The returned exception may be <see
        /// langword="null"/> when inside a multiple assert block.
        /// </summary>
        /// <typeparam name="TActual">Type of the expected exception</typeparam>
        /// <param name="code">A piece of code to execute</param>
        [OverloadResolutionPriority(1)]
        public static TActual? Throws<TActual>(Action code)
            where TActual : Exception
        {
            return Throws<TActual>(code, string.Empty, null);
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
        [OverloadResolutionPriority(1)]
        public static Exception? Catch(Action code, string message, params object?[]? args)
        {
            return Throws(new InstanceOfTypeConstraint(typeof(Exception)), code, message, args);
        }

        /// <summary>
        /// Verifies that a delegate throws an exception when called and returns it. The returned exception may be <see
        /// langword="null"/> when inside a multiple assert block.
        /// </summary>
        /// <param name="code">A piece of code to execute</param>
        [OverloadResolutionPriority(1)]
        public static Exception? Catch(Action code)
        {
            return Throws(new InstanceOfTypeConstraint(typeof(Exception)), code);
        }

        /// <summary>
        /// Verifies that a delegate throws an exception of a certain Type or one derived from it when called and
        /// returns it. The returned exception may be <see langword="null"/> when inside a multiple assert block.
        /// </summary>
        /// <param name="expectedExceptionType">The expected Exception Type</param>
        /// <param name="code">A piece of code to execute</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        [OverloadResolutionPriority(1)]
        public static Exception? Catch(Type expectedExceptionType, Action code, string message, params object?[]? args)
        {
            return Throws(new InstanceOfTypeConstraint(expectedExceptionType), code, message, args);
        }

        /// <summary>
        /// Verifies that a delegate throws an exception of a certain Type or one derived from it when called and
        /// returns it. The returned exception may be <see langword="null"/> when inside a multiple assert block.
        /// </summary>
        /// <param name="expectedExceptionType">The expected Exception Type</param>
        /// <param name="code">A piece of code to execute</param>
        [OverloadResolutionPriority(1)]
        public static Exception? Catch(Type expectedExceptionType, Action code)
        {
            return Throws(new InstanceOfTypeConstraint(expectedExceptionType), code);
        }

        #endregion

        #region Catch<TActual>

        /// <summary>
        /// Verifies that a delegate throws an exception of a certain Type or one derived from it when called and
        /// returns it. The returned exception may be <see langword="null"/> when inside a multiple assert block.
        /// </summary>
        /// <param name="code">A piece of code to execute</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        [OverloadResolutionPriority(1)]
        public static TActual? Catch<TActual>(Action code, string message, params object?[]? args)
            where TActual : System.Exception
        {
            return (TActual?)Throws(new InstanceOfTypeConstraint(typeof(TActual)), code, message, args);
        }

        /// <summary>
        /// Verifies that a delegate throws an exception of a certain Type or one derived from it when called and
        /// returns it. The returned exception may be <see langword="null"/> when inside a multiple assert block.
        /// </summary>
        /// <param name="code">A piece of code to execute</param>
        [OverloadResolutionPriority(1)]
        public static TActual? Catch<TActual>(Action code)
            where TActual : System.Exception
        {
            return (TActual?)Throws(new InstanceOfTypeConstraint(typeof(TActual)), code);
        }

        #endregion

        #region DoesNotThrow

        /// <summary>
        /// Verifies that a delegate does not throw an exception
        /// </summary>
        /// <param name="code">A piece of code to execute</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        [OverloadResolutionPriority(1)]
        public static void DoesNotThrow(Action code, string message, params object?[]? args)
        {
            Assert.That(code, new ThrowsNothingConstraint(), () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that a delegate does not throw an exception.
        /// </summary>
        /// <param name="code">A piece of code to execute</param>
        [OverloadResolutionPriority(1)]
        public static void DoesNotThrow(Action code)
        {
            DoesNotThrow(code, string.Empty, null);
        }

        #endregion
    }
}
