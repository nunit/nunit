// ***********************************************************************
// Copyright (c) 2018 Charlie Poole, Rob Prouse
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

#if ASYNC
using System;
using System.Threading.Tasks;
using NUnit.Framework.Constraints;

namespace NUnit.Framework
{
    public static partial class AsyncAssert
    {
        /// <summary>
        /// Verifies that an async delegate throws an exception when called
        /// and returns it.
        /// </summary>
        /// <param name="code">A TestDelegate</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static Task<Exception> Catch(AsyncTestDelegate code, string message, params object[] args)
        {
            return Throws(new InstanceOfTypeConstraint(typeof(Exception)), code, message, args);
        }

        /// <summary>
        /// Verifies that an async delegate throws an exception when called
        /// and returns it.
        /// </summary>
        /// <param name="code">A TestDelegate</param>
        public static Task<Exception> Catch(AsyncTestDelegate code)
        {
            return Throws(new InstanceOfTypeConstraint(typeof(Exception)), code);
        }

        /// <summary>
        /// Verifies that an async delegate throws an exception of a certain Type
        /// or one derived from it when called and returns it.
        /// </summary>
        /// <param name="expectedExceptionType">The expected Exception Type</param>
        /// <param name="code">A TestDelegate</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static Task<Exception> Catch(Type expectedExceptionType, AsyncTestDelegate code, string message, params object[] args)
        {
            return Throws(new InstanceOfTypeConstraint(expectedExceptionType), code, message, args);
        }

        /// <summary>
        /// Verifies that an async delegate throws an exception of a certain Type
        /// or one derived from it when called and returns it.
        /// </summary>
        /// <param name="expectedExceptionType">The expected Exception Type</param>
        /// <param name="code">A TestDelegate</param>
        public static Task<Exception> Catch(Type expectedExceptionType, AsyncTestDelegate code)
        {
            return Throws(new InstanceOfTypeConstraint(expectedExceptionType), code);
        }

        /// <summary>
        /// Verifies that an async delegate throws an exception of a certain Type
        /// or one derived from it when called and returns it.
        /// </summary>
        /// <param name="code">A TestDelegate</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
#if NET40
        // Approximate TPL implementation since the types needed for the async keyword are not declared.
        public static Task<TActual> Catch<TActual>(AsyncTestDelegate code, string message, params object[] args) where TActual : Exception
        {
            return Throws(new InstanceOfTypeConstraint(typeof(TActual)), code, message, args).ContinueWith(task => (TActual)task.Result);
        }
#else
        public static async Task<TActual> Catch<TActual>(AsyncTestDelegate code, string message, params object[] args) where TActual : Exception
        {
            return (TActual)await Throws(new InstanceOfTypeConstraint(typeof(TActual)), code, message, args).ConfigureAwait(false);
        }
#endif

        /// <summary>
        /// Verifies that an async delegate throws an exception of a certain Type
        /// or one derived from it when called and returns it.
        /// </summary>
        /// <param name="code">A TestDelegate</param>
        public static Task<TActual> Catch<TActual>(AsyncTestDelegate code) where TActual : Exception
        {
            return Catch<TActual>(code, string.Empty, null);
        }
    }
}
#endif
