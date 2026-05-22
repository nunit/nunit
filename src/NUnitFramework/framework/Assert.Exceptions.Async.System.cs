// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading.Tasks;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    public partial class Assert
    {
        #region ThrowsAsync

        /// <summary>
        /// Verifies that an async delegate throws a particular exception when called.
        /// </summary>
        /// <remarks>
        /// The exception resolved by the returned <see cref="System.Threading.Tasks.Task"/>
        /// may be <see langword="null"/> when inside a multiple assert block.
        /// </remarks>
        /// <param name="expression">A constraint to be satisfied by the exception</param>
        /// <param name="asyncCode">An async piece of code to execute</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static async Task<Exception?> ThrowsAsync(IResolveConstraint expression, Func<Task> asyncCode, string message, params object?[]? args)
        {
            Exception? caughtException = null;

            using (new TestExecutionContext.IsolatedContext())
            {
                try
                {
                    await asyncCode();
                }
                catch (Exception e)
                {
                    caughtException = e;
                }
            }

            Assert.That(caughtException, expression, () => ConvertMessageWithArgs(message, args));

            return caughtException;
        }

        /// <summary>
        /// Verifies that an async delegate throws a particular exception when called.
        /// </summary>
        /// <remarks>
        /// The exception resolved by the returned <see cref="System.Threading.Tasks.Task"/>
        /// may be <see langword="null"/> when inside a multiple assert block.
        /// </remarks>
        /// <param name="expression">A constraint to be satisfied by the exception</param>
        /// <param name="asyncCode">An async piece of code to execute</param>
        public static Task<Exception?> ThrowsAsync(IResolveConstraint expression, Func<Task> asyncCode)
        {
            return ThrowsAsync(expression, asyncCode, string.Empty, null);
        }

        /// <summary>
        /// Verifies that an async delegate throws a particular exception when called.
        /// </summary>
        /// <remarks>
        /// The exception resolved by the returned <see cref="System.Threading.Tasks.Task"/>
        /// may be <see langword="null"/> when inside a multiple assert block.
        /// </remarks>
        /// <param name="expectedExceptionType">The exception Type expected</param>
        /// <param name="asyncCode">An async piece of code to execute</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static Task<Exception?> ThrowsAsync(Type expectedExceptionType, Func<Task> asyncCode, string message, params object?[]? args)
        {
            return ThrowsAsync(new ExceptionTypeConstraint(expectedExceptionType), asyncCode, message, args);
        }

        /// <summary>
        /// Verifies that an async delegate throws a particular exception when called.
        /// </summary>
        /// <remarks>
        /// The exception resolved by the returned <see cref="System.Threading.Tasks.Task"/>
        /// may be <see langword="null"/> when inside a multiple assert block.
        /// </remarks>
        /// <param name="expectedExceptionType">The exception Type expected</param>
        /// <param name="asyncCode">An async piece of code to execute</param>
        public static Task<Exception?> ThrowsAsync(Type expectedExceptionType, Func<Task> asyncCode)
        {
            return ThrowsAsync(new ExceptionTypeConstraint(expectedExceptionType), asyncCode, string.Empty, null);
        }

        #endregion

        #region ThrowsAsync<TActual>

        /// <summary>
        /// Verifies that an async delegate throws a particular exception when called.
        /// </summary>
        /// <remarks>
        /// The exception resolved by the returned <see cref="System.Threading.Tasks.Task"/>
        /// may be <see langword="null"/> when inside a multiple assert block.
        /// </remarks>
        /// <typeparam name="TActual">Type of the expected exception</typeparam>
        /// <param name="asyncCode">An async piece of code to execute</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static async Task<TActual?> ThrowsAsync<TActual>(Func<Task> asyncCode, string message, params object?[]? args)
            where TActual : Exception
        {
            return (TActual?)await ThrowsAsync(new ExceptionTypeConstraint<TActual>(), asyncCode, message, args);
        }

        /// <summary>
        /// Verifies that an async delegate throws a particular exception when called.
        /// </summary>
        /// <remarks>
        /// The exception resolved by the returned <see cref="System.Threading.Tasks.Task"/>
        /// may be <see langword="null"/> when inside a multiple assert block.
        /// </remarks>
        /// <typeparam name="TActual">Type of the expected exception</typeparam>
        /// <param name="asyncCode">An async piece of code to execute</param>
        public static Task<TActual?> ThrowsAsync<TActual>(Func<Task> asyncCode)
            where TActual : Exception
        {
            return ThrowsAsync<TActual>(asyncCode, string.Empty, null);
        }

        #endregion

        #region CatchAsync

        /// <summary>
        /// Verifies that an async delegate throws an exception when called and returns it.
        /// </summary>
        /// <remarks>
        /// The exception resolved by the returned <see cref="System.Threading.Tasks.Task"/>
        /// may be <see langword="null"/> when inside a multiple assert block.
        /// </remarks>
        /// <param name="asyncCode">An async piece of code to execute</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static Task<Exception?> CatchAsync(Func<Task> asyncCode, string message, params object?[]? args)
        {
            return CatchAsync<Exception>(asyncCode, message, args);
        }

        /// <summary>
        /// Verifies that an async delegate throws an exception when called and returns it.
        /// </summary>
        /// <remarks>
        /// The exception resolved by the returned <see cref="System.Threading.Tasks.Task"/>
        /// may be <see langword="null"/> when inside a multiple assert block.
        /// </remarks>
        /// <param name="asyncCode">An async piece of code to execute</param>
        public static Task<Exception?> CatchAsync(Func<Task> asyncCode)
        {
            return CatchAsync<Exception>(asyncCode);
        }

        /// <summary>
        /// Verifies that an async delegate throws an exception of a certain Type or one derived from it when called and
        /// returns it.
        /// </summary>
        /// <remarks>
        /// The exception resolved by the returned <see cref="System.Threading.Tasks.Task"/>
        /// may be <see langword="null"/> when inside a multiple assert block.
        /// </remarks>
        /// <param name="expectedExceptionType">The expected Exception Type</param>
        /// <param name="asyncCode">An async piece of code to execute</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static Task<Exception?> CatchAsync(Type expectedExceptionType, Func<Task> asyncCode, string message, params object?[]? args)
        {
            return ThrowsAsync(new InstanceOfTypeConstraint(expectedExceptionType), asyncCode, message, args);
        }

        /// <summary>
        /// Verifies that an async delegate throws an exception of a certain Type or one derived from it when called and
        /// returns it.
        /// </summary>
        /// <remarks>
        /// The exception resolved by the returned <see cref="System.Threading.Tasks.Task"/>
        /// may be <see langword="null"/> when inside a multiple assert block.
        /// </remarks>
        /// <param name="expectedExceptionType">The expected Exception Type</param>
        /// <param name="asyncCode">An async piece of code to execute</param>
        public static Task<Exception?> CatchAsync(Type expectedExceptionType, Func<Task> asyncCode)
        {
            return ThrowsAsync(new InstanceOfTypeConstraint(expectedExceptionType), asyncCode);
        }

        #endregion

        #region CatchAsync<TActual>

        /// <summary>
        /// Verifies that an async delegate throws an exception of a certain Type or one derived from it when called and
        /// returns it.
        /// </summary>
        /// <remarks>
        /// The exception resolved by the returned <see cref="System.Threading.Tasks.Task"/>
        /// may be <see langword="null"/> when inside a multiple assert block.
        /// </remarks>
        /// <param name="asyncCode">An async piece of code to execute</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static async Task<TActual?> CatchAsync<TActual>(Func<Task> asyncCode, string message, params object?[]? args)
            where TActual : Exception
        {
            return (TActual?)await ThrowsAsync(new InstanceOfTypeConstraint<TActual>(), asyncCode, message, args);
        }

        /// <summary>
        /// Verifies that an async delegate throws an exception of a certain Type or one derived from it when called and
        /// returns it.
        /// </summary>
        /// <remarks>
        /// The exception resolved by the returned <see cref="System.Threading.Tasks.Task"/>
        /// may be <see langword="null"/> when inside a multiple assert block.
        /// </remarks>
        /// <param name="asyncCode">An async piece of code to execute</param>
        public static async Task<TActual?> CatchAsync<TActual>(Func<Task> asyncCode)
            where TActual : Exception
        {
            return (TActual?)await ThrowsAsync(new InstanceOfTypeConstraint<TActual>(), asyncCode);
        }

        #endregion

        #region DoesNotThrowAsync

        /// <summary>
        /// Verifies that an async delegate does not throw an exception
        /// </summary>
        /// <param name="asyncCode">An async piece of code to execute</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void DoesNotThrowAsync(Func<Task> asyncCode, string message, params object?[]? args)
        {
            Assert.That(asyncCode, new ThrowsNothingConstraint(), () => ConvertMessageWithArgs(message, args));
        }

        /// <summary>
        /// Verifies that an async delegate does not throw an exception.
        /// </summary>
        /// <param name="asyncCode">An async piece of code to execute</param>
        public static void DoesNotThrowAsync(Func<Task> asyncCode)
        {
            DoesNotThrowAsync(asyncCode, string.Empty, null);
        }

        #endregion
    }
}
