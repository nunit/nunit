// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
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

namespace NUnit.Framework
{
    /// <summary>
    /// Class used to guard against unexpected argument values
    /// or operations by throwing an appropriate exception.
    /// </summary>
    internal static class Guard
    {
        /// <summary>
        /// Throws an exception if an argument is null
        /// </summary>
        /// <param name="value">The value to be tested</param>
        /// <param name="name">The name of the argument</param>
        public static void ArgumentNotNull(object value, string name)
        {
            if (value == null)
                throw new ArgumentNullException("Argument " + name + " must not be null", name);
        }

        /// <summary>
        /// Throws an exception if a string argument is null or empty
        /// </summary>
        /// <param name="value">The value to be tested</param>
        /// <param name="name">The name of the argument</param>
        public static void ArgumentNotNullOrEmpty(string value, string name)
        {
            ArgumentNotNull(value, name);

            if (value == string.Empty)
                throw new ArgumentException("Argument " + name +" must not be the empty string", name);
        }

        /// <summary>
        /// Throws an ArgumentOutOfRangeException if the specified condition is not met.
        /// </summary>
        /// <param name="condition">The condition that must be met</param>
        /// <param name="message">The exception message to be used</param>
        /// <param name="paramName">The name of the argument</param>
        public static void ArgumentInRange(bool condition, string message, string paramName)
        {
            if (!condition)
                throw new ArgumentOutOfRangeException(paramName, message);
        }

        /// <summary>
        /// Throws an ArgumentException if the specified condition is not met.
        /// </summary>
        /// <param name="condition">The condition that must be met</param>
        /// <param name="message">The exception message to be used</param>
        /// <param name="paramName">The name of the argument</param>
        public static void ArgumentValid(bool condition, string message, string paramName)
        {
            if (!condition)
                throw new ArgumentException(message, paramName);
        }

        /// <summary>
        /// Throws an InvalidOperationException if the specified condition is not met.
        /// </summary>
        /// <param name="condition">The condition that must be met</param>
        /// <param name="message">The exception message to be used</param>
        public static void OperationValid(bool condition, string message)
        {
            if (!condition)
                throw new InvalidOperationException(message);
        }
    }
}
