// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    internal static class ExceptionExtensions
    {
        /// <summary>
        /// If <see cref="Exception.StackTrace"/> throws, returns "SomeException was thrown by the Exception.StackTrace
        /// property." See also <see cref="Assert.GetEnvironmentStackTraceWithoutThrowing"/>.
        /// </summary>
        public static string? GetStackTraceWithoutThrowing(this Exception exception)
        {
            ArgumentNullException.ThrowIfNull(exception);

            try
            {
                return exception.StackTrace;
            }
            catch (Exception ex)
            {
                return ex.GetType().Name + " was thrown by the Exception.StackTrace property.";
            }
        }

        /// <summary>
        /// If <see cref="Exception.Message"/> throws, returns "SomeException was thrown by the Exception.Message
        /// property."
        /// </summary>
        public static string GetMessageWithoutThrowing(this Exception exception)
        {
            ArgumentNullException.ThrowIfNull(exception);

            try
            {
                return exception.Message;
            }
            catch (Exception ex)
            {
                return ex.GetType().Name + " was thrown by the Exception.Message property.";
            }
        }

        /// <summary>
        /// If <see cref="Exception.Data"/> throws, returns "SomeException was thrown by the Exception.Data property."
        /// </summary>
        public static Result<IDictionary> GetDataWithoutThrowing(this Exception exception)
        {
            ArgumentNullException.ThrowIfNull(exception);

            try
            {
                return Result.Success(exception.Data);
            }
            catch (Exception ex)
            {
                return Result.Error<IDictionary>(ex.GetType().Name + " was thrown by the Exception.Data property.");
            }
        }

        /// <summary>
        /// Sets the result state and message of the current test before the exception is thrown,
        /// so code that catches the exception can read them from <see cref="TestContext.Result"/>.
        /// </summary>
        public static ResultStateException SetEarlyResult(this ResultStateException exception)
        {
            TestExecutionContext.CurrentContext.CurrentResult.SetEarlyResult(exception.ResultState, exception.Message);
            return exception;
        }
    }
}
