// ***********************************************************************
// Copyright (c) 2010 Charlie Poole, Rob Prouse
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

using System;
using System.Globalization;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// ExceptionHelper provides static methods for working with exceptions
    /// </summary>
    public class ExceptionHelper
    {
#if NET20 || NET35 || NET40
        private static readonly Action<Exception> PreserveStackTrace;

        static ExceptionHelper()
        {
            var method = typeof(Exception).GetMethod("InternalPreserveStackTrace", BindingFlags.Instance | BindingFlags.NonPublic);

            if (method != null)
            {
                try
                {
                    PreserveStackTrace = (Action<Exception>)Delegate.CreateDelegate(typeof(Action<Exception>), method);
                    return;
                }
                catch (InvalidOperationException) { }
            }
            PreserveStackTrace = _ => { };
        }
#endif

        /// <summary>
        /// Rethrows an exception, preserving its stack trace
        /// </summary>
        /// <param name="exception">The exception to rethrow</param>
        public static void Rethrow(Exception exception)
        {
#if NET20 || NET35 || NET40
            PreserveStackTrace(exception);
            throw exception;
#else
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(exception).Throw();
#endif
        }

        /// <summary>
        /// Builds up a message, using the Message field of the specified exception
        /// as well as any InnerExceptions. Optionally excludes exception names, 
        /// creating a more readable message.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="excludeExceptionNames">Flag indicating whether exception names should be excluded.</param>
        /// <returns>A combined message string.</returns>
        public static string BuildMessage(Exception exception, bool excludeExceptionNames=false)
        {
            Guard.ArgumentNotNull(exception, nameof(exception));

            StringBuilder sb = new StringBuilder();
            if (!excludeExceptionNames)
                sb.AppendFormat("{0} : ", exception.GetType());
            sb.Append(GetExceptionMessage(exception));

            foreach (Exception inner in FlattenExceptionHierarchy(exception))
            {
                sb.Append(Environment.NewLine);
                sb.Append("  ----> ");
                if (!excludeExceptionNames)
                    sb.AppendFormat("{0} : ", inner.GetType());
                sb.Append(GetExceptionMessage(inner));
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
            StringBuilder sb = new StringBuilder(GetSafeStackTrace(exception));

            foreach (Exception inner in FlattenExceptionHierarchy(exception))
            {
                sb.Append(Environment.NewLine);
                sb.Append("--");
                sb.Append(inner.GetType().Name);
                sb.Append(Environment.NewLine);
                sb.Append(GetSafeStackTrace(inner));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets the stack trace of the exception. If no stack trace
        /// is provided, returns "No stack trace available".
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>A string representation of the stack trace.</returns>
        private static string GetSafeStackTrace(Exception exception)
        {
            try
            {
                return exception.StackTrace;
            }
            catch (Exception)
            {
                return "No stack trace available";
            }
        }

        private static string GetExceptionMessage(Exception ex)
        {
            if (string.IsNullOrEmpty(ex.Message))
            {
                // Special handling for Mono 5.0, which returns an empty message
                var fnfEx = ex as System.IO.FileNotFoundException;
                return fnfEx != null
                    ? "Could not load assembly. File not found: " + fnfEx.FileName
                    : "No message provided";
            }

            return ex.Message;
        }

        private static List<Exception> FlattenExceptionHierarchy(Exception exception)
        {
            var result = new List<Exception>();

            if (exception is ReflectionTypeLoadException)
            {
                var reflectionException = exception as ReflectionTypeLoadException;
                result.AddRange(reflectionException.LoaderExceptions);

                foreach (var innerException in reflectionException.LoaderExceptions)
                    result.AddRange(FlattenExceptionHierarchy(innerException));
            }
#if ASYNC
            if (exception is AggregateException)
            {
                var aggregateException = (exception as AggregateException);
                result.AddRange(aggregateException.InnerExceptions);

                foreach (var innerException in aggregateException.InnerExceptions)
                    result.AddRange(FlattenExceptionHierarchy(innerException));
            }
            else
#endif
            if (exception.InnerException != null)
            {
                result.Add(exception.InnerException);
                result.AddRange(FlattenExceptionHierarchy(exception.InnerException));
            }

            return result;
        }
    }
}
