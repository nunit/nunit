// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// ExceptionHelper provides static methods for working with exceptions
    /// </summary>
    public static class ExceptionHelper
    {
        /// <summary>
        /// Rethrows an exception, preserving its stack trace
        /// </summary>
        /// <param name="exception">The exception to rethrow</param>
        [DoesNotReturn]
        public static void Rethrow(Exception exception)
        {
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(exception).Throw();
        }

        /// <summary>
        /// Builds up a message, using the Message field of the specified exception
        /// as well as any InnerExceptions. Optionally excludes exception names,
        /// creating a more readable message.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="excludeExceptionNames">Flag indicating whether exception names should be excluded.</param>
        /// <returns>A combined message string.</returns>
        public static string BuildMessage(Exception exception, bool excludeExceptionNames = false)
        {
            Guard.ArgumentNotNull(exception, nameof(exception));

            StringBuilder sb = new StringBuilder();
            if (!excludeExceptionNames)
                sb.AppendFormat("{0} : ", exception.GetType());
            sb.Append(GetExceptionMessage(exception));
            AppendExceptionDataContents(exception, sb);

            foreach (Exception inner in FlattenExceptionHierarchy(exception))
            {
                sb.Append(Environment.NewLine);
                sb.Append("  ----> ");
                if (!excludeExceptionNames)
                    sb.AppendFormat("{0} : ", inner.GetType());
                sb.Append(GetExceptionMessage(inner));
                AppendExceptionDataContents(inner, sb);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Builds up a message, using the Message field of the specified exception
        /// as well as any InnerExceptions.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>A combined stack trace.</returns>
        public static string BuildStackTrace(Exception exception)
        {
            StringBuilder sb = new StringBuilder(exception.GetStackTraceWithoutThrowing());

            foreach (Exception inner in FlattenExceptionHierarchy(exception))
            {
                sb.Append(Environment.NewLine);
                sb.Append("--");
                sb.Append(inner.GetType().Name);
                sb.Append(Environment.NewLine);
                sb.Append(inner.GetStackTraceWithoutThrowing());
            }

            return sb.ToString();
        }

        private static string GetExceptionMessage(Exception ex)
        {
            var message = ex.GetMessageWithoutThrowing();

            if (string.IsNullOrEmpty(message))
            {
                // Special handling for Mono 5.0, which returns an empty message
                return ex is System.IO.FileNotFoundException fnfEx
                    ? "Could not load assembly. File not found: " + fnfEx.FileName
                    : "No message provided";
            }

            return message;
        }

        private static void AppendExceptionDataContents(Exception ex, StringBuilder sb)
        {
            var data = ex.GetDataWithoutThrowing();

            if (data.IsError(out var message))
            {
                sb.AppendLine();
                sb.Append(message);
            }
            else
            {
                IDictionary dictionary = data.Value!;
                if (dictionary.Count != 0)
                {
                    sb.AppendLine();
                    sb.Append("Data:");
                    foreach (DictionaryEntry kvp in dictionary)
                    {
                        sb.AppendLine();
                        sb.AppendFormat("  {0}: {1}", kvp.Key, kvp.Value?.ToString() ?? "<null>");
                    }
                }
            }
        }

        private static List<Exception> FlattenExceptionHierarchy(Exception exception)
        {
            var result = new List<Exception>();

            if (exception is ReflectionTypeLoadException reflectionException)
            {
                foreach (var innerException in reflectionException.LoaderExceptions)
                {
                    if (innerException is not null)
                        result.Add(exception);
                }

                foreach (var innerException in reflectionException.LoaderExceptions)
                {
                    if (innerException is not null)
                        result.AddRange(FlattenExceptionHierarchy(innerException));
                }
            }
            if (exception is AggregateException aggregateException)
            {
                result.AddRange(aggregateException.InnerExceptions);

                foreach (var innerException in aggregateException.InnerExceptions)
                    result.AddRange(FlattenExceptionHierarchy(innerException));
            }
            else if (exception.InnerException is not null)
            {
                result.Add(exception.InnerException);
                result.AddRange(FlattenExceptionHierarchy(exception.InnerException));
            }

            return result;
        }

        /// <summary>
        /// Executes a parameterless synchronous or async delegate and returns the exception it throws, if any.
        /// </summary>
        internal static Exception? RecordException(Delegate parameterlessDelegate, string parameterName)
        {
            Guard.ArgumentNotNull(parameterlessDelegate, parameterName);

            Guard.ArgumentValid(
                parameterlessDelegate.GetType().GetMethod("Invoke")?.GetParameters().Length == 0,
                $"The actual value must be a parameterless delegate but was {parameterlessDelegate.GetType().Name}.",
                parameterName);

            Guard.ArgumentNotAsyncVoid(parameterlessDelegate, parameterName);

            using (new TestExecutionContext.IsolatedContext())
            {
                if (AsyncToSyncAdapter.IsAsyncOperation(parameterlessDelegate))
                {
                    try
                    {
                        AsyncToSyncAdapter.Await(TestExecutionContext.CurrentContext, parameterlessDelegate.DynamicInvokeWithTransparentExceptions);
                    }
                    catch (Exception ex)
                    {
                        return ex;
                    }
                }
                else
                {
                    try
                    {
                        parameterlessDelegate.DynamicInvokeWithTransparentExceptions();
                    }
                    catch (Exception ex)
                    {
                        return ex;
                    }
                }
            }

            return null;
        }

        internal static async Task<Exception?> RecordExceptionAsync<T>(Func<Task<T>> parameterlessDelegate, string parameterName)
        {
            Guard.ArgumentNotNull(parameterlessDelegate, parameterName);

            try
            {
                await parameterlessDelegate();
            }
            catch (Exception e)
            {
                return e;
            }

            return null;
        }

        /// <summary>
        /// Unwraps the exception of type <see cref="TargetInvocationException"/> to its InnerException.
        /// </summary>
        /// <param name="exception">The exception to unwrap.</param>
        /// <returns>The InnerException is available, otherwise the <paramref name="exception"/>.</returns>
        public static Exception Unwrap(this Exception exception)
        {
            if (exception is NUnitException nUnitException &&
                nUnitException.InnerException is not null)
            {
                exception = nUnitException.InnerException;
            }

            while (exception is TargetInvocationException targetInvocationException &&
                targetInvocationException.InnerException is not null &&
                targetInvocationException.StackTrace?.Contains("NUnit.Framework") is true)
            {
                exception = targetInvocationException.InnerException;
            }

            return exception;
        }
    }
}
