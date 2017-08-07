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
#if !NET45 && !NETSTANDARD1_3 && !NETSTANDARD1_6
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
#if NET45 || NETSTANDARD1_3 || NETSTANDARD1_6
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(exception).Throw();
#else
            PreserveStackTrace(exception);
            throw exception;
#endif
        }

        // TODO: Move to a utility class
        /// <summary>
        /// Builds up a message, using the Message field of the specified exception
        /// as well as any InnerExceptions.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>A combined message string.</returns>
        public static string BuildMessage(Exception exception)
        {
            const bool isFriendlyMessage = false;
            return BuildMessage(exception, isFriendlyMessage);
        }

        /// <summary>
        /// Builds up a message, using the Message field of the specified exception
        /// as well as any InnerExceptions. Excludes exception names, creating more readable message
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>A combined message string.</returns>
        public static string BuildFriendlyMessage(Exception exception)
        {
            const bool isFriendlyMessage = true;
            return BuildMessage(exception, isFriendlyMessage);
        }

        /// <summary>
        /// Builds up a message, using the Message field of the specified exception
        /// as well as any InnerExceptions.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>A combined stack trace.</returns>
        public static string BuildStackTrace(Exception exception)
        {
            StringBuilder sb = new StringBuilder(GetStackTrace(exception));

            foreach (Exception inner in FlattenExceptionHierarchy(exception))
            {
                sb.Append(Environment.NewLine);
                sb.Append("--");
                sb.Append(inner.GetType().Name);
                sb.Append(Environment.NewLine);
                sb.Append(GetStackTrace(inner));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets the stack trace of the exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>A string representation of the stack trace.</returns>
        public static string GetStackTrace(Exception exception)
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

        private static string BuildMessage(Exception exception, bool isFriendlyMessage)
        {
            StringBuilder sb = new StringBuilder();
            WriteException(sb, exception, isFriendlyMessage);

            foreach (Exception inner in FlattenExceptionHierarchy(exception))
            {
                sb.Append(Environment.NewLine);
                sb.Append("  ----> ");
                WriteException(sb, inner, isFriendlyMessage);
            }

            return sb.ToString();
        }

        private static void WriteException(StringBuilder sb, Exception inner, bool isFriendlyMessage)
        {
            if (!isFriendlyMessage)
            {
                sb.AppendFormat(CultureInfo.CurrentCulture, "{0} : ", inner.GetType().ToString());
            }
            sb.Append(inner.Message);
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
