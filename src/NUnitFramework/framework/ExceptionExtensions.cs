// ***********************************************************************
// Copyright (c) 2019 Charlie Poole, Rob Prouse
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
