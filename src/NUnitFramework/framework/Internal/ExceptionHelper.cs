// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;

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
                        AsyncToSyncAdapter.Await(parameterlessDelegate.DynamicInvokeWithTransparentExceptions);
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
                targetInvocationException.InnerException is not null)
            {
                exception = targetInvocationException.InnerException;
            }

            return exception;
        }

#if THREAD_ABORT
#if NETFRAMEWORK
        private static readonly FieldInfo? RemoteStackTraceField =
            typeof(Exception).GetField("_remoteStackTraceString", BindingFlags.NonPublic | BindingFlags.Instance);
#endif

        /// <summary>
        /// Stores the provided stack trace into the specified <see cref="Exception"/> instance.
        /// </summary>
        /// <param name="source">The unthrown <see cref="Exception"/> instance.</param>
        /// <param name="stackTrace">The stack trace string to persist within <paramref name="source"/>. This is normally acquired
        /// from the <see cref="Exception.StackTrace"/> property from the remote exception instance.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> or <paramref name="stackTrace"/> argument was null.</exception>
        /// <exception cref="InvalidOperationException">The <paramref name="source"/> argument was previously thrown or previously had a stack trace stored into it.</exception>
        /// <returns>The <paramref name="source"/> exception instance.</returns>
        /// <remarks>
        /// This method populates the <see cref="Exception.StackTrace"/> property from an arbitrary string value.
        /// The typical use case is the transmission of <see cref="Exception"/> objects across processes with high fidelity,
        /// allowing preservation of the exception object's stack trace information. .NET does not attempt to parse the
        /// provided string value.
        /// </remarks>
        public static Exception SetRemoteStackTrace(this Exception source, string stackTrace)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (stackTrace is null)
            {
                throw new ArgumentNullException(nameof(stackTrace));
            }

#if NETFRAMEWORK
            RemoteStackTraceField?.SetValue(source, stackTrace);
#else
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.SetRemoteStackTrace(source, stackTrace);
#endif

            return source;
        }
#endif
        }
}
