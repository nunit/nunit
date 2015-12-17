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

#if NET_4_0 || NET_4_5 || PORTABLE
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
        /// <param name="expression">A constraint to be satisfied by the exception</param>
        /// <param name="code">A TestSnippet delegate</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static Task<Exception> ThrowsAsync(IResolveConstraint expression, AsyncTestDelegate code, string message, params object[] args)
        {
            Exception caughtException;
            try
            {
                var task = code();
                return task.ContinueWith(t =>
                {
                    // Case where code() either
                    // - succeeded (synchronously or asynchronously)
                    // - failed asynchronously
                    var ex = t.IsFaulted ? t.Exception.InnerException : null;
                    Assert.That(ex, expression, message, args);
                    return ex;
                }, TaskScheduler.Default);
            }
            // Case where code() failed synchronously
            catch (Exception ex)
            {
                caughtException = ex;
            }

            Assert.That(caughtException, expression, message, args);

            return TaskFromResult(caughtException);
        }

        /// <summary>
        /// Verifies that an async delegate throws a particular exception when called.
        /// </summary>
        /// <param name="expression">A constraint to be satisfied by the exception</param>
        /// <param name="code">A TestSnippet delegate</param>
        public static Task<Exception> ThrowsAsync(IResolveConstraint expression, AsyncTestDelegate code)
        {
            return ThrowsAsync(expression, code, string.Empty, null);
        }

        /// <summary>
        /// Verifies that an async delegate throws a particular exception when called.
        /// </summary>
        /// <param name="expectedExceptionType">The exception Type expected</param>
        /// <param name="code">A TestDelegate</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static Task<Exception> ThrowsAsync(Type expectedExceptionType, AsyncTestDelegate code, string message, params object[] args)
        {
            return ThrowsAsync(new ExceptionTypeConstraint(expectedExceptionType), code, message, args);
        }

        /// <summary>
        /// Verifies that an async delegate throws a particular exception when called.
        /// </summary>
        /// <param name="expectedExceptionType">The exception Type expected</param>
        /// <param name="code">A TestDelegate</param>
        public static Task<Exception> ThrowsAsync(Type expectedExceptionType, AsyncTestDelegate code)
        {
            return ThrowsAsync(new ExceptionTypeConstraint(expectedExceptionType), code, string.Empty, null);
        }

        #endregion

        #region ThrowsAsync<TActual>

        /// <summary>
        /// Verifies that an async delegate throws a particular exception when called.
        /// </summary>
        /// <typeparam name="TActual">Type of the expected exception</typeparam>
        /// <param name="code">A TestDelegate</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static Task<TActual> ThrowsAsync<TActual>(AsyncTestDelegate code, string message, params object[] args) where TActual : Exception
        {
            return Cast<TActual>(ThrowsAsync(typeof (TActual), code, message, args));
        }

        /// <summary>
        /// Verifies that an async delegate throws a particular exception when called.
        /// </summary>
        /// <typeparam name="TActual">Type of the expected exception</typeparam>
        /// <param name="code">A TestDelegate</param>
        public static Task<TActual> ThrowsAsync<TActual>(AsyncTestDelegate code) where TActual : Exception
        {
            return ThrowsAsync<TActual>(code, string.Empty, null);
        }

        #endregion

        #region CatchAsync

        /// <summary>
        /// Verifies that an async delegate throws an exception when called
        /// and returns it.
        /// </summary>
        /// <param name="code">A TestDelegate</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static Task<Exception> CatchAsync(AsyncTestDelegate code, string message, params object[] args)
        {
            return ThrowsAsync(new InstanceOfTypeConstraint(typeof(Exception)), code, message, args);
        }

        /// <summary>
        /// Verifies that an async delegate throws an exception when called
        /// and returns it.
        /// </summary>
        /// <param name="code">A TestDelegate</param>
        public static Task<Exception> CatchAsync(AsyncTestDelegate code)
        {
            return ThrowsAsync(new InstanceOfTypeConstraint(typeof(Exception)), code);
        }

        /// <summary>
        /// Verifies that an async delegate throws an exception of a certain Type
        /// or one derived from it when called and returns it.
        /// </summary>
        /// <param name="expectedExceptionType">The expected Exception Type</param>
        /// <param name="code">A TestDelegate</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static Task<Exception> CatchAsync(Type expectedExceptionType, AsyncTestDelegate code, string message, params object[] args)
        {
            return ThrowsAsync(new InstanceOfTypeConstraint(expectedExceptionType), code, message, args);
        }

        /// <summary>
        /// Verifies that an async delegate throws an exception of a certain Type
        /// or one derived from it when called and returns it.
        /// </summary>
        /// <param name="expectedExceptionType">The expected Exception Type</param>
        /// <param name="code">A TestDelegate</param>
        public static Task<Exception> CatchAsync(Type expectedExceptionType, AsyncTestDelegate code)
        {
            return ThrowsAsync(new InstanceOfTypeConstraint(expectedExceptionType), code);
        }

        #endregion

        #region CatchAsync<TActual>

        /// <summary>
        /// Verifies that an async delegate throws an exception of a certain Type
        /// or one derived from it when called and returns it.
        /// </summary>
        /// <param name="code">A TestDelegate</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static Task<TActual> CatchAsync<TActual>(AsyncTestDelegate code, string message, params object[] args) where TActual : System.Exception
        {
            return Cast<TActual>(ThrowsAsync(new InstanceOfTypeConstraint(typeof (TActual)), code, message, args));
        }

        /// <summary>
        /// Verifies that an async delegate throws an exception of a certain Type
        /// or one derived from it when called and returns it.
        /// </summary>
        /// <param name="code">A TestDelegate</param>
        public static Task<TActual> CatchAsync<TActual>(AsyncTestDelegate code) where TActual : System.Exception
        {
            return Cast<TActual>(ThrowsAsync(new InstanceOfTypeConstraint(typeof (TActual)), code));
        }

        #endregion

        #region DoesNotThrowAsync

        private static readonly Task CompletedTask = TaskFromResult(0);

        /// <summary>
        /// Verifies that an async delegate does not throw an exception
        /// </summary>
        /// <param name="code">A TestDelegate</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static Task DoesNotThrowAsync(AsyncTestDelegate code, string message, params object[] args)
        {
            Exception caughtException;
            try
            {
                var task = code();
                return task.ContinueWith(t =>
                {
                    // Case where code() either
                    // - succeeded (synchronously or asynchronously)
                    // - failed asynchronously
                    var ex = t.IsFaulted ? t.Exception.InnerException : null;
                    Assert.That(ex, new ExceptionNotThrownConstraint(), message, args);
                }, TaskScheduler.Default);
            }
            // Case where code() failed synchronously
            catch (Exception ex)
            {
                caughtException = ex;
            }

            Assert.That(caughtException, new ExceptionNotThrownConstraint(), message, args);

            return CompletedTask;
        }
        /// <summary>
        /// Verifies that an async delegate does not throw an exception.
        /// </summary>
        /// <param name="code">A TestDelegate</param>
        public static Task DoesNotThrowAsync(AsyncTestDelegate code)
        {
            return DoesNotThrowAsync(code, string.Empty, null);
        }

        #endregion

        #region Helpers

        private static Task<T> TaskFromResult<T>(T value)
        {
#if NET_4_5 || PORTABLE
            return Task.FromResult(value);
#else
            var tcs = new TaskCompletionSource<T>();
            tcs.SetResult(value);
            return tcs.Task;

#endif
        }

        private static Task<TActual> Cast<TActual>(Task<Exception> task) where TActual : Exception
        {
            return task
                .ContinueWith(t =>
                {
                    if (t.IsFaulted)
                        ExceptionHelper.Rethrow(t.Exception.InnerException);
                    return (TActual)t.Result;
                }, TaskScheduler.Default);
        }

        #endregion
    }
}
#endif