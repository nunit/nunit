// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;

namespace NUnit.Framework
{
    internal static class ExceptionExtensions
    {
        /// <summary>
        /// If <see cref="Exception.StackTrace"/> throws, returns "SomeException was thrown by the Exception.StackTrace
        /// property." See also <see cref="Assert.GetEnvironmentStackTraceWithoutThrowing"/>.
        /// </summary>
        // https://github.com/dotnet/coreclr/issues/19698 is also currently present in .NET Framework 4.7 and 4.8. A
        // race condition between threads reading the same PDB file to obtain file and line info for a stack trace
        // results in AccessViolationException when the stack trace is accessed even indirectly e.g. Exception.ToString.
        [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
        public static string GetStackTraceWithoutThrowing(this Exception exception)
        {
            if (exception is null) throw new ArgumentNullException(nameof(exception));

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
        // https://github.com/dotnet/coreclr/issues/19698 is also currently present in .NET Framework 4.7 and 4.8. A
        // race condition between threads reading the same PDB file to obtain file and line info for a stack trace
        // results in AccessViolationException when the stack trace is accessed even indirectly e.g. Exception.ToString.
        [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
        public static string GetMessageWithoutThrowing(this Exception exception)
        {
            if (exception is null) throw new ArgumentNullException(nameof(exception));

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
        // https://github.com/dotnet/coreclr/issues/19698 is also currently present in .NET Framework 4.7 and 4.8. A
        // race condition between threads reading the same PDB file to obtain file and line info for a stack trace
        // results in AccessViolationException when the stack trace is accessed even indirectly e.g. Exception.ToString.
        [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
        public static Result<IDictionary> GetDataWithoutThrowing(this Exception exception)
        {
            if (exception is null) throw new ArgumentNullException(nameof(exception));

            try
            {
                return Result.Success(exception.Data);
            }
            catch (Exception ex)
            {
                return Result.Error<IDictionary>(ex.GetType().Name + " was thrown by the Exception.Data property.");
            }
        }
    }
}
